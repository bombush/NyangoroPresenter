using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;

namespace Nyangoro.Plugins.MediaPlayer
{
    abstract public class PlaylistItem
    {
        public string extension { get; protected set; }
        public Uri path { get; protected set;}
        public bool playable { get; set; }

        protected string displayName { get; set; }

        public IMediaProcessor processor {get; protected set;}

        public PlaylistItem(List<IMediaProcessor> processors)
        {
        }

        public override string ToString()
        {
            return this.displayName;
        }

        public virtual void Play()
        {
            if (this.processor == null)
                return;

            if (this.processor.GetActiveItem() != this)
                this.processor.SetActiveItem(this);

            this.processor.Play();
        }

        public virtual void DisplayMedia(MediaPlayerController controller)
        {
            if(this.processor != null)
                controller.DisplayMedia((FrameworkElement)this.processor.GetRootElement());
        }

        public virtual void AssignMediaProcessor(List<IMediaProcessor> processors)
        {
        }

        /**
         * Sets a flag whether the file can be played or not
         */
        public virtual void AutoSetPlayable()
        {
            if (this.processor == null)
                this.playable = false;
            else
                this.playable = true;
        }
    }
}
