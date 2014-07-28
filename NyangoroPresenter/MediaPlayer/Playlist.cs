using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

            this.contents.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.HandleContentsChanged);
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
            this.StopActive();
            this.Stopped = true;
        }

        public void Pause()
        {
            this.activeItem.Pause();
        }

        protected void ActivateItem(PlaylistItem item)
        {
            if (item == null)
                throw new Exception("Playlist item to activate is null");

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

        public void StopActive()
        {
            if (this.activeItem == null)
                return;
            
            this.activeItem.EndReached -= new EventHandler(activeItem_EndReached);
            this.activeItem.Stop();
            this.activeItem = null;
        }

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
            bool endReached = false;
            if (this.PlaybackOrder == PLAYBACK_LINEAR)
            {
                if (!this.TrySelectNextItem())
                    endReached = true;
            }
            else if (this.PlaybackOrder == PLAYBACK_RANDOM)
            {
                if (!this.TrySelectRandomItem())
                    endReached = true;
            }

            if (endReached)
            {
                this.HandlePlaylistEndReached();
                return;
            }

            this.PlaySelected();
        }
        #endregion

        //if playlist processing is not stopped, continue with the next item in the playlist
        public void activeItem_EndReached(object sender, EventArgs e)
        {
            this.activeItem.EndReached -= new EventHandler(activeItem_EndReached);
            if(!this.Stopped)
                this.PlayNext();
        }


        protected bool TrySelectNextItem()
        {
            int nextIndex = box.SelectedIndex+1;
            if(nextIndex < this.contents.Count){
                box.SelectedIndex = nextIndex;
                return true;
            } else {
                return false;
            }
        }

        protected bool TrySelectRandomItem()
        {
            Random random = new Random();
            int rnr = random.Next(0, this.contents.Count);
            this.box.SelectedIndex = rnr;

            return true;
        }

        protected void HandlePlaylistEndReached()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.UnsetActive();
            this.ResetBox();
        }

        protected void UnsetActive()
        {
            this.activeItem = null;
        }

        protected void ResetBox()
        {
            this.box.UnselectAll();
        }

        protected void HandleContentsChanged(Object sender,	NotifyCollectionChangedEventArgs e)
        {
        }
    }
}
//@TODO Handle stopping on Play next item
//@TODO Randomization
//@TODO Removing already played items
//@TODO Playlist Save/Load