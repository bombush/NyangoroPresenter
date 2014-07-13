using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using LibVLC.NET;
using LibVLC.NET.Presentation;


namespace Nyangoro.Plugins.MediaPlayer
{
    class VlcMediaProcessor : IMediaProcessor
    {
        public event EventHandler EndReached; 

        private string[] playableFileTypes = { ".avi", ".mp4", ".mkv"}; 

        private Nyangoro.Plugins.MediaPlayer.VlcDisplayControl displayRoot;
        private LibVLC.NET.Presentation.MediaElement mediaElement;
        private PlaylistItem activeItem;

        public VlcMediaProcessor()
        {
            this.displayRoot = new Nyangoro.Plugins.MediaPlayer.VlcDisplayControl();
            Viewbox viewbox = (Viewbox)this.displayRoot.Content;
            this.mediaElement = (LibVLC.NET.Presentation.MediaElement)viewbox.Child;
        }

        //Get roots element to append to the plugin root
        public FrameworkElement GetRootElement()
        {
            return this.displayRoot;
        }

        /*
         * @TODO: Probably not too good to have the same reference in two places
         */
        public void SetActiveItem(Nyangoro.Plugins.MediaPlayer.PlaylistItem item)
        {
            this.activeItem = item;
            this.mediaElement.Source = item.path;
        }

        public PlaylistItem GetActiveItem()
        {
            return this.activeItem;
        }

        //neexistuje tady neco jako trida FileType??
        //Gets an array of types the implementing processor is capable of playing
        public string[] GetPlayableFileTypes()
        {
            return this.playableFileTypes;
        }

        public void Play()
        {
            this.mediaElement.Play();
        }

        public void Stop()
        {
            this.mediaElement.Stop();
        }

        public void Pause()
        {
            this.mediaElement.Pause();
        }
    }
}
//@TODO ucesat playing and IsPlaying
