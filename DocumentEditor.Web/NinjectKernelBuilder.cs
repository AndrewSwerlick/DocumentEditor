using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Raven.Client;
using Raven.Client.Embedded;

namespace DocumentEditor.Web
{
    public static class NinjectKernelBuilder
    {
        public static IKernel BuildKernal()
        {
            IKernel kernel = new StandardKernel();
            kernel.Bind<IDocumentStore>()
           .ToMethod(context =>
           {
               var documentStore = new EmbeddableDocumentStore { DataDirectory = "App_Data", UseEmbeddedHttpServer = true, };
               return documentStore.Initialize();
           })
           .InSingletonScope();

            return kernel;
        }
    }
}