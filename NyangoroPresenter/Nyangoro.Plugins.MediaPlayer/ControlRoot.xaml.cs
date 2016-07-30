using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Nyangoro.Plugins.MediaPlayer
{
    /// <summary>
    /// Interaction logic for ControlRoot.xaml
    /// </summary>
    public partial class ControlRoot : PluginControlRoot
    {
        Point lastMouseClick = new Point();
        
        //keeps track of items removed form selection during a mosue press
        System.Collections.IList itemsWaitingForUnselect = null;

        bool draggingScrollbar = false;

        new public MediaPlayerController Controller { 
            get { return (MediaPlayerController)this.controller; } 
            private set { this.controller = value; } 
        }

        public ControlRoot()
        {
            InitializeComponent();
        }

        public ListBox GetPlaylistBox()
        {
            Grid rootGrid = (Grid)this.Content;
            Grid playlistGrid = (Grid)rootGrid.FindName("PlaylistGrid");

            return (ListBox)playlistGrid.FindName("PlaylistBox");
        }

        public void SetController(MediaPlayerController controller)
        {
            this.Controller = controller;
        }

        private void AddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            this.Controller.HandleAddToPlaylistClick();
        }

        private void PlaylistBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // if item removed from selection, postpone until MouseLeftButtonUp
            if (Mouse.LeftButton == MouseButtonState.Pressed && e.AddedItems.Count == 0)
            {
                ListBox playlistBox = (ListBox)sender;
                this.itemsWaitingForUnselect = e.RemovedItems;

                foreach (PlaylistItem item in e.RemovedItems)
                {
                    playlistBox.SelectedItems.Add(item);
                }                
            }
        }

        private void PlaylistBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
#if DEBUG
             this.Controller.HandlePlaylistMouseDoubleClick();
#else
            
            try
            {
                this.Controller.HandlePlaylistMouseDoubleClick();
            }
            catch(Exception excp)
            {
                MessageBox.Show(excp.Message);
                return;
            }
#endif
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
this.Controller.HandlePlayClick(sender, e);
#else
            try
            {
                this.Controller.HandlePlayClick(sender, e);
            }
            catch(Exception excp)
            {
                MessageBox.Show(excp.Message);
                return;
            }
