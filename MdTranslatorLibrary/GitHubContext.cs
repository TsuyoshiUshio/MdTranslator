using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MdTranslatorLibrary
{
    public interface IGitHubContext
    {
        Task<HttpResponseMessage> DeleteAsync(string url);
    }
    public class GitHubContext : IGitHubContext
    {
        private HttpClient client;
        public GitHubContext(HttpClient client)
        {
            this.client = client;
            client.DefaultRequestHeaders.Add("User-Agent", "MdTranslator");
            client.DefaultRequestHeaders.Add("Authorization", $"token {Environment.GetEnvironmentVariable("GitHubToken")}");
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return await client.DeleteAsync(url);           
        }
    }
}
