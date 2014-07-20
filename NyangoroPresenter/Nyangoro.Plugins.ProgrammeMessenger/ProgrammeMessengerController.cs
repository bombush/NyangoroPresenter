using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nyangoro.Plugins.ProgrammeMessenger
{
    public class ProgrammeMessengerController : Nyangoro.Plugins.PluginController
    {
        Programme.ProgrammeController programmeController;

        public ProgrammeMessengerController(ProgrammeMessenger core, ControlRoot controlRoot, PresentationRoot presentationRoot) : base(core, controlRoot, presentationRoot)
        {
        }

        public void Init()
        {
            this.programmeController = new Programme.ProgrammeController();
            programmeController.Init();

            this.PresentationRoot.Content = programmeController.RootControl;
        }

        public void Run()
        {
            this.programmeController.Run();
        }

    }
}
