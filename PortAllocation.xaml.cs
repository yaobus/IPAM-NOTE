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
	/// PortAllocation.xaml 的交互逻辑
	/// </summary>
	public partial class PortAllocation : Window
	{
		public PortAllocation()
		{
			InitializeComponent();
		}

		private void PortAllocation_OnLoaded(object sender, RoutedEventArgs e)
		{
			string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";
			string dbName = "Address_database.db";

			dbFilePath = dbFilePath + dbName;

			dbClass = new DbClass(dbFilePath);

			dbClass.OpenConnection();

			DeviceBlock.Text = DataBrige.SelectDeviceInfo.Name + " " + DataBrige.SelectDeviceInfo.Number;
			ModelTextBlock.Text ="型号:"+ DataBrige.SelectDeviceInfo.Model;
			DescriptionBlock.Text= "备注:" + DataBrige.SelectDeviceInfo.Description;
			string type = DataBrige.SelectDevicePortInfo.PortType;
			
			switch (type)
			{
				case "E":
					TypeBlock.Text = "当前选择RJ45网口："+ DataBrige.SelectDevicePortInfo.PortNumber;
					break;

				case "F":
					TypeBlock.Text = "当前选择光纤网口：" + DataBrige.SelectDevicePortInfo.PortNumber;
					break;
				case "D":
					TypeBlock.Text = "当前选择硬盘插槽：" + DataBrige.SelectDevicePortInfo.PortNumber;
					break;
				case "M":
					TypeBlock.Text = "当前选择管理接口：" + DataBrige.SelectDevicePortInfo.PortNumber; 
					break;
			}

			Tag1TextBox.Text = DataBrige.SelectDevicePortInfo.PortTag1;
			Tag2TextBox.Text = DataBrige.SelectDevicePortInfo.PortTag2;
			Tag3TextBox.Text = DataBrige.SelectDevicePortInfo.PortTag3;
			DescriptionTextBox.Text = DataBrige.SelectDevicePortInfo.Description;

		}

		private DbClass dbClass;

		private void SaveButton_OnClick(object sender, RoutedEventArgs e)
		{
			string tableName = DataBrige.SelectDeviceInfo.TableName;
			string type = DataBrige.SelectDevicePortInfo.PortType;
			string port = DataBrige.SelectDevicePortInfo.PortNumber;

			string	sql = string.Format($"UPDATE {tableName} SET \"PortStatus\" = 1, \"PortTag1\" = '{Tag1TextBox.Text}'  , \"PortTag2\" = '{Tag2TextBox.Text}' , \"PortTag3\" = '{Tag3TextBox.Text}', \"Description\" = '{DescriptionTextBox.Text}' WHERE (PortType = '{type}' AND PortNumber = '{port}')");
			
			dbClass.ExecuteQuery(sql);//写入端口信息



			int index = 0;
			foreach (var item in DataBrige.DevicePortInfos)
			{
				if (item.PortType == type && item.PortNumber == port)
				{
					DataBrige.DevicePortInfos[index].PortStatus = 1;
					DataBrige.DevicePortInfos[index].PortTag1 = Tag1TextBox.Text;
					DataBrige.DevicePortInfos[index].PortTag2 = Tag2TextBox.Text;
					DataBrige.DevicePortInfos[index].PortTag3 = Tag3TextBox.Text;
					DataBrige.DevicePortInfos[index].Description = DescriptionTextBox.Text;
					

					break;
				}

				index++;
			}

			


			this.Close();
		}

		private void PortAllocation_OnClosing(object sender, CancelEventArgs e)
		{
			this.DialogResult = true;
		}
	}
}
