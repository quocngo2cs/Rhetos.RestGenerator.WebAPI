
using Autofac;
using System.ComponentModel.Composition;
namespace Rhetos.RestGenerator
{
    [Export(typeof(Module))]
    public class RestGeneratorModuleConfiguration : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            Rhetos.Extensibility.Plugins.FindAndRegisterPlugins<IRestGeneratorPlugin>(builder);

            base.Load(builder);
        }
    }
}
