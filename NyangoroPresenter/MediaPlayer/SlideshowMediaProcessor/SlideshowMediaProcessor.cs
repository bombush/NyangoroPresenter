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

        //end reached flag
        protected const int FlagEndReached = 1;

        protected bool imageEndReached = false;
        protected bool audioEndReached = false;

        private Nyangoro.Plugins.MediaPlayer.SlideshowDisplayControl displayRoot;
        private Image imageDisplay;

        private string[] playableFileTypes = { MediaPlayer.CustomFileTypes.ImageBatch };

        //this should be shared between Slideshow and Vlc Processors
        LibVLC.NET.Presentation.MediaElement mediaPlayer;

        PlaylistItemImageBatch playlistItem;

        //private string imageDir;
        //private string audioDir;

        Uri activeImage;
        Uri activeAudio;

        //TimeSpan currentTimeTotal;
        //TimeSpan currentTimeActiveImage;
        //TimeSpan targetTimeActiveImage;

        System.Timers.Timer imageTimer;

        bool paused = false;

        public SlideshowMediaProcessor()
        {
            this.displayRoot = new SlideshowDisplayControl();
            this.imageDisplay = (Image)this.displayRoot.FindName("SlideshowImage");

            this.mediaPlayer = new LibVLC.NET.Presentation.MediaElement();
            this.mediaPlayer.EndReached += this.mediaPlayer_EndReached;
        }

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
            this.audioEndReached = false;
            this.imageEndReached = false;

            if(this.playlistItem == null)
                throw new Exception("Trying to play an empty playlist item");

            if (!this.paused)
            {
                // this.PrepareMedia();
                this.InitTimer();

                Uri image = this.playlistItem.PopNextActiveImage();
                this.FadeInPlayImage(image);

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

        /*
         * REFACTOR: obecna metoda PlayAnimated, do ktere se poslou jako parametry
         * animace (nastup a odchod). Zkombinovat s PrepareAudio
         * DUVOD: pamatovat si posloupnost PrepareAudio();FadeInPlayAudio(); je trochu oser
         * jo, a naucit se UML
         *//*
        protected void FadeInPlayAudio()
        {
            this.mediaPlayer.Play();
            this.FadeInAudio();
        }*/

            /*
        protected void StopAudio()
        {
            this.FadeOutStopAudio();
        }*/

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

        /*
        protected void FadeInAudio()
        {
            DoubleAnimation animationVolumeFadeIn = AnimationFactory.CreateFadeIn(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));
            
            this.mediaPlayer.BeginAnimation(LibVLC.NET.Presentation.MediaElement.VolumeProperty, animationVolumeFadeIn);
        }*/

        /*
        protected void FadeOutStopAudio()
        {
            DoubleAnimation animationVolumeFadeOut = AnimationFactory.CreateFadeOut(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));
            animationVolumeFadeOut.Completed += this.animationVolumeFadeOut_Completed;

            this.mediaPlayer.BeginAnimation(LibVLC.NET.Presentation.MediaElement.VolumeProperty, animationVolumeFadeOut);
        }*/

        /*
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
        */
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
            this.imageTimer.Stop();
            this.activeImage = null;

            callback();
        }

        protected void CleanupStopImage()
        {
            this.imageTimer.Stop();
            this.activeImage = null;
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

        /*
        protected void FadeOutImage()
        {
            DoubleAnimation animation = AnimationFactory.CreateFadeOut(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));
            this.imageDisplay.BeginAnimation(FrameworkElement.OpacityProperty, animation);
        }*/

        /*
        protected void FadeOutStopImage()
        {

        }*/
        /*
        protected void PrepareMedia()
        {
            this.audioEndReached = false;
            this.imageEndReached = false;
            /*
            Uri audio = this.playlistItem.PopNextActiveSong();
            if (audio != null)
            {
                this.ReadyAudio(audio);
            }
            else
            {
                this.audioEndReached = true;
            }

            this.ReadyImage(this.playlistItem.PopNextActiveImage());*/
        /*}*/

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

        //Stop playback and free any unneeded resources
        public void Stop()
        {
            this.StopImage();
            this.StopAudio();
            this.imageTimer.Dispose();
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
            this.imageTimer.AutoReset = false;
            this.imageTimer.Elapsed += this.imageTimer_Elapsed;
        }


        #region events

        //EVENTS
        // better thread handling
        protected void imageTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.imageDisplay.Dispatcher.Invoke(new Action(delegate() { this.ImageTimerElapsed(); }), new object[0]);
        }

        protected void ImageTimerElapsed()
        {
            //using lambda cuz lambdas are so much fun - almost like jQuery
            if (!this.playlistItem.IsImageWaiting())
            {
                // much lambda! such callback! wow!
                DoubleAnimation fadeOut = AnimationFactory.CreateFadeOut(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));
                DoubleAnimation fadeOutAudio = AnimationFactory.CreateFadeOut(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));

                this.StopAudio(fadeOut, () => this.OnAudioBatchEndReached());
                this.StopImage(fadeOutAudio, () => this.OnImageBatchEndReached());
                //this.TransitionVideoFadeOut(() => this.OnImageBatchEndReached());
            }
            else
            {
                Uri nextImage = this.playlistItem.PopNextActiveImage();
                DoubleAnimation fadeOut = AnimationFactory.CreateFadeOut(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));

                DoubleAnimation fadeIn = AnimationFactory.CreateFadeIn(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));
                this.StopImage(fadeOut, () => this.PlayImage(nextImage, fadeIn));
            }
        }

        protected void PlayImage(Uri image, AnimationTimeline animation)
        {
            this.imageEndReached = false;
            this.ReadyImage(image);
            this.imageTimer.Start();
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

        protected void TransitionVideoFadeOut(Action callback)
        {
            DoubleAnimation animation = AnimationFactory.CreateFadeOut(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));
            //much jQuery.on()! wow! C# is so dynamic!
            animation.Completed += new EventHandler((object sender, EventArgs e) => callback());
            this.imageDisplay.BeginAnimation(FrameworkElement.OpacityProperty, animation);
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

        /*
        protected void animationVolumeFadeOut_Completed(object sender, EventArgs e)
        {
            this.mediaPlayer.Stop();
        }
        
        protected void finalVolumeFadeOut_Completed(object sender, EventArgs e)
        {
            //fire EndReached event
            //prepsat tak, aby se vyuzival synchronizing object na timeru a ne tenhle bordel s dispatcherem
            this.imageDisplay.Dispatcher.Invoke(new Action(delegate() { this.Stop(); }), new object[0]);
            this.imageDisplay.Dispatcher.Invoke(new Action(delegate() { this.EndReached(this, null); }), new object[0]);
        }*/

        protected void mediaPlayer_EndReached(object sender, RoutedEventArgs e)
        {
            //using lambda cuz lambdas are so much fun - almost like jQuery
            if (!this.playlistItem.IsSongWaiting())
            {
                // much lambda! such callback! wow!
                //DoubleAnimation fadeOut = AnimationFactory.CreateFadeOut(TimeSpan.FromSeconds(SlideshowMediaProcessor.FadeInSeconds));
               // this.StopImage(fadeOut, () => this.OnImageBatchEndReached());
                //this.TransitionVideoFadeOut(() => this.OnImageBatchEndReached());
                this.StopAudio(()=>this.OnAudioBatchEndReached());
            }
            else
            {
                Uri nextSong = this.playlistItem.PopNextActiveSong();
                this.StopAudio(() => this.PlayAudio(nextSong));
            }
            /*
            Uri nextSong = this.playlistItem.PopNextActiveSong(); 
            if(nextSong != null)
            {
                this.ReadyAudio(nextSong);
                this.FadeInPlayAudio();
            }*/
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
