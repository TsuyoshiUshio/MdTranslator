using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace DIBindings
{
    public interface IServiceProviderBuilder
    {
        IServiceProvider BuildServiceProvider();
        ILoggerFactory LoggerFactory { get; set; }
    }
}
