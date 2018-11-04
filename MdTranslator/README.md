# MdTranslator

Translate markdown document on your repository.

# Motivation

When I run the international event in Japan, we made a lot of effort to translate the contents into Japanese. 
I'd like to automate this effort by the power of the Durable Functions. 

# Outcome 

* Create a new branch 
* Translate all `.md` document into the target language

# Configration

Create `local.settings.json` with adding GitHubAccessToken and Cognitive Service Translator key here. For the FunctionApp, You need to add `GitHubToken` and `TranslatorKey` on your AppSettings on your FunctionApp.

```
{
    "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "GitHubToken": "YOUR_GITHUB_PERSONAL_ACCESS_TOKEN_HERE",
    "TranslatorKey": "YOUR_COGNITIVE_SERVICE_TRANSLATOR_KEY_HERE"
  }
}
```

