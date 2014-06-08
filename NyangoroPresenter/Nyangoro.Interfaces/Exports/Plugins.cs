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
        FrameworkElement GetPresentationRoot();
        FrameworkElement GetControlRoot();
        void Run();
        void Display();
        void Standby();
        bool IsRunning();
        bool IsDisplayed();
        void Unload();
    }

    [InheritedExport(typeof(Nyangoro.Interfaces.IPluginHolder))]
    public interface IPluginHolder {
        IPlugin GetByType(string type);
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
