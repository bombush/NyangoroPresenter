using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;


namespace Nyangoro.Plugins.MediaPlayer
{
    /**
     * @TODO: This should be a singleton
     */
    public class Playlist
    {
        public event EventHandler ItemActivated;

        #region fields and properties

        //playback order types
        public const int PLAYBACK_LINEAR = 0;
        public const int PLAYBACK_RANDOM = 1;
        public const string PLAYLIST_FILENAME = "playlist.nya";

        protected int playbackOrder = PLAYBACK_LINEAR;
        public int PlaybackOrder
        {
            get { return this.playbackOrder; }
            set { this.playbackOrder = value; }
        }

        public ObservableCollection<PlaylistItem> contents { get; private set; }
        public PlaylistItem activeItem { get; private set; }
        public int activeIndex { get; private set; }

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
            this.activeIndex = -1;

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

            EventHandler handler = new EventHandler(this.activeItem_EndReached);
            this.activeItem.EndReached += handler;
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

        protected void ActivateItem(PlaylistItem item, int index)
        {
            if (item == null)
                throw new Exception("Playlist item to activate is null");

            this.activeItem = item;
            this.activeIndex = index;

            this.ItemActivated(this, EventArgs.Empty);

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
            this.UnsetActive();
        }

        protected void ActivateSelected()
        {
            PlaylistItem item;
            if (this.box.SelectedItem == null)
                throw new Exception("No item selected to set active");

            //item = (PlaylistItem)this.box.SelectedItem;
            int index = this.box.SelectedIndex;
            item = (PlaylistItem)this.box.Items[index];

            this.ActivateItem(item, index);
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

        public void RemoveIndex(int index)
        {
            this.contents.RemoveAt(index);
        }

        //Removed the selected item from playlist but keeps playing
        public void RemoveSelected()
        {
            int index = this.box.SelectedIndex;
            if(index != -1)
                this.RemoveIndex(index);
        }
        #endregion

        //if playlist processing is not stopped, continue with the next item in the playlist
        public void activeItem_EndReached(object sender, EventArgs e)
        {
            this.activeItem.EndReached -= new EventHandler(activeItem_EndReached);
            if(!this.Stopped)
                this.PlayNext();
        }

        public static void ShuffleList<T>(IList<T> list)
        {
            int n = list.Count;
            Random random = new Random();

            while (n > 1)
            {
                n--;

                int index = random.Next(n+1);
                T item = list[n];
                T randomItem = list[index];
                list[index] = item;
                list[n] = randomItem;
            }
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

        //nejak ujednotit zpusob pristupu ke clenum playlistu
        public void SetActive(int index)
        {
            if(this.contents.Count > index){
                this.activeIndex = index;
                this.box.SelectedIndex = index;
                this.ActivateSelected();
                //this.activeItem = this.contents.ElementAt(index);
            }
        }

        protected void UnsetActive()
        {
            this.activeItem = null;
            this.activeIndex = -1;
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
//@TODO Randomization
//@TODO Removing already played items
//@TODO Playlist Save/Load