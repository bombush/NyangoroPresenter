using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Nyangoro.Plugins.MediaPlayer
{
    class PlaylistItemImageBatch : PlaylistItem
    {

        // tyhle listy prepsat jako tridu, ktera se o predavani, mazani a reset
        // bude starat sama (SelfRefreshingList)
        static List<Uri> GlobalImagesToPlay;
        static List<Uri> GlobalAudioToPlay;
        static string[] ImageTypes = { ".jpg", ".png", ".jpeg", ".gif", ".bmp"};
        static string[] AudioTypes = { ".wma", ".mp3", ".flac", ".wav"};

        public List<Uri> activeImageBatch {get; protected set;}
        public List<Uri> activeAudioBatch { get; protected set; }

        string imageDir;
        string audioDir;

        protected const int ImagesInBatch = 6;
        public const int ImageDisplaySec = 15;
        protected const int SongsInBatch = 100;

        public const int TypeImage = 1;
        public const int TypeAudio = 2;

        public PlaylistItemImageBatch(List<IMediaProcessor> processors, Nyangoro.Plugins.MediaPlayer.MediaPlayer pluginCore) : base(processors)
        {
            this.displayName = "--image batch--";

            this.imageDir = pluginCore.Dir + "\\slideshow\\images\\";
            this.audioDir = pluginCore.Dir + "\\slideshow\\audio\\";

            this.activeImageBatch = new List<Uri>();
            this.activeAudioBatch = new List<Uri>();

            //this should probably be moved to the abstract constructor
            this.AssignMediaProcessor(processors);
            this.AutoSetPlayable();
        }

        public override void AssignMediaProcessor(List<IMediaProcessor> processors)
        {
            foreach (IMediaProcessor processor in processors)
            {
                if (processor.GetPlayableFileTypes().Contains(MediaPlayer.CustomFileTypes.ImageBatch))
                {
                    this.processor = processor;
                }
            }
        }

        public override void Play()
        {
            //init static batches if required
            if (PlaylistItemImageBatch.GlobalAudioToPlay == null)
                PlaylistItemImageBatch.GlobalAudioToPlay = new List<Uri>();

            if (PlaylistItemImageBatch.GlobalImagesToPlay == null)
                PlaylistItemImageBatch.GlobalImagesToPlay = new List<Uri>();

            //init all batches
            if (this.activeImageBatch == null || this.activeImageBatch.Count == 0)
                this.SetRandomBatchImages();

            if (this.activeAudioBatch == null || this.activeAudioBatch.Count == 0)
                this.SetRandomBatchAudio();

            base.Play();
        }

        public Uri PopNextActiveImage()
        {
            if (this.activeImageBatch.Count == 0)
                return null;

            Uri item = this.activeImageBatch[0];
            this.activeImageBatch.RemoveAt(0);
            return item;
        }

        public Uri PopNextActiveSong()
        {
            if (this.activeAudioBatch.Count == 0)
            {
                this.SetRandomBatchAudio();
                
                if (this.activeAudioBatch.Count == 0)
                {
                    return null;
                }
            }

            Uri item = this.activeAudioBatch[0];
            this.activeAudioBatch.RemoveAt(0);
            return item;
        }

        public bool IsImageWaiting()
        {
            return (this.activeImageBatch.Count > 0);
        }

        public bool IsSongWaiting()
        {
            return (this.activeAudioBatch.Count > 0);
        }

        public void SetRandomBatchImages()
        {
            if (PlaylistItemImageBatch.GlobalImagesToPlay.Count == 0)
                PlaylistItemImageBatch.AutoFillUriList(this.imageDir, PlaylistItemImageBatch.TypeImage);

            Playlist.ShuffleList<Uri>(PlaylistItemImageBatch.GlobalImagesToPlay);

            this.activeImageBatch = this.PopRandomBatchFromList(PlaylistItemImageBatch.ImagesInBatch, PlaylistItemImageBatch.GlobalImagesToPlay);
        }

        public void SetRandomBatchAudio()
        {
            if (PlaylistItemImageBatch.GlobalAudioToPlay.Count == 0)
                PlaylistItemImageBatch.AutoFillUriList(this.audioDir, PlaylistItemImageBatch.TypeAudio);

            this.activeAudioBatch = this.PopRandomBatchFromList(PlaylistItemImageBatch.SongsInBatch, PlaylistItemImageBatch.GlobalAudioToPlay);
        }

        protected static void AutoFillUriList(string dir, int type)
        {
            string[] filePaths = Directory.GetFiles(dir);
            foreach (string path in filePaths)
            {
                if (type == PlaylistItemImageBatch.TypeImage && PlaylistItemImageBatch.ImageTypes.Contains(Path.GetExtension(path)))
                {
                    PlaylistItemImageBatch.GlobalImagesToPlay.Add(new Uri(path));
                } 
                else if(type == PlaylistItemImageBatch.TypeAudio && PlaylistItemImageBatch.AudioTypes.Contains(Path.GetExtension(path))) 
                {
                    PlaylistItemImageBatch.GlobalAudioToPlay.Add(new Uri(path));
                }
           }
        }

        protected List<Uri> PopRandomBatchFromList(int count, List<Uri> list)
        {
            List<Uri> outList = new List<Uri>();
            Random random = new Random();

            while (outList.Count < count && list.Count > 0)
            {
                int rnd = random.Next(0 , (list.Count-1));
                outList.Add(list.ElementAt(rnd));
                list.RemoveAt(rnd);
            }
            
            return outList;
        }

        //in-code config numbers. Bad bad bad! (Length calculation implementation postponed until after natsu)
        /*
        protected override void CalculateLength()
        {
            if (this.activeAudioBatch == null || this.activeImageBatch.Count == 0)
                this.Length = TimeSpan.FromSeconds(this.activeImageBatch.Count * 30);
            else
                base.CalculateLength();
        }
         * */
    }
}
