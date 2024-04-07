using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IPAM_NOTE.DevicePage;

namespace IPAM_NOTE
{
	/// <summary>
	/// AddDeviceWindow.xaml 的交互逻辑
	/// </summary>
	public partial class AddDeviceWindow : Window
	{
		public AddDeviceWindow()
		{
			InitializeComponent();
            GeneralNetworkDevice.CloseParentWindowRequested += GeneralNetworkDevice_CloseParentWindowRequested; ;
        }

        private void GeneralNetworkDevice_CloseParentWindowRequested(object sender, EventArgs e) =>Close();



        private void TopControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = TopControl.SelectedIndex;



            switch (index)
            {
                case 0:
                    DevicePanel.Children.Clear();
                    GeneralNetworkDevice generalNetworkDevice = new GeneralNetworkDevice();
                    DevicePanel.Children.Add(generalNetworkDevice);


                    break;

                case 1:

                    break;
            }
        }

        private void AddDeviceWindow_OnClosing(object sender, CancelEventArgs e)
        {
            this.DialogResult = true;
        }


        private void AddDeviceWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            DevicePanel.Children.Clear();
            GeneralNetworkDevice generalNetworkDevice = new GeneralNetworkDevice();
            DevicePanel.Children.Add(generalNetworkDevice);

        }
    }
}
