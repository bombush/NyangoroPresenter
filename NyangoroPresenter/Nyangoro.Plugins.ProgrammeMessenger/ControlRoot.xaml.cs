using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nyangoro.Plugins.ProgrammeMessenger
{
    /// <summary>
    /// Interaction logic for ControlRoot.xaml
    /// </summary>
    public partial class ControlRoot : Nyangoro.Plugins.PluginControlRoot
    {
        new public ProgrammeMessengerController Controller
        {
            get { return (ProgrammeMessengerController)this.controller; }
            private set { this.controller = value; }
        }

        public ControlRoot()
        {
            InitializeComponent();
        }

        public void SetController(ProgrammeMessengerController controller)
        {
            this.Controller = controller;
        }

        private void ProgrammeMessengerToggleMessage_Click(object sender, RoutedEventArgs e)
        {
            this.Controller.HandleProgrammerMessengerToggleMessageClick(sender, e);
        }

        private void ProgrammeMessengerUpdate_Click(object sender, RoutedEventArgs e)
        {
            this.Controller.HandleProgrammeMessengerUpdateClick();
        }
    }
}