#endif
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            this.Controller.HandleStopClick(sender, e);
        }

        private void AddImageBatch_Click(object sender, RoutedEventArgs e)
        {
            this.Controller.AddImageBatchClick();
        }

        private void LoadPlaylist_Click(object sender, RoutedEventArgs e)
        {
            this.Controller.HandleLoadPlaylistClick();
        }

        private void ShufflePlaylist_Click(object sender, RoutedEventArgs e)
        {
            this.Controller.HandleShufflePlaylistClick();
        }

        private void ClearPlaylist_Click(object sender, RoutedEventArgs e)
        {
            this.Controller.HandleClearPlaylistClick();
        }

        private void RemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            this.Controller.HandleRemoveSelectedClick();
        }

        private void PlaylistDown_Click(object sender, RoutedEventArgs e)
        {
            this.Controller.HandlePlaylistDownClick();
        }

        private void PlaylistUp_Click(object sender, RoutedEventArgs e)
        {
            this.Controller.HandlePlaylistUpClick();
        }

        private void PlaylistBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point originalPosition = e.GetPosition(null);
            lastMouseClick = originalPosition;

            ListBox playlistBox = (ListBox)sender;
            int dropIndex = this.GetMouseoverItemIndex(playlistBox, e.GetPosition);
            if (dropIndex == -1)
            {
                this.draggingScrollbar = true;
            }
        }

        private void PlaylistBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            if (this.draggingScrollbar)           
                return;            

            // prevent selection change on drag
            UIElement box = (UIElement)sender;
            box.ReleaseMouseCapture();


            Point currentPosition = e.GetPosition(null);
            if (Math.Abs(lastMouseClick.X - currentPosition.X) < SystemParameters.MinimumHorizontalDragDistance
                && Math.Abs(lastMouseClick.Y - currentPosition.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;

            }
            else
            {
                ListBox playlistBox = (ListBox)sender;
                //sort
                IEnumerable < PlaylistItem > itemsSortedEnumerable = playlistBox.SelectedItems.Cast<PlaylistItem>()                  
                                                                        .OrderBy(itm => playlistBox.Items.IndexOf(itm));                
                List<PlaylistItem> itemsSorted = itemsSortedEnumerable.ToList<PlaylistItem>();

                //start drag&drop
                DataObject dragItems = new DataObject(DataFormats.Serializable, itemsSorted);
                DragDrop.DoDragDrop((DependencyObject)sender, dragItems, DragDropEffects.Move);
            }

            e.Handled = true;
            
        }

        private void PlaylistBox_Drop(object sender, DragEventArgs e)
        {

                ListBox playlistBox = (ListBox)sender;
                ObservableCollection<PlaylistItem> playlistContents = (ObservableCollection<PlaylistItem>)playlistBox.ItemsSource;
                IDataObject data = e.Data;
                System.Collections.IList draggedItems = (System.Collections.IList)data.GetData(DataFormats.Serializable);
                List<PlaylistItem> itemsToInsert = new List<PlaylistItem>();
                PlaylistItem activeItem = null;

                //not ready
                if (playlistBox.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                {
                    MessageBox.Show("Containers generation not finished yet.");
                    return;
                }
                
             // return if dropped over one of the dragged items
                int moverIndex = this.GetMouseoverItemIndex(playlistBox, e.GetPosition);
                if (moverIndex > -1 && playlistBox.SelectedItems.Contains(playlistBox.Items.GetItemAt(moverIndex)))
                {
                    return;
                }
            //return if out of bounds
                if (moverIndex == -1)
                    return;
                

                // get active item            
                for (int i = 0; i < draggedItems.Count; i++)
                {
                    PlaylistItem currItem = (PlaylistItem)draggedItems[i];
                    if (currItem == this.Controller.GetPlaylist().activeItem)
                    {
                        activeItem = currItem;
                    }
                }

                // inset dragged items to toInsert collection before removing
                for (int i = 0; i < draggedItems.Count; i++)
                {
                    PlaylistItem item = (PlaylistItem)draggedItems[i];
                    itemsToInsert.Add(item);
                }

                // remove dragged items from listBox
                for (int i = 0; i < itemsToInsert.Count; i++)
                {
                    PlaylistItem item = (PlaylistItem)itemsToInsert[i];

                    if (item != activeItem)
                        playlistContents.Remove(item);
                }

                int dropIndex = this.GetMouseoverItemIndex(playlistBox, e.GetPosition);
                if (dropIndex == -1)
                {
                    dropIndex = playlistBox.Items.Count - 1;
                }
                PlaylistItem dropItem = (PlaylistItem)playlistBox.Items[dropIndex];

                //insert dragged items
                for (int i = 0; i < itemsToInsert.Count; i++)
                {
                    PlaylistItem item = itemsToInsert[i];
                    int insertIndex = this.Controller.GetPlaylist().FindItemIndex(dropItem);

                    if (item == activeItem)
                    {
                        int activeIndex = this.Controller.GetPlaylist().FindItemIndex(activeItem);
                        this.SafeShiftItemBefore(activeItem, insertIndex);
                    }
                    else
                    {
                        playlistContents.Insert(insertIndex, item);
                        //select again
                        playlistBox.SelectedItems.Add(item);
                    }
                    
                }

                //reset items waiting for unselect
                this.itemsWaitingForUnselect = null;
        }

        /**
         *  Shifts an item without destroying its visual modifications.
         */
        protected void SafeShiftItemBefore(PlaylistItem item, int targetIndex)
        {
            if (targetIndex == -1)
                targetIndex = 0;

            int currentIndex = this.Controller.GetPlaylist().FindItemIndex(item);
            ObservableCollection<PlaylistItem> playlistContents = (ObservableCollection<PlaylistItem>) this.GetPlaylistBox().ItemsSource;  

            int distance = Math.Abs(targetIndex - currentIndex);
            if (targetIndex > currentIndex)
            {
                // step forward
                int deltaItemPosition = 0;
                while(deltaItemPosition < distance-1) {
                    deltaItemPosition++;
                    playlistContents.Move(currentIndex + deltaItemPosition, currentIndex + deltaItemPosition - 1);
                }
            }
            else if (targetIndex < currentIndex)
            {
                //step back
                int deltaItemPosition = 0;
                while (deltaItemPosition < distance)
                {
                    deltaItemPosition++;
                    playlistContents.Move(currentIndex - deltaItemPosition, currentIndex - deltaItemPosition + 1);
                }
            }
        }


        private int GetMouseoverItemIndex(ListBox playlistBox, GetPositionDelegate getPosition)
        {
            int returnIndex = -1;
            for (int i = 0; i < playlistBox.Items.Count; i++ )
            {
                Visual target = (Visual)this.GetListBoxItem(playlistBox, i);
                if(this.IsMouseOverTarget(target, getPosition)) 
                {
                    returnIndex = i;
                }
            }

            return returnIndex;
        }


        private bool IsMouseOverTarget(Visual target, GetPositionDelegate getPosition)
        {
            Rect targetBounds = this.GetListBoxItemBoundsRect((ListBoxItem)target);            
            Point mousePos = getPosition((IInputElement)target);

            return targetBounds.Contains(mousePos);
        }

        private Rect GetListBoxItemBoundsRect(ListBoxItem item)
        {
            Vector offset = VisualTreeHelper.GetOffset(item);

            return new Rect(0, 0, item.ActualWidth, item.ActualHeight);
        }

        public ListBoxItem GetListBoxItem(ListBox lv, int index)
        {
            if (lv.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;
            return lv.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
        } 

        delegate Point GetPositionDelegate(IInputElement element);

        private void PlaylistBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.itemsWaitingForUnselect != null)
            {
                ListBox playListBox = (ListBox)sender;
                // reset cache of items removed from selection during this mouse press
                foreach (PlaylistItem item in this.itemsWaitingForUnselect)
                {
                    playListBox.SelectedItems.Remove(item);
                }

                this.itemsWaitingForUnselect = null;
            }

            this.draggingScrollbar = false;
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            this.Controller.ColorPlaylistItemsByStatus();
        }

        private void PlaylistBox_DragOver(object sender, DragEventArgs e)
        {
            FrameworkElement container = sender as FrameworkElement;

            if (container == null)
            {
                return;
            }

            ScrollViewer scrollViewer = GetFirstVisualChild<ScrollViewer>(container);

            if (scrollViewer == null)
            {
                return;
            }

            double tolerance = 60;
            double verticalPos = e.GetPosition(container).Y;
            double offset = 0.05;

            if (verticalPos < tolerance) // Top of visible list? 
            {
                //Scroll up
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - ((tolerance - verticalPos) * offset));
            }
            else if (verticalPos > container.ActualHeight - tolerance) //Bottom of visible list? 
            {
                //Scroll down
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + ((verticalPos - container.ActualHeight + tolerance) * offset));
            }
        }

        public static T GetFirstVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = GetFirstVisualChild<T>(child);
                    if (childItem != null)
                    {
                        return childItem;
                    }
                }
            }

            return null;
        }
    }
}
