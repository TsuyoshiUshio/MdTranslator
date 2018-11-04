using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MdTranslatorLibrary
{
    public interface IGitHubRepository
    {
        Task DeleteBranchAsync(string owner, string repo, string branchName);
        Task<Branch> GetBranchAsync(string owner, string repo, string branchName);
    }
    public class GitHubRepository : IGitHubRepository
    {
        private IGitHubContext context;
        private const string RestAPIBase = "https://api.github.com/repos";
        public GitHubRepository(IGitHubContext context)
        {
            this.context = context;
        }
        public async Task DeleteBranchAsync(string owner, string repo, string branchName)
        {
            await context.DeleteAsync($"{RestAPIBase}/{owner}/{repo}/git/refs/heads/{branchName}");
        }

        public async Task<Branch> GetBranchAsync(string owner, string repo, string branchName)
        {
            var response =
                await context.GetAsync($"{RestAPIBase}/{owner}/{repo}/branches/{branchName}");
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<Branch>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                throw new RestAPICallException(response.StatusCode.ToString(), response.ReasonPhrase, response.RequestMessage);
            }
        }

        public async Task CreateBranchAsync(string owner, string repo, string branchName, string sha)
        {
            var branchRef = new BranchRef
            {
                Ref = $"refs/heads/{branchName}",
                sha = sha
            };
            var json = JsonConvert.SerializeObject(branchRef);
            var response = await context.PostAsync($"{RestAPIBase}/{owner}/{repo}/git/refs", json);
            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new RestAPICallException(response.StatusCode.ToString(), response.ReasonPhrase, response.RequestMessage);
            }
        }
    }
}
