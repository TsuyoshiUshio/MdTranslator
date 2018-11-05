using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DIBindings;
using MdTranslatorLibrary;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace MdTranslator
{
    public static class UpdateDocumentActivity
    {
        [FunctionName("UpdateDocument")]
        public static async Task UpdateDocumentAsync([ActivityTrigger] TranslationOrchestrationContext context, [Inject] IGitHubService service, ILogger log)
        {
            await service.UpdateFileContentsAsync(context.Owner, context.Repo, context.Path, context.TargetBranch, context.TranslatedText,
                context.TargetSha, context.Language);
        }
    }
}
