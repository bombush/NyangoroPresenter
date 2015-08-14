using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        public Nyangoro.Core.Layout.PresentationScreen activeScreen { get; private set; }
        public Nyangoro.Core.Layout.ControlScreen controlScreen { get; private set; }
        
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
            this.DisplayControlScreen();
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
            this.DisplayControls(plugins);
        }

        public void SwitchToScreen(int screenIndex)
        {
            this.RemoveCurrentScreen();
            this.DisplayScreen(this.screens[screenIndex]);
        }

        //plz REFACTOR!!!
        private void DisplayScreen(Nyangoro.Core.Layout.PresentationScreen screen)
        {
            Viewbox presentationContentRoot = (Viewbox)presentationWindow.Content;
            presentationContentRoot.Child = (FrameworkElement)screen.rootElement;

            Grid screenRoot = (Grid)screen.rootElement;

            try
            {
                //works with 2015 layout
                //Image natsuLogo = (Image)screenRoot.FindName("Natsulogo");
                //string uri = (Path.Combine(Config.Get("working_dir"), this.screenPath, "images", "natsulogo.png"));
                //natsuLogo.Source = new BitmapImage(new Uri(Path.Combine(Config.Get("working_dir"), this.screenPath, "images", "natsulogo.png")));

                //Image butaneko = (Image)screenRoot.FindName("Butaneko");
                //string uri = (Path.Combine(Config.Get("working_dir"), this.screenPath, "images", "natsulogo.png"));
                //butaneko.Source = new BitmapImage(new Uri(Path.Combine(Config.Get("working_dir"), this.screenPath, "images", "butaneko.png")));


                //works with 2015 layout. 2015 hackish background!!! yay!!!
                ImageBrush myBrush = new ImageBrush();
                Image image = new Image();
                image.Source = new BitmapImage(
                    new Uri(Path.Combine(Config.Get("working_dir"), this.screenPath, "images", "2015_background.png")));
                myBrush.ImageSource = image.Source;
                screenRoot.Background = myBrush; 
   
                //2015 image hack. yay!!!
                Image gundamneko = (Image)screenRoot.FindName("Gundamneko");
                //string uri = (Path.Combine(Config.Get("working_dir"), this.screenPath, "images", "natsulogo.png"));
                gundamneko.Source = new BitmapImage(new Uri(Path.Combine(Config.Get("working_dir"), this.screenPath, "images", "gundamneko2.png")));
            }
            catch {
                MessageBox.Show("Failed to load screen overlay picture");
            }

            this.activeScreen = screen;
        }

        private void DisplayControls(PluginHolder holder)
        {
            foreach(Nyangoro.Interfaces.IPlugin plugin in holder.members){
                this.controlScreen.DisplayPlugin(plugin);
            }
        }

        private void DisplayControlScreen()
        {
            Panel controlRoot = this.controlWindow.Content as Panel;
            if (controlRoot == null)
                throw new Exception("No control root found");

            this.controlScreen = new Layout.ControlScreen(controlRoot);
        }
    }
}
