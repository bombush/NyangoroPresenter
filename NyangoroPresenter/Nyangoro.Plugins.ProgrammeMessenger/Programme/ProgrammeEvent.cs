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

        public ProgrammeEvent()
        {
        }

        public string GetText()
        {
            return this.title;
        }
    }
}
