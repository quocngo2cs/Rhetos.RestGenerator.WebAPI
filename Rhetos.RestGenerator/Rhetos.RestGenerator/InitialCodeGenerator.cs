using System;
using System.IO;
using System.Web.Routing;
using System.Xml;
using Rhetos.Compiler;
using Rhetos.Dsl;

namespace Rhetos.WebApiRestGenerator
{
    public class InitialCodeGenerator : IRestGeneratorPlugin
    {
        public const string RhetosRestClassesTag = "/*InitialCodeGenerator.RhetosRestClassesTag*/";
        //public const string ServiceRegistrationTag = "/*InitialCodeGenerator.ServiceRegistrationTag*/";
        //public const string ServiceInitializationTag = "/*InitialCodeGenerator.ServiceInitializationTag*/";

        private const string CodeSnippet =
@"
using Autofac;
using Autofac.Builder;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.WebApiRestGenerator.Utilities;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System.Threading.Tasks;
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
using System.Net.Http;
using System.Web.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using System.Runtime.Serialization.Formatters.Binary;
using Rhetos.Utilities;
using Rhetos.WebApiRestGenerator.Security;
using Rhetos.WebApiRestGenerator.Utilities;
namespace Rhetos.WebApiRest
{
    
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
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
                ContractResolver = new DefaultContractResolver()
            };
            config.MapHttpAttributeRoutes();
            
            var scope = AutofacHostFactory.Container.BeginLifetimeScope(builder =>
            {
                builder.RegisterType<ServiceUtility>().InstancePerRequest();
                builder.RegisterType<OwinUserInfo>().As<IUserInfo>().InstancePerRequest();
                builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            });
            config.DependencyResolver = new AutofacWebApiDependencyResolver(scope);
            
            IDataProtector dataProtector = appBuilder.CreateDataProtector(
                    ""SampleApplicationDataProtector"",
                    ""ApplicationCookie"", ""v1"");

            var ticketDataFormat = new CustomTicketDataFormat(dataProtector);

            appBuilder.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = ""ApplicationCookie"",
                TicketDataFormat = ticketDataFormat,
                CookieName = "".ASPXAUTH""
            });
            appBuilder.UseWebApi(config);
        }
    }

    [System.ComponentModel.Composition.Export(typeof(Rhetos.IService))]
    public class RestServiceInitializer : Rhetos.IService
    {
        public void Initialize()
        {
            System.Web.Routing.RouteTable.Routes.Add(new MatchAllPrefixRoute(""REST"", new WebAPIRestRouteHandler()));
            string baseAddress = ""http://"" + Configurations.WebApiHost + "":"" + Configurations.WebApiPort + ""/"";
            WebApp.Start<Startup>(url: baseAddress);
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        public void InitializeApplicationInstance(System.Web.HttpApplication context)
        {
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
            codeBuilder.AddReferencesFromDependency(typeof(System.Diagnostics.Stopwatch));
            codeBuilder.AddReferencesFromDependency(typeof(XmlReader));

            // Registration
            codeBuilder.AddReferencesFromDependency(typeof(System.ComponentModel.Composition.ExportAttribute));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.Utilities.IUserInfo));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.WebApiRestGenerator.Security.OwinUserInfo));
            codeBuilder.AddReferencesFromDependency(typeof(Autofac.Builder.RegistrationBuilder));

            //WCF
            codeBuilder.AddReferencesFromDependency(typeof(System.ServiceModel.ServiceContractAttribute));
            codeBuilder.AddReferencesFromDependency(typeof(System.ServiceModel.Activation.AspNetCompatibilityRequirementsAttribute));
            codeBuilder.AddReferencesFromDependency(typeof(System.ServiceModel.Web.WebServiceHost));
            codeBuilder.AddReferencesFromDependency(typeof(System.ServiceModel.Activation.WebServiceHostFactory));
            codeBuilder.AddReferencesFromDependency(typeof(System.Web.Routing.RouteTable));
            codeBuilder.AddReferencesFromDependency(typeof(Route));
            // Web Api
            codeBuilder.AddReference(Path.Combine(_rootPath, "Plugins", "System.Web.Http.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Plugins", "System.Web.Http.WebHost.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Plugins", "Owin.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Plugins", "Autofac.Integration.WebApi.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Autofac.Integration.Wcf.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Plugins", "Microsoft.Owin.Host.HttpListener.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Plugins", "Microsoft.Owin.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Plugins", "Microsoft.Owin.Security.Cookies.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Plugins", "Microsoft.Owin.Security.dll"));
            codeBuilder.AddReferencesFromDependency(typeof(System.Web.HttpApplication));
            codeBuilder.AddReferencesFromDependency(typeof(System.Net.Http.HttpMessageHandler));
            codeBuilder.AddReferencesFromDependency(typeof(System.Net.Http.Formatting.JsonMediaTypeFormatter));
            codeBuilder.AddReferencesFromDependency(typeof(Newtonsoft.Json.Serialization.DefaultContractResolver));
            codeBuilder.AddReferencesFromDependency(typeof(Newtonsoft.Json.JsonSerializerSettings));
            codeBuilder.AddReferencesFromDependency(typeof(Microsoft.Owin.Hosting.WebApp));
            codeBuilder.AddReferencesFromDependency(typeof(Microsoft.Owin.Security.DataProtection.IDataProtector));
            codeBuilder.AddReferencesFromDependency(typeof(Autofac.Integration.WebApi.AutofacWebApiDependencyResolver));
            codeBuilder.AddReferencesFromDependency(typeof(System.Reflection.Assembly));
            codeBuilder.AddReferencesFromDependency(typeof(System.Web.Security.FormsAuthenticationTicket));
            codeBuilder.AddReferencesFromDependency(typeof(Autofac.ContainerBuilder));
            codeBuilder.AddReferencesFromDependency(typeof(Owin.WebApiAppBuilderExtensions));

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
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.WebApiRestGenerator.Utilities.ServiceUtility));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.WebApiRestGenerator.Utilities.WebAPIRestRouteHandler));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.WebApiRestGenerator.Utilities.MatchAllPrefixRoute));

            codeBuilder.AddReference(Path.Combine(_rootPath, "ServerDom.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Autofac.dll"));
        }

    }

}
