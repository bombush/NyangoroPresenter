using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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


        public Playlist GetPlaylist()
        {
            return this.PluginCore.Playlist;
        }

        //prasarny
        /*
        static public Grid vlcTextGrid;
        static public TextBox vlcText;
         * /

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

        public void BindPlaylistEvents()
        {
            this.PluginCore.Playlist.ItemActivated += new EventHandler(this.Playlist_ItemActivated);
        }

        protected void HandlePlaylistContentsChanged(Object sender,	NotifyCollectionChangedEventArgs e)
        {
            this.ColorPlaylistItemsByStatus();
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

            this.ColorPlaylistItemsByStatus();
            this.PluginCore.Playlist.SyncActiveIndexToItem();

            this.SavePlaylist();
        }

        private void AddPlaylistItems(string[] filenames)
        {
            foreach(string s in filenames)
            {
                PlaylistItemFile item = new PlaylistItemFile(this.PluginCore.processors, s);
                this.PluginCore.Playlist.AddItem((PlaylistItem)item);
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
            this.PluginCore.Playlist.AddItem((PlaylistItem)item);
            this.SavePlaylist();
        }

        public void DisplayMedia(FrameworkElement mediaRoot)
        {
            Viewbox viewbox = (Viewbox)((Grid)((Border)this.PresentationRoot.Content).Child).FindName("MediaViewbox");
            viewbox.Child = mediaRoot;
           // viewbox.Content = mediaRoot;
        }


        #region Playlist IO - very stupid SHOULD NOT BE IN THE CONTROLLER.
        //REFACTOR: This should be moved to playlist ASAP. Just let playlist handle creating Items and stuff by itself (maybe)

        protected void SavePlaylist()
        {
            this.PlaylistExportXml();
        }

        public void LoadPlaylist()
        {
            try
            {
                this.PlaylistFromXml(this.PluginCore.Dir + Playlist.PLAYLIST_FILENAME);
            }
            catch
            {
                return;
            }
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

                xml.WriteStartElement("LastActive");
                xml.WriteValue(this.PluginCore.Playlist.activeIndex);
                xml.WriteEndElement();

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

            FileStream fs;
            XmlTextReader xml;

            try
            {
                fs = new FileStream(path, FileMode.Open);
                xml = new XmlTextReader(fs);
            }
            catch
            {
                return;
            }

            // After all items are loaded to the playlist, an item at lastActiveIndex+1 will
            // be set as active.
            int lastActiveIndex = -1;

            Dictionary<string, string> temp = null;
            do
            {
                if (xml.Name == "LastActive" && xml.IsStartElement())
                {
                    try
                    {
                        lastActiveIndex = Int32.Parse(xml.ReadInnerXml());
                    }
                    catch (FormatException e)
                    {
                        FormatException exp = e;
                        lastActiveIndex = -1;
                    }
                }

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

            this.PluginCore.Playlist.SetActive(lastActiveIndex + 1);
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

        protected void Playlist_ItemActivated(object sender, EventArgs e)
        {
            //save playlist in a new thread
            Thread thr = new Thread(new ThreadStart(this.SavePlaylist));
            thr.Start();

            this.ColorPlaylistItemsByStatus();
        }

        public void ColorPlaylistItemsByStatus()
        {
            ListBox box = this.GetPlaylistBox();
            PlaylistItem activeItem = this.PluginCore.Playlist.activeItem;

            for (int i = 0; i < box.Items.Count; i++)
            {
                PlaylistItem item = (PlaylistItem)box.Items[i];
                ListBoxItem boxItem = this.ControlRoot.GetListBoxItem(box, i);
                
                if (item == activeItem)
                {                    
                    boxItem.Background = Brushes.Green;
                }
                else
                {
                    boxItem.Background = Brushes.Transparent;
                }
            }
        }



        public void HandleClearPlaylistClick()
        {
            this.PluginCore.Playlist.contents.Clear();
        }

        public void HandleRemoveSelectedClick()
        {
            this.PluginCore.Playlist.RemoveSelected();
            this.SavePlaylist();
        }

        public void HandlePlaylistDownClick()
        {
            this.PluginCore.Playlist.SelectedMoveDown();
        }

        public void HandlePlaylistUpClick()
        {
            this.PluginCore.Playlist.SelectedMoveUp();
        }

    }
}
