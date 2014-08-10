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
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows;


namespace Nyangoro.Core.Host
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ControlWindow : Window
    {
        public event PresentationModeToggleEventHandler PresentationModeToggle;

        public delegate void PresentationModeToggleEventHandler(object sender, PresentationModeToggleEventArgs e);

        public class PresentationModeToggleEventArgs : EventArgs
        {
            public ToggleButton toggle;

            public PresentationModeToggleEventArgs(ToggleButton toggle)
            {
                this.toggle = toggle;
            }
        }

        public ControlWindow()
        {
            InitializeComponent();
        }

        //bubble up the event to App core
        private void TogglePresentationMode_Click(object sender, RoutedEventArgs e)
        {
            PresentationModeToggleEventArgs args = new PresentationModeToggleEventArgs((ToggleButton)sender);
            this.PresentationModeToggle(this, args);
        }
    }
}
