using IPAM_NOTE.DatabaseOperation;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static IPAM_NOTE.ViewMode;
using Button = System.Windows.Controls.Button;
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

		private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			AddressListView.ItemsSource = AddressInfos;

			string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";
			string dbName = "Address_database.db";

			dbFilePath = dbFilePath+dbName;

			dbClass = new DbClass(dbFilePath);
			dbClass.OpenConnection();

			LoadNetworkInfo(dbClass.connection);


		}

		private void InsertData()
		{
			string insertQuery = "INSERT INTO TableName (Column1, Column2) VALUES ('Value1', 123)";
			SQLiteCommand command = new SQLiteCommand(insertQuery, dbClass.connection);
			command.ExecuteNonQuery();
		}

		//已分配网段
		 ObservableCollection<AddressInfo> AddressInfos = new ObservableCollection<AddressInfo>();

		public void LoadNetworkInfo(SQLiteConnection connection)
		{
			AddressInfos.Clear();
			try
			{
				string query = "SELECT * FROM Network"; // 表名替换成你的实际表名
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
					AddressInfos.Add(new AddressInfo(id, tableName,network,netmask,description,del));


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
			


		}

		private void ListButton_OnClick(object sender, RoutedEventArgs e)
		{


		}



		private void AddressListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			int index = AddressListView.SelectedIndex;

			DataBrige.TempAddress = (ViewMode.AddressInfo)AddressListView.SelectedItem;

			string tableName;

			if (index != -1)
			{
				tableName = AddressInfos[index].TableName;
			
				LoadAddressConfig(tableName);
				//MessageBox.Show(tableName);
			}
		}



		///// <summary>
		///// 从数据库获取网段信息
		///// </summary>
		///// <param name="sql"></param>
		//private void LoadAddressData(int groupId)
		//{

		//	AddressInfos.Clear();


		//	string sql = string.Format("SELECT * FROM network WHERE authority LIKE '%g{0}p%'", groupId);


		//	// 查询IP段信息
		//	// MySqlDataReader reader = DbClass.CarrySqlCmd(sql);


		//	while (reader.Read())
		//	{
		//		int id = reader.GetInt32("id");
		//		string tableName = reader.GetString("tableName");
		//		string network = reader.GetString("network");
		//		string netmask = reader.GetString("netmask");
		//		string attention = reader.GetString("attention");
		//		string description = reader.GetString("description");
		//		string authority = reader.GetString("authority");
		//		string creator = reader.GetString("creator");
		//		DateTime date = reader.GetDateTime("date");


		//		AddressInfos.Add(new AddressInfo(id, tableName, network, netmask, attention, description, authority, creator, date));
		//	}

		//	reader.Dispose();


		//}



		/// <summary>
		/// 加载IP地址图形化表
		/// </summary>
		private void LoadAddressConfig(string tableName)
		{


			string sql = string.Format("SELECT * FROM {0} ORDER BY Address ASC", tableName);


			SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);
			
			SQLiteDataReader reader = command.ExecuteReader();


			List<IpAddressInfo> ipAddressInfos = new List<IpAddressInfo>();

			while (reader.Read())
			{

				int address = Convert.ToInt32(reader["Address"]);
				int addressStatus = Convert.ToInt32(reader["AddressStatus"]);
				string user = reader["User"].ToString();
				string description = reader["Description"].ToString();


				IpAddressInfo ipAddress = new IpAddressInfo(address, addressStatus,user, description);

				ipAddressInfos.Add(ipAddress);
			}

			//reader.Dispose();

			WriteAddressConfig(ipAddressInfos);
		}



		/// <summary>
		/// 配置图形化显示
		/// </summary>
		/// <param name="ipAddressInfos"></param>
		private void WriteAddressConfig(List<IpAddressInfo> ipAddressInfos)
		{

			int x = ipAddressInfos.Count;

			for (int i = 0; i < x; i++)
			{
				//bool status = false;

				string description = null;
				Brush colorBrush = null;
				int status = ipAddressInfos[i].AddressStatus;

				switch (status)
				{
					case 0:
						colorBrush = Brushes.DimGray;
						description = "网段IP地址";

						break;

					case 1:
						colorBrush = Brushes.LightSeaGreen;
						description = "保留IP地址";

						break;

					

				}
				Button newButton = new Button()
				{
					Width = 60,
					Height = 30,
					Background = colorBrush,
					Foreground = Brushes.AliceBlue,
					Content = i.ToString(),
					
					ToolTip = description,
					Tag = status,
					Style = (Style)this.FindResource("MaterialDesignRaisedLightButton"),
					Margin = new Thickness(5),

				};



				newButton.Click += NewButton_Click; ;





				//显示到图形化区域
				GraphicsPlan.Children.Add(newButton);

			}

		}



		private void NewButton_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button button)
			{

				int tag = Convert.ToInt32(button.Tag);



				if (tag == 2)
				{
					string text = button.Content.ToString();

					if (button.Background == Brushes.LimeGreen)
					{
						//紫色代表被选中
						button.Background = Brushes.BlueViolet;

						Button bt = new Button();


						string name = "bt" + button.Content;

						bt.Content = button.Content;
						bt.Background = button.Background;
						bt.Height = button.Height;
						bt.Width = button.Width;
						bt.Margin = button.Margin;
						bt.Style = (Style)this.FindResource("StaticResource MaterialDesignFlatAccentBgButton");
						//IpSelectPanel.Children.Add(bt);
						//IpSelectPanel.RegisterName(name, bt);

						
					}
					else
					{
						//绿色代表未被选中
						button.Background = Brushes.LimeGreen;

						//Button bt = IpSelectPanel.FindName("bt" + button.Content.ToString()) as Button;

						//if (bt != null)
						//{
							//IpSelectPanel.Children.Remove(bt);
							//IpSelectPanel.UnregisterName("bt" + button.Content.ToString());
						//}

					}

				}

				//统计已选择IP数量
				//SelectIpNum.Text = CountIp().ToString();

				DataBrige.SelectIp = button.Content.ToString();

				Window allocation = new Allocation();
				allocation.Show();
			}
		}

		private void AddButton_OnClick(object sender, RoutedEventArgs e)
		{ 
			AddWindow addWindow = new AddWindow();

			if (addWindow.ShowDialog() == true)
			{

				// 当子窗口关闭后执行这里的代码
				LoadNetworkInfo(dbClass.connection);
			}

			
			

			
		}




		private void MinusButton_OnClick(object sender, RoutedEventArgs e)
		{
			
		}

		private void EditButton_OnClick(object sender, RoutedEventArgs e)
		{
			
		}

		private void AboutButton_OnClick(object sender, RoutedEventArgs e)
		{

		}
	}


}
