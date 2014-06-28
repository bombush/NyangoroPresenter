using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nyangoro.Plugins.SamplePlugin
{
    public class SamplePluginController : PluginController
    {
        public SamplePluginController(SamplePlugin core, ControlRoot controlRoot, PresentationRoot presentationRoot) : base(core, controlRoot, presentationRoot)
        {
        }

        public void HandleButtonClick()
        {
            Grid rootGrid = this.presentationRoot.FindName("RootGrid") as Grid;
            TextBlock textblock = new TextBlock();
            textblock.Text = "blablabla";
            rootGrid.Children.Add(textblock);

        }
    }
}
