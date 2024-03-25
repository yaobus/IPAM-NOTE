using CsvHelper;
using CsvHelper.Configuration;
using IPAM_NOTE.DatabaseOperation;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
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
using static IPAM_NOTE.ViewMode;
using LicenseContext = OfficeOpenXml.LicenseContext;
using Path = System.IO.Path;

namespace IPAM_NOTE
{
	/// <summary>
	/// ImportWindow.xaml 的交互逻辑
	/// </summary>
	public partial class ImportWindow : Window
	{
		public ImportWindow()
		{
			InitializeComponent();
		}

		private DbClass dbClass;

		private void ImportWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			NetworkComboBox.ItemsSource = DataBrige.ComboBoxAddressList;

		

		}

		/// <summary>
		/// 浏览CSV文件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BrowseButton_OnClick(object sender, RoutedEventArgs e)
		{
			// 创建 OpenFileDialog 实例
			OpenFileDialog openFileDialog = new OpenFileDialog();

			// 设置文件类型筛选，仅允许选择 CSV 文件
			openFileDialog.Filter = "CSV files (*.csv)|*.csv";

			// 显示对话框并获取用户选择的结果
			bool? result = openFileDialog.ShowDialog();

			// 如果用户选择了文件，则将文件路径加载到 TextBox 中
			if (result == true)
			{
				string selectedFilePath = openFileDialog.FileName;
				SourceData.Text = selectedFilePath;
			}

		}


