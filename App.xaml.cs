using IPAM_NOTE.DatabaseOperation;
using IPAM_NOTE.UserWindows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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

			//订阅全局异常信息
			this.DispatcherUnhandledException += App_DispatcherUnhandledException;


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
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        // 创建表的 SQL 语句
                        command.CommandText = $"CREATE TABLE \"Devices\" (\r\n  \"Id\" INTEGER PRIMARY KEY AUTOINCREMENT,\r\n  \"TableName\" TEXT,\r\n  \"Name\" TEXT,\r\n  \"Model\" TEXT,\r\n  \"Number\" TEXT,\r\n  \"People\" TEXT,\r\n  \"Date\" TEXT,\r\n  \"Description\" TEXT,\r\n  \"Eport\" integer,\r\n  \"EportTag\" TEXT,\r\n  \"Fport\" integer,\r\n  \"FportTag\" TEXT,\r\n  \"Mport\" integer,\r\n  \"MportTag\" TEXT,\r\n  \"Dport\" integer,\r\n  \"DportTag\" TEXT\r\n);";
                        command.ExecuteNonQuery();
                        command.CommandText = "CREATE TABLE \"Network\" (\r\n  \"Id\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,\r\n  \"TableName\" TEXT,\r\n  \"Network\" TEXT,\r\n  \"Netmask\" TEXT,\r\n  \"Description\" TEXT,\r\n  \"Del\" TEXT\r\n)";
                        command.ExecuteNonQuery();
                        command.CommandText = $"CREATE TABLE \"User\" (\r\n  \"Id\" INTEGER PRIMARY KEY AUTOINCREMENT,\r\n  \"Password\" TEXT);";
                        command.ExecuteNonQuery();
                        command.CommandText = $"CREATE TABLE \"ModelPreset\" (\r\n  \"Id\" INTEGER PRIMARY KEY AUTOINCREMENT, \r\n \"ModelType\" TEXT, \r\n \"Brand\" TEXT,\r\n  \"Model\" TEXT,\r\n  \"Ethernet\" TEXT,\r\n  \"Fiber\" TEXT,\r\n  \"Disk\" TEXT\r\n，,\r\n  \"Manage\" TEXT\r\n);";
                        command.ExecuteNonQuery();
                    }
                }
			}

            

        }

        

        /// <summary>
        /// 全局异常处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			// 处理异常
			// 记录异常信息、显示友好的错误提示框等
			Console.WriteLine(e.Exception);
			e.Handled = true; // 标记为已处理，防止应用程序终止
		}


    }
}
