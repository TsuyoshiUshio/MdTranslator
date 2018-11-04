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
            await service.CreateBranchAsync("TsuyoshiUshio", "TranslationTarget", "master", "jp");
            log.LogInformation("Delete master-jp repo done.");
            return "Hello";
        }
    }
}