		/// <summary>
		/// 导入数据
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private  void ImportButton_OnClick(object sender, RoutedEventArgs e)
		{


			MessageBoxResult result = MessageBox.Show("注意！导入数据将覆盖数据库内原有数据，是否继续？\r\r\r如果导入之后出现异常可前往数据恢复页面\r数据库恢复页面在主窗口左下角'更多功能'\r进入之后选择'备份还原'即可看到恢复选项", "危险操作", MessageBoxButton.YesNo,
				MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes)
			{
				if (NetworkComboBox.SelectedIndex != -1 && File.Exists(SourceData.Text))
				{
					string tableName = DataBrige.ComBoxAddressInfos[NetworkComboBox.SelectedIndex].TableName;

					ImportDataFromCsv(SourceData.Text, tableName);
				}
				else
				{
					MessageBox.Show("未选择被导入的网段或未找到被导入的CSV文件", "注意", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}


		}




		private  void ImportDataFromCsv(string csvFilePath,string tableName)
		{
			ProgressBar.IsIndeterminate = true;
			string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";
			string dbName = "Address_database.db";

			dbFilePath = dbFilePath + dbName;

			// 打开 SQLite 数据库连接
			string connectionString = string.Format("Data Source={0};Version=3;", dbFilePath);

			try
			{
				// 检查CSV文件编码并转换为UTF-8
				string utf8CsvFilePath = CheckAndConvertCsvToUtf8(csvFilePath);


				using (SQLiteConnection connection = new SQLiteConnection(connectionString))
				{
					connection.Open();

					// 创建一个 SQLiteCommand 对象来执行 SQL 查询和命令
					using (SQLiteCommand command = connection.CreateCommand())
					{

						// 将 CSV 文件中的数据读取到一个列表中
						List<ViewMode.IpAddressInfoFromCsv> records;

						using (var reader = new StreamReader(utf8CsvFilePath))
						using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
						{
							records = csv.GetRecords<ViewMode.IpAddressInfoFromCsv>().ToList();



							// 遍历 CSV 文件中的每一行数据
							foreach (var record in records)
							{
								// 从 CSV 记录中获取需要插入或更新到数据库的字段值
								string address = record.Address.ToString();



								// 获取其他字段的值...

								// 检查数据库中是否已存在相同的 IpAddress

								command.CommandText = string.Format("SELECT COUNT(*) FROM {0} WHERE Address = {1}",
									tableName, address);

								int count = Convert.ToInt32(command.ExecuteScalar());



								int addressStatus = record.AddressStatus;
								string user = record.User;
								string description = record.Description;


								if (count > 0)
								{
									// 数据库中已存在相同的 IpAddress，执行更新操作
									command.CommandText =
										string.Format(
											"UPDATE {0} SET AddressStatus = {1}, User = '{2}', Description = '{3}'  WHERE Address = {4}",
											tableName, addressStatus, user, description, address);


								}
								else
								{
									// 数据库中不存在相同的 IpAddress，执行插入操作
									command.CommandText =
										string.Format(
											"INSERT INTO {0} (Address, AddressStatus, User, Description) VALUES ({0}, {1}, {2}, {3})",
											tableName, address, user, description);
								}


								Console.WriteLine(command.CommandText);

								command.ExecuteNonQuery();
							}

							ProgressBar.IsIndeterminate = false;
						}

					}

					MessageBox.Show("数据导入完毕，请检查数据是否正常。", "导入完成", MessageBoxButton.OK,
						MessageBoxImage.Information);
					connection.Close();
					this.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"导入CSV文件时出错：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private string CheckAndConvertCsvToUtf8(string csvFilePath)
		{
			// 检查文件编码
			Encoding encoding = GetCsvFileEncoding(csvFilePath);

			// 如果不是UTF-8，则转换为UTF-8并返回新的文件路径
			if (encoding != Encoding.UTF8)
			{
				string utf8CsvFilePath = Path.GetTempFileName();
				ConvertCsvToUtf8(csvFilePath, utf8CsvFilePath);
				return utf8CsvFilePath;
			}

			// 文件已经是UTF-8编码，直接返回原始文件路径
			return csvFilePath;
		}

		/// <summary>
		/// 检测CSV文件编码
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		private Encoding GetCsvFileEncoding(string filePath)
		{
			using (var reader = new PinnedStreamReader(filePath))
			{
				return reader.CurrentEncoding;
			}
		}

		/// <summary>
		/// 转换CSV为UTF-8编码
		/// </summary>
		/// <param name="sourceFilePath"></param>
		/// <param name="targetFilePath"></param>
		private void ConvertCsvToUtf8(string sourceFilePath, string targetFilePath)
		{
			string csvContent = File.ReadAllText(sourceFilePath, Encoding.Default);
			File.WriteAllText(targetFilePath, csvContent, Encoding.UTF8);
		}



		private void ImportWindow_OnClosing(object sender, CancelEventArgs e)
		{
			this.DialogResult = true;
		}



		private void ExportButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (NetworkComboBox.SelectedIndex != -1)
			{
				ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

				string tableName = DataBrige.ComBoxAddressInfos[NetworkComboBox.SelectedIndex].TableName;

					LoadAddressConfig(tableName);



					// 创建 SaveFileDialog 实例
					SaveFileDialog saveFileDialog = new SaveFileDialog();
					saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
					saveFileDialog.FilterIndex = 1;
					saveFileDialog.RestoreDirectory = true;


					saveFileDialog.FileName = tableName + ".xlsx";



					// 显示 SaveFileDialog
					bool? result = saveFileDialog.ShowDialog();

					if (result == true)
					{
						// 获取用户选择的文件路径
						string filePath = saveFileDialog.FileName;
						ExportToExcel(DataBrige.ipAddressInfos, filePath,tableName);
						MessageBox.Show("网段内容导出完毕", "导出", MessageBoxButton.OK, MessageBoxImage.Information);
					}

				

			}
			else
			{
				MessageBox.Show("请选择要导出的网段", "你是否遗忘了什么步骤？", MessageBoxButton.OK, MessageBoxImage.Information);
			}




		}



		public void ExportToExcel(List<IpAddressInfo> dataList, string filePath,string tableName)
		{

			if (DataBrige.LoadType == 1)
			{
				using (var package = new ExcelPackage())
				{
					var sheet = package.Workbook.Worksheets.Add(tableName);




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
					var sheet = package.Workbook.Worksheets.Add(tableName);




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


		/// <summary>
		/// 加载IP地址表配置
		/// </summary>
		private void LoadAddressConfig(string tableName)
		{

			string sql = string.Format("SELECT * FROM {0} ORDER BY Address ASC", tableName);



			string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";
			string dbName = "Address_database.db";

			dbFilePath = dbFilePath + dbName;

			// 打开 SQLite 数据库连接
			string connectionString = string.Format("Data Source={0};Version=3;", dbFilePath);

			using (SQLiteConnection connection = new SQLiteConnection(connectionString))
			{
				connection.Open();


				SQLiteCommand command = new SQLiteCommand(sql, connection);

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

					IpAddressInfo ipAddress = new IpAddressInfo(address, addressStatus, user, description,
						IPStatus.Unknown,
						-1, hostName, macAddress);

					DataBrige.ipAddressInfos.Add(ipAddress);
				}

				DataBrige.IpAddressInfos = DataBrige.ipAddressInfos;

				//reader.Dispose();



			}
		}





		public class PinnedStreamReader : StreamReader
		{
			public PinnedStreamReader(string path) : base(path, true)
			{
			}

			public Encoding CurrentEncoding { get; private set; }

			public override int Peek()
			{
				CurrentEncoding = this.CurrentEncoding ?? DetectEncoding();
				return base.Peek();
			}

			private Encoding DetectEncoding()
			{
				byte[] preamble = new byte[5];
				this.BaseStream.Position = 0;
				this.BaseStream.Read(preamble, 0, 5);

				if (preamble[0] == 0xEF && preamble[1] == 0xBB && preamble[2] == 0xBF)
				{
					return Encoding.UTF8;
				}
				else if (preamble[0] == 0xFE && preamble[1] == 0xFF)
				{
					return Encoding.BigEndianUnicode; // UTF-16 BE
				}
				else if (preamble[0] == 0xFF && preamble[1] == 0xFE)
				{
					return Encoding.Unicode; // UTF-16 LE
				}
				else if (preamble[0] == 0 && preamble[1] == 0 && preamble[2] == 0xFE && preamble[3] == 0xFF)
				{
					return Encoding.UTF32; // UTF-32 BE
				}
				else if (preamble[0] == 0xFF && preamble[1] == 0xFE && preamble[2] == 0 && preamble[3] == 0)
				{
					return Encoding.UTF32; // UTF-32 LE
				}
				else
				{
					return Encoding.Default; // Default ANSI encoding
				}
			}
		}
	}
}
