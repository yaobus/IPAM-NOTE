using IPAM_NOTE.DatabaseOperation;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static IPAM_NOTE.ViewMode;
using Button = System.Windows.Controls.Button;
using GridView = System.Windows.Controls.GridView;
using ListView = System.Windows.Controls.ListView;
using Style = System.Windows.Style;

namespace IPAM_NOTE
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private DbClass dbClass;

		//0为图形化加载，1为列表加载
		private int LoadMode = 0;

		private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			AddressListView.ItemsSource = AddressInfos;

			string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";
			string dbName = "Address_database.db";

			dbFilePath = dbFilePath + dbName;

			dbClass = new DbClass(dbFilePath);
			dbClass.OpenConnection();

			LoadNetworkInfo(dbClass.connection);

			AddressBox.ItemsSource = ComBoxAddressList;

		}



		//已分配网段
		ObservableCollection<AddressInfo> AddressInfos = new ObservableCollection<AddressInfo>();

		//搜索区网段列表
		ObservableCollection<ComBoxAddressInfo> ComBoxAddressInfos = new ObservableCollection<ComBoxAddressInfo>();

		//搜索区ComBox信息
		private List<string> ComBoxAddressList = new List<string>();


		public void LoadNetworkInfo(SQLiteConnection connection)
		{
			AddressInfos.Clear();
			ComBoxAddressInfos.Clear();
			ComBoxAddressList.Clear();
			GraphicsPlan.Children.Clear();
			try
			{
				string query = "SELECT * FROM Network WHERE Del = 0"; 
				SQLiteCommand command = new SQLiteCommand(query, connection);
				SQLiteDataReader reader = command.ExecuteReader();

				while (reader.Read())
				{


					// 读取数据行中的每一列
					int id = Convert.ToInt32(reader["Id"]);
					string tableName = reader["TableName"].ToString();
					string network = reader["Network"].ToString();
					string netmask = reader["Netmask"].ToString();
					string description = reader["Description"].ToString();
					string del = reader["Del"].ToString();
					AddressInfos.Add(new AddressInfo(id, tableName, network, netmask, description, del));
					ComBoxAddressInfos.Add(new ComBoxAddressInfo(tableName, network));
					ComBoxAddressList.Add(network);
				}

				reader.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}");
			}
		}




		/// <summary>
		/// 图形化显示
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GraphicsButton_OnClick(object sender, RoutedEventArgs e)
		{
			LoadMode = 0;

			//AddressListView_OnSelectionChanged(null,null);

			WriteAddressConfig(DataBrige.ipAddressInfos);
		}


		/// <summary>
		/// 列表化显示
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListButton_OnClick(object sender, RoutedEventArgs e)
		{
			LoadMode = 1;
			//AddressListView_OnSelectionChanged(null, null);
			ListLoad();
		}



		private void ListLoad(int selectIndex = 0)
		{

			//清空图表
			GraphicsPlan.Children.Clear();

			// 创建一个 ListView
			ListView listView = new ListView();



			// 创建一个 GridView
			GridView gridView = new GridView();


			// 添加列到 GridView
			gridView.Columns.Add(new GridViewColumn { Header = "IP地址", DisplayMemberBinding = new Binding("Address") });
			gridView.Columns.Add(new GridViewColumn { Header = "分配给", DisplayMemberBinding = new Binding("User") });
			gridView.Columns.Add(
				new GridViewColumn { Header = "备注", DisplayMemberBinding = new Binding("Description") });
			gridView.Columns.Add(new GridViewColumn
				{ Header = "PING状态", DisplayMemberBinding = new Binding("PingStatus") });
			gridView.Columns.Add(new GridViewColumn
				{ Header = "PING耗时", DisplayMemberBinding = new Binding("PingTime") });
			gridView.Columns.Add(new GridViewColumn { Header = "主机名", DisplayMemberBinding = new Binding("HostName") });
			gridView.Columns.Add(new GridViewColumn
				{ Header = "MAC地址", DisplayMemberBinding = new Binding("MacAddress") });

			// 将 GridView 设置为 ListView 的 View
			listView.View = gridView;



			// 设置 ListView 的属性
			listView.Width = GraphicsPlan.ActualWidth - 20;
			;
			listView.Height = GraphicsPlan.ActualHeight - 20;
			listView.Margin = new Thickness(10);

			listView.ItemsSource = DataBrige.ipAddressInfos;
			listView.MouseDoubleClick += ListView_MouseDoubleClick;


			GraphicsPlan.Children.Add(listView);

			listView.SelectedIndex = selectIndex;
		}





		private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

			if (sender is ListView listView)
			{
				if (listView.SelectedIndex != -1)
				{
					IpAddressInfo ipAddressInfo = listView.SelectedItem as IpAddressInfo;

					int ip = ipAddressInfo.Address;

					if (ip != 0)
					{
						DataBrige.SelectIp = ip.ToString();
						DataBrige.SelectIndex = listView.SelectedIndex;
						DataBrige.IpAddress.HostName = DataBrige.ipAddressInfos[listView.SelectedIndex].HostName;
						DataBrige.IpAddress.MacAddress = DataBrige.ipAddressInfos[listView.SelectedIndex].MacAddress;

						Window allocation = new Allocation();
						//allocation.ShowDialog();

						if (allocation.ShowDialog() == true)
						{

							//AddressListView_OnSelectionChanged(null, null);
							if (LoadMode == 0)
							{
								WriteAddressConfig(DataBrige.ipAddressInfos);
							}
							else
							{
								ListLoad(Convert.ToInt32(ip));

							}
						}
					}




				}



			}
		}

		private void AddressListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DataBrige.LoadType = 0;
			ExportButton.IsEnabled = true;
			GraphicsPlan.Children.Clear();
			GraphicsButton.IsEnabled = true;
			int index = AddressListView.SelectedIndex;

			DataBrige.TempAddress = (ViewMode.AddressInfo)AddressListView.SelectedItem;

			string tableName;

			if (index != -1)
			{
				tableName = AddressInfos[index].TableName;
				DataBrige.SelectTableName= tableName;
				//tableName = DataBrige.TempAddress.TableName;

				MinusButton.IsEnabled = true;
				EditButton.IsEnabled = true;

				LoadAddressConfig(tableName);
				StatusTestButton.IsEnabled = true;
			}
			else
			{
				MinusButton.IsEnabled = false;
				EditButton.IsEnabled = false;
				StatusTestButton.IsEnabled = false;
			}




			if (LoadMode == 0)
			{
				WriteAddressConfig(DataBrige.ipAddressInfos);
			}
			else
			{
				ListLoad();

			}


		}








		/// <summary>
		/// 加载IP地址表
		/// </summary>
		private void LoadAddressConfig(string tableName)
		{

			string sql = string.Format("SELECT * FROM {0} ORDER BY Address ASC", tableName);


			SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);

			SQLiteDataReader reader = command.ExecuteReader();

			DataBrige.ipAddressInfos.Clear();


			while (reader.Read())
			{

				int address = Convert.ToInt32(reader["Address"]);
				int addressStatus = Convert.ToInt32(reader["AddressStatus"]);
				string user = reader["User"].ToString();
				string description = reader["Description"].ToString();
				string hostName = reader["HostName"].ToString();
				string macAddress = reader["MacAddress"].ToString();

				IpAddressInfo ipAddress = new IpAddressInfo(address, addressStatus, user, description, IPStatus.Unknown,
					-1, hostName, macAddress);

				DataBrige.ipAddressInfos.Add(ipAddress);
			}

			DataBrige.IpAddressInfos = DataBrige.ipAddressInfos;

			//reader.Dispose();




		}



		/// <summary>
		/// 配置图形化显示
		/// </summary>
		/// <param name="ipAddressInfos"></param>
		private void WriteAddressConfig(List<IpAddressInfo> ipAddressInfos)
		{
			GraphicsPlan.Children.Clear();

			int x = ipAddressInfos.Count;

			for (int i = 0; i < x; i++)
			{
				//bool status = false;

				string description = null;
				Brush colorBrush = null;
				int status = ipAddressInfos[i].AddressStatus;

				Brush fontBrush = null;


				IPStatus pingStatus = ipAddressInfos[i].PingStatus;

				switch (status)
				{
					case 0:
						colorBrush = Brushes.DimGray;
						description = "类型：网段IP地址";

						break;

					case 1:
						colorBrush = Brushes.LightSeaGreen;
						description = "类型：可选IP地址" + "\r当前在线主机：" + ipAddressInfos[i].HostName + "\rMAC：" +
						              ipAddressInfos[i].MacAddress;

						break;

					case 2:
						colorBrush = Brushes.Coral;
						description = "类型：已用IP地址\r分配：" + ipAddressInfos[i].User + "\r备注：" +
						              ipAddressInfos[i].Description + "\r当前在线主机：" + ipAddressInfos[i].HostName +
						              "\rMAC：" + ipAddressInfos[i].MacAddress;

						break;

				}


				if (pingStatus == IPStatus.Success)
				{
					fontBrush = Brushes.Red;

				}
				else
				{
					fontBrush = Brushes.AliceBlue;

				}


				Button newButton = new Button()
				{
					Width = 60,
					Height = 30,
					Background = colorBrush,
					Foreground = fontBrush,
					FontWeight = FontWeights.Bold,
					Content = ipAddressInfos[i].Address,
					ToolTip = description,
					Tag = i.ToString(),
					Style = (Style)this.FindResource("MaterialDesignRaisedButton"),
					Margin = new Thickness(5),

				};



				newButton.Click += NewButton_Click;
				;





				//显示到图形化区域
				GraphicsPlan.Children.Add(newButton);

			}

		}


		private void NewButton_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button button)
			{
				#region MyRegion




				//int tag = Convert.ToInt32(button.Tag);



				//if (tag == 2)
				//{
				//	string text = button.Content.ToString();

				//	if (button.Background == Brushes.LimeGreen)
				//	{
				//		//紫色代表被选中
				//		button.Background = Brushes.BlueViolet;

				//		Button bt = new Button();


				//		string name = "bt" + button.Content;

				//		bt.Content = button.Content;
				//		bt.Background = button.Background;
				//		bt.Height = button.Height;
				//		bt.Width = button.Width;
				//		bt.Margin = button.Margin;
				//		bt.Style = (Style)this.FindResource("StaticResource MaterialDesignFlatAccentBgButton");
				//		//IpSelectPanel.Children.Add(bt);
				//		//IpSelectPanel.RegisterName(name, bt);


				//	}
				//	else
				//	{
				//		//绿色代表未被选中
				//		button.Background = Brushes.LimeGreen;

				//		//Button bt = IpSelectPanel.FindName("bt" + button.Content.ToString()) as Button;

				//		//if (bt != null)
				//		//{
				//			//IpSelectPanel.Children.Remove(bt);
				//			//IpSelectPanel.UnregisterName("bt" + button.Content.ToString());
				//		//}

				//	}

				//}

				//统计已选择IP数量
				//SelectIpNum.Text = CountIp().ToString();
				#endregion

				
				string ip = button.Content.ToString();

				if (ip != "0")
				{
					DataBrige.SelectIp = ip;
					DataBrige.IpAddress.HostName = DataBrige.ipAddressInfos[Convert.ToInt32(ip)].HostName;
					DataBrige.IpAddress.MacAddress = DataBrige.ipAddressInfos[Convert.ToInt32(ip)].MacAddress;

					Window allocation = new Allocation();
					//allocation.ShowDialog();

					if (allocation.ShowDialog() == true)
					{

						//AddressListView_OnSelectionChanged(null, null);
						if (LoadMode == 0)
						{
							WriteAddressConfig(DataBrige.ipAddressInfos);
						}
						else
						{
							ListLoad();

						}
					}
				}



			}
		}



		private void AddButton_OnClick(object sender, RoutedEventArgs e)
		{

			DataBrige.AddStatus = 0;

			AddWindow addWindow = new AddWindow();

			if (addWindow.ShowDialog() == true)
			{

				// 当子窗口关闭后执行这里的代码
				LoadNetworkInfo(dbClass.connection);

			}





		}



		/// <summary>
		/// 删除网段
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MinusButton_OnClick(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("注意！你正在删除一个网段，是否继续？", "注意", MessageBoxButton.YesNo,
				MessageBoxImage.Information);

			if (result == MessageBoxResult.Yes)
			{
				string tableName = DataBrige.TempAddress.TableName;

				string sql = string.Format("UPDATE \"Network\" SET \"Del\" = '1' WHERE  TableName = '{0}'", tableName);

				string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";
				string dbName = "Address_database.db";

				dbFilePath = dbFilePath + dbName;

				dbClass = new DbClass(dbFilePath);
				dbClass.OpenConnection();

				dbClass.ExecuteQuery(sql);

				MainWindow_OnLoaded(null, null);

			}


		}

		/// <summary>
		/// 编辑网段
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EditButton_OnClick(object sender, RoutedEventArgs e)
		{

			DataBrige.AddStatus = 1;

			AddWindow addWindow = new AddWindow();

			if (addWindow.ShowDialog() == true)
			{
				// 当子窗口关闭后执行这里的代码
				LoadNetworkInfo(dbClass.connection);
			}





		}


		/// <summary>
		/// 关于页面
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AboutButton_OnClick(object sender, RoutedEventArgs e)
		{
			AboutWindow about = new AboutWindow();


			about.ShowDialog();

		}




		/// <summary>
		/// 批量检测IP地址在线状态
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void StatusTestButton_OnClick(object sender, RoutedEventArgs e)
		{
			Bar.Visibility = Visibility.Visible;

			IPInfo[] results = await Task.Run(() => StartPingAndGetResults());

			Console.WriteLine(results.Length);
			Console.WriteLine(DataBrige.ipAddressInfos.Count);

			//foreach (var item in results)
			//{
			//	Console.WriteLine($"Address: {item.IPAddress}, Status: {item.PingStatus}, Description: {item.PingTime}, HostName: {item.HostName}, MacAddress: {item.MACAddress}");
			//}

			for (int i = 1; i < DataBrige.ipAddressInfos.Count; i++)
			{
				DataBrige.ipAddressInfos[i].PingStatus = results[i - 1].PingStatus;
				DataBrige.ipAddressInfos[i].PingTime = results[i - 1].PingTime;
				DataBrige.ipAddressInfos[i].HostName = results[i - 1].HostName;
				DataBrige.ipAddressInfos[i].MacAddress = results[i - 1].MACAddress;

			}

			Bar.Visibility = Visibility.Hidden;

			if (LoadMode == 0)
			{
				WriteAddressConfig(DataBrige.ipAddressInfos);
			}
			else
			{
				ListLoad();

			}



		}

		private async Task<IPInfo[]> StartPingAndGetResults()
		{
			//string baseIP = "10.0.0."; // 设置基本IP地址



			string baseIP = DataBrige.TempAddress.Network.ToString().Replace("*", "");



			List<Task<IPInfo>> pingTasks = new List<Task<IPInfo>>();


			for (int i = 1; i <= 255; i++)
			{
				string ip = baseIP + i.ToString(); // 构建要ping的IP地址

				pingTasks.Add(PingAndGetInfo(ip)); // 启动ping并获取信息任务


			}

			IPInfo[] results = await Task.WhenAll(pingTasks); // 等待所有ping任务完成

			return results;
		}




		private async Task<IPInfo> PingAndGetInfo(string ip)
		{


			Ping ping = new Ping();

			PingReply reply = await ping.SendPingAsync(ip, 1000); // 异步执行ping操作

			if (reply.Status == IPStatus.Success)
			{
				string macAddress = await GetMacAddress(ip);
				string hostName = await GetHostName(ip);

				int lastIndex = ip.LastIndexOf('.');

				int address = Convert.ToInt32(ip.Substring(lastIndex + 1));

				return new IPInfo(address, reply.Status, reply.RoundtripTime, hostName, macAddress);
			}
			else
			{
				int lastIndex = ip.LastIndexOf('.');

				int address = Convert.ToInt32(ip.Substring(lastIndex + 1));

				return new IPInfo(address, reply.Status, -1, "N/A", "N/A"); // 如果 Ping 失败，主机名和 MAC 地址设置为 "N/A"
			}
		}


		private async Task<string> GetMacAddress(string ipAddress)
		{
			IPAddress ip = IPAddress.Parse(ipAddress);
			byte[] macAddressBytes = new byte[6];
			int macAddrLen = macAddressBytes.Length;
			uint macAddrLenUint = (uint)macAddrLen;

			int result = NativeMethods.SendARP((int)ip.Address, 0, macAddressBytes, ref macAddrLenUint);
			if (result != 0)
				return "N/A";

			string macAddress = BitConverter.ToString(macAddressBytes, 0, macAddrLen).Replace("-", ":");
			return macAddress;
		}

		private async Task<string> GetHostName(string ipAddress)
		{
			try
			{
				IPHostEntry hostEntry = await Dns.GetHostEntryAsync(ipAddress);
				return hostEntry.HostName;
			}
			catch (Exception)
			{
				return "N/A";
			}
		}

		internal static class NativeMethods
		{
			[System.Runtime.InteropServices.DllImport("iphlpapi.dll", ExactSpelling = true)]
			internal static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);
		}

		

		public  void ExportToExcel(List<IpAddressInfo> dataList, string filePath)
		{

			using (var package = new ExcelPackage())
			{
				var sheet = package.Workbook.Worksheets.Add(DataBrige.TempAddress.TableName);

				// 写入标题行
				sheet.Cells[1, 1].Value = "Address";
				sheet.Cells[1, 2].Value = "AddressStatus";
				sheet.Cells[1, 3].Value = "User";
				sheet.Cells[1, 4].Value = "Description";
				sheet.Cells[1, 5].Value = "PingStatus";
				sheet.Cells[1, 6].Value = "PingTime";
				sheet.Cells[1, 7].Value = "HostName";
				sheet.Cells[1, 8].Value = "MacAddress";

				// 写入数据
				int rowIndex = 2;
				foreach (var ipAddressInfo in dataList)
				{
					sheet.Cells[rowIndex, 1].Value = ipAddressInfo.Address;
					sheet.Cells[rowIndex, 2].Value = ipAddressInfo.AddressStatus;
					sheet.Cells[rowIndex, 3].Value = ipAddressInfo.User;
					sheet.Cells[rowIndex, 4].Value = ipAddressInfo.Description;
					sheet.Cells[rowIndex, 5].Value = ipAddressInfo.PingStatus;
					sheet.Cells[rowIndex, 6].Value = ipAddressInfo.PingTime;
					sheet.Cells[rowIndex, 7].Value = ipAddressInfo.HostName;
					sheet.Cells[rowIndex, 8].Value = ipAddressInfo.MacAddress;
					rowIndex++;
				}

				// 保存Excel文件
				FileInfo excelFile = new FileInfo(filePath);







				package.SaveAs(excelFile);
			}
		}



		private void ExportButton_OnClick(object sender, RoutedEventArgs e)
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


			// 创建 SaveFileDialog 实例
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
			saveFileDialog.FilterIndex = 1;
			saveFileDialog.RestoreDirectory = true;


			if (DataBrige.LoadType == 0)
			{
				// 设置默认文件名
				//saveFileDialog.FileName = DataBrige.TempAddress.TableName + ".xlsx";
				// 设置默认文件名
				saveFileDialog.FileName = DataBrige.SelectTableName + ".xlsx";
			}
			else
			{
				// 设置默认文件名
				saveFileDialog.FileName = DataBrige.SelectTableName +"_Search_"+KeyWord.Text+ ".xlsx";
			}




			// 显示 SaveFileDialog
			bool? result = saveFileDialog.ShowDialog();

			if (result == true)
			{
				// 获取用户选择的文件路径
				string filePath = saveFileDialog.FileName;
				ExportToExcel(DataBrige.ipAddressInfos, filePath);

			}


		}



		/// <summary>
		/// 搜索
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SearchButton_OnClick(object sender, RoutedEventArgs e)
		{
			DataBrige.SelectTableName = ComBoxAddressInfos[AddressBox.SelectedIndex].TableName + "_Search_" + KeyWord.Text;

			LoadMode = 1;
			ExportButton.IsEnabled = true;
			DataBrige.LoadType = 1;
			
			AddressListView.SelectedIndex = -1;
			if (AddressBox.SelectedIndex != -1)
			{

				//清空图表
				GraphicsPlan.Children.Clear();

				// 创建一个 ListView
				ListView listView = new ListView();



				// 创建一个 GridView
				GridView gridView = new GridView();


				// 添加列到 GridView
				gridView.Columns.Add(new GridViewColumn { Header = "IP地址", DisplayMemberBinding = new Binding("Address") });
				gridView.Columns.Add(new GridViewColumn { Header = "分配给", DisplayMemberBinding = new Binding("User") });
				gridView.Columns.Add(
					new GridViewColumn { Header = "备注", DisplayMemberBinding = new Binding("Description") });
				gridView.Columns.Add(new GridViewColumn
					{ Header = "PING状态", DisplayMemberBinding = new Binding("PingStatus") });
				gridView.Columns.Add(new GridViewColumn
					{ Header = "PING耗时", DisplayMemberBinding = new Binding("PingTime") });
				gridView.Columns.Add(new GridViewColumn { Header = "主机名", DisplayMemberBinding = new Binding("HostName") });
				gridView.Columns.Add(new GridViewColumn
					{ Header = "MAC地址", DisplayMemberBinding = new Binding("MacAddress") });

				// 将 GridView 设置为 ListView 的 View
				listView.View = gridView;



				// 设置 ListView 的属性
				listView.Width = GraphicsPlan.ActualWidth - 20;
				;
				listView.Height = GraphicsPlan.ActualHeight - 20;
				listView.Margin = new Thickness(10);

				listView.ItemsSource = DataBrige.ipAddressInfos;
				listView.MouseDoubleClick += ListView_MouseDoubleClick;


				GraphicsPlan.Children.Add(listView);

				LoadAddressSearch(ComBoxAddressInfos[AddressBox.SelectedIndex].TableName,KeyWord.Text);
				
				ListButton.IsChecked=true;
				GraphicsButton.IsEnabled = false;
				StatusTestButton.IsEnabled=false;
			}





		}


		/// <summary>
		/// 加载IP地址搜索项
		/// </summary>
		private void LoadAddressSearch(string tableName, string keyWord)
		{



			string sql = string.Format("SELECT * FROM `{0}` WHERE  `User`LIKE '%{1}%' OR  Description LIKE '%{2}%'", tableName, keyWord, keyWord);


			SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);

			SQLiteDataReader reader = command.ExecuteReader();

			DataBrige.ipAddressInfos.Clear();


			while (reader.Read())
			{

				int address = Convert.ToInt32(reader["Address"]);
				int addressStatus = Convert.ToInt32(reader["AddressStatus"]);
				string user = reader["User"].ToString();
				string description = reader["Description"].ToString();
				string hostName = reader["HostName"].ToString();
				string macAddress = reader["MacAddress"].ToString();

				IpAddressInfo ipAddress = new IpAddressInfo(address, addressStatus, user, description, IPStatus.Unknown,
					-1, hostName, macAddress);

				DataBrige.ipAddressInfos.Add(ipAddress);
			}

			DataBrige.IpAddressInfos = DataBrige.ipAddressInfos;

			




		}


		private void KeyWord_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				SearchButton_OnClick(null, null);
			}
		}


	}
}
