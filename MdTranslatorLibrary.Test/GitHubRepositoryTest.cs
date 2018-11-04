using System;
using System.Net;
using System.Net.Http;
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
            //         public async Task CreateBranchAsync(string owner, string repo, string branchName, string sha)
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


        private class GitHubFixture
        {
            public IGitHubContext GitHubContext => gitHubContextMock.Object;
            private Mock<IGitHubContext> gitHubContextMock;

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

            private void SetUp()
            {
                gitHubContextMock = new Mock<IGitHubContext>();
            }
        }
    }
}
