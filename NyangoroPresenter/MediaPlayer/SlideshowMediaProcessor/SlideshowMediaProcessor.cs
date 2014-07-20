using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using LibVLC.NET;
using LibVLC.NET.Presentation;

namespace Nyangoro.Plugins.MediaPlayer.SlideshowMediaProcessor
{
    class SlideshowMediaProcessor : IMediaProcessor
    {
        private Nyangoro.Plugins.MediaPlayer.SlideshowDisplayControl displayRoot;
        private string[] playableFileTypes = { MediaPlayer.CustomFileTypes.ImageBatch };

        //this should be shared between Slideshow and Vlc Processors
        LibVLC.NET.MediaPlayer mediaPlayer;

        PlaylistItemImageBatch playlistItem;

        public SlideshowMediaProcessor()
        {
            this.mediaPlayer = new LibVLC.NET.MediaPlayer();
        }

        event EventHandler EndReached;

        //Get roots element to append to the plugin root
        public FrameworkElement GetRootElement()
        {
            return this.displayRoot;
        }

        //neexistuje tady neco jako trida FileType??
        //Gets an array of types the implementing processor is capable of playing
        string[] GetPlayableFileTypes()
        {
            return this.playableFileTypes;
        }

        PlaylistItem GetActiveItem();
        void SetActiveItem(PlaylistItem item)
        {
            this.playlistItem = (PlaylistItemImageBatch)item;
        }
        // bool IsPlaying();

        //Start playback
        void Play();
        //Stop playback and free any unneeded resources
        void Stop();
        //Pause Playback
        void Pause();
    }
}
