using IPAM_NOTE.DatabaseOperation;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
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
using Path = System.IO.Path;
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
			
			//创建数据库备份
			CreatBackup(dbFilePath);

			string dbName = "Address_database.db";


			dbFilePath = dbFilePath + dbName;



			dbClass = new DbClass(dbFilePath);
			dbClass.OpenConnection();

			LoadNetworkInfo(dbClass.connection);

			AddressBox.ItemsSource = ComBoxAddressList;

		}



		//已分配网段
		ObservableCollection<AddressInfo> AddressInfos = new ObservableCollection<AddressInfo>();



		//搜索区ComBox信息
		private List<string> ComBoxAddressList = new List<string>();


		/// <summary>
		/// 加载网段信息列表
		/// </summary>
		/// <param name="connection"></param>
		public void LoadNetworkInfo(SQLiteConnection connection)
		{
			AddressInfos.Clear();
			DataBrige.ComBoxAddressInfos.Clear();
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
					DataBrige.ComBoxAddressInfos.Add(new ComBoxAddressInfo(tableName, network));
					ComBoxAddressList.Add(network);
				}

				DataBrige.ComboBoxAddressList = ComBoxAddressList;

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


		/// <summary>
		/// 列表加载
		/// </summary>
		/// <param name="selectIndex"></param>
		private void ListLoad(int selectIndex = 0)
		{

			//清空图表
			GraphicsPlan.Children.Clear();

			// 创建一个 ListView
			ListView listView = new ListView();

			// 设置 ListView 的垂直对齐方式为拉伸
			listView.VerticalAlignment = VerticalAlignment.Stretch;

			// 设置 ListView 的内容垂直对齐方式为顶部对齐
			listView.VerticalContentAlignment = VerticalAlignment.Stretch; 

			ScrollViewer.SetCanContentScroll(listView, false);

			// 创建 ScrollViewer 控件
			ScrollViewer scrollViewer = new ScrollViewer();

			// 设置垂直滚动条的可见性为自动
			scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto; 



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

			Binding bindingWidth = new Binding("ActualWidth");
			bindingWidth.Source = GraphicsPlan;
			listView.SetBinding(ListView.WidthProperty, bindingWidth);

			// 设置 ListView 的高度为 Auto
			listView.Height = double.NaN;

			listView.Margin = new Thickness(10);

			listView.ItemsSource = DataBrige.ipAddressInfos;
			listView.SelectionChanged += ListView_SelectionChanged;
			listView.MouseDoubleClick += ListView_MouseDoubleClick;



			scrollViewer.Content = listView;
			scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;


			GraphicsPlan.Children.Add(scrollViewer);

		}

		private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			// 获取当前的 ScrollViewer 控件
			ScrollViewer currentScrollViewer = sender as ScrollViewer;

			if (currentScrollViewer != null)
			{
				// 计算滚动的增量
				double delta = e.Delta;

				// 计算新的滚动位置
				double newOffset = currentScrollViewer.VerticalOffset - delta;

				// 限制滚动位置在合理范围内
				if (newOffset < 0)
				{
					newOffset = 0;
				}
				else if (newOffset > currentScrollViewer.ScrollableHeight)
				{
					newOffset = currentScrollViewer.ScrollableHeight;
				}

				// 设置新的滚动位置
				currentScrollViewer.ScrollToVerticalOffset(newOffset);
			}

			// 标记事件为已处理，以阻止默认的滚动行为
			e.Handled = true;

		}







		/// <summary>
		/// 表项被选择
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ListView listView)
			{
				Console.WriteLine("当前表项INDEX：" + listView.SelectedIndex);
				if (listView.SelectedIndex != -1)
				{
					IpAddressInfo ipAddressInfo = listView.SelectedItem as IpAddressInfo;
					int ip = ipAddressInfo.Address;
					Console.WriteLine("当前表项IP："+ip);
					if (ip != 0)
					{
						DataBrige.SelectIp = ip.ToString();
						DataBrige.SelectIndex = listView.SelectedIndex;
						Console.WriteLine("DataBrige.SelectIndex=" + DataBrige.SelectIndex);
						DataBrige.IpAddress.HostName = DataBrige.ipAddressInfos[listView.SelectedIndex].HostName;
						DataBrige.IpAddress.MacAddress = DataBrige.ipAddressInfos[listView.SelectedIndex].MacAddress;
					}
				}
			}
		}

		/// <summary>
		/// 表项被双击
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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


		/// <summary>
		/// 网段列表表项被选择
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddressListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DataBrige.LoadType = 0;
			ExportButton.IsEnabled = true;
			GraphicsPlan.Children.Clear();
			GraphicsButton.IsEnabled = true;
			int index = AddressListView.SelectedIndex;
			AddressBox.SelectedIndex = AddressListView.SelectedIndex;

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
		/// 加载IP地址表配置
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

			//int firstIp = ipAddressInfos[0].Address;

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



			#region Backup

			//int x = ipAddressInfos.Count;

			//for (int i = 0; i < x; i++)
			//{
			//	//bool status = false;

			//	string description = null;
			//	Brush colorBrush = null;
			//	int status = ipAddressInfos[i].AddressStatus;

			//	Brush fontBrush = null;


			//	IPStatus pingStatus = ipAddressInfos[i].PingStatus;

			//	switch (status)
			//	{
			//		case 0:
			//			colorBrush = Brushes.DimGray;
			//			description = "类型：网段IP地址";

			//			break;

			//		case 1:
			//			colorBrush = Brushes.LightSeaGreen;
			//			description = "类型：可选IP地址" + "\r当前在线主机：" + ipAddressInfos[i].HostName + "\rMAC：" +
			//			              ipAddressInfos[i].MacAddress;

			//			break;

			//		case 2:
			//			colorBrush = Brushes.Coral;
			//			description = "类型：已用IP地址\r分配：" + ipAddressInfos[i].User + "\r备注：" +
			//			              ipAddressInfos[i].Description + "\r当前在线主机：" + ipAddressInfos[i].HostName +
			//			              "\rMAC：" + ipAddressInfos[i].MacAddress;

			//			break;

			//	}


			//	if (pingStatus == IPStatus.Success)
			//	{
			//		fontBrush = Brushes.Red;

			//	}
			//	else
			//	{
			//		fontBrush = Brushes.AliceBlue;

			//	}


			//	Button newButton = new Button()
			//	{
			//		Width = 60,
			//		Height = 30,
			//		Background = colorBrush,
			//		Foreground = fontBrush,
			//		FontWeight = FontWeights.Bold,
			//		Content = ipAddressInfos[i].Address,
			//		ToolTip = description,
			//		Tag = i.ToString(),
			//		Style = (Style)this.FindResource("MaterialDesignRaisedButton"),
			//		Margin = new Thickness(5),

			//	};



			//	newButton.Click += NewButton_Click;
			//	;





			//	//显示到图形化区域
			//	GraphicsPlan.Children.Add(newButton);

			//}

			#endregion
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

				DataBrige.SelectButtonTag = Convert.ToInt32(button.Tag);

				IpAddressInfo ipAddressInfo = DataBrige.ipAddressInfos[DataBrige.SelectButtonTag] as IpAddressInfo;
				


				int ip = ipAddressInfo.Address;

				if (ip != 0)
				{
					DataBrige.SelectIp = ip.ToString();
					DataBrige.SelectIndex = Convert.ToInt32(button.Tag);
					Console.WriteLine("DataBrige.SelectIndex=" + DataBrige.SelectIndex);
					DataBrige.IpAddress.HostName = ipAddressInfo.HostName;
					DataBrige.IpAddress.MacAddress = ipAddressInfo.MacAddress;

					Window allocation = new Allocation();


					if (allocation.ShowDialog() == true)
					{

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

			if (DataBrige.LoadType ==1)
			{
				using (var package = new ExcelPackage())
				{
					var sheet = package.Workbook.Worksheets.Add(DataBrige.SelectTableName);




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
			else
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
				saveFileDialog.FileName = DataBrige.ComBoxAddressInfos[AddressBox.SelectedIndex].TableName +"_Search_"+KeyWord.Text+ ".xlsx";
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
			SearchClear.IsEnabled = true;

			LoadMode = 1;
			ExportButton.IsEnabled = true;
			


			//搜索后的状态
			DataBrige.LoadType = 1;
			DataBrige.SelectNetwork = AddressBox.SelectedIndex;
			//AddressListView.SelectedIndex = -1;

			if (AddressBox.SelectedIndex != -1)
			{
				DataBrige.SelectTableName = DataBrige.ComBoxAddressInfos[AddressBox.SelectedIndex].TableName + "_Search_" + KeyWord.Text;

				//清空图表
				GraphicsPlan.Children.Clear();

				// 创建一个 ListView
				ListView listView = new ListView();

				// 创建 ScrollViewer 控件
				ScrollViewer scrollViewer = new ScrollViewer();
				scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto; // 设置垂直滚动条的可见性为自动
				listView.VerticalAlignment = VerticalAlignment.Top; // 设置 ListView 的垂直对齐方式为顶部对齐
				scrollViewer.Content = listView;


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

				Binding bindingWidth = new Binding("ActualWidth");
				bindingWidth.Source = GraphicsPlan;
				listView.SetBinding(ListView.WidthProperty, bindingWidth);



				listView.Margin = new Thickness(10);

				listView.ItemsSource = DataBrige.ipAddressInfos;

				listView.SelectionChanged += ListView_SelectionChanged;
				listView.MouseDoubleClick += ListView_MouseDoubleClick;

				scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;

				GraphicsPlan.Children.Add(scrollViewer);

				LoadAddressSearch(DataBrige.ComBoxAddressInfos[AddressBox.SelectedIndex].TableName,KeyWord.Text);
				
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



			string sql = string.Format("SELECT * FROM `{0}` WHERE  (`User`LIKE '%{1}%' OR  Description LIKE '%{2}%') AND AddressStatus != 1", tableName, keyWord, keyWord);

			Console.WriteLine(sql);

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

		/// <summary>
		/// 清空搜索
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SearchClear_OnClick(object sender, RoutedEventArgs e)
		{
			string tableName;

			if (AddressListView.SelectedIndex == -1)
			{
				tableName = DataBrige.ComBoxAddressInfos[AddressBox.SelectedIndex].TableName;
			}
			else
			{
				tableName = DataBrige.ComBoxAddressInfos[AddressListView.SelectedIndex].TableName;
			}


			GraphicsButton.IsEnabled = true;
			StatusTestButton.IsEnabled = true;

			LoadAddressConfig(tableName);
			ListLoad();
			SearchClear.IsEnabled = false;
		}



		/// <summary>
		/// 导入向导
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ImportButton_OnClick(object sender, RoutedEventArgs e)
		{
			ImportWindow importWindow = new ImportWindow();

			

			if (importWindow.ShowDialog() == true)
			{

				AddressListView_OnSelectionChanged(null, null);
				// 当子窗口关闭后执行这里的代码
				//LoadNetworkInfo(dbClass.connection);

			}
		}


		/// <summary>
		/// 窗口尺寸改变
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			//if (LoadMode == 1)
			//{
			//	ListView listView = (ListView)GraphicsPlan.FindName("DynamicListView");

			//	listView.Width = GraphicsPlan.ActualWidth - 20;
			//	listView.Height = GraphicsPlan.ActualHeight - 20;
			//	listView.Margin = new Thickness(10);
			//}
		}



		private void CreatBackup(string sourceFilePath)
		{

			string dbFileName = "Address_database.db";
			string dbFilePath = Path.Combine(sourceFilePath, dbFileName);


			// 如果源文件不存在，则无需备份
			if (!File.Exists(dbFilePath))
			{
				Console.WriteLine("源文件不存在，无法备份。");
				return;
			}

			string backupDirectoryPath = Path.Combine(sourceFilePath, "Backup");

			// 创建备份目录（如果不存在）
			if (!Directory.Exists(backupDirectoryPath))
			{
				Directory.CreateDirectory(backupDirectoryPath);
			}

			// 获取备份文件夹内的所有备份文件
			string[] backupFiles = Directory.GetFiles(backupDirectoryPath, "*.bak");

			// 如果存在备份文件，则选择最新的一个进行比较
			if (backupFiles.Length > 0)
			{
				string latestBackupFilePath = backupFiles.OrderByDescending(f => File.GetLastWriteTime(f)).First();

				// 如果最新备份文件与源文件相同，则无需备份
				if (FilesAreIdentical(dbFilePath, latestBackupFilePath))
				{
					Console.WriteLine("最新备份和源文件相同，无需备份。");
					return;
				}
			}


			// 构建备份文件名
			string backupFileName = $"Address_database.db_{DateTime.Now:yyyy年M月d日HH.mm}.bak";
			string backupFilePath =Path.Combine(backupDirectoryPath, backupFileName);


			try
			{
				// 进行备份
				File.Copy(dbFilePath, backupFilePath, true);
				Console.WriteLine("备份已创建：" + backupFilePath);
			}
			catch (DirectoryNotFoundException ex)
			{
				Console.WriteLine("备份目录未找到：" + ex.Message);
			}

		}

		/// <summary>
		/// 检查两个文件是否相同
		/// </summary>
		/// <param name="file1"></param>
		/// <param name="file2"></param>
		/// <returns></returns>
		private bool FilesAreIdentical(string file1, string file2)
		{
			using (var hashAlgorithm = SHA256.Create())
			{
				using (var stream1 = File.OpenRead(file1))
				using (var stream2 = File.OpenRead(file2))
				{
					var hash1 = hashAlgorithm.ComputeHash(stream1);


					var hash2 = hashAlgorithm.ComputeHash(stream2);

					return StructuralComparisons.StructuralEqualityComparer.Equals(hash1, hash2);
				}
			}
		}
	}
}
