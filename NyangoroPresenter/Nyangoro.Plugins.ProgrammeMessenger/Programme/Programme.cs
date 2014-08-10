using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nyangoro.Plugins.ProgrammeMessenger.Programme
{
    class Programme
    {
        //all events in programme
        List<ProgrammeEvent> allEvents;

        // the active pool from which events are selected to display
        // is refreshed/filtered when fetching a new batch to conform to
        // whatever requirements are there for displaying programme
        List<ProgrammeEvent> activePool;

        public Programme()
        {
            this.allEvents = new List<ProgrammeEvent>();
            this.activePool = new List<ProgrammeEvent>();
        }

        public void Load()
        {
            this.LoadFromXml();
            this.InitActivePool();
        }

        protected void LoadFromXml()
        {
            if(!this.TryLoadFromNyangoroXml())
                this.TryLoadFromCondroidXml();
        }

        protected bool TryLoadFromNyangoroXml()
        {
            return false;
        }

        //Adds all today's events to the active pool
        // Or later we can use some arbitrary filters using lambda expressions.. sweet
        protected void InitActivePool()
        {
            foreach (ProgrammeEvent evt in this.allEvents)
            {
                #if DEBUG
                //tady by to chtelo sjednotit filtry
                if(evt.start.Hour >= DateTime.Now.Hour)
                    this.activePool.Add(evt);
                #else
                if (evt.start.Day == DateTime.Now.Day)
                    this.activePool.Add(evt);
                #endif
            }
        }

        protected bool TryLoadFromCondroidXml()
        {
            ProgrammeIOCondroid condroidIO = new ProgrammeIOCondroid();
            try
            {
                this.allEvents = condroidIO.GetMainStageUpcomingEvents();
            }
            catch
            {
                return true;
            }
            return true;
        }

        //BACHA NA OUT OF RANGE EXCEPTION
        public List<ProgrammeEvent> GetNextBatchRandom(int number)
        {
            List<ProgrammeEvent> programmeEvents = new List<ProgrammeEvent>();
            Random rnd = new Random();
            int[] selectedNumbers = new int[number];
            //init array. 0 is a valid index in a List
            for (int i = 0; i < selectedNumbers.Length; i++)
                selectedNumbers[i] = -1;

            this.CleanActivePool();

            if (this.activePool.Count <= number)
                return this.activePool;

            for (int i = 0; i < number; i++)
            {
                int random;

                do
                {
                  random = rnd.Next(0, (this.activePool.Count - 1));
                }
                while (selectedNumbers.Contains(random));

                programmeEvents.Add(this.activePool.ElementAt(random));
            }

            return programmeEvents;
        }

        //filter out events that are not applicable for displaying
        protected void CleanActivePool()
        {
            List<ProgrammeEvent> temp = new List<ProgrammeEvent>();

            foreach (ProgrammeEvent evt in this.activePool)
            {                
                if (this.IsEventApplicableForDisplay(evt))
                {
                    temp.Add(evt);
                }
            }

            this.activePool = temp;
        }

        protected bool IsEventApplicableForDisplay(ProgrammeEvent evt)
        {
            #if DEBUG
            if (evt.start.Hour < DateTime.Now.Hour)
                return false;
                return true;
            #else
                if (evt.start < DateTime.Now)
                    return false;

                if (evt.start.Day != DateTime.Now.Day)
                    return false;

                return true;
            #endif
        }
    }
}
