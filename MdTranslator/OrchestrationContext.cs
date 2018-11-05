using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MdTranslator
{
    public class OrchestrationContext
    {
        [JsonProperty("owner")]
        public string Owner { get; set; }
        [JsonProperty("repo")]
        public string Repo { get; set; }
        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("sourceBranch")]
        public string SourceBranch { get; set; }
        [JsonProperty("sourceSha")]
        public string SourceSha { get; set; }
        [JsonProperty("targetBranch")]
        public string TargetBranch { get; set; }
        [JsonProperty("targetSha")]
        public string TargetSha { get; set; }
        [JsonProperty("paths")]
        public IEnumerable<string> Paths { get; set; }
    }

    public class TranslationOrchestrationContext
    {
        [JsonProperty("owner")]
        public string Owner { get; set; }
        [JsonProperty("repo")]
        public string Repo { get; set; }
        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("sourceBranch")]
        public string SourceBranch { get; set; }
        [JsonProperty("sourceSha")]
        public string SourceSha { get; set; }
        [JsonProperty("targetBranch")]
        public string TargetBranch { get; set; }
        [JsonProperty("targetSha")]
        public string TargetSha { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("sourceText")]
        public string SourceText { get; set; }
        [JsonProperty("translatedText")]
        public string TranslatedText { get; set; }

    }

    public static class OrchestrationContextExtension
    {
        public static TranslationOrchestrationContext ConvertSubOrchestrationContext(this OrchestrationContext context, string path)
        {
            return new TranslationOrchestrationContext
            {
                Owner = context.Owner,
                Repo = context.Repo,
                Language = context.Language,
                SourceBranch = context.SourceBranch,
                SourceSha = context.SourceSha,
                TargetBranch = context.TargetBranch,
                TargetSha = context.TargetSha,
                Path = path
            };
        }
    }
}
