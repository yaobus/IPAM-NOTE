using IPAM_NOTE.DatabaseOperation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static IPAM_NOTE.ViewMode;

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

		private DbClass dbClass;


		private void Allocation_OnLoaded(object sender, RoutedEventArgs e)
		{


			AddressBlock.Text = "当前所选IP为:" + DataBrige.SelectIp + " 目前使用该IP的主机为:" + DataBrige.IpAddress.HostName + " MAC为:" + DataBrige.IpAddress.MacAddress;

			if (DataBrige.LoadType == 1)

			{
				
				
				NetworkBlock.Text = "当前网段:" + DataBrige.ComBoxAddressInfos[DataBrige.SelectNetwork].Network;


				if (DataBrige.IpAddressInfos[DataBrige.SelectIndex].User == "")
				{
					UserTextBox.Text = DataBrige.IpAddress.HostName;
					DescriptionTextBox.Text = DataBrige.IpAddress.MacAddress;
				}
				else
				{
					UserTextBox.Text = DataBrige.IpAddressInfos[DataBrige.SelectIndex].User;

					DescriptionTextBox.Text = DataBrige.IpAddressInfos[DataBrige.SelectIndex].Description;
				}

			}
			else
			{
				NetworkBlock.Text = "所选网段:" + DataBrige.TempAddress.Network;
				
				
				
				if (DataBrige.IpAddressInfos[DataBrige.SelectIndex].User == "")
				{
					UserTextBox.Text = DataBrige.IpAddress.HostName;
					DescriptionTextBox.Text = DataBrige.IpAddress.MacAddress;
				}
				else
				{
					UserTextBox.Text = DataBrige.IpAddressInfos[DataBrige.SelectIndex].User;

					DescriptionTextBox.Text = DataBrige.IpAddressInfos[DataBrige.SelectIndex].Description;
				}
			}














			string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";
			string dbName = "Address_database.db";

			dbFilePath = dbFilePath + dbName;

			dbClass = new DbClass(dbFilePath);
			dbClass.OpenConnection();


		}


		/// <summary>
		/// 保存分配
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (UserTextBox.Text != "" && UserTextBox.Text != "N/A" && DescriptionTextBox.Text != "" && DescriptionTextBox.Text != "N/A")
			{
				string tableName = DataBrige.TempAddress.TableName;

				string sql = string.Format("UPDATE {0} SET \"User\" = '{1}', \"AddressStatus\" = '2' , \"Description\" = '{2}' WHERE Address = {3}", tableName, UserTextBox.Text, DescriptionTextBox.Text, DataBrige.SelectIp);

				if (DataBrige.LoadType == 0)
				{
					DataBrige.ipAddressInfos[Convert.ToInt32(DataBrige.SelectIp)].User = UserTextBox.Text;
					DataBrige.ipAddressInfos[Convert.ToInt32(DataBrige.SelectIp)].Description = DescriptionTextBox.Text;
					DataBrige.ipAddressInfos[Convert.ToInt32(DataBrige.SelectIp)].AddressStatus = 2;
				}
				else
				{
					DataBrige.ipAddressInfos[DataBrige.SelectIndex].User = UserTextBox.Text;
					DataBrige.ipAddressInfos[DataBrige.SelectIndex].Description = DescriptionTextBox.Text;
					DataBrige.ipAddressInfos[DataBrige.SelectIndex].AddressStatus = 2;
				}




				//Console.WriteLine(sql);

				dbClass.ExecuteQuery(sql);



				this.Close();

			}
			else
			{
				MessageBox.Show("请填写地址分配信息！", "信息不完整", MessageBoxButton.OK, MessageBoxImage.Information);
			}


		}

		/// <summary>
		/// 删除分配
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <exception cref="NotImplementedException"></exception>
		private void ReleaseButton_OnClick(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("你正在删除该IP地址的分配信息，此操作不可逆！继续？", "注意", MessageBoxButton.YesNo, MessageBoxImage.Information);

			if (result == MessageBoxResult.Yes)
			{
				string tableName = DataBrige.ComBoxAddressInfos[DataBrige.SelectNetwork].TableName;

				Console.WriteLine("TableName:" +tableName) ;


				string sql = string.Format("UPDATE {0} SET \"User\" = '', \"AddressStatus\" = '1' , \"Description\" = '' WHERE Address = {1}", tableName, DataBrige.SelectIp);

				Console.WriteLine(sql);

				dbClass.ExecuteQuery(sql);
				Console.WriteLine("DataBrige.ipAddressInfos.Count=" + DataBrige.ipAddressInfos.Count);
				Console.WriteLine("DataBrige.SelectIp="+ DataBrige.SelectIp);

				DataBrige.ipAddressInfos[Convert.ToInt32(DataBrige.SelectIndex)].User = "";
				DataBrige.ipAddressInfos[Convert.ToInt32(DataBrige.SelectIndex)].Description = "";
				DataBrige.ipAddressInfos[Convert.ToInt32(DataBrige.SelectIndex)].AddressStatus = 1;

				this.Close();
			}

		}

		/// <summary>
		/// 窗口关闭
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Allocation_OnClosing(object sender, CancelEventArgs e)
		{
			this.DialogResult = true;
		}



		private void PingButton_OnClick(object sender, RoutedEventArgs e)
		{
			//int lastIndex = DataBrige.TempAddress.Network.LastIndexOf('.');
			string ip = DataBrige.TempAddress.Network.Replace("*", "") + DataBrige.SelectIp;

			string arguments = $"-t {ip}";

			// 创建一个新的ProcessStartInfo对象
			ProcessStartInfo processStartInfo = new ProcessStartInfo
			{
				FileName = "cmd.exe", // 要执行的程序是cmd.exe
				Arguments = $"/k ping {arguments}", // /k参数告诉cmd执行完命令后不会自动退出
				UseShellExecute = false, // 必须设置为false，否则无法重定向标准输入输出
				CreateNoWindow = false // 创建一个新窗口来显示CMD窗口
			};

			// 创建一个新的Process对象
			Process process = new Process
			{
				StartInfo = processStartInfo
			};

			// 启动进程
			process.Start();
		}
	}
}
