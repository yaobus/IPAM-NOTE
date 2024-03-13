using IPAM_NOTE.DatabaseOperation;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
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
			ListView.ItemsSource = ipAddressInfos;

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
		ObservableCollection<IpAddressInfo> ipAddressInfos = new ObservableCollection<IpAddressInfo>();


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
					string network = reader["TableName"].ToString();

					ipAddressInfos.Add(new IpAddressInfo(id, network));


				}

				reader.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}");
			}
		}

	}


}
