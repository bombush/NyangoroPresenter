using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace Nyangoro.Plugins.Clock
{
    public class Clock : Nyangoro.Plugins.Plugin
    {
       // protected const double FontSize = 70;

        DateTime lastUpdate;

        public PresentationRoot PresentationRoot
        {
            get { return (PresentationRoot)this.presentationRoot; }
            private set { this.presentationRoot = value; }
        }

        public Clock()
            : base()
        {
            this.presentationRoot = new PresentationRoot();
        }

        public override bool Init()
        {
            base.Init();

            this.lastUpdate = DateTime.Now;

            return true;
        }

        public override void Run()
        {
            base.Run();

            this.RunClock();              
        }

        protected void RunClock()
        {
            TextBlock hour = new TextBlock();
           // hour.FontSize = Clock.FontSize;
            hour.Text = this.lastUpdate.ToString("HH");

            TextBlock colon = new TextBlock();
            colon.Text = ":";
         //   colon.FontSize = Clock.FontSize;

            TextBlock minute = new TextBlock();
            minute.Text = this.lastUpdate.ToString("mm");
         //   minute.FontSize = Clock.FontSize;

            StackPanel textGrid = (StackPanel)this.PresentationRoot.Content;
            textGrid.Children.Add(hour);
            textGrid.Children.Add(colon);
            textGrid.Children.Add(minute);

            //create and run clock colon animation
            DoubleAnimationUsingKeyFrames animation = this.CreateColonAnimation();
            colon.BeginAnimation(TextBlock.OpacityProperty, animation);


            Timer secondTimer = new Timer(1000);
            secondTimer.Elapsed += (sender, e) => this.RefreshClock(hour, minute);
            secondTimer.Start();
        }

        protected DoubleAnimationUsingKeyFrames CreateColonAnimation()
        {
            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();
            animation.RepeatBehavior = RepeatBehavior.Forever;
            animation.AutoReverse = true;


            List<LinearDoubleKeyFrame> keyFrames = new List<LinearDoubleKeyFrame>();

            keyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
            keyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.1))));
            keyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2))));
            keyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5))));

            foreach (LinearDoubleKeyFrame keyFrame in keyFrames)
            {
                animation.KeyFrames.Add(keyFrame);
            }

            return animation;
        }

        protected void RefreshClock(TextBlock hour, TextBlock minute)
        {
            if (DateTime.Now.Minute != this.lastUpdate.Minute)
                minute.Dispatcher.BeginInvoke((Action)delegate() { minute.Text = DateTime.Now.ToString("mm"); }, new object[0]);

            if (DateTime.Now.Hour != this.lastUpdate.Hour)
                minute.Dispatcher.BeginInvoke((Action)delegate() { hour.Text = DateTime.Now.ToString("HH"); }, new object[0]);

            this.lastUpdate = DateTime.Now;
        }
    }
}
