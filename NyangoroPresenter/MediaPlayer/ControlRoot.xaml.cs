using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nyangoro.Plugins.MediaPlayer
{
    /// <summary>
    /// Interaction logic for ControlRoot.xaml
    /// </summary>
    public partial class ControlRoot : PluginControlRoot
    {

        public ControlRoot()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayerController controller = (MediaPlayerController)this.controller;
            controller.HandleButtonClick();
        }

        public void SetController(MediaPlayerController controller){
            this.controller = (MediaPlayerController)controller;
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
