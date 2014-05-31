using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Nyangoro.Interfaces
{
    [InheritedExport(typeof(Nyangoro.Interfaces.IPlugin))]
    public interface IPlugin {
        public bool Init();
        public Grid getPresentationRoot();
        public Grid getControlRoot();
        public void Standby();
        public void Unload();
    }

    [InheritedExport(typeof(Nyangoro.Interfaces.IPluginHolder))]
    public interface IPluginHolder {
        public IPlugin getByType(string type);
    }

    abstract class Plugin : IPlugin
    {
        //[Import(typeof(Nyangoro.Interfaces.IService))]
        //private IService services;

        Plugin()
        {
        }

        //init windows
        //send reference to this to windows
        //PluginControl class
    }
}
