using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Nyangoro.Interfaces;


namespace Nyangoro.Plugins.MediaPlayer
{
    public interface IMediaProcessor
    {
        //Get roots element to append to the plugin root
        FrameworkElement GetRootElement();

        //neexistuje tady neco jako trida FileType??
        //Gets an array of types the implementing processor is capable of playing
        string[] GetPlayableTypes();

        Uri fileToPlay { get; set; }
        void Play();
        void Stop();
        void Pause();
    }
}
