using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using Nyangoro.Interfaces;

namespace Nyangoro.Plugins.ProgrammeMessenger.Programme
{

    static class Extensions
    {
        public static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + " ..";
        }
    }

    //@TODO: some animation helper to concentrate all animation-related stuff on one place.
    //       Possibly make an application-wide service?
    class ProgrammeController
    {
        const int EventTitleLength = 50;

        Programme programme;
        List<ProgrammeEvent> eventsShowing;
        List<TextBlock> programmeBlocks;

        Grid programmeGrid;
        public UserControl RootControl {get; protected set;}

        double eventAppearanceDelay = 0.3;

        //animation factories
        ProgrammeAnimationTimelineFactory programmeAnimationFactory;

        //coutner for completed timelines in a batch
        int counterTimelinesCompleted = 0;

        public ProgrammeController()
        {
            this.programmeAnimationFactory = new ProgrammeAnimationTimelineFactory();
            this.RootControl = new PresentationProgramme();
            this.programme = new Programme();
            this.programmeBlocks = new List<TextBlock>();
            this.eventsShowing = new List<ProgrammeEvent>();
        }

        public void Init()
        {
            this.programme.Load();

            Grid programmeGrid = (Grid)this.RootControl.FindName("programmeGrid");
            this.programmeGrid = programmeGrid;

            int nrRows = this.programmeGrid.RowDefinitions.Count;

            //fill rows
            for (int i = 0; i < nrRows; i++)
            {
                this.programmeBlocks.Add((TextBlock)this.programmeGrid.Children[i]);
            }
        }

        public void Run()
        {
            this.ShowNextBatch();
        }

        protected void ShowNextBatch()
        {
            List<ProgrammeEvent> events = this.GetNextToShow();
            this.eventsShowing = events;

            for (int i = 0; i < this.eventsShowing.Count; i++)
            {
                TextBlock txtBlock = this.programmeBlocks.ElementAt(i);
                ProgrammeEvent prgEvent = this.eventsShowing.ElementAt(i);

                txtBlock.Inlines.Clear();

                List<Run> runs = this.MakeEventTextRuns(prgEvent);

                foreach (Run run in runs)
                {
                    txtBlock.Inlines.Add(run);
                }
                
                //txtBlock.Text = prgEvent.GetText();
            }

            this.AnimateEvents();
        }

        protected List<Run> MakeEventTextRuns(ProgrammeEvent prgEvent)
        {
            List<Run> runs = new List<Run>();

            //time
            Run timeRun = new Run(prgEvent.start.ToString("HH:mm"));
            timeRun.FontWeight = FontWeights.Bold;
            runs.Add(timeRun);

            //location
            Run locRun = new Run(prgEvent.location);
            runs.Add(locRun);

            //title
            Run titleRun = new Run(prgEvent.title.Truncate(ProgrammeController.EventTitleLength));
            titleRun.FontWeight = FontWeights.Bold;
            runs.Add(titleRun);

            //author
            Run authorRun = new Run("("+prgEvent.author+")");
            runs.Add(authorRun);

            //add spaces
            runs.ForEach(r => r.Text += "   ");

            return runs;
        }

 

        protected List<ProgrammeEvent> GetNextToShow()
        {
            List<ProgrammeEvent> events = this.programme.GetNextBatchRandom(this.programmeBlocks.Count);
            this.SortEventsByStart(ref events);
            return events;
        }

        protected void AnimateEvents()
        {
            for (int i = 0; i < this.eventsShowing.Count; i++)
            {
                TextBlock programmeBlock = this.programmeBlocks[i];

                ParallelTimeline timeline = this.programmeAnimationFactory.GetTimeline();
                timeline.BeginTime = TimeSpan.FromSeconds(this.eventAppearanceDelay * i);

                ClockGroup clockGroup = timeline.CreateClock();
                programmeBlock.ApplyAnimationClock(TextBlock.OpacityProperty, (AnimationClock)clockGroup.Children[0]);
                clockGroup.Completed += new EventHandler(this.timeline_Completed);
            }
        }

        protected void timeline_Completed(object sender, EventArgs args)
        {
            this.counterTimelinesCompleted++;

            if (counterTimelinesCompleted == this.GetCountBatchTimelines())
            {
                counterTimelinesCompleted = 0;
                this.ShowNextBatch();
            }
        }

        protected int GetCountBatchTimelines()
        {
            return this.eventsShowing.Count;
        }

        public void Hide()
        {
            this.RootControl.Opacity = 0;
        }

        public void Show()
        {
            this.RootControl.Opacity = 1;
        }

        protected void SortEventsByStart(ref List<ProgrammeEvent> events)
        {
            events = events.OrderBy(i => i.start).ToList();
        }
    }
}
