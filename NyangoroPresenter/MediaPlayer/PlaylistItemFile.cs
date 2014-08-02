using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nyangoro.Plugins.MediaPlayer
{
    class PlaylistItemFile : PlaylistItem
    {
        //vyresit kdyz ukazuju na neexuiswtujici soubor
        public PlaylistItemFile(List<IMediaProcessor> processors, string filepath) : base(processors)
        {
            this.path = new Uri(filepath);
            this.extension = Path.GetExtension(filepath);
            this.displayName = Path.GetFileNameWithoutExtension(filepath);

            this.AssignMediaProcessor(processors);
            this.AutoSetPlayable();
        }

        /**
         * Searches for a IMediaProcessor in the passes list of processors and assigns
         * the one that can play the file.
         */ 
        public override void AssignMediaProcessor(List<IMediaProcessor> processors)
        {
            base.AssignMediaProcessor(processors);

            foreach(IMediaProcessor processor in processors)
            {
                if (processor.GetPlayableFileTypes().Contains(this.extension))
                {
                    this.processor = processor;
                }
            }
        }
    }
}
