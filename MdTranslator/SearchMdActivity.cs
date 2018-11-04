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
        public static async Task<IEnumerable<string>> SearchMdActivityAsync([ActivityTrigger] string branchSha, [Inject] IGitHubService service, ILogger log)
        {
            return await service.SearchMdFilePaths("TsuyoshiUshio", "TranslationTarget", branchSha);
        }
    }
}
