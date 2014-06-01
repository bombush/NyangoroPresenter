using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Nyangoro.Core.Layout
{
    class Screen
    {
        public FrameworkElement rootElement { get; private set; }

        public Screen(FrameworkElement screenRoot)
        {
            this.rootElement = screenRoot;
        }
    }
}
