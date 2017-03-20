using System;
using Rhetos.Web;
using Rhetos.Compiler;
using Rhetos.Dsl;
using System.IO;
using System.Xml;
using System.Web.Http;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel.Activation;
using System.Web.Routing;

namespace Rhetos.RestGenerator
{
    public class InitialCodeGenerator : IRestGeneratorPlugin
    {
        public const string RhetosRestClassesTag = "/*InitialCodeGenerator.RhetosRestClassesTag*/";
        //public const string ServiceRegistrationTag = "/*InitialCodeGenerator.ServiceRegistrationTag*/";
        //public const string ServiceInitializationTag = "/*InitialCodeGenerator.ServiceInitializationTag*/";

        private const string CodeSnippet =
@"
using Autofac;
using Module = Autofac.Module;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.RestGenerator.Utilities;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Web.Routing;
using System.Net.Http.Formatting;
using System.Web.Http;
using Autofac.Integration.Wcf;
using Autofac.Integration.WebApi;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;

namespace Rhetos.Rest
{
    [System.ServiceModel.ServiceContract]
    [System.ServiceModel.Activation.AspNetCompatibilityRequirements(RequirementsMode = System.ServiceModel.Activation.AspNetCompatibilityRequirementsMode.Allowed)]
    public class RestServiceTest
    {
        [OperationContract]
        [WebGet(UriTemplate=""/*"")]
        public string getStringTest()
        {
            IncomingWebRequestContext requestContext = WebOperationContext.Current.IncomingRequest;
            WebHeaderCollection header = requestContext.Headers;
            string cookies = header[HttpRequestHeader.Cookie];
            return ""abcd"";
        }
    }
    
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

    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings =
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: ""DefaultApi"",
                routeTemplate: ""api/{controller}/{id}"",
                defaults: new { id = RouteParameter.Optional });
            config.DependencyResolver = new AutofacWebApiDependencyResolver(AutofacServiceHostFactory.Container);
            appBuilder.UseWebApi(config);
        }
    }


    [System.ComponentModel.Composition.Export(typeof(Module))]
    public class RestServiceModuleConfiguration : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServiceUtility>().InstancePerLifetimeScope();
            builder.RegisterType<RestServiceTest>().InstancePerLifetimeScope();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            " + @"
            base.Load(builder);
        }
    }

    [System.ComponentModel.Composition.Export(typeof(Rhetos.IService))]
    public class RestServiceInitializer : Rhetos.IService
    {
        public void Initialize()
        {
            System.Web.Routing.RouteTable.Routes.Add(new System.ServiceModel.Activation.ServiceRoute(""Rest"", new WebServiceHostFactory(), typeof(RestServiceTest)));
            string baseAddress = ""http://localhost:9100/"";
            WebApp.Start<Startup>(url: baseAddress);
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        public void InitializeApplicationInstance(System.Web.HttpApplication context)
        {
        }
    }

    [RoutePrefix(""Example/Common"")]
    public class ExampleCommonController : ApiController
    {
        [HttpGet]
        [Route("""")]
        public string Get(string filter = null)
        {
            return filter;
        }
    }
" + RhetosRestClassesTag + @"
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

            //WCF
            codeBuilder.AddReferencesFromDependency(typeof(System.ServiceModel.ServiceContractAttribute));
            codeBuilder.AddReferencesFromDependency(typeof(System.ServiceModel.Activation.AspNetCompatibilityRequirementsAttribute));
            codeBuilder.AddReferencesFromDependency(typeof(System.ServiceModel.Web.WebServiceHost));
            codeBuilder.AddReferencesFromDependency(typeof(System.ServiceModel.Activation.WebServiceHostFactory));
            codeBuilder.AddReferencesFromDependency(typeof(System.Web.Routing.RouteTable));
            codeBuilder.AddReferencesFromDependency(typeof(Route));
            codeBuilder.AddReferencesFromDependency(typeof(System.ServiceModel.Web.IncomingWebRequestContext));
            codeBuilder.AddReferencesFromDependency(typeof(System.ServiceModel.Web.WebOperationContext));
            codeBuilder.AddReferencesFromDependency(typeof(System.Net.WebHeaderCollection));

            // Web Api
            codeBuilder.AddReference(Path.Combine(_rootPath, "Plugins", "System.Web.Http.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Plugins", "System.Web.Http.WebHost.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Plugins", "Owin.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Plugins", "Autofac.Integration.WebApi.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Autofac.Integration.Wcf.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Plugins", "Microsoft.Owin.Host.HttpListener.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Plugins", "Microsoft.Owin.dll"));
            //codeBuilder.AddReference(Path.Combine(_rootPath, "Plugins", "Microsoft.Owin.Security.Cookies.dll"));
            codeBuilder.AddReferencesFromDependency(typeof(System.Web.HttpApplication));
            codeBuilder.AddReferencesFromDependency(typeof(System.Net.Http.HttpMessageHandler));
            codeBuilder.AddReferencesFromDependency(typeof(System.Net.Http.Formatting.JsonMediaTypeFormatter));
            codeBuilder.AddReferencesFromDependency(typeof(Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver));
            codeBuilder.AddReferencesFromDependency(typeof(Newtonsoft.Json.JsonSerializerSettings));
            codeBuilder.AddReferencesFromDependency(typeof(Owin.WebApiAppBuilderExtensions));
            codeBuilder.AddReferencesFromDependency(typeof(Microsoft.Owin.Hosting.WebApp));
            codeBuilder.AddReferencesFromDependency(typeof(Autofac.Integration.Wcf.AutofacHostFactory));
            codeBuilder.AddReferencesFromDependency(typeof(Autofac.Integration.WebApi.AutofacWebApiDependencyResolver));
            codeBuilder.AddReferencesFromDependency(typeof(System.Reflection.Assembly));
            //codeBuilder.AddReferencesFromDependency(typeof(Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions));
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
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.RestGenerator.Utilities.ServiceUtility));

            codeBuilder.AddReference(Path.Combine(_rootPath, "ServerDom.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Autofac.dll"));
        }

    }

}