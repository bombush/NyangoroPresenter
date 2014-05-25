using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows;

namespace Nyangoro.Core.Layout
{
    [Export(typeof(Nyangoro.Core.Layout.LayoutManager))]
    public class LayoutManager
    {
        //Windows
        public Window controlWindow { get; private set; }
        public Window presentationWindow { get; private set; }

        public void InitWindows()
        {
            Window winControl = new Nyangoro.Core.Host.ControlWindow();
            this.controlWindow = winControl;
            winControl.Show();

            Window winPres = new Nyangoro.Core.Host.PresentationWindow();
            this.presentationWindow = winPres;
            winPres.Show();
        }

        public void BuildLayout()
        {
        }
    }
}
