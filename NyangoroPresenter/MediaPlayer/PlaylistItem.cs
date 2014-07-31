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
        public event EventHandler EndReached;

        public string extension { get; protected set; }
        public Uri path { get; protected set;}
        public bool playable { get; set; }

        protected string displayName { get; set; }

        public IMediaProcessor processor {get; protected set;}

        /* Length handling postponed until after Natsu
         * 
        protected TimeSpan length;
        public TimeSpan Length
        { 
            get
            {
              if(this.length == null)
                 this.CalculateLength();

              return this.length;
            }
            protected set;
        }*/

        public PlaylistItem(List<IMediaProcessor> processors)
        {
        }

        public override string ToString()
        {
            return this.displayName;
        }

        #region playback
        public virtual void Play()
        {
            if (this.processor == null)
                return;

            if (this.processor.GetActiveItem() != this)
                this.processor.SetActiveItem(this);

            this.BindProcessorEvents();

            this.processor.Play();
        }

        public virtual void Stop()
        {
            this.UnbindProcessorEvents();
            this.processor.Stop();
        }

        public virtual void Pause()
        {
            this.processor.Pause();
        }

        #endregion


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

        protected void BindProcessorEvents()
        {
           if(this.processor != null)
                this.processor.EndReached += this.processor_EndReached;
        }

        protected void UnbindProcessorEvents()
        {
            if (this.processor != null)
                this.processor.EndReached -= this.processor_EndReached;
        }

        //bubble up the end reached event
        public void processor_EndReached(object sender, EventArgs e)
        {
            this.UnbindProcessorEvents();
            if(this.EndReached != null)
                this.EndReached(this, EventArgs.Empty);
        }

        /* Postponed until after Natsu
        protected virtual void CalculateLength()
        {
            this.length = this.processor.CalculateItemLength(this);
        }
         * */
    }
}
