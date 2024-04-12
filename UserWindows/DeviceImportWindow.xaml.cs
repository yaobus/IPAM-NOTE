using CsvHelper;
using IPAM_NOTE.DatabaseOperation;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
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
using static IPAM_NOTE.ImportWindow;
using static IPAM_NOTE.ViewMode;

namespace IPAM_NOTE.UserWindows
{
	/// <summary>
	/// DeviceImportWindow.xaml 的交互逻辑
	/// </summary>
	public partial class DeviceImportWindow : Window
	{
		public DeviceImportWindow()
		{
			InitializeComponent();
		}


		

		private void DeviceImportWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			DevicesComboBox.ItemsSource = DataBrige.DevicesList;
		}

		/// <summary>
		/// 导出模板
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExportButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (DevicesComboBox.SelectedIndex != -1)
			{
				ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

				string tableName = DataBrige.DeviceInfos[DevicesComboBox.SelectedIndex].TableName;

				LoadDeviceConfig(tableName);



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
					ExportToExcel(DataBrige.DevicePortInfos, filePath);
					MessageBox.Show("设备内容导出完毕", "导出", MessageBoxButton.OK, MessageBoxImage.Information);
				}



			}
			else
			{
				MessageBox.Show("请先选择要导出的设备", "你是否遗忘了什么步骤？", MessageBoxButton.OK, MessageBoxImage.Information);
			}




		}



		public void ExportToExcel(List<DevicePortInfo> dataList, string filePath)
		{



				using (var package = new ExcelPackage())
				{
					var sheet = package.Workbook.Worksheets.Add(DataBrige.DeviceInfos[DevicesComboBox.SelectedIndex].TableName);


					// 写入标题行
					sheet.Cells[1, 1].Value = "PortType";
					sheet.Cells[1, 2].Value = "PortNumber";
					sheet.Cells[1, 3].Value = "PortStatus";
					sheet.Cells[1, 4].Value = "PortTag1";
					sheet.Cells[1, 5].Value = "PortTag2";
					sheet.Cells[1, 6].Value = "PortTag3";
					sheet.Cells[1, 7].Value = "Description";

					// 写入数据
					int rowIndex = 2;
					foreach (var info in dataList)
					{
						sheet.Cells[rowIndex, 1].Value = info.PortType;
						sheet.Cells[rowIndex, 2].Value = info.PortNumber;
						sheet.Cells[rowIndex, 3].Value = info.PortStatus;
						sheet.Cells[rowIndex, 4].Value = info.PortTag1;
						sheet.Cells[rowIndex, 5].Value = info.PortTag2;
						sheet.Cells[rowIndex, 6].Value = info.PortTag3; ;
						sheet.Cells[rowIndex, 7].Value = info.Description; ;
						rowIndex++;
					}


					// 保存Excel文件
					FileInfo excelFile = new FileInfo(filePath);



					package.SaveAs(excelFile);
				}
			




		}

		/// <summary>
		/// 加载IP地址表配置
		/// </summary>
		private void LoadDeviceConfig(string tableName)
		{

			string sql = string.Format("SELECT * FROM {0} ORDER BY PortType ASC", tableName);



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

				DataBrige.DevicePortInfos.Clear();


				while (reader.Read())
				{
					string portType = reader["PortType"].ToString();
					string portNumber = reader["PortNumber"].ToString();
					int portStatus = Convert.ToInt32(reader["PortStatus"]);
					string portTag1 = reader["PortTag1"].ToString();
					string portTag2 = reader["PortTag2"].ToString();
					string portTag3 = reader["PortTag3"].ToString();
					string description = reader["Description"].ToString();



					DevicePortInfo portInfo = new DevicePortInfo(portType, portNumber, portStatus, portTag1, portTag2,
						portTag3, description);

					DataBrige.DevicePortInfos.Add(portInfo);
				}



				reader.Dispose();



			}
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
		private void ImportButton_OnClick(object sender, RoutedEventArgs e)
		{

			MessageBoxResult result = MessageBox.Show("注意！导入数据将覆盖数据库内原有数据，是否继续？\r\r\r如果导入之后出现异常可前往数据恢复页面\r进入之后选择'备份还原'即可看到恢复选项", "危险操作", MessageBoxButton.YesNo,
				MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes)
			{
				if (DevicesComboBox.SelectedIndex != -1 && File.Exists(SourceData.Text))
				{
					string tableName = DataBrige.DeviceInfos[DevicesComboBox.SelectedIndex].TableName;

					ImportDataFromCsv(SourceData.Text, tableName);
				}
				else
				{
					MessageBox.Show("未选择被导入的网段或未找到被导入的CSV文件", "注意", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void ImportDataFromCsv(string csvFilePath, string tableName)
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
						List<ViewMode.DevicePortFromCsv> records;

						using (var reader = new StreamReader(utf8CsvFilePath))
						using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
						{
							records = csv.GetRecords<ViewMode.DevicePortFromCsv>().ToList();



							// 遍历 CSV 文件中的每一行数据
							foreach (var record in records)
							{
								// 从 CSV 记录中获取需要插入或更新到数据库的字段值
								string type = record.PortType;

								string number= record.PortNumber;

								

								// 检查数据库中是否已存在相同的 type和端口

								command.CommandText = string.Format($"SELECT COUNT(*) FROM {tableName} WHERE (PortType = '{type}' AND PortNumber = '{number}') ");

								int count = Convert.ToInt32(command.ExecuteScalar());



								string portStatus = record.PortStatus;
								string portTag1 = record.PortTag1;
								string portTag2 = record.PortTag2;
								string portTag3 = record.PortTag3;
								string description = record.Description;


								if (count > 0)
								{
									// 数据库中已存在相同的 端口号，执行更新操作
									command.CommandText =
										string.Format(
											$"UPDATE {tableName} SET PortStatus = {portStatus}, PortTag1 = '{portTag1}',PortTag2 = '{portTag2}', PortTag3 = '{portTag3}', Description = '{description}' WHERE (PortType ='{type}' AND PortNumber='{number}')");


								}
								else
								{
									// 数据库中不存在相同的 IpAddress，执行插入操作
									command.CommandText =
										string.Format(
											$"INSERT INTO {tableName} (PortType, PortNumber, PortTag1, PortTag2,PortTag3,Description) VALUES ({type}, {number}, {portTag1}, {portTag2}, {portTag3}, {description})");
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
				string utf8CsvFilePath = System.IO.Path.GetTempFileName();
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


	}
}
