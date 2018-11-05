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
    public static class TranslateDocumentActivity
    {
        [FunctionName("TranslateDocument")]
        public static async Task<TranslationOrchestrationContext> TranslateDocument([ActivityTrigger] TranslationOrchestrationContext context, [Inject] ITranslatorService service, ILogger log)
        {
            context.TranslatedText = await service.TranslateAsync(context.SourceText, context.Language);
            return context;
        }
    }
}
