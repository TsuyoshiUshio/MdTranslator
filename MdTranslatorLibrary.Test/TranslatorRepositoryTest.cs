using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Moq.Language.Flow;
using Newtonsoft.Json;
using Xunit;

namespace MdTranslatorLibrary.Test
{
    public class TranslatorRepositoryTest
    {

        private class KeySession : IDisposable
        {
            const string TranslationKey = "TranslatorKey";
            public string InputKey { get; set; }
            private string oldKey;

            public KeySession()
            {
                const string TranslationKey = "TranslatorKey";
                this.oldKey = Environment.GetEnvironmentVariable(TranslationKey);
                this.InputKey = "b87788fd8823309aga8888009999777";
                Environment.SetEnvironmentVariable(TranslationKey, InputKey);
            }

            public void Dispose()
            {
                Environment.SetEnvironmentVariable(TranslationKey, oldKey);
            }
        }

        [Fact]
        public async Task Translate_Normal()
        {
            using (var session = new KeySession())
            {
                var fixture = new TranslateFixture();
                var ExpectedURL =
                    $"https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&from=en&to={fixture.InputLanguage}";
                fixture.SetupTranslate(session.InputKey, ExpectedURL);
                var respository = new TranslatorRepository(fixture.TranslatorContext);
                // In the setup callback, I write Assert.
                var result = await respository.TranslateAsync(fixture.InputText, fixture.InputLanguage);
                Assert.Equal(fixture.ExpectedTranslatedText, result);
            }
        }

        [Fact]
        public async Task Translate_Exception()
        {
            using (var session = new KeySession())
            {
                var fixture = new TranslateFixture();
                var ExpectedURL =
                    $"https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&from=en&to={fixture.InputLanguage}";
                fixture.SetupTranslateFailure(ExpectedURL);
                var respository = new TranslatorRepository(fixture.TranslatorContext);
                // In the setup callback, I write Assert.
                var ex = await Assert.ThrowsAsync<RestAPICallException>(async () =>
                    await respository.TranslateAsync(fixture.InputText, fixture.InputLanguage)
                );

                Assert.Equal("InternalServerError", ex.StatusCode);
                Assert.Equal("Internal Server Error", ex.Message);
                Assert.Equal(ExpectedURL, ex.RequestMessage.RequestUri.ToString());
            }
        }

        [Fact]
        public async Task Translate_LessReturnData()
        {
            using (var session = new KeySession())
            {
                var fixture = new TranslateFixture();
                fixture.SetupTranslateLessResult();
                var respository = new TranslatorRepository(fixture.TranslatorContext);
                // In the setup callback, I write Assert.
                var result = await respository.TranslateAsync(fixture.InputText, fixture.InputLanguage);
                Assert.Equal("", result);
            }
        }

        [Fact]
        public async Task Translate_LessReturnDataSecond()
        {
            using (var session = new KeySession())
            {
                var fixture = new TranslateFixture();
                fixture.SetupTranslateLessResultSecond();
                var respository = new TranslatorRepository(fixture.TranslatorContext);
                // In the setup callback, I write Assert.
                var result = await respository.TranslateAsync(fixture.InputText, fixture.InputLanguage);
                Assert.Equal("", result);
            }
        }

        private class TranslateFixture
        {
            private Mock<ITranslatorContext> translatorContextMock;
            public ITranslatorContext TranslatorContext => translatorContextMock.Object;
                
            public string InputText { get; set; }
            public string InputLanguage { get; set; }
            public string ExpectedTranslatedText { get; set; }

            public TranslateFixture()
            {
                InputText = "hello";
                InputLanguage = "ja";
                ExpectedTranslatedText = "konnichiwa";
            }

            private void Setup()
            {
                this.translatorContextMock = new Mock<ITranslatorContext>();
            }

            public void SetupTranslate(string key, string url)
            {
                this.Setup();
                var translated = new Translated[]
                {
                    new Translated()
                    {
                        translations = new Translation[] {
                            new Translation()
                            {
                                text = ExpectedTranslatedText,
                                to = "ja"
                            },
                            new Translation()
                            {
                                text = "konchiwa",
                                to = "ja"
                            }, 
                        }
                    }
                };
                var message = new HttpResponseMessage();
                message.StatusCode = HttpStatusCode.OK;
                message.Content = new StringContent(JsonConvert.SerializeObject(translated));
                translatorContextMock.Setup(p => p.SendAsync(It.IsAny<HttpRequestMessage>()))
                    .ReturnsAsync(message)
                    .Callback<HttpRequestMessage>((msg) =>
                        {
                            msg.Headers.TryGetValues("Ocp-Apim-Subscription-Key", out IEnumerable<string> keyValue);
                            Assert.Equal(key, keyValue.FirstOrDefault());
                            Assert.Equal(url, msg.RequestUri.ToString());
                            System.Object[] body = new System.Object[] { new { Text = InputText } };
                            Assert.Equal(JsonConvert.SerializeObject(body), msg.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                        });
            }

            public void SetupTranslateLessResult()
            {
                this.Setup();
                var translated = new Translated[]
                {
                    new Translated()
                    {
                        translations = new Translation[] {
                        }
                    }
                };
                var message = new HttpResponseMessage();
                message.StatusCode = HttpStatusCode.OK;
                message.Content = new StringContent(JsonConvert.SerializeObject(translated));
                translatorContextMock.Setup(p => p.SendAsync(It.IsAny<HttpRequestMessage>()))
                    .ReturnsAsync(message);
            }

            public void SetupTranslateLessResultSecond()
            {
                this.Setup();
                var translated = new Translated[]
                {

                };
                var message = new HttpResponseMessage();
                message.StatusCode = HttpStatusCode.OK;
                message.Content = new StringContent(JsonConvert.SerializeObject(translated));
                translatorContextMock.Setup(p => p.SendAsync(It.IsAny<HttpRequestMessage>()))
                    .ReturnsAsync(message);
            }

            public void SetupTranslateFailure(string url)
            {
                this.Setup();
                var message = new HttpResponseMessage();
                message.StatusCode = HttpStatusCode.InternalServerError;

                var requestMessage = new HttpRequestMessage();
                requestMessage.RequestUri = new Uri(url);
                message.RequestMessage = requestMessage;

                translatorContextMock.Setup(p => p.SendAsync(It.IsAny<HttpRequestMessage>()))
                    .ReturnsAsync(message);
            }
        }
    }
}
