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
             context.SourceText =  await service.GetFileContents(context.Owner, context.Repo, context.TargetBranch, context.Path);
            return context;
        }
    }
}
