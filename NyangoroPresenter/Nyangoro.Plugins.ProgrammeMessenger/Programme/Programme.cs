using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nyangoro.Plugins.ProgrammeMessenger.Programme
{
    class Programme
    {
        List<ProgrammeEvent> events;

        public Programme()
        {
            this.events = new List<ProgrammeEvent>();
        }

        public void LoadFromXml()
        {
            if(!this.TryLoadFromNyangoroXml())
                this.TryLoadFromCondroidXml();
        }

        protected bool TryLoadFromNyangoroXml()
        {
            return false;
        }

        protected bool TryLoadFromCondroidXml()
        {
            ProgrammeIOCondroid condroidIO = new ProgrammeIOCondroid();
            try
            {
                this.events = condroidIO.GetMainStageUpcomingEvents();
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

            if (this.events.Count <= number)
                return this.events;

            for (int i = 0; i < number; i++)
            {
                int random;

                do
                {
                  random = rnd.Next(0, (this.events.Count - 1));
                }
                while(selectedNumbers.Contains(random));

                programmeEvents.Add(this.events.ElementAt(random));
            }

            return programmeEvents;
        }
    }
}
