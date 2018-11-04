using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace MdTranslatorLibrary.Test
{
    public class GitHubServiceTest
    {

        [Fact]
        public async Task Create_Branch_Normal_Case()
        {
            var fixture = new GitHubFixture();
            var InputOwner = "foo";
            var InputRepo = "bar";
            var InputSourceBranch = "baz";
            var InputLanguage = "qux";
            var ExpectedBranchName = $"{InputSourceBranch}-{InputLanguage}";
            fixture.SetUpCreateBranchAsync(InputOwner, InputRepo, ExpectedBranchName);
            var service = new GitHubService(fixture.GitHubRepository);
            service.CreateBranchAsync(InputOwner, InputRepo, InputSourceBranch, InputLanguage);
            fixture.VerifyCreateBranchAsync();
        }

        private class GitHubFixture
        {
            public IGitHubRepository GitHubRepository => gitHubRepositoryMock.Object;
            private Mock<IGitHubRepository> gitHubRepositoryMock;

            private string inputOwner;
            private string inputRepo;
            private string inputBranchName;

            public void SetUpCreateBranchAsync(string owner, string repo, string branchName)
            {
                this.Setup();
                this.inputOwner = owner;
                this.inputRepo = repo;
                this.inputBranchName = branchName;
        
                gitHubRepositoryMock.Setup(p => p.DeleteBranchAsync(this.inputOwner, inputRepo, inputBranchName));

            }

            public void VerifyCreateBranchAsync()
            {
                gitHubRepositoryMock.Verify(p => p.DeleteBranchAsync(this.inputOwner, inputRepo, inputBranchName));
            }
            private void Setup()
            {
                gitHubRepositoryMock = new Mock<IGitHubRepository>();
            }
        }
    }

    
}
