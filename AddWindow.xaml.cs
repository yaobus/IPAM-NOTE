using IPAM_NOTE.DatabaseOperation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
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
	/// AddWindow.xaml 的交互逻辑
	/// </summary>
	public partial class AddWindow : Window
	{
		public AddWindow()
		{
			InitializeComponent();
		}

		private DbClass dbClass;



		private void AddWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			LoadStatus=true;

			if (DataBrige.AddStatus==0)
			{
				this.Title = "添加网段";


			}
			else
			{
				this.Title = "编辑网段信息";
				IpTextBox.Text = DataBrige.TempAddress.Network;
				MaskText.Text = DataBrige.TempAddress.NetMask;
				IpDescription.Text = DataBrige.TempAddress.Description;

				CalculateNetMaskLocated();

				IpTextBox.IsEnabled = false;
				MaskText.IsEnabled = false;
				MaskSlider.IsEnabled=false;
				UpdateIPCalculations();
			}

			string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";
			string dbName = "Address_database.db";

			dbFilePath = dbFilePath + dbName;

			dbClass = new DbClass(dbFilePath);
			dbClass.OpenConnection();


		}

		/// <summary>
		/// 保存新建的网段
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			//添加网段
			if (DataBrige.AddStatus == 0)
			{

				//判断是不是瞎几把写的IP地址
				if (IsValidIp(IpTextBox.Text) == true)
				{
					//计算IP
					UpdateIPCalculations();

					//网段地址
					string ip = Network.Text;
					string netMask = MaskText.Text;

					if (IpDescription.Text != "")
					{
						string tableName;

						string sqlTemp = string.Format("SELECT COUNT(*) FROM Network WHERE `Network` = '{0}' AND `Netmask` = '{1}'", ip, netMask);

						

						int num = DbClass.ExecuteScalarTableNum(sqlTemp, dbClass.connection);

						//IP地址段已存在
						if (num > 0)
						{
							string msg = string.Format("已存在同配置网段{0}个，是否继续添加同配置网段？", num.ToString());

							MessageBoxResult result = MessageBox.Show(msg, "确认", MessageBoxButton.YesNo,
								MessageBoxImage.Information);

							if (result == MessageBoxResult.Yes)
							{
								// 用户点击了"是"按钮，执行相关操作
								tableName = CreateTableName(ip) + "_" + (num + 1).ToString();
								
								Console.WriteLine(tableName);

								//插入网段信息总表的数据
								string sql = string.Format(
									"INSERT INTO Network (TableName,Network,Netmask,Description,Del) VALUES ('{0}','{1}','{2}','{3}','{4}')",
									tableName, ip, netMask, IpDescription.Text, 0);

								dbClass.ExecuteQuery(sql);

								//创建表
								CreateTable(tableName);

								//装载初始化数据
								InitializedData(tableName);


								this.DialogResult = true;
								this.Close();



							}
							else if (result == MessageBoxResult.No)
							{
								// 用户点击了"否"按钮，取消操作或进行其他处理


							}


						}
						else
						{

							tableName = CreateTableName(ip) + "_1";

							Console.WriteLine(tableName);

							//插入网段信息总表的数据
							string sql = string.Format(
								"INSERT INTO Network (TableName,Network,Netmask,Description,Del) VALUES ('{0}','{1}','{2}','{3}','{4}')",
								tableName, ip, netMask, IpDescription.Text, 0);

							dbClass.ExecuteQuery(sql);

							//插入网段信息总表的数据

							//创建表
							CreateTable(tableName);

							//装载初始化数据
							InitializedData(tableName);


							this.DialogResult = true;
							this.Close();



						}


					}
					else
					{

						MessageBox.Show("为了便于后期管理，必须填写网段说明!", "确定", MessageBoxButton.OK, MessageBoxImage.Information);


					}

				}
				else
				{
					MessageBox.Show("IP地址不合法，请检查IP地址是否正确!", "确定", MessageBoxButton.OK, MessageBoxImage.Information);
				}

			}
			else//编辑网段
			{
				

				if (IpDescription.Text!="")
				{
					string tableName = DataBrige.TempAddress.TableName;

					string sqlTemp = string.Format("UPDATE \"Network\" SET \"Description\" = '{0}' WHERE TableName = '{1}'", IpDescription.Text, tableName);

					Console.WriteLine(sqlTemp);

					dbClass.ExecuteQuery(sqlTemp);



					this.Close();
				}





			}







		}

		/// <summary>
		/// 判断是否是合法IP
		/// </summary>
		/// <param name="ipAddress"></param>
		/// <returns></returns>
		static bool IsValidIp(string ipAddress)
		{
			IPAddress address;
			return IPAddress.TryParse(ipAddress, out address);
		}

		/// <summary>
		/// 生成表名
		/// </summary>
		/// <returns></returns>
		private string CreateTableName(string address)
		{
			string name = Network.Text;
			name = "tb_" + name.Replace(".", "_");

			return name;
		}

		/// <summary>
		/// 创建一张数据表
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		private void CreateTable(string tableName)
		{
			string sql = string.Format(
				"CREATE TABLE IF NOT EXISTS {0} (Address INTEGER  NOT NULL ,AddressStatus INTEGER , User VARCHAR ( 16 ),Description VARCHAR ( 80 ),HostName VARCHAR ( 64 ),MacAddress VARCHAR ( 20 ))",
				tableName);

			dbClass.ExecuteQuery(sql);


		}

		/// <summary>
		/// 填充网段表单初始数据
		/// </summary>
		private void InitializedData(string tableName)
		{
			string[] parts = Network.Text.Split('.');
			
			//取出第一个IP
			int  firstIp =Convert.ToInt32( parts[3]);


			parts = Broadcast.Text.Split('.');

			//取出最后一个IP,广播IP
			int LastIp = Convert.ToInt32(parts[3]);



			int x = Convert.ToInt32(NumBox.Text);

			for (int i = 0; i < x; i++)
			{
				//IP地址锁定状态：0不可用IP，1可用IP
				int addressStatus = 0;

				int ip = firstIp + i;

				

				List<string> lockip = new List<string>();

				if (ip == firstIp)
				{
					addressStatus = 0;

				}
				else
				{
					if (ip == LastIp )
					{

						addressStatus = 3;
					}
					else
					{
						addressStatus = 1;
					}
				}

				string sql = string.Format("INSERT INTO `{0}` (`Address`, `AddressStatus`) VALUES ({1}, {2})",
					tableName, ip, addressStatus);


				//Console.WriteLine(sql);
				//异步执行
				dbClass.ExecuteQuery(sql);
			}


		}


		/// <summary>
		/// 传递窗口关闭信息
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddWindow_OnClosing(object sender, CancelEventArgs e)
		{
			this.DialogResult = true;
		}


		/// <summary>
		/// 页面加载状态（页面未完成加载时不计算IP）
		/// </summary>
		public bool LoadStatus = false;



		/// <summary>
		/// 拖动滑条调整子网掩码
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MaskSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (LoadStatus == true)
			{
				//判断是不是瞎几把写的IP地址
				if (IsValidIp(IpTextBox.Text) == true)
				{
					UpdateIPCalculations();
				}
				else
				{
					IpTextBox.Text = "";
					MessageBox.Show("IP地址不合法，请检查IP地址是否正确!", "确定", MessageBoxButton.OK, MessageBoxImage.Information);
				}

				
			}


		}



		/// <summary>
		/// 计算子网掩码位数
		/// </summary>
		/// <param name="num"></param>
		private void CalculateNetMaskLocated()
		{
			string ipStr = MaskText.Text;
			int index = ipStr.LastIndexOf(".") + 1;

			string strTemp = ipStr.Substring(index, ipStr.Length - index);

			if (strTemp == "0")
			{
				strTemp = "256";
			}

			string str = Convert.ToString(Convert.ToInt32(strTemp), 2);



			index = str.IndexOf("0");


			string str2 = str.Substring(index, str.Length - index);

			// MessageBox.Show(str2);



			MaskSlider.Value = 32 - str2.Length;
		}


		/// <summary>
		/// IP地址计算
		/// </summary>
		private void UpdateIPCalculations()
		{
			try
			{
				IPAddress ip;
				if (IPAddress.TryParse(IpTextBox.Text, out ip))
				{
					int maskLength = (int)MaskSlider.Value;
					IPAddress mask = IPAddressCalculations.SubnetMaskFromPrefixLength(maskLength);
					MaskText.Text = mask.ToString();

					IPAddress networkAddress = ip.GetNetworkAddress(mask);
					Network.Text = networkAddress.ToString();

					IPAddress firstAddress = networkAddress.GetFirstUsable(ip.AddressFamily);
					First.Text = firstAddress.ToString();

					IPAddress lastAddress = networkAddress.GetLastUsable(ip.AddressFamily, maskLength);

					Last.Text = lastAddress.ToString();

					IPAddress broadcastAddress = networkAddress.GetBroadcastAddress(maskLength);
					Broadcast.Text = broadcastAddress.ToString();


					long addressCount = IPAddressCalculations.AddressCount(maskLength);
					NumBox.Text = addressCount.ToString();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


	}

}



	

