using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

/**
 * Standard plugin lifetime: Init();Display();Run();
 */
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

        /*Plugin holder the plugin instance belongs to
         * Plugin holder should be the only way the plugin can interact
         * with application core
         */
        protected Nyangoro.Interfaces.IPluginHolder holder;

        /*constructor imports custom import parameter*/
        public Plugin()
        {
        }

        /*Plugin-specific init procedure*/
        public virtual bool Init()
        {
            return true;
        }

        /*Plugin-specific run procedure*/
        public virtual void Run()
        {
        }

        /*Plugin-specific display procedure*/
        public virtual void Display()
        {
        }

        /*Plugin-specific standby procedure*/
        public virtual void Standby()
        {
        }

        /*Plugin-specific unload procedure*/
        public virtual void Unload()
        {
        }

        public virtual DependencyObject GetPresentationRoot()
        {
            return this.presentationRoot;
        }

        public virtual DependencyObject GetControlRoot()
        {
            return this.controlRoot;
        }

        public virtual void SetHolderReference(Nyangoro.Interfaces.IPluginHolder holder)
        {
            this.holder = holder;
        }

    }

    
    abstract public class PluginController
    {
        protected PluginControlRoot controlRoot;
        protected PluginPresentationRoot presentationRoot;
        protected Nyangoro.Interfaces.IPlugin pluginCore;

        public PluginController(Nyangoro.Interfaces.IPlugin pluginCore, PluginControlRoot controlRoot, PluginPresentationRoot presentationRoot)
        {
            this.pluginCore = pluginCore;
            this.controlRoot = controlRoot;
            this.presentationRoot = presentationRoot;
        }
    }

    /*
     * @TODO: make movable using Thumb (Expected Natsu 2025...)
     */
    public class PluginControlRoot : UserControl
    {
        /**
        * PROBLEM: when using derived class, fields deriving from PluginControlRoot get downcasted
        * meaning using new methods in the derived class requires another cast
        */
        public PluginController controller { get; set; }
    }

    public class PluginPresentationRoot : UserControl
    {
        
    }
}

