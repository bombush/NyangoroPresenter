using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Nyangoro.Plugins.ProgrammeMessenger
{
    public class ProgrammeMessengerController : Nyangoro.Plugins.PluginController
    {
        Programme.ProgrammeController programmeController;
        Messenger.MessengerController messengerController;

        TextBox messageTextBox;

        public ProgrammeMessengerController(ProgrammeMessenger core, ControlRoot controlRoot, PresentationRoot presentationRoot) : base(core, controlRoot, presentationRoot)
        {
        }

        public void Init()
        {
            this.programmeController = new Programme.ProgrammeController();
            programmeController.Init();

            this.messengerController = new Messenger.MessengerController();
            messengerController.Init();

            programmeController.Show();
            messengerController.Hide();

            Grid contentGrid = (Grid)this.PresentationRoot.Content;
            contentGrid.Children.Add(programmeController.RootControl);
            contentGrid.Children.Add(messengerController.RootControl);

            this.messageTextBox = (TextBox)this.ControlRoot.FindName("ProgrammeMessengerMessageTextBox");
        }

        public void Run()
        {
            this.programmeController.Run();
            this.messengerController.Run();
        }

        public void HandleProgrammerMessengerToggleMessageClick(object sender, RoutedEventArgs e)
        {
            ToggleButton originator = (ToggleButton)sender;
            if ((bool)originator.IsChecked)
            {
                this.programmeController.Hide();
                this.messengerController.Show();
            }
            else
            {
                this.messengerController.Hide();
                this.programmeController.Show();
            }
        }

        public void HandleProgrammeMessengerUpdateClick()
        {
            this.messengerController.Message = this.messageTextBox.Text;
        }

    }
}
