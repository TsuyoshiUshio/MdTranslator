using Microsoft.Azure.WebJobs.Description;
using System;

namespace DIBindings
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
    }
}
