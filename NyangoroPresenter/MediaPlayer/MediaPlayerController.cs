using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Xml;
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

        public void HandleLoadPlaylistClick()
        {
            this.LoadPlaylist();
        }

        public void HandleShufflePlaylistClick()
        {
            Playlist.ShuffleList<PlaylistItem>(this.PluginCore.Playlist.contents);
            this.SavePlaylist();
        }

        private void AddPlaylistItems(string[] filenames)
        {
            foreach(string s in filenames)
            {
                PlaylistItemFile item = new PlaylistItemFile(this.PluginCore.processors, s);
                this.PluginCore.Playlist.contents.Add(item);
            }

            this.SavePlaylist();
        }

        private void RemoveSelectedFromPlaylist()
        {
            this.PluginCore.Playlist.RemoveSelected();
        }

        //REFACTOR: SavePlaylist called on many places, make ONE access method for Add and Remove. Prevent saving and loading a playlist at the same time
        public void AddImageBatchClick()
        {
            PlaylistItemImageBatch item = new PlaylistItemImageBatch(this.PluginCore.processors, this.PluginCore);
            this.PluginCore.Playlist.contents.Add(item);
            this.SavePlaylist();
        }

        public void DisplayMedia(FrameworkElement mediaRoot)
        {
            this.PresentationRoot.Content = mediaRoot;
        }


        #region Playlist IO - very stupid SHOULD NOT BE IN THE CONTROLLER.
        //REFACTOR: This should be moved to playlist ASAP. Just let playlist handle creating Items and stuff by itself (maybe)

        protected void SavePlaylist()
        {
            this.PlaylistExportXml();
        }

        protected void LoadPlaylist()
        {
            this.PlaylistFromXml(this.PluginCore.Dir + Playlist.PLAYLIST_FILENAME);
        }

        //REFACTOR: make sure the XML is not corrupt!!!!
        protected void PlaylistExportXml()
        {
            try
            {
                string filepath = this.PluginCore.Dir + Playlist.PLAYLIST_FILENAME;
                XmlTextWriter xml = new XmlTextWriter(filepath, Encoding.UTF8);
                xml.Formatting = Formatting.Indented;

                xml.WriteStartDocument();
                xml.WriteStartElement("Playlist");
                foreach (PlaylistItem item in this.PluginCore.Playlist.contents)
                {
                    xml.WriteStartElement("Item");

                    xml.WriteStartElement("Type");
                    xml.WriteString(item.GetType().ToString());
                    xml.WriteEndElement();

                    xml.WriteStartElement("Uri");
                    if(item.path != null)
                        xml.WriteCData(item.path.ToString());
                    else
                        xml.WriteCData(String.Empty);
                    xml.WriteEndElement();

                    xml.WriteEndElement();
                }
                xml.WriteEndElement();
                xml.WriteEndDocument();
                xml.Close();
            }
            catch(Exception e)
            {
                Exception ex = e;
                return;
            }
        }

        protected void PlaylistFromXml(string path)
        {
            this.PluginCore.Playlist.contents.Clear();

            FileStream fs = new FileStream(path, FileMode.Open);
            XmlTextReader xml = new XmlTextReader(fs);

            Dictionary<string, string> temp = null;
            do
            {
                if(xml.Name == "Item" && xml.IsStartElement())
                {
                   temp = new Dictionary<string, string>();
                }

                if (xml.Name == "Item" && !xml.IsStartElement())
                {
                    if (temp != null)
                    {
                        PlaylistItem item = this.CreatePlaylistItemInstance(temp);
                        if (item != null)
                            this.PluginCore.Playlist.contents.Add(item);
                    }
                }

                if (xml.Name == "Type" && xml.IsStartElement())
                {
                    string itemType = xml.ReadInnerXml();
                    temp["Type"] = itemType;
                }

                if (xml.Name == "Uri" && xml.IsStartElement())
                {
                    xml.Read();
                    if (xml.NodeType == XmlNodeType.CDATA)
                    {
                        temp["Uri"] = xml.Value;
                    }
                    else
                    {
                        throw new Exception("Uri in invalid format");
                    }
                }
            }
            while (xml.Read());

            xml.Close();
        }

        protected PlaylistItem CreatePlaylistItemInstance(Dictionary<string, string> info)
        {
            switch(info["Type"]){
                case "Nyangoro.Plugins.MediaPlayer.PlaylistItemFile":
                    return new PlaylistItemFile(this.PluginCore.processors, info["Uri"]);
                    //break;
                case "Nyangoro.Plugins.MediaPlayer.PlaylistItemImageBatch":
                    return new PlaylistItemImageBatch(this.PluginCore.processors, this.PluginCore);
                    //break;
                default:
                    return null;
            }
        }
        #endregion
    }
}
