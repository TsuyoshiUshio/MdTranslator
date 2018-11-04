using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace MdTranslatorLibrary.Test
{
    public class GitHubRepositoryTest
    {
        [Fact]
        public void Delete_BranchAsync_NormalCase()
        {
            var InputOwner = "foo";
            var InputRepo = "bar";
            var InputBranch = "baz";
            var ExpectedURL = $"https://api.github.com/repos/{InputOwner}/{InputRepo}/git/refs/heads/{InputBranch}";
            var fixture = new GitHubFixture();
            fixture.SetUpDeleteAsync(ExpectedURL);
            var repository = new GitHubRepository(fixture.GitHubContext);
            var result = repository.DeleteBranchAsync(InputOwner, InputRepo, InputBranch);
            fixture.VerifyDeleteAsync(ExpectedURL);
        }

        [Fact]
        public async Task Get_BranchAsync_NormalCase()
        {
            var InputOwner = "foo";
            var InputRepo = "bar";
            var InputBranch = "baz";

            var ExpectedURL = $"https://api.github.com/repos/{InputOwner}/{InputRepo}/branches/{InputBranch}";
            var ExpectedSha = "qux";
            var fixture = new GitHubFixture();

            var ReturnBranch = new Branch
            {
                commit = new Commit()
                {
                    sha = ExpectedSha
                }
            };

            fixture.SetupGetBranchAsync(ExpectedURL, JsonConvert.SerializeObject(ReturnBranch));
            var repository = new GitHubRepository(fixture.GitHubContext);
            var branch = await repository.GetBranchAsync(InputOwner, InputRepo, InputBranch);
            Assert.Equal(ExpectedSha, branch.commit.sha);
        }

        [Fact]
        public async Task Get_Branch_Exception()
        {
            var InputOwner = "foo";
            var InputRepo = "bar";
            var InputBranch = "baz";

            var ExpectedURL = $"https://api.github.com/repos/{InputOwner}/{InputRepo}/branches/{InputBranch}";
            var ExpectedSha = "qux";
            var fixture = new GitHubFixture();

            fixture.SetupGetBranchWithFailure(ExpectedURL);
            var repository = new GitHubRepository(fixture.GitHubContext);
            var ex = await Assert.ThrowsAsync<RestAPICallException>(async () =>
                await repository.GetBranchAsync(InputOwner, InputRepo, InputBranch)
            );
            Assert.Equal("InternalServerError", ex.StatusCode);
            Assert.Equal("Internal Server Error", ex.Message);
            Assert.Equal(ExpectedURL, ex.RequestMessage.RequestUri.ToString());
        }

        [Fact]
        public async Task Create_Branch_Normal()
        {
            var InputOwner = "foo";
            var InputRepo = "bar";
            var InputBranchName = "baz";
            var InputSha = "qux";
            //         public async Task CreateBranchAsync(string owner, string repo, string branchName, string sha)
            var ExpectedURL = $"https://api.github.com/repos/{InputOwner}/{InputRepo}/git/refs";
            var branchRef = new BranchRef()
            {
                Ref = $"refs/heads/{InputBranchName}",
                sha = InputSha
            };
            var ExpectedJson = JsonConvert.SerializeObject(branchRef);

            var fixture = new GitHubFixture();
            fixture.SetupCreateBranch(ExpectedURL, ExpectedJson);
            var repository = new GitHubRepository(fixture.GitHubContext);
            await repository.CreateBranchAsync(InputOwner, InputRepo, InputBranchName, InputSha);
            fixture.VerifyCreateBranch(ExpectedURL, ExpectedJson);
        }

        [Fact]
        public async Task Create_Branch_Exception()
        {
            var InputOwner = "foo";
            var InputRepo = "bar";
            var InputBranchName = "baz";
            var InputSha = "qux";
            var ExpectedURL = $"https://api.github.com/repos/{InputOwner}/{InputRepo}/git/refs";
            var branchRef = new BranchRef()
            {
                Ref = $"refs/heads/{InputBranchName}",
                sha = InputSha
            };
            var ExpectedJson = JsonConvert.SerializeObject(branchRef);

            var fixture = new GitHubFixture();
            fixture.SetupCreateBranchWithFailure(ExpectedURL, ExpectedJson);
            var repository = new GitHubRepository(fixture.GitHubContext);

            var ex = await Assert.ThrowsAsync<RestAPICallException>(async () =>
                await repository.CreateBranchAsync(InputOwner, InputRepo, InputBranchName, InputSha)
            );

            Assert.Equal("InternalServerError", ex.StatusCode);
            Assert.Equal("Internal Server Error", ex.Message);
            Assert.Equal(ExpectedURL, ex.RequestMessage.RequestUri.ToString());
        }

        [Fact]
        public async Task Search_MdFiles_Normal()
        {
            var InputOwner = "foo";
            var InputRepo = "bar";
            var InputSha = "baz";
            var ExpectedURL = $"https://api.github.com/repos/{InputOwner}/{InputRepo}/git/trees/{InputSha}?recursive=1";

            var fixture = new GitHubFixture();
            var trees = new Tree[]
            {
                new Tree()
                {
                    path = "abc.txt"
                },
                new Tree()
                {
                    path = "bcd.md"
                },
                new Tree()
                {
                    path = "cde.md"
                },
                new Tree()
                {
                    path = "def.lock"
                }
            };
            var searchResult = new SearchResult();
            searchResult.tree = trees;
            var treeJson = JsonConvert.SerializeObject(searchResult);

            fixture.SetupSearchMd(ExpectedURL, treeJson);
            var repository = new GitHubRepository(fixture.GitHubContext);
            var result = await repository.SearchMdFilesAsync(InputOwner, InputRepo, InputSha);
            Assert.Equal(2, result.Count());
            var enumerator = result.GetEnumerator();
            enumerator.MoveNext();
            Assert.Equal("bcd.md", enumerator.Current.path);
            enumerator.MoveNext();
            Assert.Equal("cde.md", enumerator.Current.path);

        }

        [Fact]
        public async Task Search_MdFiles_Exception()
        {
            var InputOwner = "foo";
            var InputRepo = "bar";
            var InputSha = "baz";
            var ExpectedURL = $"https://api.github.com/repos/{InputOwner}/{InputRepo}/git/trees/{InputSha}?recursive=1";

            var fixture = new GitHubFixture();

            fixture.SetupSearchMdWithFailure(ExpectedURL);
            var repository = new GitHubRepository(fixture.GitHubContext);

            var ex = await Assert.ThrowsAsync<RestAPICallException>(async () =>
                await repository.SearchMdFilesAsync(InputOwner, InputRepo, InputSha)
            );

            Assert.Equal("InternalServerError", ex.StatusCode);
            Assert.Equal("Internal Server Error", ex.Message);
            Assert.Equal(ExpectedURL, ex.RequestMessage.RequestUri.ToString());
        }

        [Fact]
        public async Task Get_FileContents_Normal()
        {
            var InputOwner = "foo";
            var InputRepo = "bar";
            var InputBranch = "baz";
            var InputPath = "qux.md";
            var ExpectedContentURL = $"https://api.github.com/repos/{InputOwner}/{InputRepo}/contents/{InputPath}?ref={InputBranch}";
            var ExpectedDownloadURL = $"https://raw.githubusercontent.com/foo/bar/baz/qux.md";
            var ExpectedContent = "quux";
            // Task<string> GetFileContents(string owner, string repo, string branch, string path)
            // var response = await context.GetAsync($"{RestAPIBase}/{owner}/{repo}/contents/{path}?ref={branch}");
            // var response = await context.GetAsync(downloadUrl);
            var fixture = new GitHubFixture();
            fixture.SetupGetFileContents(ExpectedContentURL, ExpectedDownloadURL,ExpectedContent);
            var respository = new GitHubRepository(fixture.GitHubContext);
            var result = await respository.GetFileContents(InputOwner, InputRepo, InputBranch, InputPath);
            Assert.Equal(ExpectedContent, result);
        }

        [Fact]
        public async Task Get_FileContents_Exception_First()
        {
            var InputOwner = "foo";
            var InputRepo = "bar";
            var InputBranch = "baz";
            var InputPath = "qux.md";
            var ExpectedContentURL = $"https://api.github.com/repos/{InputOwner}/{InputRepo}/contents/{InputPath}?ref={InputBranch}";

            var fixture = new GitHubFixture();
            fixture.SetupGetFileContentsFirstFails(ExpectedContentURL);
            var respository = new GitHubRepository(fixture.GitHubContext);
            var ex = await Assert.ThrowsAsync<RestAPICallException>(async () =>
                await respository.GetFileContents(InputOwner, InputRepo, InputBranch, InputPath)
            );

            Assert.Equal("InternalServerError", ex.StatusCode);
            Assert.Equal("Internal Server Error", ex.Message);
            Assert.Equal(ExpectedContentURL, ex.RequestMessage.RequestUri.ToString());
        }

        [Fact]
        public async Task Get_FileContents_Exception_Second()
        {
            var InputOwner = "foo";
            var InputRepo = "bar";
            var InputBranch = "baz";
            var InputPath = "qux.md";
            var ExpectedContentURL = $"https://api.github.com/repos/{InputOwner}/{InputRepo}/contents/{InputPath}?ref={InputBranch}";
            var ExpectedDownloadURL = $"https://raw.githubusercontent.com/foo/bar/baz/qux.md";

            var fixture = new GitHubFixture();
            fixture.SetupGetFileContentsSecondFails(ExpectedContentURL, ExpectedDownloadURL);
            var respository = new GitHubRepository(fixture.GitHubContext);

            var ex = await Assert.ThrowsAsync<RestAPICallException>(async () =>
                await respository.GetFileContents(InputOwner, InputRepo, InputBranch, InputPath)
            );

            Assert.Equal("InternalServerError", ex.StatusCode);
            Assert.Equal("Internal Server Error", ex.Message);
            Assert.Equal(ExpectedDownloadURL, ex.RequestMessage.RequestUri.ToString());
        }

        [Fact]
        public async Task Update_FileContents_Normal()
        {
            var fixture = new GitHubFixture();
            fixture.SetUpSampleInputOperation();

            var ExpectedURL = $"https://api.github.com/repos/{fixture.InputOwner}/{fixture.InputRepo}/contents/{fixture.InputPath}";

            // On the Setup, I call Assert on the Callback.
            fixture.SetupUpdateFileContents(ExpectedURL, fixture.InputOperation);
            var repository = new GitHubRepository(fixture.GitHubContext);
            await repository.UpdateFileContents(fixture.InputOwner, fixture.InputRepo, fixture.InputPath,
                fixture.InputOperation);
        }

        [Fact]
        public async Task Update_FileContents_Exception()
        {
            var fixture = new GitHubFixture();
            fixture.SetUpSampleInputOperation();
            var ExpectedURL = $"https://api.github.com/repos/{fixture.InputOwner}/{fixture.InputRepo}/contents/{fixture.InputPath}";
            fixture.SetupUpdateFileContentsFailure(ExpectedURL, fixture.InputOperation);
            var repository = new GitHubRepository(fixture.GitHubContext);
            var ex = await Assert.ThrowsAsync<RestAPICallException>(async () =>
                    await repository.UpdateFileContents(fixture.InputOwner, fixture.InputRepo, fixture.InputPath,
                        fixture.InputOperation)
            );

            Assert.Equal("InternalServerError", ex.StatusCode);
            Assert.Equal("Internal Server Error", ex.Message);
            Assert.Equal(ExpectedURL, ex.RequestMessage.RequestUri.ToString());
        }


        private class GitHubFixture
        {
            public string InputOwner { get; set; }
            public string InputRepo { get; set; }

            public string InputBranch { get; set; }
            public string InputPath { get; set; }

            public FileOperation InputOperation { get; set; }

            public IGitHubContext GitHubContext => gitHubContextMock.Object;
            private Mock<IGitHubContext> gitHubContextMock;

            public GitHubFixture()
            {
                this.InputOwner = "foo";
                this.InputRepo = "bar";
                this.InputBranch = "baz";
                this.InputPath = "qux.md";
            }

            public void SetUpDeleteAsync(string url)
            {
                this.SetUp();
                gitHubContextMock.Setup(p => p.DeleteAsync(url)).ReturnsAsync(new HttpResponseMessage());
            }

            public void VerifyDeleteAsync(string url)
            {
                gitHubContextMock.Verify(p => p.DeleteAsync(url));
            }

            public void SetupGetBranchAsync(string url, string expectedJsonResponse)
            {
                this.SetUp();
                var message = new HttpResponseMessage();
                message.StatusCode = HttpStatusCode.OK;
                message.Content = new StringContent(expectedJsonResponse);
                gitHubContextMock.Setup(p => p.GetAsync(url)).ReturnsAsync(message);
            }

            public void SetupGetBranchWithFailure(string url)
            {
                this.SetUp();
                var message = new HttpResponseMessage();
                message.StatusCode = HttpStatusCode.InternalServerError;
                message.Content = new StringContent("Internal exception happens for some reason");

                var requestMessage = new HttpRequestMessage();
                requestMessage.RequestUri = new Uri(url);
                message.RequestMessage = requestMessage;

                gitHubContextMock.Setup(p => p.GetAsync(url)).ReturnsAsync(message);
            }

            public void SetupCreateBranch(string url, string jsonContents)
            {
                this.SetUp();
                var message = new HttpResponseMessage();
                message.StatusCode = HttpStatusCode.Created;
                message.Content = new StringContent("hello");
                gitHubContextMock.Setup(p => p.PostAsync(url, jsonContents)).ReturnsAsync(message);
            }

            public void VerifyCreateBranch(string url, string jsonContents)
            {
                gitHubContextMock.Verify(p => p.PostAsync(url, jsonContents));
            }

            public void SetupCreateBranchWithFailure(string url, string jsonContents)
            {
                this.SetUp();
                var message = new HttpResponseMessage();
                message.StatusCode = HttpStatusCode.InternalServerError;
                message.Content = new StringContent("hello");

                var requestMessage = new HttpRequestMessage();
                requestMessage.RequestUri = new Uri(url);
                message.RequestMessage = requestMessage;

                gitHubContextMock.Setup(p => p.PostAsync(url, jsonContents)).ReturnsAsync(message);
            }

            public void SetupSearchMd(string url, string jsonContents)
            {
                // var response = await context.GetAsync($"{RestAPIBase}/{owner}/{repo}/git/trees/{sha}?recursive=1");
                this.SetUp();
                var message = new HttpResponseMessage();
                message.StatusCode = HttpStatusCode.OK;
                message.Content = new StringContent(jsonContents);
                gitHubContextMock.Setup(p => p.GetAsync(url)).ReturnsAsync(message);
            }

            public void SetupSearchMdWithFailure(string url)
            {
                this.SetUp();
                var message = new HttpResponseMessage();
                message.StatusCode = HttpStatusCode.InternalServerError;

                var requestMessage = new HttpRequestMessage();
                requestMessage.RequestUri = new Uri(url);
                message.RequestMessage = requestMessage;

                gitHubContextMock.Setup(p => p.GetAsync(url)).ReturnsAsync(message);
            }

            public void SetupGetFileContents(string contentUrl, string downloadUrl, string expectedContents)
            {
                this.SetUp();
                var message = new HttpResponseMessage();
                message.StatusCode = HttpStatusCode.OK;
                var content = new Content
                {
                    download_url = downloadUrl
                };
                message.Content = new StringContent(JsonConvert.SerializeObject(content));

                gitHubContextMock.Setup(p => p.GetAsync(contentUrl)).ReturnsAsync(message);

                var downloadMessage = new HttpResponseMessage();
                downloadMessage.StatusCode = HttpStatusCode.OK;
                downloadMessage.Content = new StringContent(expectedContents);

                gitHubContextMock.Setup(p => p.GetAsync(downloadUrl)).ReturnsAsync(downloadMessage);
            }

            public void SetupGetFileContentsFirstFails(string contentUrl)
            {
                this.SetUp();
                var message = new HttpResponseMessage();
                message.StatusCode = HttpStatusCode.InternalServerError;

                var requestMessage = new HttpRequestMessage();
                requestMessage.RequestUri = new Uri(contentUrl);
                message.RequestMessage = requestMessage;

                gitHubContextMock.Setup(p => p.GetAsync(contentUrl)).ReturnsAsync(message);
            }

            public void SetupGetFileContentsSecondFails(string contentUrl, string downloadUrl)
            {
                this.SetUp();
                var message = new HttpResponseMessage();
                message.StatusCode = HttpStatusCode.OK;
                var content = new Content
                {
                    download_url = downloadUrl
                };
                message.Content = new StringContent(JsonConvert.SerializeObject(content));

                gitHubContextMock.Setup(p => p.GetAsync(contentUrl)).ReturnsAsync(message);

                var downloadMessage = new HttpResponseMessage();
                downloadMessage.StatusCode = HttpStatusCode.InternalServerError;

                var requestMessage = new HttpRequestMessage();
                requestMessage.RequestUri = new Uri(downloadUrl);
                downloadMessage.RequestMessage = requestMessage;

                gitHubContextMock.Setup(p => p.GetAsync(downloadUrl)).ReturnsAsync(downloadMessage);
            }

            public void SetUpSampleInputOperation()
            {
                var InputMessage = "hello.";
                var InputCommiterName = "Tsuyoshi Ushio";
                var InputCommiterEmail = "foo@bar.com";
                var InputContent = "konichiwa.";
                var InputSha = "qux";
  

                // public async Task UpdateFileContents(string owner, string repo, string path, FileOperation operation)
                this.InputOperation = new FileOperation
                {
                    message = InputMessage,
                    commiter = new Commiter
                    {
                        name = InputCommiterName,
                        email = InputCommiterEmail
                    },
                    branch = this.InputBranch,
                    content = Convert.ToBase64String(Encoding.UTF8.GetBytes(InputContent)),
                    sha = InputSha
                };
            }

            public void SetupUpdateFileContents(string url, FileOperation operation)
            {
                this.SetUp();
                var message = new HttpResponseMessage();
                message.StatusCode = HttpStatusCode.OK;

                gitHubContextMock.Setup(p => p.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(message)
                    .Callback<HttpRequestMessage>((meg) =>
                    {
                       Assert.Equal(HttpMethod.Put, meg.Method);
                       Assert.Equal(url, meg.RequestUri.ToString());
                       Assert.Equal(JsonConvert.SerializeObject(operation), meg.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                    });
            }

            public void SetupUpdateFileContentsFailure(string url, FileOperation operation)
            {
                this.SetUp();
                var message = new HttpResponseMessage();
                message.StatusCode = HttpStatusCode.InternalServerError;

                var requestMessage = new HttpRequestMessage();
                requestMessage.RequestUri = new Uri(url);
                message.RequestMessage = requestMessage;
                gitHubContextMock.Setup(p => p.SendAsync(It.IsAny<HttpRequestMessage>())).ReturnsAsync(message);
            }

            private void SetUp()
            {
                gitHubContextMock = new Mock<IGitHubContext>();


            }
        }
    }
}
