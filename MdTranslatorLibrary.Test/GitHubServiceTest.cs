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

            var ExpectedTargetBranchName = $"{fixture.InputSourceBranch}-{fixture.InputLanguage}";
            var ExpectedSha = "quux";

            fixture.SetUpCreateBranchAsync(fixture.InputOwner, fixture.InputRepo, fixture.InputSourceBranch, ExpectedTargetBranchName, ExpectedSha);
            var service = new GitHubService(fixture.GitHubRepository);
            var result = await service.CreateBranchAsync(fixture.InputOwner, fixture.InputRepo, fixture.InputSourceBranch, fixture.InputLanguage);
            fixture.VerifyCreateBranchAsync();
            Assert.Equal(ExpectedSha, result.commit.sha);
        }

        [Fact]
        public async Task Update_File_Normal_Case()
        {
            var fixture = new GitHubFixture(); 
        }

        private class CommitSession : IDisposable
        {
            const string CommitNameKey = "CommitName";
            private const string CommitEmailKey = "CommitEmail";

            private string oldCommitName;
            private string oldCommitEmail;

            public CommitSession(string name, string email)
            {
                this.oldCommitName = Environment.GetEnvironmentVariable(CommitNameKey);
                this.oldCommitEmail = Environment.GetEnvironmentVariable(CommitEmailKey);

                Environment.SetEnvironmentVariable(CommitNameKey, name);
                Environment.SetEnvironmentVariable(CommitEmailKey, email);
            }

            public void Dispose()
            {
                Environment.SetEnvironmentVariable(CommitNameKey, oldCommitName);
                Environment.SetEnvironmentVariable(CommitEmailKey, oldCommitEmail);
            }
        }

        private class GitHubFixture
        {
            public IGitHubRepository GitHubRepository => gitHubRepositoryMock.Object;
            private Mock<IGitHubRepository> gitHubRepositoryMock;

            public FileOperation ActualFileOperation { get; set; }

            public  string InputOwner { get; set;}
            public string InputRepo { get; set; }
            public string InputTargetBranch { get; set; }
            public string InputSourceBranch { get; set; }
            public string InputLanguage { get; set; }
            public  string InputTargetSha { get; set; }

            private string intermediateSha;

            public GitHubFixture()
            {
                this.InputOwner = "foo";
                this.InputRepo = "bar";
                this.InputSourceBranch = "baz";
                this.InputLanguage = "qux";
            }
            public void SetUpCreateBranchAsync(string owner, string repo, string sourceBranch, string targetBranch, string targetSha)
            {
                this.Setup();
                this.InputOwner = owner;
                this.InputRepo = repo;
                this.InputSourceBranch = sourceBranch;
                this.InputTargetBranch = targetBranch;
                this.InputTargetSha = targetSha;
                //var targetBranch = $"{sourceBranch}-{language}";
                //await repository.DeleteBranchAsync(owner, repo, targetBranch);
                //var branch = await repository.GetBranchAsync(owner, repo, sourceBranch);
                //await repository.CreateBranchAsync(owner, repo, targetBranch, branch.commit.sha);
                //return await repository.GetBranchAsync(owner, repo, targetBranch);
                gitHubRepositoryMock.Setup(p => p.DeleteBranchAsync(InputOwner, InputRepo, InputTargetBranch)).Returns(Task.FromResult(""));
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
                        sha = InputTargetSha
                    }
                };
                gitHubRepositoryMock.Setup(p => p.GetBranchAsync(owner, repo, targetBranch)).ReturnsAsync(returnBranch);
            }

            public void VerifyCreateBranchAsync()
            {
                gitHubRepositoryMock.Verify(p => p.DeleteBranchAsync(this.InputOwner, InputRepo, InputTargetBranch));
                gitHubRepositoryMock.Setup(p => p.GetBranchAsync(InputOwner, InputRepo, InputSourceBranch));
                gitHubRepositoryMock.Setup(p =>
                    p.CreateBranchAsync(InputOwner, InputRepo, InputTargetBranch, intermediateSha));
            }

            public void SetupUpdateFileContents(string owner, string repo, string path)
            {
                gitHubRepositoryMock.Setup(p => p.UpdateFileContents(owner, repo, path, It.IsAny<FileOperation>()))
                    .Returns(Task.FromResult(""))
                    .Callback<string, string, string, FileOperation>((a, b, c, operation) =>
                        {
                            this.ActualFileOperation = operation;
                        });
            }

            private void Setup()
            {
                gitHubRepositoryMock = new Mock<IGitHubRepository>();
            }
        }
    }

    
}
