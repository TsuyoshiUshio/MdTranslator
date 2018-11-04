using DIBindings;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using MdTranslatorLibrary;
using Microsoft.Extensions.DependencyInjection;

namespace MdTranslator
{
    public class ServiceProviderBuilder : IServiceProviderBuilder
    {
        public ILoggerFactory LoggerFactory
        {
            get; set;
        }

        public IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton<HttpClient, HttpClient>();
            services.AddSingleton<IGitHubContext, GitHubContext>();
            services.AddSingleton<IGitHubRepository, GitHubRepository>();
            services.AddSingleton<IGitHubService, GitHubService>();
            services.AddSingleton<ITranslatorContext, TranslatorContext>();
            services.AddSingleton<ITranslatorRepository, TranslatorRepository>();
            services.AddSingleton<ITranslatorService, TranslatorService>();
            return services.BuildServiceProvider(true);
        }
    }
}
