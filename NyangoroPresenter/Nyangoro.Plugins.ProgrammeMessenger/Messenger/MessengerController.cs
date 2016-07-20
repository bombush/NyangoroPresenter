using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Nyangoro.Plugins.ProgrammeMessenger.Programme;

namespace Nyangoro.Plugins.ProgrammeMessenger.Messenger
{
    class MessengerController : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string message;
        public string Message {
            get 
            { 
                return this.message; 
            }
            set
            { 
                this.message = value;
                this.OnPropertyChanged("Message");
            }
        }
        public UserControl RootControl {get; protected set;}

        //factory should be usable for both, move it up to the ProgrammeMessenger level!!!
        //or do we get a way to combine plugins on one place and then split PM to P and M?
        ProgrammeAnimationTimelineFactory programmeAnimationFactory;

        public MessengerController()
        {
            this.programmeAnimationFactory = new ProgrammeAnimationTimelineFactory();
            this.RootControl = new PresentationMessenger();
            this.Message = String.Empty;
        }

        public void Init()
        {
            Grid rootGrid = (Grid)this.RootControl.Content;
            this.RootControl.DataContext = this;
        }

        public void Run()
        {
        }

        public void Hide()
        {
            this.RootControl.Opacity = 0;
        }

        public void Show()
        {
            this.RootControl.Opacity = 1;
        }

        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
