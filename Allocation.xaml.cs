using System;
using System.Collections.Generic;
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

namespace IPAM_NOTE
{
	/// <summary>
	/// Allocation.xaml 的交互逻辑
	/// </summary>
	public partial class Allocation : Window
	{
		public Allocation()
		{
			InitializeComponent();
		}


		private void Allocation_OnLoaded(object sender, RoutedEventArgs e)
		{
			InfoTextBox.Text = "网段:" + DataBrige.TempAddress.Network + "IP地址：" + DataBrige.SelectIp;
		}
	}
}
