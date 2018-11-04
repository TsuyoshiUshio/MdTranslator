using System;
using System.Net.Http;
using Moq;
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
            private void SetUp()
            {
                gitHubContextMock = new Mock<IGitHubContext>();
            }
        }
    }
}
