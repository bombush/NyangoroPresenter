using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nyangoro.Plugins.ProgrammeMessenger.Programme
{
    class Programme
    {
        public struct FilterMode
        {
            public const int All = 0;
            public const int UpcomingToday = 1;
        }

        //FILTER MODE ALL/Upcoming today
        int filterMode = Programme.FilterMode.UpcomingToday;

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
                //tady by to chtelo sjednotit filtry, aby to vsechno prochazelo jednotnym filterm s jednotnymi podminkami
                if(evt.start.Hour >= DateTime.Now.Hour)
                    this.activePool.Add(evt);
                #else
                if(this.filterMode == Programme.FilterMode.UpcomingToday){
                    if (evt.start.Hour < DateTime.Now.Hour || evt.start.Day != DateTime.Now.Day)
                        continue;
                }
                
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

                //add to programme
                programmeEvents.Add(this.activePool.ElementAt(random));

                //add to last unfilled position in selected numbers
                for (int x = 0; x < selectedNumbers.Length; x++)
                {
                    if (selectedNumbers[x] == -1)
                    {
                        selectedNumbers[x] = random;
                        break;
                    }
                }
            }

            return programmeEvents;
        }

        //filter out events that are not applicable for displaying
        // implement using a new Thread (jus'for fun)
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
                if(this.filterMode == Programme.FilterMode.UpcomingToday){
                    if (evt.start.Hour < DateTime.Now.Hour)
                        return false;

                    if (evt.start.Day != DateTime.Now.Day)
                        return false;
                }

                return true;
            #endif
        }
    }
}
