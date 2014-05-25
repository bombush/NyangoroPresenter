using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows;

namespace Nyangoro.Core.Layout
{
    /*
     * Layout manager takes care of integrating module outputs into the respective
     * windows.
     */
    [Export(typeof(Nyangoro.Core.Layout.LayoutManager))]
    public class LayoutManager
    {
        private Window controlWindow;
        private Window presentationWindow;
        
        public LayoutManager(Window controlWindow, Window presentationWindow)
        {
            this.controlWindow = controlWindow;
            this.presentationWindow = presentationWindow;
        }

        public void BuildLayout()
        {
        }
    }
}
