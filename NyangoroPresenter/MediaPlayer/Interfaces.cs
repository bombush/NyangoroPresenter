using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using Nyangoro.Interfaces;


namespace Nyangoro.Plugins.MediaPlayer
{
    public interface IMediaProcessor
    {
        event EventHandler EndReached;

        //Get roots element to append to the plugin root
        FrameworkElement GetRootElement();

        //Gets an array of types the implementing processor is capable of playing
        string[] GetPlayableFileTypes();

        PlaylistItem GetActiveItem();
        void SetActiveItem(PlaylistItem item);

        //Start playback
        void Play();
        //Stop playback and free any unneeded resources
        void Stop();
        //Pause Playback
        void Pause();

        //TO BE IMPLEMENTED
       // bool IsPlaying();
       // TimeSpan CalculateItemLength(PlaylistItem item);
    }
}
