using System;
using Rhetos.Web;
using Rhetos.Compiler;
using Rhetos.Dsl;
using System.IO;
using System.Xml;
using System.Web.Http;
using System.Net;
using System.Runtime.Serialization;

namespace Rhetos.RestGenerator
{
    public class InitialCodeGenerator : IRestGeneratorPlugin
    {
        public const string RhetosRestClassesTag = "/*InitialCodeGenerator.RhetosRestClassesTag*/";
        public const string ServiceRegistrationTag = "/*InitialCodeGenerator.ServiceRegistrationTag*/";
        public const string ServiceInitializationTag = "/*InitialCodeGenerator.ServiceInitializationTag*/";

        private const string CodeSnippet =
@"
using Autofac;
using Module = Autofac.Module;
using Rhetos.Dom.DefaultConcepts;
//using Rhetos.RestGenerator.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Web.Http;

namespace Rhetos.Rest
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: ""DefaultApi"",
                routeTemplate: ""api/{controller}/{id}"",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }    

    public class RhetosRestGeneratorApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }

    [System.ComponentModel.Composition.Export(typeof(Module))]
    public class RestServiceModuleConfiguration : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<ServiceUtility>().InstancePerLifetimeScope();
            " + ServiceRegistrationTag + @"
            base.Load(builder);
        }
    }
    [RoutePrefix(""Example/Common"")]
    public class RestServiceExampleCommonController : ApiController
    {
        [HttpGet]
        [Route("""")]
        public string Get(string filter = null)
        {
            return filter;
        }
    }
}
";

        private static readonly string _rootPath = AppDomain.CurrentDomain.BaseDirectory;

        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            codeBuilder.InsertCode(CodeSnippet);

            // Global
            codeBuilder.AddReferencesFromDependency(typeof(Guid));
            codeBuilder.AddReferencesFromDependency(typeof(System.Linq.Enumerable));
            //codeBuilder.AddReferencesFromDependency(typeof(System.Configuration.ConfigurationElement));
            codeBuilder.AddReferencesFromDependency(typeof(System.Diagnostics.Stopwatch));
            codeBuilder.AddReferencesFromDependency(typeof(XmlReader));

            // Registration
            codeBuilder.AddReferencesFromDependency(typeof(System.ComponentModel.Composition.ExportAttribute));

            // Web Api
            codeBuilder.AddReference(Path.Combine(_rootPath, "System.Web.Http.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "System.Web.Http.WebHost.dll"));
            codeBuilder.AddReferencesFromDependency(typeof(System.Web.HttpApplication));
            codeBuilder.AddReferencesFromDependency(typeof(System.Net.Http.HttpMessageHandler));
            // Rhetos
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.IService));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.Dom.DefaultConcepts.IEntity));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.Logging.ILogger));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.Logging.LoggerHelper));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.Processing.IProcessingEngine));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.Utilities.XmlUtility));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.XmlSerialization.XmlData));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.Web.JsonErrorServiceBehavior));

            // RestGenerator
            //codeBuilder.AddReferencesFromDependency(typeof(Rhetos.RestGenerator.Utilities.ServiceUtility));

            //codeBuilder.AddReference(Path.Combine(_rootPath, "ServerDom.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Autofac.dll"));
        }

    }

}
