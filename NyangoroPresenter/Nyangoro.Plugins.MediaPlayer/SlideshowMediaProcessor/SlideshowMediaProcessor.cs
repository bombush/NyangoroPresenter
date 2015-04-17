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
        public event EventHandler EndReached;

        //conf const
        protected const int FadeInSeconds = 2;

        //flag constant
        protected const int FlagEndReached = 1;

        //flags
        protected bool imageEndReached = false;
        protected bool audioEndReached = false;
        bool paused = false;

        //display root
        private Nyangoro.Plugins.MediaPlayer.SlideshowDisplayControl displayRoot;
    
        //media elements
        private Image imageDisplay;
        //REFACTOR: this should be shared between Slideshow and Vlc Processors
        LibVLC.NET.Presentation.MediaElement mediaPlayer;

        //active media
        Uri activeImage;
        Uri activeAudio;
        PlaylistItemImageBatch playlistItem;


        private string[] playableFileTypes = { MediaPlayer.CustomFileTypes.ImageBatch };

        //timer for timing image events
        System.Timers.Timer imageTimer;

        public Grid VlcTextGrid { get; set; }


        /// <summary>
        /// CONSTRUCTOR
        /// </summary>
        public SlideshowMediaProcessor(Grid vlcTextGrid)
        {
            //this.VlcTextGrid = vlcTextGrid;

            this.displayRoot = new SlideshowDisplayControl();
            this.imageDisplay = (Image)this.displayRoot.FindName("SlideshowImage");

            this.mediaPlayer = new LibVLC.NET.Presentation.MediaElement();
            this.mediaPlayer.EndReached += this.mediaPlayer_EndReached;
        }

        #region IMediaProcessor implementations

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
           // this.VlcTextGrid.Opacity = 0;
          //  this.VlcTextGrid.Visibility = Visibility.Hidden;

            this.audioEndReached = false;
            this.imageEndReached = false;

            if(this.playlistItem == null)
                throw new Exception("Trying to play an empty playlist item");

            if (!this.paused)
            {
                this.InitTimer();

                //try playing an image. If no image found, fire EndReached and return
                Uri image = this.playlistItem.PopNextActiveImage();
                if (image != null)
                    this.FadeInPlayImage(image);
                else
                {
                    this.Stop();
                    this.EndReached(this, EventArgs.Empty);
                    return;
                }

                Uri song = this.playlistItem.PopNextActiveSong();
                if (song != null)
                    this.FadeInPlayAudio(song);
                else
                    this.audioEndReached = true;
            }
            else
            {
                //this.ResumePlay();
            }            
        }

        //Stop playback and free any unneeded resources
        public void Stop()
        {
            this.StopImage();
            this.StopAudio();
            try
            {
                if (this.imageTimer != null)
                    this.imageTimer.Dispose();
            }
            catch { }
        }

        //
        //@TODO implement pausing
        public void Pause()
        {
            this.mediaPlayer.Pause();
        }

        #endregion

        #region init methods

        protected void ReadyImage(Uri image)
        {
           // this.imageDisplay.Opacity = 0;
            this.activeImage = image;
            this.imageDisplay.Source = new BitmapImage(this.activeImage);
        }

        protected void ReadyAudio(Uri audio)
        {
            this.mediaPlayer.Source = audio;
            this.mediaPlayer.Volume = 0;
        }

        protected void InitTimer()
        {
            this.imageTimer = new System.Timers.Timer(PlaylistItemImageBatch.ImageDisplaySec*1000);
            this.imageTimer.AutoReset = false;
            this.imageTimer.Elapsed += this.imageTimer_Elapsed;
        }

        #endregion

        #region media play methods

        protected void PlayImage(Uri image, AnimationTimeline animation)
        {
            this.imageEndReached = false;
            this.ReadyImage(image);

            // If the timer has already been disposed for example as part of the Stop
            // routine, do nothing and return
            try
            {
                this.imageTimer.Start();
            }
            catch (ObjectDisposedException e)
            {
                ObjectDisposedException exc = e;
                return;
            }

            this.imageDisplay.BeginAnimation(Image.OpacityProperty, animation);
        }

        protected void PlayAudio(Uri audio, AnimationTimeline animation)
        {
            this.audioEndReached = false;
            this.ReadyAudio(audio);
            this.mediaPlayer.Play();
            this.mediaPlayer.BeginAnimation(LibVLC.NET.Presentation.MediaElement.VolumeProperty, animation);
        }

        protected void PlayAudio(Uri audio)
        {
            this.audioEndReached = false;
            this.ReadyAudio(audio);
            this.mediaPlayer.Volume = 1;
            this.mediaPlayer.Play();
        }

        //REFACTOR: pro kazdy obrazek nova instance timeru
        protected void FadeInPlayImage(Uri image)
        {
            DoubleAnimation fadeIn = AnimationFactory.CreateFadeIn(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));
            this.PlayImage(image, fadeIn);
        }

        protected void FadeInPlayAudio(Uri audio)
        {
            DoubleAnimation fadeIn = AnimationFactory.CreateFadeIn(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));
            this.PlayAudio(audio, fadeIn);
        }

        #endregion

        #region stop audio methods

        protected void StopAudio(AnimationTimeline stopAnimation, Action stopCallback)
        {
            stopAnimation.Completed += (object sender, EventArgs e) => this.CleanupStopAudio(stopCallback);
            this.mediaPlayer.BeginAnimation(LibVLC.NET.Presentation.MediaElement.VolumeProperty, stopAnimation);
        }

        protected void StopAudio(Action stopCallback)
        {
            this.CleanupStopAudio(stopCallback);
        }

        protected void StopAudio()
        {
            this.CleanupStopAudio();
        }

        protected void CleanupStopAudio(Action callback)
        {
            this.mediaPlayer.Stop();
            this.activeAudio = null;

            callback();
        }

        protected void CleanupStopAudio()
        {
            this.mediaPlayer.Stop();
            this.activeAudio = null;
        }
        #endregion

        #region stop image methods

        // stopCallback is called after image has been cleaned up completely 
        //
        //@TODO: still not as clever as initially thought because opacity is hardcoded - doesn't allow more complex animations = SUX
        // taky je naprd, ze metoda s nazvem "StopImage" vlastne ten image doopravdy nezastavi, ale jenom nastavi animaci a callback
        protected void StopImage(AnimationTimeline stopAnimation, Action stopCallback )
        {
            stopAnimation.Completed += (object sender, EventArgs e) => this.CleanupStopImage(stopCallback);
            this.imageDisplay.BeginAnimation(Image.OpacityProperty, stopAnimation);
        }

        protected void StopImage(Action stopCallback)
        {
            this.CleanupStopImage(stopCallback);
        }

        protected void StopImage()
        {
            this.CleanupStopImage();
        }

        protected void CleanupStopImage(Action callback)
        {
            if(this.imageTimer != null)
                this.imageTimer.Stop();

            this.activeImage = null;

            callback();
        }

        protected void CleanupStopImage()
        {
            if(this.imageTimer != null)
                this.imageTimer.Stop();

            this.activeImage = null;
        }

        #endregion

        #region events invokers and handlers

        protected void ImageTimerElapsed()
        {
            if (!this.playlistItem.IsImageWaiting())
            {
                DoubleAnimation fadeOutImage = AnimationFactory.CreateFadeOut(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));
                DoubleAnimation fadeOutAudio = AnimationFactory.CreateFadeOut(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));

                //stop media with animation and callback
                this.StopAudio(fadeOutAudio, () => this.OnAudioBatchEndReached());
                this.StopImage(fadeOutImage, () => this.OnImageBatchEndReached());
            }
            else
            {
                Uri nextImage = this.playlistItem.PopNextActiveImage();
                DoubleAnimation fadeOut = AnimationFactory.CreateFadeOut(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));

                DoubleAnimation fadeIn = AnimationFactory.CreateFadeIn(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));
                this.StopImage(fadeOut, () => this.PlayImage(nextImage, fadeIn));

            }
        }

        protected void OnImageBatchEndReached()
        {
            this.imageEndReached = true;
            this.OnEndReached();
        }

        protected void OnAudioBatchEndReached()
        {
            this.audioEndReached = true;
            this.OnEndReached();
        }

        protected void OnEndReached()
        {
            if (this.imageEndReached && this.audioEndReached)
                this.EndReached(this, new EventArgs());
        }


        protected void imageTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.imageDisplay.Dispatcher.Invoke(new Action(delegate() { this.ImageTimerElapsed(); }), new object[0]);
        }


        protected void mediaPlayer_EndReached(object sender, RoutedEventArgs e)
        {
            if (!this.playlistItem.IsSongWaiting())
            {
                this.StopAudio(()=>this.OnAudioBatchEndReached());
            }
            else
            {
                Uri nextSong = this.playlistItem.PopNextActiveSong();
                this.StopAudio(() => this.PlayAudio(nextSong));
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
