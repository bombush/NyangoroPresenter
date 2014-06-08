using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nyangoro.Core.Layout
{
    class PresentationScreen
    {
        public DependencyObject rootElement { get; private set; }

        /*Elements to which plugin root elements get inserted*/
        public List<PluginAnchor> anchors { get; protected set; }

        public PresentationScreen(DependencyObject screenRoot)
        {
            this.rootElement = screenRoot;
            this.anchors = this.GetPluginAnchors();
        }

        public void DisplayPlugins(Nyangoro.Core.Host.PluginHolder plugins)
        {
            foreach (PluginAnchor anchor in this.anchors)
            {
                Nyangoro.Interfaces.IPlugin plugin = plugins.GetByType(anchor.PluginType);
                if (plugin != null)
                {
                    anchor.AnchorPlugin(plugin);
                }
            }
        }

        protected List<PluginAnchor> GetPluginAnchors()
        {
            List<PluginAnchor> anchors = new List<PluginAnchor>();
            this.FillAnchorListRecursive(anchors, this.rootElement);
            return anchors;
        }

        protected void FillAnchorListRecursive(List<PluginAnchor> anchors, DependencyObject startNode)
        {
            int count = VisualTreeHelper.GetChildrenCount(startNode);
            for (int i = 0; i < count; i++)
            {
                DependencyObject current = VisualTreeHelper.GetChild(startNode, i);
                if ((current.GetType()).Equals(typeof(PluginAnchor)))
                {
                    anchors.Add(current as PluginAnchor);
                }
                else
                {
                    this.FillAnchorListRecursive(anchors, current);
                }
            }
        }
    }
}
