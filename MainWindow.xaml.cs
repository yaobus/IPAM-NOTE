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

		private void LoadNetworkInfo(SQLiteConnection connection)
		{
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

			string tableName;

			if (index != -1)
			{
				tableName = AddressInfos[index].TableName;
			
				// LoadAddressConfig(tableName);
				MessageBox.Show(tableName);
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



		///// <summary>
		///// 加载IP地址图形化表
		///// </summary>
		//private void LoadAddressConfig(string tableName)
		//{


		//	string sql = string.Format("SELECT * FROM {0} ORDER BY Address ASC", tableName);

		//	SQLiteDataReader reader = dbClass.ExecuteQuery(sql);

		//	List<IpAddressInfo> ipAddressInfos = new List<IpAddressInfo>();

		//	while (reader.Read())
		//	{
		//		int address = Convert.ToInt32(reader["Address"]);

		//		string tableName2 = reader["TableName"].ToString();


		//		string useTo = "";
		//		string userDepartment = "";

		//		string email = "";
		//		string userTel = "";
		//		string phoneNumber = "";
		//		string deviceType = "";

		//		string deviceModel = "";
		//		string deviceMac = "";
		//		string deviceAddress = "";
		//		DateTime applyTime = DateTime.MinValue;

		//		string ratify = "";
		//		DateTime ratifyTime = DateTime.MinValue;

		//		IpAddressInfo ipAddress = new IpAddressInfo(address, addressStatus, description,
		//			applyUser, useTo, userDepartment, email, userTel, phoneNumber, deviceType, deviceModel, deviceMac,
		//			deviceAddress, applyTime, ratify, ratifyTime);

		//		ipAddressInfos.Add(ipAddress);
		//	}

		//	reader.Dispose();

		//	WriteAddressConfig(ipAddressInfos);
		//}

		///// <summary>
		///// 配置图形化显示
		///// </summary>
		///// <param name="ipAddressInfos"></param>
		//private void WriteAddressConfig(List<IpAddressInfo> ipAddressInfos)
		//{

		//	int x = ipAddressInfos.Count;

		//	for (int i = 0; i < x; i++)
		//	{
		//		//bool status = false;

		//		string description = null;
		//		Brush colorBrush = null;
		//		int status = ipAddressInfos[i].AddressStatus;

		//		switch (status)
		//		{
		//			case 0:
		//				colorBrush = Brushes.DimGray;
		//				description = "网段IP地址";

		//				break;

		//			case 1:
		//				colorBrush = Brushes.DarkCyan;
		//				description = "保留IP地址";

		//				break;

		//			case 2:
		//				colorBrush = Brushes.LimeGreen;
		//				description = "可用IP地址";
		//				//status = true;

		//				break;

		//			case 3:
		//				colorBrush = Brushes.DarkOrange;
		//				description = "锁定IP地址" + "\r" + ipAddressInfos[i].Description + "\r" + ipAddressInfos[i].ApplyUser;


		//				break;

		//			case 4:
		//				colorBrush = Brushes.OrangeRed;
		//				description = "已用IP地址" + "\r" + ipAddressInfos[i].Description + "\r" + ipAddressInfos[i].ApplyUser;


		//				break;


		//			case 5:
		//				colorBrush = Brushes.LightSlateGray;
		//				description = "广播IP地址";


		//				break;

		//		}


		//		Button newButton = new Button()
		//		{
		//			Width = 55,
		//			Height = 30,
		//			Background = colorBrush,
		//			Foreground = Brushes.AliceBlue,
		//			Content = i.ToString(),
		//			ToolTip = description,
		//			Tag = status,
		//			Style = (Style)this.FindResource("MaterialDesignRaisedLightButton"),
		//			Margin = new Thickness(3),

		//		};



		//		newButton.Click += NewButton_Click;





		//		//显示到图形化区域
		//		GraphicsPlan.Children.Add(newButton);

		//	}

		//}

		private void MenuBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			MessageBox.Show(MenuBox.SelectedIndex.ToString());
		}
	}


}
