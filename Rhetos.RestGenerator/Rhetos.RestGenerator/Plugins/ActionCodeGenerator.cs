
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Rhetos.Compiler;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;
using Rhetos.Extensibility;

namespace Rhetos.WebApiRestGenerator.Plugins
{
    [Export(typeof(IRestGeneratorPlugin))]
    [ExportMetadata(MefProvider.Implements, typeof(ActionInfo))]
    public class ActionCodeGenerator : IRestGeneratorPlugin
    {
        
        private static string ServiceDefinitionCodeSnippet(ActionInfo info)
        {
            return String.Format(
@"
    
    [RoutePrefix(""api/{0}/{1}"")]
    public class {0}{1}Controller : ApiController
    {{
        private ServiceUtility _serviceUtility;

        public {0}{1}Controller(ServiceUtility serviceUtility) 
        {{
            _serviceUtility = serviceUtility;
        }}

        [HttpPost]
        [Route("""")]
        public void Execute{0}{1}({0}.{1} action)
        {{
            _serviceUtility.Execute<{0}.{1}>(action);
        }}
    }}

", info.Module.Name, info.Name);
        }

        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            var info = (ActionInfo)conceptInfo;

            //codeBuilder.InsertCode(ServiceRegistrationCodeSnippet(info), InitialCodeGenerator.ServiceRegistrationTag);
            //codeBuilder.InsertCode(ServiceInitializationCodeSnippet(info), InitialCodeGenerator.ServiceInitializationTag);
            codeBuilder.InsertCode(ServiceDefinitionCodeSnippet(info), InitialCodeGenerator.RhetosRestClassesTag);
        }
    }
}
