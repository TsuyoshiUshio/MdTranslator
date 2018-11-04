using System;
using System.Collections.Generic;
using System.Text;
using DIBindings;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(InjectWebJobsStartup))]

namespace DIBindings
{
    public class InjectWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddInject();
        }
    }
}
