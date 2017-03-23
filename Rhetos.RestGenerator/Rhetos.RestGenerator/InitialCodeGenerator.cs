using System;
using System.IO;
using System.Web.Routing;
using System.Xml;
using Rhetos.Compiler;
using Rhetos.Dsl;

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
using System.Security.Claims;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Security.Principal;

namespace Rhetos.WebApiRest
{
    public class CustomTicketDataFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly TicketDataFormat innerDataFormat;

        public CustomTicketDataFormat(IDataProtector protector)
        {
            innerDataFormat = new TicketDataFormat(protector);
        }

        string ISecureDataFormat<AuthenticationTicket>.Protect(AuthenticationTicket data)
        {
            var output = innerDataFormat.Protect(data);
            return output;
        }

        AuthenticationTicket ISecureDataFormat<AuthenticationTicket>.Unprotect(string protectedText)
        {
            FormsAuthenticationTicket ticket;
            try
            {
                ticket = FormsAuthentication.Decrypt(protectedText);
            }
            catch
            {
                return null;
            }

            if (ticket == null)
            {
                return null;
            }
            var authProperties = new AuthenticationProperties()
            {
                ExpiresUtc = ticket.Expiration.ToUniversalTime(),
                IsPersistent = ticket.IsPersistent,
                IssuedUtc = ticket.IssueDate.ToUniversalTime()
            };
            var identities = new[]
            {
                new Claim(ClaimTypes.Name, ticket.Name)
            };

            var identity = new ClaimsIdentity(identities,""ApplicationCookie"");

            var authTicket = new AuthenticationTicket(identity, authProperties);

            return authTicket;
        }
    }

    public class WebApiCookieAuthentication : CookieAuthenticationProvider
    {
        public override Task ValidateIdentity(CookieValidateIdentityContext context)
        {
            var cookies = context.OwinContext.Request.Cookies;
            return base.ValidateIdentity(context);
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
                ContractResolver = new DefaultContractResolver()
            };
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: ""DefaultApi"",
                routeTemplate: ""api/{controller}/{id}"",
                defaults: new { id = RouteParameter.Optional });
            
            config.DependencyResolver = new AutofacWebApiDependencyResolver(AutofacServiceHostFactory.Container);

            IDataProtector dataProtector = appBuilder.CreateDataProtector(
                    ""SampleApplicationDataProtector"",
                    ""ApplicationCookie"", ""v1"");

            var ticketDataFormat = new CustomTicketDataFormat(dataProtector);

            appBuilder.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = ""ApplicationCookie"",
                Provider = new WebApiCookieAuthentication(),
                TicketDataFormat = ticketDataFormat,
                CookieName = "".ASPXAUTH""
            });
            appBuilder.UseWebApi(config);
        }
    }


    [System.ComponentModel.Composition.Export(typeof(Module))]
    public class RestServiceModuleConfiguration : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServiceUtility>().InstancePerLifetimeScope();
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
            System.Web.Routing.RouteTable.Routes.Add(new MatchAllPrefixRoute(""REST"", new WebAPIRestRouteHandler()));
            string baseAddress = ""http://localhost:9100/"";
            WebApp.Start<Startup>(url: baseAddress);
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        public void InitializeApplicationInstance(System.Web.HttpApplication context)
        {
        }
    }

    [RoutePrefix(""Rest/Example/Common"")]
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
            codeBuilder.AddReferencesFromDependency(typeof(System.Web.HttpContext));
            codeBuilder.AddReferencesFromDependency(typeof(System.Net.Http.HttpClient));
            codeBuilder.AddReferencesFromDependency(typeof(System.Net.Http.HttpClientHandler));
            codeBuilder.AddReferencesFromDependency(typeof(System.Net.Http.HttpRequestMessage));
            codeBuilder.AddReferencesFromDependency(typeof(System.Net.Http.HttpMethod));
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
            codeBuilder.AddReferencesFromDependency(typeof(Owin.WebApiAppBuilderExtensions));
            codeBuilder.AddReferencesFromDependency(typeof(Microsoft.Owin.Hosting.WebApp));
            codeBuilder.AddReferencesFromDependency(typeof(Autofac.Integration.Wcf.AutofacHostFactory));
            codeBuilder.AddReferencesFromDependency(typeof(Autofac.Integration.WebApi.AutofacWebApiDependencyResolver));
            codeBuilder.AddReferencesFromDependency(typeof(System.Reflection.Assembly));
            codeBuilder.AddReferencesFromDependency(typeof(Microsoft.Owin.Security.Cookies.CookieAuthenticationProvider));
            codeBuilder.AddReferencesFromDependency(typeof(Microsoft.Owin.Security.AuthenticationTicket));
            codeBuilder.AddReferencesFromDependency(typeof(Microsoft.Owin.Security.DataProtection.IDataProtector));
            codeBuilder.AddReferencesFromDependency(typeof(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter));
            codeBuilder.AddReferencesFromDependency(typeof(System.Web.Security.FormsAuthenticationTicket));
            codeBuilder.AddReferencesFromDependency(typeof(System.Security.Claims.ClaimsIdentity));
            codeBuilder.AddReferencesFromDependency(typeof(System.Security.Claims.Claim));

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
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.RestGenerator.Utilities.WebAPIRestRouteHandler));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.RestGenerator.Utilities.MatchAllPrefixRoute));

            codeBuilder.AddReference(Path.Combine(_rootPath, "ServerDom.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Autofac.dll"));
        }

    }

}
