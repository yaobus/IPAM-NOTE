using ControlzEx.Standard;
using IPAM_NOTE.DatabaseOperation;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
			WindowsShow();
		}

		private DbClass dbClass;

		//0为图形化加载，1为列表加载
		private int LoadMode = 0;

		private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			CheckVer();

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


			//生成搜索域列表
			CreatDomainList();
			DomainComboBox.ItemsSource = domainList;


		}


		private async void CheckVer()
		{

			var versionChecker = new GitHubVersionChecker("yaobus", "IPAM-NOTE");
			string latestVersion = await versionChecker.GetLatestVersionAsync();
			

			

			// 定义匹配版本号的正则表达式
			Regex regex = new Regex(@"\d+\.\d+");

			// 在输入字符串中搜索匹配的版本号
			Match match = regex.Match(latestVersion);

			// 如果找到匹配的版本号，则输出
			if (match.Success)
			{
				double version = Convert.ToDouble( match.Value);

				if (version > DataBrige.Ver)
				{
					
					//发现新版本则闪烁提示
					ButtonProgressAssist.SetIsIndeterminate(AboutButton, true);
					AboutButton.ToolTip = "发现新版本:"+ latestVersion;
					DataBrige.LatestVersion = version;

					//获取下载地址
					DataBrige.DownloadUrl = await versionChecker.GetDownloadUrlAsync();
				}
				else
				{
					ButtonProgressAssist.SetIsIndeterminate(AboutButton, false);
					DataBrige.LatestVersion = 0;

					//清空下载地址
					DataBrige.DownloadUrl = "";
				}

			}




		}


		/// <summary>
		/// 设置窗口显示方式，分辨率小于1080P就全屏显示，否则居中显示
		/// </summary>
		private void WindowsShow()
		{

			// 获取屏幕的宽度和高度
			double screenWidth = SystemParameters.PrimaryScreenWidth;
			double screenHeight = SystemParameters.PrimaryScreenHeight;

			// 设定最大分辨率
			double maxResolutionWidth = 1920;
			double maxResolutionHeight = 1080;

			// 如果分辨率小于等于1920x1080，则最大化窗口
			if (screenWidth <= maxResolutionWidth && screenHeight <= maxResolutionHeight)
			{
				WindowState = WindowState.Maximized;
			}
			else // 否则，居中显示窗口
			{
				WindowStartupLocation = WindowStartupLocation.CenterScreen;
			}
		}



		private List<string> domainList = new List<string>();

		/// <summary>
		/// 创建搜索范围表项
		/// </summary>
		private void CreatDomainList()
		{
			domainList.Add("用户");
			domainList.Add("备注");
			domainList.Add("主机名");
			domainList.Add("MAC");
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
				SortSql = query;
				SQLiteCommand command = new SQLiteCommand(query, connection);
				SQLiteDataReader reader = command.ExecuteReader();
				int i = 0;
				while (reader.Read())
				{
					i++;
					// 读取数据行中的每一列
					int id = i;
					string tableName = reader["TableName"].ToString();
					string network = reader["Network"].ToString();
					string netmask = reader["Netmask"].ToString();
					string description = reader["Description"].ToString();
					string del = reader["Del"].ToString();
					AddressInfos.Add(new AddressInfo(id, tableName, network, netmask, description, del));
					DataBrige.ComBoxAddressInfos.Add(new ComBoxAddressInfo(tableName, network, netmask));
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
			PingNumBox.Visibility = Visibility.Hidden;

			DataBrige.SelectMode = 0;//单选多选状态清空
									
			ListLoad();
		}


		/// <summary>
		/// 列表加载
		/// </summary>
		/// <param name="selectIndex"></param>
		private void ListLoad(int selectIndex = 0)
		{
			//禁用多选模式
			MultipleSelect.IsEnabled=false;
			DataBrige.SelectMode = 0;
			DataBrige.SelectedIpAddress.Clear();
			MultipleSelectStatus.Visibility = Visibility.Hidden;
			MultipleSelect.IsChecked=false;




			//清空图表
			GraphicsPlan.Children.Clear();
			Graphics.Children.Clear();

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
				//Console.WriteLine("当前表项INDEX：" + listView.SelectedIndex);
				if (listView.SelectedIndex != -1)
				{
					IpAddressInfo ipAddressInfo = listView.SelectedItem as IpAddressInfo;
					int ip = ipAddressInfo.Address;
					Console.WriteLine("当前表项IP：" + ip);


					//计算广播IP
					int broadcast = 0;
					IPAddress ipAddress;
					IPAddress mask = IPAddress.Parse(DataBrige.TempAddress.NetMask);

					MaskText.Text = DataBrige.TempAddress.NetMask;



					//如果网段IP中有*则替换为0
					string tempNetwork = DataBrige.TempAddress.Network;

					if (tempNetwork.IndexOf("*") != -1)
					{
						tempNetwork = tempNetwork.Replace("*", "0");

					}


					Network.Text = tempNetwork;
					
					//计算IP地址
					UpdateIPCalculations();

					if (IPAddress.TryParse(tempNetwork, out ipAddress))
					{
						IPAddress networkAddress = ipAddress.GetNetworkAddress(mask);
						int maskLength = IPAddressCalculations.CalculateSubnetMaskLength(mask);
						IPAddress broadcastAddress = networkAddress.GetBroadcastAddress(maskLength);

						string[] parts = broadcastAddress.ToString().Split('.');

						//取出广播IP
						broadcast = Convert.ToInt32(parts[3]);
						Console.WriteLine(broadcast);
					}


					//取出网段IP
					string[] parts2 = tempNetwork.Split('.');

					
					int firstIp = Convert.ToInt32(parts2[3]);



					if (ip != firstIp && ip != broadcast)
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

					//Console.WriteLine("MouseDoubleClickIp="+ip.ToString());


					//计算广播IP
					string[] parts = Broadcast.Text.Split('.');
					int broadcast = Convert.ToInt32(parts[3]);

					//Console.WriteLine("broadcast=" + broadcast.ToString());

					//计算网段IP
					string[] parts2 = Network.Text.Split('.');
					int firstIp = Convert.ToInt32(parts2[3]);


					//Console.WriteLine("firstIp=" + firstIp.ToString());

					if (ip != firstIp && ip != broadcast)
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
			DataBrige.SelectMode = 0;//单选多选状态清空
			DataBrige.SelectedIpAddress.Clear();//多选地址列表清空

			PingNumBox.Visibility = Visibility.Hidden;//隐藏PING统计

			DataBrige.ipAddressPingInfos.Clear();
			DataBrige.LoadType = 0;
			DataBrige.SearchType = 1;
			ExportButton.IsEnabled = true;
			GraphicsPlan.Children.Clear();
			GraphicsButton.IsEnabled = true;
			int index = AddressListView.SelectedIndex;

			AddressBox.SelectedIndex = -1;
			//AddressBox.SelectedIndex = AddressListView.SelectedIndex;

			DataBrige.TempAddress = (ViewMode.AddressInfo)AddressListView.SelectedItem;

			string tableName;

			if (index != -1)
			{
				//清空状态栏计算结果
				ClearStatusBox();


				tableName = AddressInfos[index].TableName;
				DataBrige.SelectTableName = tableName;
				//tableName = DataBrige.TempAddress.TableName;

				MinusButton.IsEnabled = true;
				EditButton.IsEnabled = true;

				LoadAddressConfig(tableName);
				StatusTestButton.IsEnabled = true;


				//如果网段IP中有*则替换为0
				string tempNetwork = AddressInfos[index].Network;

				if (tempNetwork.IndexOf("*") != -1)
				{
					tempNetwork = tempNetwork.Replace("*", "0");
				}


				Network.Text = tempNetwork;
				
				MaskText.Text = AddressInfos[index].NetMask;

				//计算IP地址信息
				UpdateIPCalculations();

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
		/// IP地址计算
		/// </summary>
		private void UpdateIPCalculations()
		{
			try
			{
				IPAddress ip;
				if (IPAddress.TryParse(Network.Text, out ip))
				{

					IPAddress mask = IPAddress.Parse(MaskText.Text);
					int maskLength = IPAddressCalculations.CalculateSubnetMaskLength(mask);


					IPAddress networkAddress = ip.GetNetworkAddress(mask);
					//Network.Text = networkAddress.ToString();

					IPAddress firstAddress = networkAddress.GetFirstUsable(ip.AddressFamily);
					First.Text = firstAddress.ToString();

					IPAddress lastAddress = networkAddress.GetLastUsable(ip.AddressFamily, maskLength);

					Last.Text = lastAddress.ToString();

					IPAddress broadcastAddress = networkAddress.GetBroadcastAddress(maskLength);
					Broadcast.Text = broadcastAddress.ToString();


					long addressCount = IPAddressCalculations.AddressCount(maskLength);
					NumBox.Text = addressCount.ToString();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK,
					MessageBoxImage.Error);
			}
		}


		/// <summary>
		/// 清空地址计算
		/// </summary>
		private void ClearStatusBox()
		{
			Network.Text = "";
			MaskText.Text = "";
			First.Text = "";
			Last.Text = "";
			Broadcast.Text = "";
			NumBox.Text = "";


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
			Graphics.Children.Clear();

			MultipleSelect.IsEnabled = true;
			DataBrige.SelectMode = 0;
			DataBrige.SelectedIpAddress.Clear();
			

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
						colorBrush = Brushes.DarkCyan;
						description = "类型：可选IP地址" + "\r当前在线主机：" + ipAddressInfos[i].HostName + "\rMAC：" +
						              ipAddressInfos[i].MacAddress;

						break;

					case 2:
						colorBrush = Brushes.Coral;
						description = "类型：已用IP地址\r分配：" + ipAddressInfos[i].User + "\r备注：" +
						              ipAddressInfos[i].Description + "\r主机名：" + ipAddressInfos[i].HostName +
						              "\rMAC：" + ipAddressInfos[i].MacAddress;

						break;

					case 3:
						colorBrush = Brushes.DarkMagenta;
						description = "类型：广播IP地址";

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
					Style = (Style)this.FindResource("MaterialDesignFlatSecondaryDarkBgButton"),
					BorderThickness = new Thickness(0),
					Margin = new Thickness(5),

				};



				newButton.Click += NewButton_Click;
				





				//显示到图形化区域
				Graphics.Children.Add(newButton);

			}



		}


		private void NewButton_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button button)
			{
				DataBrige.SelectButtonTag = Convert.ToInt32(button.Tag);

				IpAddressInfo ipAddressInfo = DataBrige.ipAddressInfos[DataBrige.SelectButtonTag] as IpAddressInfo;

				int ip = ipAddressInfo.Address;

				if (MultipleSelect.IsChecked == false)//如果是单选
				{

					//DataBrige.SelectButtonTag = Convert.ToInt32(button.Tag);

					//IpAddressInfo ipAddressInfo = DataBrige.ipAddressInfos[DataBrige.SelectButtonTag] as IpAddressInfo;

					//int ip = ipAddressInfo.Address;

					if (ip != DataBrige.ipAddressInfos[0].Address && ip != DataBrige.ipAddressInfos[DataBrige.ipAddressInfos.Count - 1].Address)
					{
						DataBrige.SelectIp = ip.ToString();
						DataBrige.SelectIndex = Convert.ToInt32(button.Tag);
						Console.WriteLine("DataBrige.SelectIndex=" + DataBrige.SelectIndex);
						DataBrige.IpAddress.HostName = ipAddressInfo.HostName;
						DataBrige.IpAddress.MacAddress = ipAddressInfo.MacAddress;


						Window allocation = new Allocation();

						if (allocation.ShowDialog() == true)
						{
							DataBrige.SelectMode = 0;

							MultipleSelectStatus.Visibility = Visibility.Hidden;//隐藏多选按钮

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
				else//如果是多选
				{

					if (ip != DataBrige.ipAddressInfos[0].Address && ip != DataBrige.ipAddressInfos[DataBrige.ipAddressInfos.Count - 1].Address)
					{


						if (DataBrige.SelectMode == 0)//初次选择
						{
							if (ipAddressInfo.AddressStatus == 1)//准备分配地址
							{
								DataBrige.SelectMode = 1;
								button.Background = Brushes.DarkGreen;
								Icon.Kind = PackIconKind.CogOutline;



							}
							else//准备释放地址
							{
								DataBrige.SelectMode = 2;
								button.Background = Brushes.DarkRed;
								Icon.Kind = PackIconKind.CogCounterclockwise;

							}
							DataBrige.SelectedIpAddress.Add(ipAddressInfo.Address);//添加第一个地址到列表
						}
						else//再次选择
						{
							if (ipAddressInfo.AddressStatus == DataBrige.SelectMode && ipAddressInfo.AddressStatus == 1)//准备分配地址
							{

								//地址不存在则添加
								if (!DataBrige.SelectedIpAddress.Contains(ipAddressInfo.Address))
								{
									DataBrige.SelectedIpAddress.Add(ipAddressInfo.Address);
									button.Background = Brushes.DarkGreen;
								}
								else//地址已存在，则删除
								{
									DataBrige.SelectedIpAddress.Remove(ipAddressInfo.Address);
									button.Background = Brushes.DarkCyan;
								}



							}
							else
							{
								if (ipAddressInfo.AddressStatus == DataBrige.SelectMode && ipAddressInfo.AddressStatus == 2)//准备释放地址
								{

									//地址不存在则添加
									if (!DataBrige.SelectedIpAddress.Contains(ipAddressInfo.Address))
									{
										DataBrige.SelectedIpAddress.Add(ipAddressInfo.Address);
										button.Background = Brushes.DarkRed;
									}
									else//地址已存在，则删除
									{
										DataBrige.SelectedIpAddress.Remove(ipAddressInfo.Address);
										button.Background = Brushes.Coral;

									}

								}

							}



						}


						if (DataBrige.SelectedIpAddress.Count > 0)
						{
							MultipleSelectStatus.Visibility = Visibility.Visible;
							//GraphicsPlan.Height = GraphicsPlan.Height - 30;
							CountBox.Text = DataBrige.SelectedIpAddress.Count.ToString();
						}
						else
						{
							MultipleSelectStatus.Visibility = Visibility.Collapsed;
							//GraphicsPlan.Height = GraphicsPlan.Height + 30;
							DataBrige.SelectMode = 0;

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
				LoadNetworkInfoSort(dbClass.connection,SortSql);

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

				//string sql = string.Format("DELETE \"Network\" SET \"Del\" = '1' WHERE  TableName = '{0}'", tableName);
				string sql = string.Format("DELETE FROM Network WHERE  TableName = '{0}'", tableName);

				string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";
				string dbName = "Address_database.db";

				dbFilePath = dbFilePath + dbName;

				dbClass = new DbClass(dbFilePath);
				dbClass.OpenConnection();

				dbClass.ExecuteQuery(sql);
				sql= string.Format("DROP TABLE '{0}'", tableName);

				dbClass.ExecuteQuery(sql);

				LoadNetworkInfoSort(dbClass.connection,SortSql);
				//MainWindow_OnLoaded(null, null);

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
				LoadNetworkInfoSort(dbClass.connection,SortSql);
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
			PingNumBox.Text = "";

			ButtonProgressAssist.SetIsIndeterminate(StatusTestButton, true);


			

			IPInfo[] results = await Task.Run(() => StartPingAndGetResults());

			Console.WriteLine(results.Length);
			Console.WriteLine(DataBrige.ipAddressInfos.Count);


			DataBrige.ipAddressPingInfos = DataBrige.ipAddressInfos;

			int count = 0;//统计PING有响应的地址数量

			for (int i = 0; i < DataBrige.ipAddressInfos.Count; i++)
			{
				IPStatus status = results[i].PingStatus;

				if (status == IPStatus.Success)
				{
					count += 1;
					
				}

				DataBrige.ipAddressInfos[i].PingStatus = status;
				DataBrige.ipAddressInfos[i].PingTime = results[i].PingTime;
				DataBrige.ipAddressPingInfos[i].HostName = results[i].HostName;
				DataBrige.ipAddressPingInfos[i].MacAddress = results[i].MACAddress;

			}
			PingNumBox.Visibility = Visibility.Visible;
			ButtonProgressAssist.SetIsIndeterminate(StatusTestButton, false);
			PingNumBox.Text = count.ToString();

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

			string baseIP = DataBrige.TempAddress.Network;

			if (baseIP.IndexOf("*") != -1)
			{
				baseIP = baseIP.Replace("*", "0");
			}

			baseIP = GetFirstThreeSegments(baseIP);

		

			List<Task<IPInfo>> pingTasks = new List<Task<IPInfo>>();




			for (int i = 0; i <= DataBrige.ipAddressInfos.Count - 1; i++)
			{

				string ip = baseIP + DataBrige.ipAddressInfos[i].Address; // 构建要ping的IP地址
				
				Console.WriteLine(ip);

				pingTasks.Add(PingAndGetInfo(ip)); // 启动ping并获取信息任务


			}

			IPInfo[] results = await Task.WhenAll(pingTasks); // 等待所有ping任务完成

			return results;
		}

		/// <summary>
		/// 取出IP地址前三部分
		/// </summary>
		/// <param name="ipAddressString"></param>
		/// <returns></returns>
		static string GetFirstThreeSegments(string ipAddressString)
		{
			if (IPAddress.TryParse(ipAddressString, out IPAddress ipAddress))
			{
				string[] segments = ipAddress.ToString().Split('.');
				if (segments.Length >= 3)
				{
					return $"{segments[0]}.{segments[1]}.{segments[2]}.";
				}
				else
				{
					return "Invalid IP address";
				}
			}
			else
			{
				return "Invalid IP address format";
			}
		}


		private async Task<IPInfo> PingAndGetInfo(string ip)
		{


			Ping ping = new Ping();
			
			PingReply reply = await ping.SendPingAsync(ip, 600); // 异步执行ping操作
			



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



		public void ExportToExcel(List<IpAddressInfo> dataList, string filePath)
		{

			if (DataBrige.LoadType == 1)
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


		public void ListViewExportToExcel(List<SearchInfo> dataList, string filePath)
		{


			using (var package = new ExcelPackage())
			{
				var sheet = package.Workbook.Worksheets.Add(DataBrige.SelectTableName);



				// 写入标题行
				sheet.Cells[1, 1].Value = "Network";
				sheet.Cells[1, 2].Value = "NetMask";
				sheet.Cells[1, 3].Value = "Address";
				sheet.Cells[1, 4].Value = "AddressStatus";
				sheet.Cells[1, 5].Value = "User";
				sheet.Cells[1, 6].Value = "Description";
				sheet.Cells[1, 7].Value = "PingStatus";
				sheet.Cells[1, 8].Value = "PingTime";
				sheet.Cells[1, 9].Value = "HostName";
				sheet.Cells[1, 10].Value = "MacAddress";

				// 写入数据
				int rowIndex = 2;
				foreach (var ipAddressInfo in dataList)
				{
					sheet.Cells[rowIndex, 1].Value = ipAddressInfo.Network;
					sheet.Cells[rowIndex, 2].Value = ipAddressInfo.Netmask;
					sheet.Cells[rowIndex, 3].Value = ipAddressInfo.Address;
					sheet.Cells[rowIndex, 4].Value = ipAddressInfo.AddressStatus;
					sheet.Cells[rowIndex, 5].Value = ipAddressInfo.User;
					sheet.Cells[rowIndex, 6].Value = ipAddressInfo.Description;
					sheet.Cells[rowIndex, 7].Value = "";
					sheet.Cells[rowIndex, 8].Value = "";
					sheet.Cells[rowIndex, 9].Value = ipAddressInfo.HostName;
					sheet.Cells[rowIndex, 10].Value = ipAddressInfo.MacAddress;
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
				saveFileDialog.FileName = DataBrige.SelectTableName  + KeyWord.Text + ".xlsx";
			}




			// 显示 SaveFileDialog
			bool? result = saveFileDialog.ShowDialog();

			if (result == true)
			{
				// 获取用户选择的文件路径
				string filePath = saveFileDialog.FileName;


				if (DataBrige.SearchType == 0)
				{
					ListViewExportToExcel(DataBrige.searchInfos,filePath);
				}
				else
				{
					ExportToExcel(DataBrige.ipAddressInfos, filePath);
				}



			}


		}



		/// <summary>
		/// 搜索
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SearchButton_OnClick(object sender, RoutedEventArgs e)
		{
			//清空PING结果
			DataBrige.ipAddressPingInfos.Clear();

			//单选多选状态清空
			DataBrige.SelectMode = 0;
			DataBrige.SelectedIpAddress.Clear();
			MultipleSelectStatus.Visibility = Visibility.Hidden;
			MultipleSelect.IsChecked = false;

			//启用清空搜索按钮
			SearchClear.IsEnabled = true;

			//创建搜索标签
			LoadMode = 1;

			//启用导出按钮
			ExportButton.IsEnabled = true;

			PingNumBox.Visibility = Visibility.Hidden;//隐藏PING统计

			//搜索后的状态
			DataBrige.LoadType = 1;
			DataBrige.SelectNetwork = AddressBox.SelectedIndex;

			//索引为-1的时候搜索全部网段，不为-1的时候搜索指定网段
			if (AddressBox.SelectedIndex != -1)
			{
				DataBrige.SearchType = 1;

				DataBrige.SelectTableName = DataBrige.ComBoxAddressInfos[AddressBox.SelectedIndex].TableName + "_Search_" + KeyWord.Text;

				//清空图表
				GraphicsPlan.Children.Clear();
				Graphics.Children.Clear();

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

				LoadAddressSearch(DataBrige.ComBoxAddressInfos[AddressBox.SelectedIndex].TableName, KeyWord.Text);

				ListButton.IsChecked = true;
				GraphicsButton.IsEnabled = false;
				StatusTestButton.IsEnabled = false;
			}
			else//搜索全部网段
			{
				DataBrige.SearchType = 0;

				DataBrige.SelectTableName = "Search_All_Table_" + KeyWord.Text;

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

				gridView.Columns.Add(new GridViewColumn { Header = "所属网段", DisplayMemberBinding = new Binding("Network") });
				gridView.Columns.Add(new GridViewColumn { Header = "子网掩码", DisplayMemberBinding = new Binding("Netmask") });
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



				listView.SelectionChanged += ListView_SelectionChanged1; ;

				listView.MouseDoubleClick += ListView_MouseDoubleClick1; ;

				scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;

				GraphicsPlan.Children.Add(scrollViewer);

				//LoadAddressSearch(DataBrige.ComBoxAddressInfos[AddressBox.SelectedIndex].TableName, KeyWord.Text);

				DataBrige.searchInfos = SearchInTables();

				listView.ItemsSource = DataBrige.searchInfos;

				ListButton.IsChecked = true;
				GraphicsButton.IsEnabled = false;
				StatusTestButton.IsEnabled = false;












			}





		}


		/// <summary>
		/// 双击全局搜索的表单表项
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListView_MouseDoubleClick1(object sender, MouseButtonEventArgs e)
		{
			if (sender is ListView listView)
			{
				if (listView.SelectedIndex != -1)
				{
					SearchInfo ipAddressInfo = listView.SelectedItem as SearchInfo;

					int ip = ipAddressInfo.Address;

					//计算广播IP
					string[] parts = Broadcast.Text.Split('.');
					int broadcast = Convert.ToInt32(parts[3]);



					//计算网段IP
					string[] parts2 = Network.Text.Split('.');
					int firstIp = Convert.ToInt32(parts2[3]);


					if (ip != firstIp && ip != broadcast)
					{

						Window allocation = new Allocation();
						//allocation.ShowDialog();

						if (allocation.ShowDialog() == true)
						{
							listView.ItemsSource = null;
							DataBrige.SelectMode = 0;
							MultipleSelectStatus.Visibility = Visibility.Hidden;//隐藏多选按钮

							if (DataBrige.SearchType == 0)
							{
								
								listView.ItemsSource = DataBrige.searchInfos;
							}
							else
							{

								listView.ItemsSource = DataBrige.IpAddressInfos;
							}


							//ListLoad(ip);

						}
					}




				}



			}
		}

		/// <summary>
		/// 单击全局搜索的表单表项
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListView_SelectionChanged1(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ListView listView)
			{
				//Console.WriteLine("当前表项INDEX：" + listView.SelectedIndex);
				if (listView.SelectedIndex != -1)
				{
					SearchInfo ipAddressInfo = listView.SelectedItem as SearchInfo;
					int ip = ipAddressInfo.Address;

					//计算广播IP
					int broadcast = 0;
					IPAddress ipAddress;
					IPAddress mask = IPAddress.Parse(ipAddressInfo.Netmask);

					//如果网段IP中有*则替换为0
					string tempNetwork = ipAddressInfo.Network;

					if (tempNetwork.IndexOf("*") != -1)
					{
						tempNetwork = tempNetwork.Replace("*", "0");
					}



					if (IPAddress.TryParse(tempNetwork, out ipAddress))
					{
						IPAddress networkAddress = ipAddress.GetNetworkAddress(mask);
						int maskLength = IPAddressCalculations.CalculateSubnetMaskLength(mask);
						IPAddress broadcastAddress = networkAddress.GetBroadcastAddress(maskLength);
						
						string[] parts = broadcastAddress.ToString().Split('.');

						//取出广播IP
						broadcast = Convert.ToInt32(parts[3]);
						Console.WriteLine(broadcast);
					}


					//取出网段IP
					string[] parts2 = tempNetwork.Split('.');

					//取出广播IP
					int	firstIp = Convert.ToInt32(parts2[3]);




					if (ip != firstIp && ip != broadcast)
					{
						DataBrige.SelectIp = ip.ToString();
						DataBrige.SelectIndex = listView.SelectedIndex;
						DataBrige.SelectSearchInfo = ipAddressInfo;

					}


					Network.Text = tempNetwork;
					MaskText.Text = ipAddressInfo.Netmask;
					UpdateIPCalculations();

				}
			}
		}





		

		public List<SearchInfo> SearchInTables()
		{
			List<SearchInfo> searchResults = new List<SearchInfo>();

			using (SQLiteCommand cmd = dbClass.connection.CreateCommand())
			{
				// 获取Network表中的所有表名，并检查Del状态
				cmd.CommandText = "SELECT * FROM Network WHERE Del != 1";
				using (SQLiteDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						string tableName = reader["TableName"].ToString();
						string network = reader["Network"].ToString();
						string netmask = reader["Netmask"].ToString();
						List<SearchInfo> tableSearchResults = SearchInTable(tableName, network, netmask);
						searchResults.AddRange(tableSearchResults);
					}
				}
			}

			return searchResults;
		}

		private List<SearchInfo> SearchInTable(string tableName, string network, string netmask)
		{
			List<SearchInfo> tableSearchResults = new List<SearchInfo>();

			// 构造针对特定表的查询语句
			string query = ConstructQueryForTable(tableName);

			Console.WriteLine(query);

			using (SQLiteCommand cmd = dbClass.connection.CreateCommand())
			{
				cmd.CommandText = query;
				using (SQLiteDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						// 将查询结果映射到SearchInfo对象
						SearchInfo searchInfo = MapToSearchInfo(reader, tableName, network, netmask);
						tableSearchResults.Add(searchInfo);
					}
				}
			}

			return tableSearchResults;
		}


		/// <summary>
		/// 构建查询语句
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		private string ConstructQueryForTable(string tableName)
		{

			int index = DomainComboBox.SelectedIndex;

			string keyword = KeyWord.Text;

			string sql;


			//判断是精确搜索还是模糊搜索,选中为精确，非选中为模糊
			if (Accurate.IsChecked == true)
			{

				sql = string.Format(" = '{0}'", keyword);
			}
			else
			{
				sql = string.Format("LIKE '%{0}%'", keyword);
			}



			switch (index)
			{


				case 0://搜索用户

					return string.Format("SELECT * FROM  {0} WHERE `AddressStatus`=2 AND `User` {1}", tableName, sql);


				case 1://搜索备注

					return string.Format("SELECT * FROM  {0} WHERE `AddressStatus`=2 AND `Description` {1}", tableName, sql);


				case 2://搜索主机名

					return string.Format("SELECT * FROM  {0} WHERE `AddressStatus`=2 AND `HostName` {1}", tableName, sql);



				case 3://搜索MAC

					return string.Format("SELECT * FROM  {0} WHERE `AddressStatus`=2 AND `MacAddress` {1}", tableName, sql);


				default://搜索全部字段

					return string.Format("SELECT * FROM  {0} WHERE `AddressStatus`=2 AND (`User` {1}  OR `Description` {1}  OR `HostName` {1} OR `MacAddress` {1} )", tableName, sql);

			}




		}

		/// <summary>
		/// 映射搜索结果
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="tableName"></param>
		/// <param name="network"></param>
		/// <param name="netmask"></param>
		/// <returns></returns>
		private SearchInfo MapToSearchInfo(SQLiteDataReader reader, string tableName, string network, string netmask)
		{
			// 将查询结果映射到SearchInfo对象的方法，根据数据库字段和SearchInfo属性的映射自定义
			SearchInfo searchInfo = new SearchInfo
			{
				TableName = tableName,
				Network = network,
				Netmask = netmask,
				Address =Convert.ToInt32(reader["Address"]),
				AddressStatus = Convert.ToInt32( reader["AddressStatus"]),
				User = reader["User"].ToString(),
				Description = reader["Description"].ToString(),
				HostName = reader["HostName"].ToString(),
				MacAddress = reader["MacAddress"].ToString()
			};

			return searchInfo;
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

			if (AddressListView.SelectedIndex != -1)
			{

				tableName = DataBrige.ComBoxAddressInfos[AddressListView.SelectedIndex].TableName;
				Network.Text = DataBrige.ComBoxAddressInfos[AddressListView.SelectedIndex].Network;
				MaskText.Text = DataBrige.ComBoxAddressInfos[AddressListView.SelectedIndex].NetMask;
			}
			else
			{
				if (AddressBox.SelectedIndex != -1)
				{

					tableName = DataBrige.ComBoxAddressInfos[AddressBox.SelectedIndex].TableName;
					Network.Text = DataBrige.ComBoxAddressInfos[AddressBox.SelectedIndex].Network;
					MaskText.Text = DataBrige.ComBoxAddressInfos[AddressBox.SelectedIndex].NetMask;

				}
				else
				{
					tableName = DataBrige.ComBoxAddressInfos[0].TableName;
					Network.Text = DataBrige.ComBoxAddressInfos[0].Network;
					MaskText.Text = DataBrige.ComBoxAddressInfos[0].NetMask;
				}

			}






			GraphicsButton.IsEnabled = true;
			StatusTestButton.IsEnabled = true;

			LoadAddressConfig(tableName);
			ListLoad();
			SearchClear.IsEnabled = false;
			AddressBox.SelectedIndex = -1;
			DomainComboBox.SelectedIndex = -1;




			//计算IP地址信息
			UpdateIPCalculations();
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
			string backupFilePath = Path.Combine(backupDirectoryPath, backupFileName);


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

		private void AddressBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (AddressBox.SelectedIndex != -1)
			{
				DataBrige.ipAddressPingInfos.Clear();
				DataBrige.SearchType = 1;
				DataBrige.TempAddress = (ViewMode.AddressInfo)AddressInfos[AddressBox.SelectedIndex];
			}
		}


		//按索引排序
		private int IdSort = 0;

		//按网段排序
		private int NetworkSort = 0;

		//按备注排序
		private int DescriptionSort = 0;

		private string SortSql;

		/// <summary>
		/// 按索引排序
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void IndexButton_OnClick(object sender, RoutedEventArgs e)
		{

			string query;
			if (IdSort == 0)
			{
				query = "SELECT * FROM Network WHERE Del = 0 ORDER BY Id DESC";

				IdSort = 1;
			}
			else
			{
				query = "SELECT * FROM Network WHERE Del = 0 ORDER BY Id ASC";
				IdSort = 0;
			}

			

			LoadNetworkInfoSort(dbClass.connection, query);

		}

		/// <summary>
		/// 加载网段信息列表
		/// </summary>
		/// <param name="connection"></param>
		public void LoadNetworkInfoSort(SQLiteConnection connection,string query)
		{
			SortSql = query;

			AddressInfos.Clear();
			DataBrige.ComBoxAddressInfos.Clear();
			ComBoxAddressList.Clear();
			GraphicsPlan.Children.Clear();
			try
			{
				
				SQLiteCommand command = new SQLiteCommand(query, connection);
				SQLiteDataReader reader = command.ExecuteReader();
				int i = 0;
				while (reader.Read())
				{
					i++;
					// 读取数据行中的每一列
					int id = i;
					string tableName = reader["TableName"].ToString();
					string network = reader["Network"].ToString();
					string netmask = reader["Netmask"].ToString();
					string description = reader["Description"].ToString();
					string del = reader["Del"].ToString();
					AddressInfos.Add(new AddressInfo(id, tableName, network, netmask, description, del));
					DataBrige.ComBoxAddressInfos.Add(new ComBoxAddressInfo(tableName, network, netmask));
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
		/// 按IP地址排序
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NetworkButton_OnClick(object sender, RoutedEventArgs e)
		{
			string query;
			if (NetworkSort == 0)
			{
				query = "SELECT * FROM Network WHERE Del = 0 ORDER BY Network DESC";

				NetworkSort = 1;
			}
			else
			{
				query = "SELECT * FROM Network WHERE Del = 0 ORDER BY Network ASC";
				NetworkSort = 0;
			}

			LoadNetworkInfoSort(dbClass.connection, query);
		}


		private void DescriptionButton_OnClick(object sender, RoutedEventArgs e)
		{
			string query;
			if (DescriptionSort == 0)
			{
				query = "SELECT * FROM Network WHERE Del = 0 ORDER BY Description DESC";

				DescriptionSort = 1;
			}
			else
			{
				query = "SELECT * FROM Network WHERE Del = 0 ORDER BY Description ASC";
				DescriptionSort = 0;
			}

			LoadNetworkInfoSort(dbClass.connection, query);
		}


		private void MultipleSelect_Click(object sender, RoutedEventArgs e)		
		{
			DataBrige.SelectMode = 0;
			DataBrige.SelectedIpAddress.Clear();

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
		/// 一键分配
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DistributionButton_OnClick(object sender, RoutedEventArgs e)
		{

			Window allocation = new Allocation();

			if (allocation.ShowDialog() == true)
			{
				MultipleSelectStatus.Visibility = Visibility.Hidden;//隐藏多选按钮
				DataBrige.SelectMode = 0;
				DataBrige.SelectedIpAddress.Clear();

				if (LoadMode == 0)
				{
					WriteAddressConfig(DataBrige.ipAddressInfos);
				}
				else
				{
					ListLoad();

				}
			}

			//if (DataBrige.SelectMode == 1)//批量分配
			//{
			//	foreach (var ip in DataBrige.SelectedIpaddress)
			//	{




			//	}



			//}
			//else
			//{
			//	if (DataBrige.SelectMode == 2)//批量释放
			//	{




			//	}
			//}
		}
	}
}
