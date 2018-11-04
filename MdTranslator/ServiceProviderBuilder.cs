using DIBindings;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MdTranslator
{
    public class ServiceProviderBuilder : IServiceProviderBuilder
    {
        public ILoggerFactory LoggerFactory
        {
            get; set; }

        public IServiceProvider BuildServiceProvider()
        {
            throw new NotImplementedException();
        }
    }
}
