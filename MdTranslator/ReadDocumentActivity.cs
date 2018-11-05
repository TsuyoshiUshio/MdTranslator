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
    public static class ReadDocumentActivity
    {
        [FunctionName("ReadDocument")]
        public static async Task<TranslationOrchestrationContext> ReadDocumentAsync([ActivityTrigger] TranslationOrchestrationContext context, [Inject] IGitHubService service, ILogger log)
        {
            var result =  await service.GetFileContents(context.Owner, context.Repo, context.TargetBranch, context.Path);
            context.TargetSha = result.Item1.sha; // Update sha with the target document.
            context.SourceText = result.Item2;
            return context;
        }
    }
}
