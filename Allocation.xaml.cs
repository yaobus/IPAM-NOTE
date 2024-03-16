using IPAM_NOTE.DatabaseOperation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace IPAM_NOTE
{
	/// <summary>
	/// Allocation.xaml 的交互逻辑
	/// </summary>
	public partial class Allocation : Window
	{
		public Allocation()
		{
			InitializeComponent();
		}

		private DbClass dbClass;

		private void Allocation_OnLoaded(object sender, RoutedEventArgs e)
		{
			
			NetworkBlock.Text = "网段:" + DataBrige.TempAddress.Network;

			AddressBlock.Text = "当前所选IP:" + DataBrige.SelectIp;

			UserTextBox.Text = DataBrige.IpAddressInfos[Convert.ToInt32(DataBrige.SelectIp)].User;

			DescriptionTextBox.Text = DataBrige.IpAddressInfos[Convert.ToInt32(DataBrige.SelectIp)].Description;

			string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";
			string dbName = "Address_database.db";

			dbFilePath = dbFilePath + dbName;

			dbClass = new DbClass(dbFilePath);
			dbClass.OpenConnection();


		}


		/// <summary>
		/// 保存分配
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (UserTextBox.Text != "" && DescriptionTextBox.Text != "")
			{
				string tableName=DataBrige.TempAddress.TableName;

				string sql = string.Format("UPDATE {0} SET \"User\" = '{1}', \"AddressStatus\" = '2' , \"Description\" = '{2}' WHERE Address = {3}", tableName,UserTextBox.Text,DescriptionTextBox.Text,DataBrige.SelectIp);

				//Console.WriteLine(sql);

				dbClass.ExecuteQuery(sql);

				this.Close();

			}
			else
			{
				MessageBox.Show("请填写地址分配信息！", "信息不完整", MessageBoxButton.OK, MessageBoxImage.Information);
			}


		}

		/// <summary>
		/// 删除分配
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <exception cref="NotImplementedException"></exception>
		private void ReleaseButton_OnClick(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result= MessageBox.Show("你正在删除该IP地址的分配信息，此操作不可逆！继续？", "注意", MessageBoxButton.YesNo, MessageBoxImage.Information);

			if (result == MessageBoxResult.Yes)
			{
				string tableName = DataBrige.TempAddress.TableName;

				string sql = string.Format("UPDATE {0} SET \"User\" = '', \"AddressStatus\" = '1' , \"Description\" = '' WHERE Address = {1}", tableName, DataBrige.SelectIp);

				//Console.WriteLine(sql);

				dbClass.ExecuteQuery(sql);

				this.Close();
			}

		}

		/// <summary>
		/// 窗口关闭
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Allocation_OnClosing(object sender, CancelEventArgs e)
		{
			this.DialogResult = true;
		}


	}
}
