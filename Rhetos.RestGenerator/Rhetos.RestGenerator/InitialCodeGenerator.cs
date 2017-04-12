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
using Module = Autofac.Module;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.WebApiRestGenerator.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Web.Http;
using Autofac.Integration.WebApi;
using System.Reflection;
using Rhetos.Utilities;

namespace Rhetos.WebApiRest
{

    [System.ComponentModel.Composition.Export(typeof(Module))]
    public class AutofacModuleConfiguration : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServiceUtility>().InstancePerRequest();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            base.Load(builder);
        }
    }

    [RoutePrefix(""api/example/common"")]
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
            codeBuilder.AddReferencesFromDependency(typeof(Autofac.Builder.RegistrationBuilder));
            
            // Web Api
            codeBuilder.AddReference(Path.Combine(_rootPath, "System.Web.Http.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Autofac.Integration.WebApi.dll"));

            codeBuilder.AddReferencesFromDependency(typeof(System.Web.HttpApplication));
            codeBuilder.AddReferencesFromDependency(typeof(System.Net.Http.HttpMessageHandler));
            codeBuilder.AddReferencesFromDependency(typeof(System.Net.Http.Formatting.JsonMediaTypeFormatter));
            codeBuilder.AddReferencesFromDependency(typeof(Newtonsoft.Json.Serialization.DefaultContractResolver));
            codeBuilder.AddReferencesFromDependency(typeof(Newtonsoft.Json.JsonSerializerSettings));
            codeBuilder.AddReferencesFromDependency(typeof(Autofac.Integration.WebApi.AutofacWebApiDependencyResolver));
            codeBuilder.AddReferencesFromDependency(typeof(System.Reflection.Assembly));
            codeBuilder.AddReferencesFromDependency(typeof(Autofac.ContainerBuilder));

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

            codeBuilder.AddReference(Path.Combine(_rootPath, "ServerDom.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Autofac.dll"));
        }

    }

}
