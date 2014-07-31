using System;
using System.Collections.Generic;
using System.IO;
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
        new protected Nyangoro.Plugins.MediaPlayer.ControlRoot ControlRoot
        { 
            get { return (Nyangoro.Plugins.MediaPlayer.ControlRoot)this.controlRoot; }
            set { this.controlRoot = value; }
        }

        new protected Nyangoro.Plugins.MediaPlayer.PresentationRoot PresentationRoot
        {
            get { return (Nyangoro.Plugins.MediaPlayer.PresentationRoot)this.presentationRoot; }
            set { this.presentationRoot = value; }
        }

        new protected Nyangoro.Plugins.MediaPlayer.MediaPlayer PluginCore {
            get { return (Nyangoro.Plugins.MediaPlayer.MediaPlayer)this.pluginCore; }
            set { this.pluginCore = value; } 
        }

        /**
         * Question: WHY THIS???? 
         */
        public MediaPlayerController(MediaPlayer core, ControlRoot controlRoot, PresentationRoot presentationRoot) : base(core, controlRoot, presentationRoot)
        {
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
            PluginCore.Playlist.StopActive();
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

        public void AddImageBatchClick()
        {
            PlaylistItemImageBatch item = new PlaylistItemImageBatch(this.PluginCore.processors, this.PluginCore);
            this.PluginCore.Playlist.contents.Add(item);
        }

        public void DisplayMedia(FrameworkElement mediaRoot)
        {
            this.PresentationRoot.Content = mediaRoot;
        }


        #region Playlist IO - very stupid
        //REFACTOR: This should be moved to playlist ASAP

        protected void SavePlaylist()
        {
            string playlistString = this.PlaylistToString();

            //make this into a service, generally handling IO in a safe and corruption-prone way
            this.WritePlaylistFile(playlistString);
        }

        protected string PlaylistToString()
        {
            string playlistString = "";
            foreach (PlaylistItem item in this.PluginCore.Playlist.contents)
            {
                string path = item.path.AbsolutePath;
                playlistString += path + System.Environment.NewLine;
            }

            return playlistString;
        }

        protected void PlaylistFromString(string playlistString)
        {
            StringReader sr = new StringReader(playlistString);

            string path = "";
            while ((path = sr.ReadLine()) != null)
            {
                if (!String.IsNullOrWhiteSpace(path))
                {
                    PlaylistItemFile item = new PlaylistItemFile(this.PluginCore.processors, path);
                    //REFACTOR: why the hell do I need to contents.Add() instead of directly? Stupid and unintuitive
                    this.PluginCore.Playlist.contents.Add(item);
                }
            }
        }

        protected void WritePlaylistFile(string playlistString)
        {
            try
            {
                string filepath = this.PluginCore.Dir + Playlist.PLAYLIST_FILENAME;
                FileStream fs = new FileStream(filepath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(playlistString);
            }
            catch
            {
                return;
            }
        }

        protected void LoadPlaylist()
        {
            string playlistString = this.ReadPlaylistFile();
            this.PlaylistFromString(playlistString);
        }

        protected string ReadPlaylistFile()
        {
            string fileString = "";
            try
            {
                string filepath = this.PluginCore.Dir + Playlist.PLAYLIST_FILENAME;
                FileStream fs = new FileStream(filepath, FileMode.Open);
                StreamReader sr = new StreamReader(fs);

                fileString = sr.ReadToEnd();
            }
            catch
            {
                return "";
            }

            return fileString;
        }
        #endregion
    }
}
