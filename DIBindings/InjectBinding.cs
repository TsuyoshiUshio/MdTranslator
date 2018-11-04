using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace DIBindings
{
    internal class InjectBinding : IBinding
    {
        private readonly Type type;
        private readonly IServiceProvider serviceProvider;

        internal InjectBinding(IServiceProvider serviceProvider, Type type)
        {
            this.serviceProvider = serviceProvider;
            this.type = type;
        }

        public bool FromAttribute => true;

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context) =>
            Task.FromResult((IValueProvider)new InjectValueProvider(value));

        public async Task<IValueProvider> BindAsync(BindingContext context)
        {
            await Task.Yield();
            var scope = InjectBindingProvider.Scopes.GetOrAdd(context.FunctionInstanceId, (_) => this.serviceProvider.CreateScope());
            var value = scope.ServiceProvider.GetRequiredService(this.type);
            return await BindAsync(value, context.ValueContext);
        }

        public ParameterDescriptor ToParameterDescriptor() => new ParameterDescriptor();
        
        private class InjectValueProvider : IValueProvider
        {
            private readonly object value;
            public InjectValueProvider(object value) => this.value = value;
            public Type Type => this.value.GetType();
            public Task<object> GetValueAsync() => Task.FromResult(this.value);
            public string ToInvokeString() => this.value.ToString();
        }

    }
}
