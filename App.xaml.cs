using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace IPAM_NOTE
{
	/// <summary>
	/// App.xaml 的交互逻辑
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{

			base.OnStartup(e);

			// 检查 SQLite 数据库文件是否存在
			string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";
			string dbName = "Address_database.db";

			if (!Directory.Exists(dbFilePath))
			{
				Directory.CreateDirectory(dbFilePath);
			}

			dbFilePath = Path.Combine(dbFilePath, dbName);


			if (!File.Exists(dbFilePath))
			{
				// 如果数据库文件不存在，则创建一个新的数据库文件
				SQLiteConnection.CreateFile(dbFilePath);

				// 创建数据库表
				using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
				{
					connection.Open();
					string createTableQuery = "CREATE TABLE \"Network\" (\r\n  \"Id\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,\r\n  \"TableName\" TEXT,\r\n  \"Network\" TEXT,\r\n  \"Netmask\" TEXT,\r\n  \"Description\" TEXT,\r\n  \"Del\" TEXT\r\n)";
					using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
					{
						command.ExecuteNonQuery();
					}

				}
			}

		}
	}
}
