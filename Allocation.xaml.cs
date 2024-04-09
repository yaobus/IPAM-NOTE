using IPAM_NOTE.DatabaseOperation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ControlzEx.Standard;
using static IPAM_NOTE.ViewMode;
using Button = System.Web.UI.WebControls.Button;
using System.Runtime.InteropServices;
using MaterialDesignThemes.Wpf;

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

		private string PingAddress;


		private void Allocation_OnLoaded(object sender, RoutedEventArgs e)
		{

			if (DataBrige.SelectMode == 0 || DataBrige.SelectedIpAddress.Count == 1)//单个分配
			{
				if (DataBrige.SearchType == 0)
				{
					NetworkBlock.Text = "所在网段:" + DataBrige.SelectSearchInfo.Network;

					//如果网段IP中有*则替换为0
					string tempNetwork = DataBrige.SelectSearchInfo.Network;

					if (tempNetwork.IndexOf("*") != -1)
					{
						tempNetwork = tempNetwork.Replace("*", "0");

					}

					PingAddress = GetFirstThreeSegments(tempNetwork) + DataBrige.SelectIp;

					AddressBlock.Text = "所选地址:" + PingAddress;


					if (DataBrige.SelectSearchInfo.User == "")
					{
						UserTextBox.Text = "";

						DescriptionTextBox.Text = "";

						HostNameTextBox.Text = "";

						MacTextBox.Text = "";

					}
					else
					{
						UserTextBox.Text = DataBrige.SelectSearchInfo.User;

						DescriptionTextBox.Text = DataBrige.SelectSearchInfo.Description;

						HostNameTextBox.Text = DataBrige.SelectSearchInfo.HostName;

						MacTextBox.Text = DataBrige.SelectSearchInfo.MacAddress;
					}


				}
				else
				{
					//1为指定网段搜索加载，0为指定网段全部加载
					if (DataBrige.LoadType == 1)

					{

						NetworkBlock.Text = "所在网段:" + DataBrige.ComBoxAddressInfos[DataBrige.SelectNetwork].Network;

						//如果网段IP中有*则替换为0
						string tempNetwork = DataBrige.ComBoxAddressInfos[DataBrige.SelectNetwork].Network;

						if (tempNetwork.IndexOf("*") != -1)
						{
							tempNetwork = tempNetwork.Replace("*", "0");
						}

						Console.WriteLine("所在网段:" + GetFirstThreeSegments(tempNetwork));


						PingAddress = GetFirstThreeSegments(tempNetwork) + DataBrige.SelectIp;

						AddressBlock.Text = "所选地址:" + PingAddress;



						if (DataBrige.ipAddressInfos[DataBrige.SelectIndex].User == "")
						{

							UserTextBox.Text = "";

							DescriptionTextBox.Text = "";

							HostNameTextBox.Text = "";

							MacTextBox.Text = "";

						}
						else
						{
							UserTextBox.Text = DataBrige.ipAddressInfos[DataBrige.SelectIndex].User;

							DescriptionTextBox.Text = DataBrige.ipAddressInfos[DataBrige.SelectIndex].Description;

							HostNameTextBox.Text = DataBrige.ipAddressInfos[DataBrige.SelectIndex].HostName;

							MacTextBox.Text = DataBrige.ipAddressInfos[DataBrige.SelectIndex].MacAddress;
						}

					}
					else//全部加载
					{
						NetworkBlock.Text = "所在网段:" + DataBrige.TempAddress.Network;

                       

                        //如果网段IP中有*则替换为0
                        string tempNetwork = DataBrige.TempAddress.Network;

						if (tempNetwork.IndexOf("*") != -1)
						{
							tempNetwork = tempNetwork.Replace("*", "0");

						}

						PingAddress = GetFirstThreeSegments(tempNetwork) + DataBrige.SelectIp;

						AddressBlock.Text = "所选地址:" + PingAddress;




						if (DataBrige.ipAddressInfos[DataBrige.SelectIndex].User == "")
						{


							UserTextBox.Text = "";

							DescriptionTextBox.Text = "";

							HostNameTextBox.Text = "";

							MacTextBox.Text = "";


						}
						else
						{
							UserTextBox.Text = DataBrige.ipAddressInfos[DataBrige.SelectIndex].User;

							DescriptionTextBox.Text = DataBrige.ipAddressInfos[DataBrige.SelectIndex].Description;

							HostNameTextBox.Text = DataBrige.ipAddressInfos[DataBrige.SelectIndex].HostName;

							MacTextBox.Text = DataBrige.ipAddressInfos[DataBrige.SelectIndex].MacAddress;
						}
					}
				}


				PingHostNameBox.Text = DataBrige.ipAddressInfos[DataBrige.SelectIndex].OnlineHostName;
				
				PingMacBox.Text = DataBrige.ipAddressInfos[DataBrige.SelectIndex].OnlineMacAddress;


			}
			else//批量分配
			{
				//禁用无关按钮
				StatusPing.IsEnabled = false;
				StatusPing.IsEnabled = false;
				PingButton.IsEnabled = false;
				WebOpen.IsEnabled = false;
				CopyHostInfo.IsEnabled = false;
				HostNameTextBox.IsEnabled = false;
				MacTextBox.IsEnabled = false;

				if (DataBrige.SelectMode == 1)//批量分配
				{
					ReleaseButton.IsEnabled = false;
				}
				else//批量释放
				{
					UserTextBox.IsEnabled=false;
					DescriptionTextBox.IsEnabled=false;
					SaveButton.IsEnabled = false;
				}

				//显示网段信息

				NetworkBlock.Text = "所在网段:" + DataBrige.TempAddress.Network;

				string address = null;

				for (int a = 0; a < DataBrige.SelectedIpAddress.Count; a++)
				{

					if (a < 6)
					{
						address += (DataBrige.SelectedIpAddress[a].ToString() + ",");
					}
					else
					{

						address = address.Remove(address.Length - 1);

						address += "等" + DataBrige.SelectedIpAddress.Count.ToString() + "个地址";
						break;
					}

				}

				if (address[address.Length - 1] == ',')
				{
					// 删除最后一个字符
					address = address.Remove(address.Length - 1);
				}


				AddressBlock.Text = "所选地址:" + address;



			}




			string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";
			string dbName = "Address_database.db";

			dbFilePath = dbFilePath + dbName;

			dbClass = new DbClass(dbFilePath);

			dbClass.OpenConnection();


			//获取到的主机名和MAC与备案的不一样则显示红色
			if (HostNameTextBox.Text != PingHostNameBox.Text && PingHostNameBox.Text != "N/A")
			{
				PingHostNameBox.Foreground = Brushes.DarkRed;
			}


			if (MacTextBox.Text != PingMacBox.Text && PingMacBox.Text != "N/A")
			{
				PingMacBox.Foreground = Brushes.DarkRed;
			}

		}



		/// <summary>
		/// 取出IP地址前三部分
		/// </summary>
		/// <param name="ipAddressString"></param>
		/// <returns></returns>
		static string GetFirstThreeSegments(string ipAddressString)
		{
			if (IPAddress.TryParse(ipAddressString, out IPAddress ipAddress))
			{
				string[] segments = ipAddress.ToString().Split('.');
				if (segments.Length >= 3)
				{
					return $"{segments[0]}.{segments[1]}.{segments[2]}.";
				}
				else
				{
					return "不正确的IP地址";
				}
			}
			else
			{
				return "不正确的IP格式";
			}
		}



		/// <summary>
		/// 保存分配
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveButton_OnClick(object sender, RoutedEventArgs e)
		{


			if (UserTextBox.Text != "" && UserTextBox.Text != "N/A" && DescriptionTextBox.Text != "" && DescriptionTextBox.Text != "N/A")
			{
				string sql;


				if (DataBrige.SelectMode != 0)//多选分配
				{
					if (DataBrige.SelectedIpAddress.Count == 1)//如果多选模式下只选了1个
					{
						string tableName = DataBrige.TempAddress.TableName;

						int ip = DataBrige.SelectedIpAddress[0];

						sql = string.Format("UPDATE {0} SET \"User\" = '{1}', \"AddressStatus\" = '1'  , \"Description\" = '{2}' , \"HostName\" = '{3}', \"MacAddress\" = '{4}' WHERE Address = {5}", tableName, UserTextBox.Text, DescriptionTextBox.Text, HostNameTextBox.Text, MacTextBox.Text, ip);


						int index = 0;

						foreach (var item in DataBrige.ipAddressInfos)
						{
							if (item.Address == ip)
							{
								DataBrige.ipAddressInfos[index].User = UserTextBox.Text;
								DataBrige.ipAddressInfos[index].Description = DescriptionTextBox.Text;
								DataBrige.ipAddressInfos[index].AddressStatus = 2;
								DataBrige.searchInfos[index].HostName = HostNameTextBox.Text;
								DataBrige.searchInfos[index].MacAddress = MacTextBox.Text;

								break;
							}

							index++;
						}



						dbClass.ExecuteQuery(sql);//单选分配时执行
					}
					else//正常多选
					{
						string tableName = DataBrige.TempAddress.TableName;

						foreach (var ip in DataBrige.SelectedIpAddress)
						{
							sql = string.Format("UPDATE {0} SET \"User\" = '{1}', \"AddressStatus\" = '2'  , \"Description\" = '{2}' , \"HostName\" = '{3}', \"MacAddress\" = '{4}' WHERE Address = {5}", tableName, UserTextBox.Text, DescriptionTextBox.Text, HostNameTextBox.Text, MacTextBox.Text, ip);

							int index = 0;
							foreach (var item in DataBrige.ipAddressInfos)
							{
								if (item.Address == ip)
								{
									DataBrige.ipAddressInfos[index].User = UserTextBox.Text;
									DataBrige.ipAddressInfos[index].Description = DescriptionTextBox.Text;
									DataBrige.ipAddressInfos[index].AddressStatus = 2;

									break;
								}

								index++;
							}




							dbClass.ExecuteQuery(sql);//多选分配时执行
						}




					}


				}
				else//单选分配
				{
					if (DataBrige.SearchType == 0)
					{
						string tableName = DataBrige.SelectSearchInfo.TableName;

						sql = string.Format("UPDATE {0} SET \"User\" = '{1}', \"AddressStatus\" = '2'  , \"Description\" = '{2}' , \"HostName\" = '{3}', \"MacAddress\" = '{4}' WHERE Address = {5}", tableName, UserTextBox.Text, DescriptionTextBox.Text, HostNameTextBox.Text, MacTextBox.Text, DataBrige.SelectIp);



						DataBrige.searchInfos[DataBrige.SelectIndex].User = UserTextBox.Text;
						DataBrige.searchInfos[DataBrige.SelectIndex].Description = DescriptionTextBox.Text;
						DataBrige.searchInfos[DataBrige.SelectIndex].AddressStatus = 2;
						DataBrige.searchInfos[DataBrige.SelectIndex].HostName = HostNameTextBox.Text;
						DataBrige.searchInfos[DataBrige.SelectIndex].MacAddress = MacTextBox.Text;


					}
					else
					{
						string tableName = DataBrige.TempAddress.TableName;

						sql = string.Format("UPDATE {0} SET \"User\" = '{1}', \"AddressStatus\" = '2'  , \"Description\" = '{2}' , \"HostName\" = '{3}', \"MacAddress\" = '{4}' WHERE Address = {5}", tableName, UserTextBox.Text, DescriptionTextBox.Text, HostNameTextBox.Text, MacTextBox.Text, DataBrige.SelectIp);

						if (DataBrige.LoadType == 0)
						{
							DataBrige.ipAddressInfos[DataBrige.SelectIndex].User = UserTextBox.Text;
							DataBrige.ipAddressInfos[DataBrige.SelectIndex].Description = DescriptionTextBox.Text;
							DataBrige.ipAddressInfos[DataBrige.SelectIndex].AddressStatus = 2;
							DataBrige.ipAddressInfos[DataBrige.SelectIndex].HostName = HostNameTextBox.Text;
							DataBrige.ipAddressInfos[DataBrige.SelectIndex].MacAddress = MacTextBox.Text;
						}
						else
						{
							DataBrige.ipAddressInfos[DataBrige.SelectIndex].User = UserTextBox.Text;
							DataBrige.ipAddressInfos[DataBrige.SelectIndex].Description = DescriptionTextBox.Text;
							DataBrige.ipAddressInfos[DataBrige.SelectIndex].AddressStatus = 2;
							DataBrige.ipAddressInfos[DataBrige.SelectIndex].HostName = HostNameTextBox.Text;
							DataBrige.ipAddressInfos[DataBrige.SelectIndex].MacAddress = MacTextBox.Text;
						}


					}

					dbClass.ExecuteQuery(sql);//单选分配时执行

				}








				//Console.WriteLine(sql);




				this.DialogResult = true;
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
			MessageBoxResult result = MessageBox.Show("你正在删除该IP地址的分配信息，此操作不可逆！继续？", "注意", MessageBoxButton.YesNo, MessageBoxImage.Information);

			if (result == MessageBoxResult.Yes)
			{
				string tableName;


				if (DataBrige.SelectMode != 0) //多选释放
				{
					if (DataBrige.SelectedIpAddress.Count == 1) //如果多选模式下只选了1个
					{
						tableName = DataBrige.TempAddress.TableName;

						int ip = DataBrige.SelectedIpAddress[0];

						string sql =
							string.Format(
								"UPDATE {0} SET \"User\" = '', \"AddressStatus\" = 1 , \"Description\" = '' , \"HostName\" = '' , \"MacAddress\" = '' WHERE Address = {1}",
								tableName, ip);

						int index = 0;

						foreach (var item in DataBrige.ipAddressInfos)
						{
							if (item.Address == ip)
							{
								DataBrige.ipAddressInfos[index].User = "";
								DataBrige.ipAddressInfos[index].Description = "";
								DataBrige.ipAddressInfos[index].AddressStatus = 1;
								DataBrige.searchInfos[index].HostName = "";
								DataBrige.searchInfos[index].MacAddress = "";

								break;
							}

							index++;
						}

						dbClass.ExecuteQuery(sql); //单选分配时执行


					}
					else //正常多选
					{
						 tableName = DataBrige.TempAddress.TableName;

						foreach (var ip in DataBrige.SelectedIpAddress)
						{
							string sql = string.Format("UPDATE {0} SET \"User\" = '', \"AddressStatus\" = 1 , \"Description\" = '' , \"HostName\" = '' , \"MacAddress\" = '' WHERE Address = {1}", tableName, ip);
							
							int index = 0;

							foreach (var item in DataBrige.ipAddressInfos)
							{
								if (item.Address == ip)
								{
									DataBrige.ipAddressInfos[index].User = "";
									DataBrige.ipAddressInfos[index].Description = "";
									DataBrige.ipAddressInfos[index].AddressStatus = 1;

									break;
								}

								index++;
							}




							dbClass.ExecuteQuery(sql);//多选分配时执行
						}









					}
				}

				else
				{
					if (DataBrige.SearchType == 0)
					{
						tableName = DataBrige.SelectSearchInfo.TableName;
						DataBrige.searchInfos[DataBrige.SelectIndex].User = "";
						DataBrige.searchInfos[DataBrige.SelectIndex].Description = "";
						DataBrige.searchInfos[DataBrige.SelectIndex].HostName = "";
						DataBrige.searchInfos[DataBrige.SelectIndex].MacAddress = "";
						DataBrige.searchInfos[DataBrige.SelectIndex].AddressStatus = 1;

					}
					else
					{
						tableName = DataBrige.TempAddress.TableName;
						DataBrige.ipAddressInfos[Convert.ToInt32(DataBrige.SelectIndex)].User = "";
						DataBrige.ipAddressInfos[Convert.ToInt32(DataBrige.SelectIndex)].Description = "";
						DataBrige.ipAddressInfos[Convert.ToInt32(DataBrige.SelectIndex)].HostName = "";
						DataBrige.ipAddressInfos[Convert.ToInt32(DataBrige.SelectIndex)].MacAddress = "";
						DataBrige.ipAddressInfos[Convert.ToInt32(DataBrige.SelectIndex)].AddressStatus = 1;
					}

					string sql = string.Format("UPDATE {0} SET \"User\" = '', \"AddressStatus\" = 1 , \"Description\" = '' , \"HostName\" = '' , \"MacAddress\" = '' WHERE Address = {1}", tableName, DataBrige.SelectIp);

					dbClass.ExecuteQuery(sql);//单选释放
				}









				//Console.WriteLine("DataBrige.ipAddressInfos.Count=" + DataBrige.ipAddressInfos.Count);
				//Console.WriteLine("DataBrige.SelectIp=" + DataBrige.SelectIp);



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
			DataBrige.SelectMode = 0;
			DataBrige.SelectedIpAddress.Clear();
			this.DialogResult = true;
		}



		private void PingButton_OnClick(object sender, RoutedEventArgs e)
		{
			//int lastIndex = DataBrige.TempAddress.Network.LastIndexOf('.');


			string arguments = $"-t {PingAddress}";

			// 创建一个新的ProcessStartInfo对象
			ProcessStartInfo processStartInfo = new ProcessStartInfo
			{
				FileName = "cmd.exe", // 要执行的程序是cmd.exe
				Arguments = $"/k ping {arguments}", // /k参数告诉cmd执行完命令后不会自动退出
				UseShellExecute = false, // 必须设置为false，否则无法重定向标准输入输出
				CreateNoWindow = false // 创建一个新窗口来显示CMD窗口
			};

			// 创建一个新的Process对象
			Process process = new Process
			{
				StartInfo = processStartInfo
			};

			// 启动进程
			process.Start();
		}



		private void WebOpen_OnClick(object sender, RoutedEventArgs e)
		{

			Process.Start("http://" + PingAddress);
		}



		/// <summary>
		/// 提取主机信息
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CopyHostInfo_OnClick(object sender, RoutedEventArgs e)
		{
			if (PingHostNameBox.Text != "N/A" && PingHostNameBox.Text != "")
			{
				HostNameTextBox.Text = PingHostNameBox.Text;
			}

			if (PingMacBox.Text != "N/A" && PingMacBox.Text != "")
			{
				MacTextBox.Text = PingMacBox.Text;
			}
		}


		/// <summary>
		/// 直接PING测试
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void StatusPing_OnClick(object sender, RoutedEventArgs e)
		{

			ButtonProgressAssist.SetIsIndeterminate(StatusPing, true);

			string ipAddress = PingAddress; // 要检测的IP地址
			try
			{
				IPAddress ip = IPAddress.Parse(ipAddress);

				// 获取主机名
				string hostName = await GetHostNameAsync(ip);
				PingHostNameBox.Text = hostName;
				if (HostNameTextBox.Text != hostName && hostName != "N/A")
				{
					PingHostNameBox.Foreground = Brushes.DarkRed;
				}
				else
				{
					PingHostNameBox.Foreground = Brushes.AliceBlue;
				}




				// 获取MAC地址
				string macAddress = await GetMacAddressAsync(ip);
				PingMacBox.Text = macAddress;

				if (MacTextBox.Text != macAddress && macAddress != "N/A")
				{
					PingMacBox.Foreground = Brushes.DarkRed;
				}
				else
				{
					PingHostNameBox.Foreground = Brushes.AliceBlue;
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine("获取主机名失败");
			}

			ButtonProgressAssist.SetIsIndeterminate(StatusPing, false);

			if (HostNameTextBox.Text != PingHostNameBox.Text && PingHostNameBox.Text != "N/A")
			{
				PingHostNameBox.Foreground = Brushes.DarkRed;
			}


			if (MacTextBox.Text != PingMacBox.Text && PingMacBox.Text != "N/A")
			{
				PingMacBox.Foreground = Brushes.DarkRed;
			}


		}






		private Task<string> GetHostNameAsync(IPAddress ipAddress)
		{
			return Task.Run(() =>
			{

				try
				{
					return Dns.GetHostEntry(ipAddress).HostName;
				}
				catch (Exception e)
				{
					return "N/A";
				}

			});
		}

		private Task<string> GetMacAddressAsync(IPAddress ipAddress)
		{
			return Task.Run(() =>
			{
				try
				{
					byte[] macAddr = new byte[6];
					uint macAddrLen = (uint)macAddr.Length;
					StringBuilder strAddr = new StringBuilder();

					// 调用 SendARP 函数获取 MAC 地址
					uint result = SendARP(BitConverter.ToUInt32(ipAddress.GetAddressBytes(), 0), 0, macAddr, ref macAddrLen);
					if (result == 0)
					{
						for (int i = 0; i < macAddr.Length; i++)
						{
							strAddr.AppendFormat("{0:X2}", macAddr[i]);
							if (i != macAddr.Length - 1)
							{
								strAddr.Append(":");
							}
						}
					}
					return strAddr.ToString();
				}
				catch (Exception e)
				{
					return "N/A";
				}



			});
		}


		[DllImport("iphlpapi.dll", SetLastError = true)]
		private static extern uint SendARP(uint destIp, uint srcIp, byte[] pMacAddr, ref uint phyAddrLen);
	}
}
