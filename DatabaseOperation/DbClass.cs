using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			using (SQLiteCommand command = new SQLiteCommand(query, connection))
			{
				using (SQLiteDataReader reader = command.ExecuteReader())
				{
					return reader;
				}
			}
		}
	}
}
