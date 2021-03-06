﻿using System;
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
using System.Windows.Shapes;

namespace Nyangoro.Core.Host
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PresentationWindow : Window
    {
        public PresentationWindow()
        {                      
            InitializeComponent();

            #if DEBUG
                this.AllowsTransparency = false;
                this.WindowStyle=WindowStyle.SingleBorderWindow;
            #endif
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if both buttons are pressed at once, DragMove throws exception
            if(Mouse.LeftButton == MouseButtonState.Pressed && Mouse.RightButton == MouseButtonState.Released)
                this.DragMove();
        }
    }
}
