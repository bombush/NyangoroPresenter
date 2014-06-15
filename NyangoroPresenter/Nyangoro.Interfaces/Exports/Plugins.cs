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
        DependencyObject GetPresentationRoot();
        DependencyObject GetControlRoot();
        void SetHolderReference(Nyangoro.Interfaces.IPluginHolder holder);
        bool running { get; set; }
        bool displayed { get; set; }

        bool Init();
        void Display();
        void Run();
        void Standby();
        void Unload();
    }


    [InheritedExport(typeof(Nyangoro.Interfaces.IPluginHolder))]
    public interface IPluginHolder {
        IPlugin GetByType(string type);
    }
}

namespace Nyangoro.Plugins{

    abstract public class Plugin : Nyangoro.Interfaces.IPlugin
    {
        /*Services to be imported from the core*/
        protected Nyangoro.Interfaces.IServiceHolder services;

        /*Root element for the presentation control*/
        public DependencyObject presentationRoot { get; protected set; }
        /*Root element for the control panel control*/
        public DependencyObject controlRoot { get; protected set; }

        /*flag whether the plugin is currently running*/
        public bool running { get; set; }
        public bool displayed { get; set; }

        protected Nyangoro.Interfaces.IPluginHolder holder;

        /*constructor imports custom import parameter*/
        public Plugin()
        {
        }

        /*Plugin-specific init procedure*/
        public bool Init()
        {
            return true;
        }

        /*Plugin-specific run procedure*/
        public void Run()
        {
        }

        /*Plugin-specific display procedure*/
        public void Display()
        {
        }

        /*Plugin-specific standby procedure*/
        public void Standby()
        {
        }

        /*Plugin-specific unload procedure*/
        public void Unload()
        {
        }

        public DependencyObject GetPresentationRoot()
        {
            return this.presentationRoot;
        }

        public DependencyObject GetControlRoot()
        {
            return this.controlRoot;
        }

        public void SetHolderReference(Nyangoro.Interfaces.IPluginHolder holder)
        {
            this.holder = holder;
        }

    }
}

