using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nyangoro.Core.Layout
{
    public class ControlScreen
    {
        /*
         * @TODO: Use Canvas instead of Grid to make elements movable
         */
        public Panel rootElement;

        public ControlScreen(Panel rootElement)
        {
            if (rootElement == null)
                throw new Exception("Control screen root element missing!");

            this.rootElement = rootElement;
        }

        public void DisplayPlugin(Nyangoro.Interfaces.IPlugin plugin)
        {
            if (plugin.GetControlRoot() != null)
            {
                this.rootElement.Children.Add(plugin.GetControlRoot() as UIElement);
            }
        }
    }
}
