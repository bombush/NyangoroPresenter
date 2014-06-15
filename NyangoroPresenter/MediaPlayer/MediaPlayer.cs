using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nyangoro.Interfaces;

namespace Nyangoro.Plugins.MediaPlayer
{
    public class MediaPlayer : Nyangoro.Plugins.Plugin
    {
        public MediaPlayer()
        {
            this.presentationRoot = new PresentationRoot();
        }
    }
}
