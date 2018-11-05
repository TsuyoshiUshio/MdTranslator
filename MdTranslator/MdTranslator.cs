using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MdTranslator
{
    public static class MdTranslator
    {
        [FunctionName("MdTranslator")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var outputs = new List<string>();
            // Create Branch
            var translationContext =
                await context.CallActivityAsync<OrchestrationContext>("CreateBranchActivity",
                    context.GetInput<OrchestrationContext>());
            // Search Md Files 
            translationContext = await context.CallActivityAsync<OrchestrationContext>("SearchMdActivity",
                translationContext);
            // Update All MdFiles
            var tasks = new List<Task<string>>();
            foreach (var path in translationContext.Paths)
            {
                tasks.Add(context.CallSubOrchestratorAsync<string>("TranslationSubOrchestrator", 
                    translationContext.ConvertSubOrchestrationContext(path)));
                // In case you've got throttling. You might get an conflict. It covered by retry.
                //var waitTime = context.CurrentUtcDateTime.Add(TimeSpan.FromSeconds(3));
                //await context.CreateTimer(waitTime, CancellationToken.None);
            }

            await Task.WhenAll(tasks);
            return tasks.Select(p => p.Result).ToList();
        }

        [FunctionName("TranslationSubOrchestrator")]
        public static async Task<string> TranslationSubOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var translationContext = context.GetInput<TranslationOrchestrationContext>();
            var retryOptions = new RetryOptions(TimeSpan.FromMinutes(1), 5);
            // Read Document
            translationContext =
                await context.CallActivityWithRetryAsync<TranslationOrchestrationContext>("ReadDocument", retryOptions,
                    translationContext);
            translationContext =
                await context.CallActivityWithRetryAsync<TranslationOrchestrationContext>("TranslateDocument",
                    retryOptions,
                    translationContext);
            await context.CallActivityWithRetryAsync("UpdateDocument", retryOptions, translationContext);
            return translationContext.Path;
        }


        [FunctionName("MdTranslator_Start")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            var body = await req.Content.ReadAsStringAsync();
            var context = JsonConvert.DeserializeObject<OrchestrationContext>(body);
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("MdTranslator", context);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            log.LogInformation($"Owner: {context.Owner} Repo: {context.Repo} SourceBranch: {context.SourceBranch} Language: {context.Language}");
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
