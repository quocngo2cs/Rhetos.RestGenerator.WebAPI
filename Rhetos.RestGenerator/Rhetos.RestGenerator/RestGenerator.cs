using Rhetos.Compiler;
using Rhetos.Extensibility;
using Rhetos.Logging;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICodeGenerator = Rhetos.Compiler.ICodeGenerator;

namespace Rhetos.WebApiRestGenerator
{
    [Export(typeof(IGenerator))]
    public class RestGenerator : IGenerator
    {
        private readonly IPluginsContainer<IRestGeneratorPlugin> _plugins;
        private readonly ICodeGenerator _codeGenerator;
        private readonly IAssemblyGenerator _assemblyGenerator;
        private readonly ILogger _logger;
        private readonly ILogger _sourceLogger;

        public static string GetAssemblyPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Generated", "ApiService.dll");
        }

        public RestGenerator(
            IPluginsContainer<IRestGeneratorPlugin> plugins,
            ICodeGenerator codeGenerator,
            ILogProvider logProvider,
            IAssemblyGenerator assemblyGenerator
        )
        {
            _plugins = plugins;
            _codeGenerator = codeGenerator;
            _assemblyGenerator = assemblyGenerator;

            _logger = logProvider.GetLogger("RestGenerator");
            _sourceLogger = logProvider.GetLogger("Rest service");
        }

        public void Generate()
        {
            IAssemblySource assemblySource = _codeGenerator.ExecutePlugins(_plugins, "/*", "*/", new InitialCodeGenerator());
            _logger.Trace("References: " + string.Join(", ", assemblySource.RegisteredReferences));
            _sourceLogger.Trace(assemblySource.GeneratedCode);
            CompilerParameters parameters = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = false,
                OutputAssembly = GetAssemblyPath(),
                IncludeDebugInformation = true,
                CompilerOptions = ""
            };
            _assemblyGenerator.Generate(assemblySource, parameters);

            string sourceFile = GetAssemblyPath();
            string destinationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "ApiService.dll");
            File.Copy(sourceFile, destinationFile, true);
        }

        public IEnumerable<string> Dependencies
        {
            get { return new[] { "" }; }
        }
    }
}
