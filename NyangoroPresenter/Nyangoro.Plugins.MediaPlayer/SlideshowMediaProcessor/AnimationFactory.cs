using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;

namespace Nyangoro.Plugins.MediaPlayer
{
    public class AnimationFactory
    {
        public static DoubleAnimation CreateFadeIn(TimeSpan length)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1, new System.Windows.Duration(length));
            return animation;
        }

        public static DoubleAnimation CreateFadeOut(TimeSpan length)
        {
            DoubleAnimation animation = new DoubleAnimation(1, 0, new System.Windows.Duration(length));
            return animation;
        }
    }
}
