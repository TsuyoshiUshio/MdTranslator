using System;
using System.Threading.Tasks;

namespace MdTranslatorLibrary
{
    public interface IGitHubService
    {
        Task<Branch> CreateBranchAsync(string owner, string repo, string sourceBranch, string language);
    }
    public class GitHubService : IGitHubService
    {
        private IGitHubRepository repository;
        public GitHubService(IGitHubRepository repository)
        {
            this.repository = repository;
        }
        /// <summary>
        /// This method create a new branch from the targetBranch.
        /// The new branch name becomes {targetBranch}-{language}
        /// </summary>
        /// <param name="owner">the GitHub owner. https://github.com/owner</param>
        /// <param name="repo">the GitHub repository name. https://github.com/owner/repo</param>
        /// <param name="sourceBranch">Source branch (e.g. master)</param>
        /// <param name="language">Language to translate</param>
        /// <returns><see cref="Branch"/></returns>
        public async Task<Branch> CreateBranchAsync(string owner, string repo, string sourceBranch, string language)
        {
            var targetBranch = $"{sourceBranch}-{language}";
            await repository.DeleteBranchAsync(owner, repo, targetBranch);
            var branch = await repository.GetBranchAsync(owner, repo, sourceBranch);
            await repository.CreateBranchAsync(owner, repo, targetBranch, branch.commit.sha);
            return await repository.GetBranchAsync(owner, repo, targetBranch);
        }

    }
}
