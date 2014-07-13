using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nyangoro.Plugins;
using Nyangoro.Interfaces;

namespace Nyangoro.Plugins.ProgrammeMessenger
{
    /**
     * This plugin displays programme information and custom messages
     * on the bottom of the presentation screen.
     * 
     * @TODO: split into two plugins that will be switched between - ProgrammeDisplay
     * and CustomMessagesDisplay
     */
    public class ProgrammeMessenger : Nyangoro.Plugins.Plugin
    {
        new public ProgrammeMessengerController Controller { get { return (ProgrammeMessengerController)this.controller; } set { this.controller = value; } }

        public Nyangoro.Plugins.ProgrammeMessenger.ControlRoot ControlRoot { get { return (ControlRoot)this.controlRoot; } set { this.controlRoot = value; } }

        public ProgrammeMessenger()
        {
            this.presentationRoot = new PresentationRoot();
            this.controlRoot = new ControlRoot();
        }
    }
}
