using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nyangoro.Plugins.ProgrammeMessenger.Programme
{
    class ProgrammeEvent
    {
        public int id { get; set; }
        public string title { get; set; }
        public DateTime start { get; set; }
        public string location { get; set; }
        public string author { get; set; }

        public ProgrammeEvent()
        {
        }

        public string GetText()
        {
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("cs-CZ");
           // return culture.DateTimeFormat.GetDayName(this.start.DayOfWeek) + "  " + this.start.ToString("HH:mm");
            return this.start.ToString("HH:mm") + "   " + this.location + "     " + this.title;
        }
    }
}
