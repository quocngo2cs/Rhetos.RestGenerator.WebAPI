using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using System.ComponentModel.Composition;
using Rhetos.Utilities;
using Rhetos.RestGenerator.Security;

namespace Rhetos.RestGenerator
{
    [Export(typeof(Module))]
    public class RestGeneratorModuleConfiguration : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<OwinUserInfo>().As<IUserInfo>().InstancePerLifetimeScope();
            Rhetos.Extensibility.Plugins.FindAndRegisterPlugins<IRestGeneratorPlugin>(builder);
           
            base.Load(builder);
        }
    }
}
