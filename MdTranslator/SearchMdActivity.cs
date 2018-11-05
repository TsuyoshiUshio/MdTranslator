using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DIBindings;
using MdTranslatorLibrary;
using Microsoft.Extensions.Logging;

namespace MdTranslator
{
    public static class SearchMdActivity
    {
        [FunctionName("SearchMdActivity")]
        public static async Task<OrchestrationContext> SearchMdActivityAsync([ActivityTrigger] OrchestrationContext context, [Inject] IGitHubService service, ILogger log)
        {
            var paths = await service.SearchMdFilePaths(context.Owner, context.Repo, context.TargetSha);
            context.Paths = paths;
            return context;
        }
    }
}
