using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
            var ExpectedTargetBranchName = $"{InputSourceBranch}-{InputLanguage}";
            var ExpectedSha = "quux";

            fixture.SetUpCreateBranchAsync(InputOwner, InputRepo, InputSourceBranch, ExpectedTargetBranchName, ExpectedSha);
            var service = new GitHubService(fixture.GitHubRepository);
            var result = await service.CreateBranchAsync(InputOwner, InputRepo, InputSourceBranch, InputLanguage);
            fixture.VerifyCreateBranchAsync();
            Assert.Equal(ExpectedSha, result.commit.sha);
        }

        private class GitHubFixture
        {
            public IGitHubRepository GitHubRepository => gitHubRepositoryMock.Object;
            private Mock<IGitHubRepository> gitHubRepositoryMock;

            private string inputOwner;
            private string inputRepo;
            private string inputTargetBranch;
            private string inputSourceBranch;
            private string inputTargetSha;
            private string intermediateSha;
            public void SetUpCreateBranchAsync(string owner, string repo, string sourceBranch, string targetBranch, string targetSha)
            {
                this.Setup();
                this.inputOwner = owner;
                this.inputRepo = repo;
                this.inputSourceBranch = sourceBranch;
                this.inputTargetBranch = targetBranch;
                this.inputTargetSha = targetSha;
                //var targetBranch = $"{sourceBranch}-{language}";
                //await repository.DeleteBranchAsync(owner, repo, targetBranch);
                //var branch = await repository.GetBranchAsync(owner, repo, sourceBranch);
                //await repository.CreateBranchAsync(owner, repo, targetBranch, branch.commit.sha);
                //return await repository.GetBranchAsync(owner, repo, targetBranch);
                gitHubRepositoryMock.Setup(p => p.DeleteBranchAsync(inputOwner, inputRepo, inputTargetBranch)).Returns(Task.FromResult(""));
                this.intermediateSha = "xahigehizhidhgex";
                var intermediateBranch = new Branch
                {
                    commit = new Commit()
                    {
                        sha = this.intermediateSha
                    }
                };
                gitHubRepositoryMock.Setup(p => p.GetBranchAsync(owner, repo, sourceBranch)).ReturnsAsync(intermediateBranch);
                gitHubRepositoryMock.Setup(p =>
                    p.CreateBranchAsync(owner, repo, targetBranch, intermediateBranch.commit.sha)).Returns(Task.FromResult(""));
                var returnBranch = new Branch
                {
                    commit = new Commit()
                    {
                        sha = inputTargetSha
                    }
                };
                gitHubRepositoryMock.Setup(p => p.GetBranchAsync(owner, repo, targetBranch)).ReturnsAsync(returnBranch);
            }

            public void VerifyCreateBranchAsync()
            {
                gitHubRepositoryMock.Verify(p => p.DeleteBranchAsync(this.inputOwner, inputRepo, inputTargetBranch));
                gitHubRepositoryMock.Setup(p => p.GetBranchAsync(inputOwner, inputRepo, inputSourceBranch));
                gitHubRepositoryMock.Setup(p =>
                    p.CreateBranchAsync(inputOwner, inputRepo, inputTargetBranch, intermediateSha));
            }
            private void Setup()
            {
                gitHubRepositoryMock = new Mock<IGitHubRepository>();
            }
        }
    }

    
}
