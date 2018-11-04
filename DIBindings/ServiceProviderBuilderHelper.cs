using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;

namespace DIBindings
{
    internal static class ServiceProviderBuilderHelper
    {
        internal static IServiceProviderBuilder GetBuilder(Assembly assembly)
        {
            var builderType = typeof(IServiceProviderBuilder);
            var builder = assembly.GetExportedTypes().Single(t => builderType.IsAssignableFrom(t));
            return (IServiceProviderBuilder)Activator.CreateInstance(builder);
        }
    }
}
