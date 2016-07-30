using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;

namespace Nyangoro.Plugins.ProgrammeMessenger.Programme
{
    class ProgrammeAnimationTimelineFactory
    {
        public ParallelTimeline GetTimeline()
        {
            ParallelTimeline timeline = new ParallelTimeline();

            //create children
            DoubleAnimationUsingKeyFrames animation1 = this.CreateAnimationOpacityAppear();

            //add children
            timeline.Children.Add(animation1);

            return timeline;
        }

        public DoubleAnimationUsingKeyFrames CreateAnimationOpacityAppear()
        {
            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();

            List<LinearDoubleKeyFrame> keyFrames = new List<LinearDoubleKeyFrame>();
            keyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1))));
            keyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(20))));
            keyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(22))));
            keyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(27))));

            foreach(LinearDoubleKeyFrame keyFrame in keyFrames){
                animation.KeyFrames.Add(keyFrame);
            }

            return animation;
        }
    }
}
