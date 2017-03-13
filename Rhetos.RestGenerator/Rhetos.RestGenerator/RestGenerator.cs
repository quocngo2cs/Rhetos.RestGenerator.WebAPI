using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Rhetos.Compiler;
using Rhetos.Extensibility;
using Rhetos.Logging;
using ICodeGenerator = Rhetos.Compiler.ICodeGenerator;

namespace Rhetos.RestGenerator
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
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Generated", "RestService.dll");
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
            Console.WriteLine(GetAssemblyPath());
            _assemblyGenerator.Generate(assemblySource, parameters);
        }

        public IEnumerable<string> Dependencies
        {
            get { return new[] { "" }; }
        }
    }
}
