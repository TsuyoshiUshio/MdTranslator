using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MdTranslatorLibrary
{
    public interface ITranslatorContext
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
    public class TranslatorContext : ITranslatorContext
    {
        private HttpClient client;
        public TranslatorContext(HttpClient client)
        {
            this.client = client;
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return client.SendAsync(request);
        }
    }
}
