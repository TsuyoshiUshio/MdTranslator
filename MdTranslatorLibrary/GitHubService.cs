using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace MdTranslatorLibrary
{
    public interface IGitHubService
    {
        Task<Branch> CreateBranchAsync(string owner, string repo, string sourceBranch, string language);
        Task<IEnumerable<string>> SearchMdFilePaths(string owner, string repo, string sha);
        Task<ValueTuple<Content, string>> GetFileContents(string owner, string repo, string branch, string path);

        Task UpdateFileContentsAsync(string owner, string repo, string path, string branch, string text, string sha,
            string language);
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

        public async Task<IEnumerable<string>> SearchMdFilePaths(string owner, string repo, string sha)
        {
            var results = await repository.SearchMdFilesAsync(owner, repo, sha);
            return results.Select(p => p.path);
        }

        public Task<ValueTuple<Content, string>> GetFileContents(string owner, string repo, string branch, string path)
        {
           return repository.GetFileContents(owner, repo, branch, path);
          
        }

        public async Task UpdateFileContentsAsync(string owner, string repo, string path, string branch, string text, string sha, string language)
        {
            var operation = new FileOperation()
            {
                message = $"Generate {language} version",
                commiter = new Commiter()
                {
                    name = Environment.GetEnvironmentVariable("CommitName"),
                    email = Environment.GetEnvironmentVariable("CommitEmail")
                },
                branch = branch,
                content = Convert.ToBase64String(Encoding.UTF8.GetBytes(text)),
                sha = sha
            };
            await repository.UpdateFileContents(owner, repo, path, operation);

        }
    }
}
