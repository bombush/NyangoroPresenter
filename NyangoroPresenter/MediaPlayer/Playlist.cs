using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace Nyangoro.Plugins.MediaPlayer
{
    /**
     * @TODO: This should be a singleton
     */
    public class Playlist
    {
        public ObservableCollection<PlaylistItem> contents { get; private set; }
        public PlaylistItem activeItem { get; private set; }

        private ListBox box;
        private MediaPlayer pluginCore;

        public Playlist(ListBox box, MediaPlayer pluginCore)
        {
            this.box = box;
            this.contents = new ObservableCollection<PlaylistItem>();
            this.pluginCore = pluginCore;
        }

        //@TODO: handle empty playlist
        //@TODO: if something else is playing, stop it
        public void Play()
        {
            if (this.contents.Count == 0)
                return;

            if(this.activeItem == null)
                return;

            this.activeItem.Play();
        }

        public void PlaySelected()
        {
            this.SetSelectedActive();
            this.Play();
        }

        protected void SetSelectedActive()
        {
            PlaylistItem item;
            if (this.box.SelectedItem == null)
                item = this.contents[0];
            else
                item = (PlaylistItem)this.box.SelectedItem;

            this.activeItem = item;

            MediaPlayerController controller = this.pluginCore.Controller;
            this.activeItem.DisplayMedia(controller);
        }
    }
}
