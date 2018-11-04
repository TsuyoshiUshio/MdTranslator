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
        public static async Task<string> CreateBranchActivityAsync([ActivityTrigger] string name, [Inject] IGitHubService service, ILogger log)
        {
            var branch = await service.CreateBranchAsync("TsuyoshiUshio", "TranslationTarget", "master", "ja");
            log.LogInformation("Create master-ja repo done.");
            return branch.commit.sha;
        }
    }
}
