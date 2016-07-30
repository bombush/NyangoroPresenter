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
        void SetServicesReference(Nyangoro.Interfaces.IServiceHolder holder);
        bool running { get; set; }
        bool displayed { get; set; }

        bool Init();
        void Display();
        void Run();
        void Standby();
        void Unload();

        string GetName();
    }


    [InheritedExport(typeof(Nyangoro.Interfaces.IPluginHolder))]
    public interface IPluginHolder {
        IPlugin GetByType(string type);
        string GetPluginDir(IPlugin plugin);
    }

}

namespace Nyangoro.Plugins{
    
    abstract public class Plugin : Nyangoro.Interfaces.IPlugin
    {
        //@TODO: import services and holder using MEF constructor injection

        /*Plugin holder the plugin instance belongs to
         * Plugin holder should be the only way the plugin can interact
         * with application core
         */
        protected Nyangoro.Interfaces.IPluginHolder holder;

        /*
         * Core services holder
         * Core services are injected into the Plugin on holder Init()
         */
        protected Nyangoro.Interfaces.IServiceHolder coreServices;


        /*Root element for the presentation control*/
        public DependencyObject presentationRoot { get; protected set; }
        /*Root element for the control panel control*/
        public DependencyObject controlRoot { get; protected set; }

        /*flag whether the plugin is currently running*/
        public bool running { get; set; }
        public bool displayed { get; set; }

        /*Controller handles UI interaction*/
        protected PluginController controller;
        //Override with new to return the correct type in your plugin
        public PluginController Controller { get { return this.controller; } set { this.controller = value; } }

        protected string Name { get; set; }
        public string Dir { get; protected set; }

        //SERVICES REFERENCES, DIRS etc. move to Constructor
        /*constructor imports custom import parameter*/
        public Plugin()
        {
            this.SetName();
        }

        /*Plugin-specific init procedure*/
        public virtual bool Init()
        {
            this.SetDir();
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

        public virtual void SetServicesReference(Nyangoro.Interfaces.IServiceHolder holder)
        {
            this.coreServices = holder;
        }

        public string GetName()
        {
            return this.Name;
        }

        protected virtual void SetName()
        {
            //implement with the correct name of the plugin
        }

        //set the dir. Override if no dirname needed
        protected virtual void SetDir()
        {
            this.Dir = this.holder.GetPluginDir(this);
        }
    }

    
    abstract public class PluginController
    {
        protected PluginControlRoot controlRoot;
        protected PluginPresentationRoot presentationRoot;
        protected Nyangoro.Interfaces.IPlugin pluginCore;

        //Override with new to return the correct type
        public PluginControlRoot ControlRoot { get { return this.controlRoot; } set { this.controlRoot = value; } }
        public PluginPresentationRoot PresentationRoot { get { return this.presentationRoot; } set { this.presentationRoot = value; } }
        public Nyangoro.Interfaces.IPlugin PluginCore { get { return this.pluginCore; } set { this.pluginCore = value; } }

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
        protected PluginController controller;
        //Override with new to use the correct type in your plugin
        public PluginController Controller { get { return this.controller; } set { this.controller = value; } }
    }

    public class PluginPresentationRoot : UserControl
    {
        
    }
}

