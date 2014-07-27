using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using LibVLC.NET;
using LibVLC.NET.Presentation;

namespace Nyangoro.Plugins.MediaPlayer
{
    //@TODO: napsat konec prehravani nejak rozumneji
    public class SlideshowMediaProcessor : IMediaProcessor
    {
        //conf const
        protected const int FadeInSeconds = 2;

        //end reached flag
        protected const int FlagEndReached = 1;

        private Nyangoro.Plugins.MediaPlayer.SlideshowDisplayControl displayRoot;
        private Image imageDisplay;

        private string[] playableFileTypes = { MediaPlayer.CustomFileTypes.ImageBatch };

        //this should be shared between Slideshow and Vlc Processors
        LibVLC.NET.Presentation.MediaElement mediaPlayer;

        PlaylistItemImageBatch playlistItem;

        private string imageDir;
        private string audioDir;

        Uri activeImage;
        Uri activeAudio;

        TimeSpan currentTimeTotal;
        TimeSpan currentTimeActiveImage;
        TimeSpan targetTimeActiveImage;

        System.Timers.Timer imageTimer;

        bool paused = false;

        public SlideshowMediaProcessor()
        {
            this.displayRoot = new SlideshowDisplayControl();
            this.imageDisplay = (Image)this.displayRoot.FindName("SlideshowImage");

            this.mediaPlayer = new LibVLC.NET.Presentation.MediaElement();
            this.mediaPlayer.EndReached += this.mediaPlayer_EndReached;
        }

        public event EventHandler EndReached;

        //Get roots element to append to the plugin root
        public FrameworkElement GetRootElement()
        {
            return this.displayRoot;
        }

        //neexistuje tady neco jako trida FileType??
        //Gets an array of types the implementing processor is capable of playing
        public string[] GetPlayableFileTypes()
        {
            return this.playableFileTypes;
        }

        public PlaylistItem GetActiveItem()
        {
            return this.playlistItem;
        }

        public void SetActiveItem(PlaylistItem item)
        {
            this.playlistItem = (PlaylistItemImageBatch)item;
        }
        // bool IsPlaying();

        //Start playback
        public void Play()
        {
            if(this.playlistItem == null)
                throw new Exception("Trying to play an empty playlist item");

            if(!this.paused){
                this.PrepareMedia();
                this.InitTimer();
            }

            this.FadeInPlayImage();
            this.FadeInPlayAudio();
        }

        protected void FadeInImage()
        {
            DoubleAnimation animation = AnimationFactory.CreateFadeIn(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));
            this.imageDisplay.BeginAnimation(FrameworkElement.OpacityProperty, animation);
        }


        /*
        protected void ThreadedAudioFadeInLoop()
        {
            for(;;)
            {
                if(this.mediaPlayer.Volume >= 1){
                    this.mediaPlayer.Volume = 1;
                    break;
                }

                //WIP here
                this.mediaPlayer.Dispatcher.BeginInvoke();
                this.mediaPlayer.Volume += 0.1;
                Thread.Sleep(100);
            }
        }*/

        protected void FadeInPlayImage()
        {
            this.imageTimer.Start();
            this.FadeInImage();
        }

        protected void FadeInPlayAudio()
        {
            this.mediaPlayer.Play();
            this.FadeInAudio();
        }

        protected void StopAudio()
        {
            this.FadeOutStopAudio();
        }

        protected void FadeInAudio()
        {
            DoubleAnimation animationVolumeFadeIn = AnimationFactory.CreateFadeIn(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));
            
            this.mediaPlayer.BeginAnimation(LibVLC.NET.Presentation.MediaElement.VolumeProperty, animationVolumeFadeIn);
        }

        protected void FadeOutStopAudio()
        {
            DoubleAnimation animationVolumeFadeOut = AnimationFactory.CreateFadeOut(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));
            animationVolumeFadeOut.Completed += this.animationVolumeFadeOut_Completed;

            this.mediaPlayer.BeginAnimation(LibVLC.NET.Presentation.MediaElement.VolumeProperty, animationVolumeFadeOut);
        }

        protected void FadeOutStopAudio(int flag)
        {
            if(flag == SlideshowMediaProcessor.FlagEndReached)
            {
                DoubleAnimation finalVolumeFadeOut = AnimationFactory.CreateFadeOut(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));
                finalVolumeFadeOut.Completed += this.finalVolumeFadeOut_Completed;

                this.mediaPlayer.BeginAnimation(LibVLC.NET.Presentation.MediaElement.VolumeProperty, finalVolumeFadeOut);
            } 
            else
                this.FadeOutStopAudio();
        }

        protected void StopImage()
        {
            this.imageTimer.Stop();
            this.FadeOutImage();
        }

        protected void FadeOutImage()
        {
            DoubleAnimation animation = AnimationFactory.CreateFadeOut(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));
            //WTF thread????? |Use Dispatcher?
            this.imageDisplay.BeginAnimation(FrameworkElement.OpacityProperty, animation);
        }

        protected void PrepareMedia()
        {
            this.activeAudio = this.playlistItem.PopNextActiveSong();
            if (activeImage != null)
            {
                this.mediaPlayer.Source = this.activeAudio;
                this.mediaPlayer.Volume = 0;
            }

            this.imageDisplay.Opacity = 0;

            this.activeImage = this.playlistItem.PopNextActiveImage();
            this.imageDisplay.Source = new BitmapImage(this.activeImage);
        }

        //Stop playback and free any unneeded resources
        public void Stop()
        {
            this.mediaPlayer.Stop();
            this.imageDisplay.Source = null;
        }

        //
        //@TODO implement pausing
        public void Pause()
        {
            this.mediaPlayer.Pause();
        }

        protected void InitTimer()
        {
            this.imageTimer = new System.Timers.Timer(PlaylistItemImageBatch.ImageDisplaySec*1000);
            this.imageTimer.Elapsed += this.imageTimer_Elapsed;
        }


        #region events

        //EVENTS
        protected void imageTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.StopImage();

            Uri nextImage = this.playlistItem.PopNextActiveImage();

            if(nextImage == null)
            {
                this.OnImageBatchEndReached();
            }
            else
            {
                this.activeImage = nextImage;
                this.FadeInPlayImage();
            }

        }

        protected void OnImageBatchEndReached()
        {
            this.FadeOutStopAudio(SlideshowMediaProcessor.FlagEndReached);
        }

        protected void animationVolumeFadeOut_Completed(object sender, EventArgs e)
        {
            this.mediaPlayer.Stop();
        }

        protected void finalVolumeFadeOut_Completed(object sender, EventArgs e)
        {
            //fire EndReached event
            this.EndReached(this, null);
        }

        protected void mediaPlayer_EndReached(object sender, RoutedEventArgs e)
        {
            Uri nextSong = this.playlistItem.PopNextActiveSong(); 
            if(nextSong != null)
            {
                this.activeAudio = nextSong;
                this.FadeInPlayAudio();
            }
        }

        #endregion

        /**
         * VLC cannot give us the length of played media so we postpone
         * implementation of this until we find a better way
         * 
        public TimeSpan CalculateItemLength(PlaylistItemImageBatch item)
        {
            List<Uri> audioBatch = item.activeAudioBatch;

            foreach(Uri audioUri in audioBatch)
            {
                
            }

            return new TimeSpan();
        }
         */
    }
}
