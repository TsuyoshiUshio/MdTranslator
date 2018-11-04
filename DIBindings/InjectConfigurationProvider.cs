using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Config;
using System.Reflection;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Extensions.Logging;

namespace DIBindings
{
    [Extension("Inject")]
    public class InjectConfigurationProvider : IExtensionConfigProvider
    {
        private ILoggerFactory _loggerFactory;
        public InjectConfigurationProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            context
                .AddBindingRule<InjectAttribute>()
                .Bind(new InjectBindingProvider(_loggerFactory));
            var filter = new ScopeCleanupFilter();
            var registry = context.GetExtensionRegistry();
            registry.RegisterExtension(typeof(IFunctionInvocationFilter), filter);
            registry.RegisterExtension(typeof(IFunctionExceptionFilter), filter);
        }
    }

    public static class ExtensionConfigContextExtensions
    {
        public static IExtensionRegistry GetExtensionRegistry(this ExtensionConfigContext context)
        {
            // private readonly IExtensionRegistry _extensionRegistry;
            var field = context.GetType().GetField("_extensionRegistry",
                BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
            return (IExtensionRegistry) field.GetValue(context);
        }
    }
}
