﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using System.ComponentModel.Composition;
using Rhetos.Utilities;

namespace Rhetos.WebApiRestGenerator
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
