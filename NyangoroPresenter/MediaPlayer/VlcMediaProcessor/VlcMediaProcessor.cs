using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using LibVLC.NET;
using LibVLC.NET.Presentation;


namespace Nyangoro.Plugins.MediaPlayer
{

    static class Extensions
    {
        public static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + " ..";
        }
    }

    class VlcMediaProcessor : IMediaProcessor
    {
        public event EventHandler EndReached;

        //CONST
        public const int LabelTextLength = 43;

        private string[] playableFileTypes = { ".avi", ".mp4", ".mkv"}; 

        private Nyangoro.Plugins.MediaPlayer.VlcDisplayControl displayRoot;
        private LibVLC.NET.Presentation.MediaElement mediaElement;
        private PlaylistItem activeItem;

        private TextBlock textDisplay;

        //dalsi prasarna
        public Grid VlcTextGrid { get; set; }

        public VlcMediaProcessor(Grid vlcTextGrid)
        {
            this.VlcTextGrid = vlcTextGrid;

            this.displayRoot = new Nyangoro.Plugins.MediaPlayer.VlcDisplayControl();
            this.mediaElement = (LibVLC.NET.Presentation.MediaElement)this.displayRoot.FindName("MediaElement");

            //this.textDisplay = new TextBlock();
            //this.textDisplay = (TextBlock)this.displayRoot.FindName("VlcTextBlock");


            this.textDisplay = (TextBlock)this.VlcTextGrid.FindName("VlcTextBlock");

            try
            {
                //this.textGrid = (Grid)this.displayRoot.FindName("VlcTextGrid");

                //PRASAAAARNA!!!! REFACTORR!!!!!!
                Image label = (Image)VlcTextGrid.FindName("Label");
                //string uri = (Path.Combine(Config.Get("working_dir"), this.screenPath, "images", "natsulogo.png"));
                label.Source = new BitmapImage(new Uri(Path.Combine(Directory.GetCurrentDirectory(), "plugins", "mediaplayer", "images", "label.png")));
            }
            catch { MessageBox.Show("Could not find Vlc label png"); }

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
            this.textDisplay.Text = Path.GetFileNameWithoutExtension(this.activeItem.path.LocalPath).Truncate(VlcMediaProcessor.LabelTextLength);
            this.FadeInTextDisplay();

            if (File.Exists(this.activeItem.path.LocalPath))
                this.mediaElement.Play();
            else
            {
                this.Stop();
                this.EndReached(this, EventArgs.Empty);
            }
        }

        public void Stop()
        {
            this.HideTextDisplay();
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
            this.HideTextDisplay();

            this.EndReached(this, EventArgs.Empty);
        }

        protected void FadeInTextDisplay()
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1, new System.Windows.Duration(TimeSpan.FromSeconds(1)));
            //DoubleAnimation animation2 = new DoubleAnimation(0, 1, new System.Windows.Duration(TimeSpan.FromSeconds(1)));

            this.VlcTextGrid.Visibility = Visibility.Visible;
            this.VlcTextGrid.BeginAnimation(Grid.OpacityProperty, animation);
            //I do this because of a bug(?) which displays the label on switch to Slideshow
            //((Image)this.VlcTextGrid.FindName("Label")).BeginAnimation(Image.OpacityProperty, animation2);
        }

        protected void HideTextDisplay()
        {
            this.VlcTextGrid.Opacity = 0;
            this.VlcTextGrid.Visibility = Visibility.Hidden;
        }
    }
}
//@TODO ucesat playing and IsPlaying
