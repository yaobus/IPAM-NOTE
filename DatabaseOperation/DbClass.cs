using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf.Transitions;

namespace IPAM_NOTE.DatabaseOperation
{
	internal class DbClass
	{


		public SQLiteConnection connection;


		public DbClass(string dbPath)
		{

			connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
		

		}







		public void OpenConnection()
		{
			if (connection.State != System.Data.ConnectionState.Open)
			{
				connection.Open();
			}
		}


		public void CloseConnection()
		{
			if (connection.State != System.Data.ConnectionState.Closed)
			{
				connection.Close();
			}
		}

		public void CreateTable(string query)
		{
			SQLiteCommand command = new SQLiteCommand(query, connection);
			command.ExecuteNonQuery();
		}


		public  SQLiteDataReader ExecuteQuery(string query)
		{

            Console.WriteLine(query);
			using (SQLiteCommand command = new SQLiteCommand(query, connection))
			{
				using (SQLiteDataReader reader = command.ExecuteReader())
				{
					return reader;
				}
			}
		}




		// 执行 SQL 查询函数
		static void ExecuteSql(SQLiteConnection connection, string sqlQuery)
		{
			// 创建命令对象
			using (SQLiteCommand command = new SQLiteCommand(sqlQuery, connection))
			{
				try
				{
					// 执行 SQL 命令
					int rowsAffected = command.ExecuteNonQuery();

					// 输出受影响的行数
					Console.WriteLine($"Rows Affected: {rowsAffected}");
				}
				catch (Exception ex)
				{
					// 处理异常
					Console.WriteLine($"Error executing SQL query: {ex.Message}");
				}
			}
		}



		/// <summary>
		/// 查询表中的数据数量，返回数据条数
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static int ExecuteScalarTableNum(string sql,SQLiteConnection connection)
		{
			
				

				using (SQLiteCommand command = new SQLiteCommand(sql,connection))
				{
					// 使用 ExecuteScalar 获取总行数
					int rowCount = Convert.ToInt32(command.ExecuteScalar());

					return rowCount;

					//MessageBox.Show( GlobalFunction.IpAddressConvert.IpToDecimal(IpTextBox.Text).ToString());

				}


			

		}


		/// <summary>
		/// 异步插入数据
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public async Task InsertDataAsync(string sql)
		{
			using (connection)
			{
				await connection.OpenAsync();

				using (SQLiteCommand command = connection.CreateCommand())
				{
					
					command.CommandText = sql;

					// 异步执行插入操作
					await command.ExecuteNonQueryAsync();
				}
			}
		}


        // 检测表是否存在
        public bool IsTableExists(string tableName)
        {

                
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}';";
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        return reader.HasRows; // 如果有行，则表存在，否则不存在
                    }
                }
            
        }


        // 创建设备表
        public void CreateTableIfNotExists(string tableName)
        {
            if (!IsTableExists(tableName))
            {

                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        // 创建表的 SQL 语句
                        command.CommandText = $"CREATE TABLE \"Devices\" (\r\n  \"Id\" INTEGER PRIMARY KEY AUTOINCREMENT,\r\n  \"TableName\" TEXT,\r\n  \"Name\" TEXT,\r\n  \"Model\" TEXT,\r\n  \"Number\" TEXT,\r\n  \"People\" TEXT,\r\n  \"Date\" TEXT,\r\n  \"Description\" TEXT,\r\n  \"Eport\" integer,\r\n  \"EportTag\" TEXT,\r\n  \"Fport\" integer,\r\n  \"FportTag\" TEXT,\r\n  \"Mport\" integer,\r\n  \"MportTag\" TEXT,\r\n  \"Dport\" integer,\r\n  \"DportTag\" TEXT\r\n);";
                        command.ExecuteNonQuery();
                    }
                
            }
        }
    }
}
