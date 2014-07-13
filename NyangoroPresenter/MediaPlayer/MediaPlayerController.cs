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
using Microsoft.Win32;
using Nyangoro.Plugins;
using Nyangoro.Interfaces;

namespace Nyangoro.Plugins.MediaPlayer
{
    public class MediaPlayerController : PluginController
    {
        protected Nyangoro.Plugins.MediaPlayer.ControlRoot ControlRoot
        { 
            get { return (Nyangoro.Plugins.MediaPlayer.ControlRoot)this.controlRoot; }
            set { this.controlRoot = value; }
        }

        protected Nyangoro.Plugins.MediaPlayer.PresentationRoot PresentationRoot
        {
            get { return (Nyangoro.Plugins.MediaPlayer.PresentationRoot)this.presentationRoot; }
            set { this.presentationRoot = value; }
        }

        protected Nyangoro.Plugins.MediaPlayer.MediaPlayer PluginCore {
            get { return (Nyangoro.Plugins.MediaPlayer.MediaPlayer)this.pluginCore; }
            set { this.pluginCore = value; } 
        }

        /**
         * Question: WHY THIS???? 
         */
        public MediaPlayerController(MediaPlayer core, ControlRoot controlRoot, PresentationRoot presentationRoot) : base(core, controlRoot, presentationRoot)
        {
        }

        public void HandleButtonClick()
        {
            Grid rootGrid = this.PresentationRoot.FindName("RootGrid") as Grid;
            TextBlock textblock = new TextBlock();
            textblock.Text = "blablabla";
            rootGrid.Children.Add(textblock);

        }

        public void BindPlaylistToControl()
        {
            ListBox playlistBox = this.GetPlaylistBox();
            playlistBox.ItemsSource = this.PluginCore.Playlist.contents;
        }

        public ListBox GetPlaylistBox()
        {
            Grid controlContent = (Grid)this.ControlRoot.Content;
            ListBox playlistBox = (ListBox)controlContent.FindName("PlaylistBox");
            return playlistBox;
        }

        public void HandleAddToPlaylistClick()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;

            Nullable<bool> result = dialog.ShowDialog();
            if (result != null)
            {
                string[] filenames = dialog.FileNames;
                this.AddPlaylistItems(filenames);
            }
        }

        public void HandlePlaylistMouseDoubleClick()
        {
            PluginCore.Playlist.PlaySelected();
        }

        public void HandlePlayClick(object sender, RoutedEventArgs e)
        {
            PluginCore.Playlist.Play();
        }

        public void HandleStopClick(object sender, RoutedEventArgs e)
        {
            PluginCore.Playlist.Stop();
        }

        private void AddPlaylistItems(string[] filenames)
        {
            foreach(string s in filenames)
            {
                PlaylistItemFile item = new PlaylistItemFile(this.PluginCore.processors, s);
                this.PluginCore.Playlist.contents.Add(item);
            }
        }

        public void DisplayMedia(FrameworkElement mediaRoot)
        {
            this.PresentationRoot.Content = mediaRoot;
        }
    }
}
