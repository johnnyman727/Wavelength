using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using Wavelength.WLSocket;
using Wavelength.JSON;
using Wavelength.ShrapnelProtocol;
using Wavelength.TestClasses;

namespace Wavelength
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            //Create a socket
            //SocketClient _sc = new SocketClient();

            // Test the receive functionality
            //_sc.TestWrite();

            //Test JSON Writing capabilities
            //JSONHandler handle = new JSONHandler();
            //handle.Test();

            //Test Binary length writing capabilities
            //Person bob = new Person("Ted", "Haanson");
            //ShrapnelHandler.SendMessage(JSONHandler.ObjectToJSON<Person>(bob));

            //Test receiving functionality
            SocketClient sc = new SocketClient();
            sc.Connect();
            sc.Receive();
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }
    }
}