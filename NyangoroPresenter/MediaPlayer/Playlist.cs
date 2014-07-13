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
        #region fields and properties

        //playback order types
        public const int PLAYBACK_LINEAR = 0;
        public const int PLAYBACK_RANDOM = 1;

        protected int playbackOrder = PLAYBACK_LINEAR;
        public int PlaybackOrder
        {
            get { return this.playbackOrder; }
            set { this.playbackOrder = value; }
        }

        public ObservableCollection<PlaylistItem> contents { get; private set; }
        public PlaylistItem activeItem { get; private set; }

        private ListBox box;
        private MediaPlayer pluginCore;

        //Flag: is playlist processing stopped?
        public bool Stopped {get; private set;}

        #endregion

        #region constructor
        public Playlist(ListBox box, MediaPlayer pluginCore)
        {
            this.box = box;
            this.contents = new ObservableCollection<PlaylistItem>();
            this.pluginCore = pluginCore;
        }
        #endregion


        #region Main playback methods

        //@TODO: handle empty playlist
        //@TODO: if something else is playing, stop it
        public void Play()
        {
            if (this.contents.Count == 0)
                return;

            if (this.activeItem == null)
                this.AutoActivate();

            this.activeItem.EndReached += new EventHandler(activeItem_EndReached);
            this.activeItem.Play();
        }

        public void Stop()
        {
            this.Stopped = true;
            this.activeItem.Stop();
            this.activeItem.EndReached -= new EventHandler(activeItem_EndReached);
            this.activeItem = null;
        }

        public void Pause()
        {
            this.activeItem.Pause();
        }

        protected void ActivateItem(PlaylistItem item)
        {
            this.activeItem = item;
            this.PresentActive(this.pluginCore.Controller);
        }
        #endregion


        #region Presentation screen interaction logic

        protected void PresentActive(MediaPlayerController controller)
        {
            this.activeItem.DisplayMedia(controller);
        }
        #endregion


        #region Playlist management logic

        protected void ActivateSelected()
        {
            PlaylistItem item;
            if (this.box.SelectedItem == null)
                throw new Exception("No item selected to set active");

            item = (PlaylistItem)this.box.SelectedItem;

            this.ActivateItem(item);
        }

        protected void AutoActivate()
        {
            if(this.box.SelectedIndex == -1)
                this.box.SelectedIndex = 0;

            this.ActivateSelected();
        }

        public void PlaySelected()
        {
            this.ActivateSelected();
            this.Play();
        }

        /**
         * Select the next item in playlist based on the playback order and play
         */
        public void PlayNext()
        {
            if (this.PlaybackOrder == PLAYBACK_LINEAR)
                this.SelectNextItem();
            else if (this.PlaybackOrder == PLAYBACK_RANDOM)
                this.SelectRandomItem();

            this.PlaySelected();
        }
        #endregion

        //if playlist processing is not stopped, continue with the next item in the playlist
        public void activeItem_EndReached(object sender, EventArgs e)
        {
            if(!this.Stopped)
                this.PlayNext();
        }


        protected void SelectNextItem()
        {
            box.SelectedIndex += 1;
        }

        protected void SelectRandomItem()
        {
            Random random = new Random();
            int rnr = random.Next(0, this.contents.Count);
            this.box.SelectedIndex = rnr;
        }

    }
}
//@TODO Handle stopping on Play next item
//@TODO Randomization
//@TODO Removing already played items
//@TODO Playlist Save/Load