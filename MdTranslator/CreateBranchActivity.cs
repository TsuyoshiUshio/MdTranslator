using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DIBindings;
using MdTranslatorLibrary;

namespace MdTranslator
{
    public static class CreateBranchActivity
    {
        [FunctionName("CreateBranchActivity")]
        public static async Task<OrchestrationContext> CreateBranchActivityAsync([ActivityTrigger] OrchestrationContext context, [Inject] IGitHubService service, ILogger log)
        {
            var branch = await service.CreateBranchAsync(context.Owner, context.Repo, context.SourceBranch, context.Language);
            context.TargetSha = branch.commit.sha;
            context.TargetBranch = $"{context.SourceBranch}-{context.Language}";
            log.LogInformation($"Created the {context.Repo}-{context.Language} repo done.");
            return context;
        }
    }
}
