using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Ninject;

namespace DocumentEditor.Web.Infrastructure
{
    public class SignalRDependencyResolver : DefaultDependencyResolver
    {
        private readonly IKernel _kernel;

        public SignalRDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override object GetService(Type serviceType)
        {
            var service = base.GetService(serviceType) ?? _kernel.Get(serviceType);
            return service;
        }
    }
}