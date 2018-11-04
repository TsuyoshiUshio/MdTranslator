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
        Task<HttpResponseMessage> GetAsync(string url);
        Task<HttpResponseMessage> PostAsync(string url, string contents);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

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

        public Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return client.DeleteAsync(url);           
        }

        public Task<HttpResponseMessage> GetAsync(string url)
        {
            return client.GetAsync(new Uri(url));
        }

        public Task<HttpResponseMessage> PostAsync(string url, string contents)
        {
            return client.PostAsync(new Uri(url), new StringContent(contents));
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return client.SendAsync(request);
        }
    }
}
