using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nyangoro.Plugins.MediaPlayer
{
    class PlaylistItemImageBatch : PlaylistItem
    {
        public override void AssignMediaProcessor(List<IMediaProcessor> processors)
        {
            foreach (IMediaProcessor processor in processors)
            {
                if (processor.GetPlayableFileTypes().Contains(MediaPlayer.CustomFileTypes.ImageBatch))
                {
                    this.processor = processor;
                    this.BindProcessorEvents();
                }
            }
        }
    }
}
