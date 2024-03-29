using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using DocumentEditor.Web.Infrastructure;
using Microsoft.AspNet.SignalR;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Database.Server;

[assembly: WebActivator.PreApplicationStartMethod(typeof(DocumentEditor.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(DocumentEditor.Web.App_Start.NinjectWebCommon), "Stop")]

namespace DocumentEditor.Web.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var documentStore = new EmbeddableDocumentStore
                {
                    UseEmbeddedHttpServer = true,
                    DataDirectory = "App_Data",
                    Configuration =
                        {
                            Port = 12345,
                        },
                    Conventions =
                        {
                            CustomizeJsonSerializer = MvcApplication.SetupSerializer
                        }
                };
            documentStore.Initialize();
            var manager = new SubscriptionManager(documentStore);

            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            kernel.Bind<IDocumentStore>()
                  .ToMethod(context => documentStore)
                  .InSingletonScope();
            RegisterServices(kernel);
            kernel.Bind<SubscriptionManager>().ToMethod(context => manager).InSingletonScope();
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            GlobalHost.DependencyResolver = new SignalRDependencyResolver(kernel);

            GlobalConfiguration.Configuration.Services.Replace(
                typeof (IHttpControllerActivator),
                new NinjectControllerActivator(kernel));
        }
    }
}
