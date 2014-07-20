using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Nyangoro.Interfaces;

namespace Nyangoro.Plugins.MediaPlayer
{

    public class MediaPlayer : Nyangoro.Plugins.Plugin
    {
        //IMPORTANT!!!
        //STRIBRO: Properties are fine but not writing the Capital letter when accessing from the instance can screw stuff up pretty badly 
        new public MediaPlayerController Controller { get { return (MediaPlayerController)this.controller; } set { this.controller = value; } }

        public Nyangoro.Plugins.MediaPlayer.ControlRoot ControlRoot { get { return (ControlRoot)this.controlRoot; } set { this.controlRoot = value; } }

        public List<IMediaProcessor> processors { get; private set; }
        //IMediaProcessor activeProcessor;
        public Playlist Playlist { get; private set; }

        //Contains a list of custom file types for playlist items
        public static struct CustomFileTypes
        {
            public static readonly string ImageBatch = "ImageBatch";
        }

        public MediaPlayer()
        {
            this.presentationRoot = new PresentationRoot();
            this.controlRoot = new ControlRoot();
            this.Controller = new MediaPlayerController(this, (ControlRoot)this.controlRoot, (PresentationRoot)this.presentationRoot);
            this.Playlist = new Playlist(this.ControlRoot.GetPlaylistBox(), this);
            this.processors = new List<IMediaProcessor>();

            ControlRoot controlRoot = this.controlRoot as ControlRoot;
            controlRoot.SetController(this.Controller);
        }

        public override bool Init()
        {
            base.Init();

            this.LoadProcessors();
            this.Controller.BindPlaylistToControl();

            return true;
        }

        public override void Run()
        {
            base.Run();
        }

        private void LoadProcessors()
        {
            this.processors.Add(new VlcMediaProcessor());
        }
    }
}
