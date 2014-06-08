using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows;
using System.IO;
using System.Windows.Markup;

namespace Nyangoro.Core.Host
{
    /*
     * Layout manager takes care of integrating module outputs into the respective
     * windows.
     */
    [Export(typeof(Nyangoro.Core.Host.LayoutManager))]
    public class LayoutManager
    {
        //path to directory containing screens in XAML
        string screenPath = @"screens\";

        private Window controlWindow;
        private Window presentationWindow;
        private List<Nyangoro.Core.Layout.PresentationScreen> screens;
        private PluginManager pluginManager;
        public Nyangoro.Core.Layout.PresentationScreen activeScreen { get; private set; }
        
        public LayoutManager(Window controlWindow, Window presentationWindow)
        {
            this.controlWindow = controlWindow;
            this.presentationWindow = presentationWindow;
            this.screens = new List<Nyangoro.Core.Layout.PresentationScreen>();
        }

        public void InitLayout()
        {
            this.LoadScreensAvailable();
            this.DisplayDefaultScreen();
        }

        public void BuildLayout()
        {
        }

        private void LoadScreensAvailable()
        {
            if (!Directory.Exists(this.screenPath))
                throw new DirectoryNotFoundException("Screens directory not found");

            string[] files = Directory.GetFiles(this.screenPath, "*.xaml");
            if(files.Length == 0)
                throw new IOException("Screens directory is empty");

            foreach(string file in files){
                StreamReader sr = new StreamReader(file);
                FrameworkElement screenRoot = XamlReader.Load(sr.BaseStream) as FrameworkElement;
                Nyangoro.Core.Layout.PresentationScreen screen = new Nyangoro.Core.Layout.PresentationScreen(screenRoot);
                this.screens.Add(screen);
            }
        }

        private void DisplayDefaultScreen()
        {
            this.DisplayScreen(screens[0]);
        }

        private void RemoveCurrentScreen()
        {
            this.presentationWindow.Content = null;
        }

        public void DisplayPlugins(PluginHolder plugins)
        {
            this.activeScreen.DisplayPlugins(plugins);
        }

        public void SwitchToScreen(int screenIndex)
        {
            this.RemoveCurrentScreen();
            this.DisplayScreen(this.screens[screenIndex]);
        }

        private void DisplayScreen(Nyangoro.Core.Layout.PresentationScreen screen)
        {
            this.presentationWindow.Content = screen.rootElement;
            this.activeScreen = screen;
        }
    }
}
