using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MdTranslatorLibrary
{
    public interface IGitHubRepository
    {
        Task DeleteBranchAsync(string owner, string repo, string branchName);
    }
    public class GitHubRepository : IGitHubRepository
    {
        private IGitHubContext context;
        public GitHubRepository(IGitHubContext context)
        {
            this.context = context;
        }
        public async Task DeleteBranchAsync(string owner, string repo, string branchName)
        {
            await context.DeleteAsync($"https://api.github.com/repos/{owner}/{repo}/git/refs/heads/{branchName}");
        }

        public async Task<Branch> GetBranchAsync(string branchName)
        {
            return null;
        }

        public async Task CreateBranchAsync(string branchName, string sha)
        {

        }
    }
}
