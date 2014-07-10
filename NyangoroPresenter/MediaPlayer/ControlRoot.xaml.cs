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

        public MediaPlayerController Controller { 
            get { return (MediaPlayerController)this.controller; } 
            private set { this.controller = value; } 
        }

        public ControlRoot()
        {
            InitializeComponent();
        }

        public ListBox GetPlaylistBox()
        {
            Grid rootGrid = (Grid)this.Content;
            Grid playlistGrid = (Grid)rootGrid.FindName("PlaylistGrid");

            return (ListBox)playlistGrid.FindName("PlaylistBox");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayerController controller = (MediaPlayerController)this.controller;
            controller.HandleButtonClick();
        }

        public void SetController(MediaPlayerController controller)
        {
            this.Controller = controller;
        }

        private void AddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            this.Controller.HandleAddToPlaylistClick();
        }

        private void PlaylistBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void PlaylistBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Controller.HandlePlaylistMouseDoubleClick();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            this.Controller.HandlePlayClick(sender, e);
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
