using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIBindings
{
    public static class InjectWebJobsBuilderExtensions
    {
        public static IWebJobsBuilder AddInject(this IWebJobsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddExtension<InjectConfigurationProvider>();
            return builder;
        }
    }
}
