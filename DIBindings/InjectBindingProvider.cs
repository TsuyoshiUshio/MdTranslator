using Microsoft.Azure.WebJobs.Host.Bindings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DIBindings
{
    public class InjectBindingProvider : IBindingProvider
    {
        public static readonly ConcurrentDictionary<Guid, IServiceScope> Scopes = new ConcurrentDictionary<Guid, IServiceScope>();

        private IServiceProvider serviceProvider;
        private ILoggerFactory _loggerFactory;
        public InjectBindingProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (this.serviceProvider == null)
            {
                this.serviceProvider = CreateServiceProvider(context.Parameter.Member.DeclaringType.Assembly);
            }
            IBinding binding = new InjectBinding(this.serviceProvider, context.Parameter.ParameterType);
            return Task.FromResult(binding);
        }
        private IServiceProvider CreateServiceProvider(Assembly assembly)
        {
            var builder = ServiceProviderBuilderHelper.GetBuilder(assembly);
            builder.LoggerFactory = _loggerFactory;
            return builder.BuildServiceProvider();
        }

    }
}
