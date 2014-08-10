using System;
using System.Collections.Generic;
using System.IO;
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
        private TextBlock textDisplay;

        public VlcMediaProcessor()
        {
            this.displayRoot = new Nyangoro.Plugins.MediaPlayer.VlcDisplayControl();
            this.mediaElement = (LibVLC.NET.Presentation.MediaElement)this.displayRoot.FindName("MediaElement");

            this.textDisplay = (TextBlock)this.displayRoot.FindName("VlcTextBlock");

            this.mediaElement.EndReached +=new RoutedEventHandler(this.mediaElement_EndReached);
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
            this.textDisplay.Text = Path.GetFileNameWithoutExtension(this.activeItem.path.LocalPath);
            this.mediaElement.Play();
        }

        public void Stop()
        {
            this.textDisplay.Text = null;
            this.mediaElement.Stop();
        }

        public void Pause()
        {
            this.mediaElement.Pause();
        }

        public TimeSpan CalculateItemLength(PlaylistItem item)
        {
            return new TimeSpan();
        }

        protected void mediaElement_EndReached(object sender, RoutedEventArgs e)
        {
            this.EndReached(this, EventArgs.Empty);
        }
    }
}
//@TODO ucesat playing and IsPlaying
