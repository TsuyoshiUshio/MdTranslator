using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MdTranslatorLibrary
{
    public interface IGitHubRepository
    {
        Task DeleteBranchAsync(string owner, string repo, string branchName);
        Task<Branch> GetBranchAsync(string owner, string repo, string branchName);

        Task CreateBranchAsync(string owner, string repo, string branchName, string sha);

        Task<IEnumerable<Tree>> SearchMdFilesAsync(string owner, string repo, string sha);

        Task<string> GetFileContents(string owner, string repo, string branch, string path);

        Task UpdateFileContents(string owner, string repo, string path, FileOperation operation);
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

        public async Task<IEnumerable<Tree>> SearchMdFilesAsync(string owner, string repo, string sha)
        {
            var response = await context.GetAsync($"{RestAPIBase}/{owner}/{repo}/git/trees/{sha}?recursive=1");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var search = JsonConvert.DeserializeObject<SearchResult>(await response.Content.ReadAsStringAsync());
                return search.tree.Where(p => p.path.EndsWith(".md"));
            }
            else
            {
                throw new RestAPICallException(response.StatusCode.ToString(), response.ReasonPhrase, response.RequestMessage);
            }
        }

        public async Task UpdateFileContents(string owner, string repo, string path, FileOperation operation)
        {
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Put;
                request.RequestUri = new Uri($"https://api.github.com/repos/{owner}/{repo}/contents/{path}");
                request.Content = new StringContent(JsonConvert.SerializeObject(operation), Encoding.UTF8, "application/json");
                var response = await context.SendAsync(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new RestAPICallException(response.StatusCode.ToString(), response.ReasonPhrase, response.RequestMessage);
                }
            }
        }

        public async Task<string> GetFileContents(string owner, string repo, string branch, string path)
        {
            var response = await context.GetAsync($"{RestAPIBase}/{owner}/{repo}/contents/{path}?ref={branch}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = JsonConvert.DeserializeObject<Content>(await response.Content.ReadAsStringAsync());
                return await DownloadContents(content.download_url);
            } 
            else
            {
                throw new RestAPICallException(response.StatusCode.ToString(), response.ReasonPhrase, response.RequestMessage);
            }
        }

        private async Task<string> DownloadContents(string downloadUrl)
        {
            var response = await context.GetAsync(downloadUrl);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new RestAPICallException(response.StatusCode.ToString(), response.ReasonPhrase, response.RequestMessage);
            }
        }
    }
}
