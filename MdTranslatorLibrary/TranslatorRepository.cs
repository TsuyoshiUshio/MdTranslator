using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MdTranslatorLibrary
{
    public interface ITranslatorRepository
    {
        Task<string> TranslateAsync(string text, string language);
    }
    public class TranslatorRepository : ITranslatorRepository
    {
        private ITranslatorContext context;
        private string key;
        public TranslatorRepository(ITranslatorContext context)
        {
            this.context = context;
            this.key = Environment.GetEnvironmentVariable("TranslatorKey");
        }
        public async Task<string> TranslateAsync(string text, string language)
        {
            System.Object[] body = new System.Object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(GetUrl(language));
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);

                var response = await context.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var translated = JsonConvert.DeserializeObject<Translated[]>(responseBody);
                    // Translation should be there.
                    return translated.FirstOrDefault()?.translations.FirstOrDefault()?.text ?? "";
                }
                else
                {
                    throw new RestAPICallException(response.StatusCode.ToString(), response.ReasonPhrase, response.RequestMessage);
                }
            }
        }

        private string GetUrl(string language)
        {
            var host = "https://api.cognitive.microsofttranslator.com";
            var path = "/translate?api-version=3.0";
            var params_ = $"&from=en&to={language}";
            return host + path + params_;
        }
    }
}
