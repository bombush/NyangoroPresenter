using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows;

namespace Nyangoro.Interfaces
{
    [InheritedExport(typeof(Nyangoro.Interfaces.IPlugin))]
    public interface IPlugin {
        bool Init();
        FrameworkElement getPresentationRoot();
        FrameworkElement getControlRoot();
        void Standby();
        bool isRunning();
        bool isDisplayed();
        void Unload();
        //screen element name to attach the plugin display root to 
        string getScreenAnchorPoint();
    }

    [InheritedExport(typeof(Nyangoro.Interfaces.IPluginHolder))]
    public interface IPluginHolder {
        IPlugin getByType(string type);
    }

    /*
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
    }*/
}
