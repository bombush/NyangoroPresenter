using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Nyangoro.Interfaces;

namespace Nyangoro.Plugins.MediaPlayer
{

    public class MediaPlayer : Nyangoro.Plugins.Plugin
    {
        MediaPlayerController controller;

        List<IMediaProcessor> processors;
        IMediaProcessor activeProcessor;

        public MediaPlayer()
        {
            this.presentationRoot = new PresentationRoot();
            this.controlRoot = new ControlRoot();
            this.controller = new MediaPlayerController(this, (ControlRoot)this.controlRoot, (PresentationRoot)this.presentationRoot);

            ControlRoot controlRoot = this.controlRoot as ControlRoot;
            controlRoot.SetController(controller);
        }

        public override bool Init()
        {
            base.Init();

            this.LoadProcessors();

            return true;
        }

        public override void Run()
        {
            base.Run();
        }

        private void LoadProcessors()
        {
        }
    }
}
