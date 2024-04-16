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
using MaterialDesignThemes.Wpf;
using System.Net;
using System.Web.UI.WebControls;
using static IPAM_NOTE.ViewMode;

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
            ModelTextBlock.Text = "型号:" + DataBrige.SelectDeviceInfo.Model;
            DescriptionBlock.Text = "备注:" + DataBrige.SelectDeviceInfo.Description;

            if (DataBrige.portList.Count == 0)//单个分配
            {
                
                string type = DataBrige.SelectDevicePortInfo.PortType;


                //Console.WriteLine("STATUS:"+DataBrige.SelectDevicePortInfo.PortStatus);


				if (DataBrige.SelectDevicePortInfo.PortStatus == "0" || DataBrige.SelectDevicePortInfo.PortStatus == "")
				{
					EnableBox.IsChecked = false;
				}
				else
				{
					if (DataBrige.SelectDevicePortInfo.PortStatus == "1" || DataBrige.SelectDevicePortInfo.PortStatus == "已分配")
					{
						EnableBox.IsChecked = true;
					}
					
					
				}





				switch (type)
                {
                    case "E" :
                        TypeBlock.Text = "当前选择RJ45网口：" + DataBrige.SelectDeviceInfo.EportTag + DataBrige.SelectDevicePortInfo.PortNumber;
                        Tag1Panel.ClearValue(HintAssist.HintProperty);
                        HintAssist.SetHint(Tag1Panel, "VLANID");
                        break;

                    case "F":
                        TypeBlock.Text = "当前选择光纤网口：" + DataBrige.SelectDeviceInfo.FportTag + DataBrige.SelectDevicePortInfo.PortNumber;

                        Tag1Panel.ClearValue(HintAssist.HintProperty);
                        HintAssist.SetHint(Tag1Panel, "VLANID");
                        break;
                    case "D":
                        TypeBlock.Text = "当前选择硬盘插槽：" + DataBrige.SelectDeviceInfo.DportTag + DataBrige.SelectDevicePortInfo.PortNumber;
                        Tag1Panel.ClearValue(HintAssist.HintProperty);
                        HintAssist.SetHint(Tag1Panel, "硬盘信息:");

                        break;

                    case "M":
                        TypeBlock.Text = "当前选择管理接口：" + DataBrige.SelectDeviceInfo.MportTag + DataBrige.SelectDevicePortInfo.PortNumber;
                        Tag1Panel.ClearValue(HintAssist.HintProperty);
                        HintAssist.SetHint(Tag1Panel, "管理地址:");
                        break;
                    case "RJ45网口":
	                    TypeBlock.Text = "当前选择RJ45网口："  + DataBrige.SelectDevicePortInfo.PortNumber;
	                    Tag1Panel.ClearValue(HintAssist.HintProperty);
	                    HintAssist.SetHint(Tag1Panel, "VLANID");
	                    break;

                    case "光纤网口":
	                    TypeBlock.Text = "当前选择光纤网口："  + DataBrige.SelectDevicePortInfo.PortNumber;

	                    Tag1Panel.ClearValue(HintAssist.HintProperty);
	                    HintAssist.SetHint(Tag1Panel, "VLANID");
	                    break;
                    case "硬盘插槽":
	                    TypeBlock.Text = "当前选择硬盘插槽："  + DataBrige.SelectDevicePortInfo.PortNumber;
	                    Tag1Panel.ClearValue(HintAssist.HintProperty);
	                    HintAssist.SetHint(Tag1Panel, "硬盘信息:");

	                    break;

                    case "管理接口":
	                    TypeBlock.Text = "当前选择管理接口："  + DataBrige.SelectDevicePortInfo.PortNumber;
	                    Tag1Panel.ClearValue(HintAssist.HintProperty);
	                    HintAssist.SetHint(Tag1Panel, "管理地址:");
	                    break;
				}

                Tag1TextBox.Text = DataBrige.SelectDevicePortInfo.PortTag1;
                Tag2TextBox.Text = DataBrige.SelectDevicePortInfo.PortTag2;
                Tag3TextBox.Text = DataBrige.SelectDevicePortInfo.PortTag3;
                DescriptionTextBox.Text = DataBrige.SelectDevicePortInfo.Description;
            }
            else//多个分配
            {
	           // string type = DataBrige.SelectDevicePortInfo.PortType;

	            if (DataBrige.SelectDevicePortStatus == "0")//未分配状态
	            {
		            EnableBox.IsChecked = false;
	            }
	            else
	            {
		            EnableBox.IsChecked = true;//已分配状态
		            Tag1TextBox.IsEnabled = false;
                    Tag2TextBox.IsEnabled = false;
                    Tag3TextBox.IsEnabled = false;
                    DescriptionTextBox.IsEnabled= false;

	            }

				string ports = "";

                for (int i = 0; i < DataBrige.portList.Count; i++)
                {
                    if (i < 4)
                    {
                        ports += (DataBrige.portList[i].ToString() + ",");
                    }
                    else
                    {

                        ports = ports.Remove(ports.Length - 1);

                        ports += "等" + DataBrige.portList.Count.ToString() + "个";

                        break;
                    }
                }

                if (ports[ports.Length - 1] == ',')
                {
                    // 删除最后一个字符
                    ports = ports.Remove(ports.Length - 1);
                }


                switch (DataBrige.SelectDevicePortType)
                {
                    case "E":
                        TypeBlock.Text = "当前选择RJ45网口：" + DataBrige.SelectDeviceInfo.EportTag + ports + "端口";
                        Tag1Panel.ClearValue(HintAssist.HintProperty);
                        HintAssist.SetHint(Tag1Panel, "对端设备:");
                        break;

                    case "F":
                        TypeBlock.Text = "当前选择光纤网口：" + DataBrige.SelectDeviceInfo.FportTag + ports + "端口";

                        Tag1Panel.ClearValue(HintAssist.HintProperty);
                        HintAssist.SetHint(Tag1Panel, "对端设备:");
                        break;
                    case "D":
                        TypeBlock.Text = "当前选择硬盘插槽：" + DataBrige.SelectDeviceInfo.DportTag + ports + "槽位";
                        Tag1Panel.ClearValue(HintAssist.HintProperty);
                        HintAssist.SetHint(Tag1Panel, "硬盘信息:");

                        break;

                    case "M":
                        TypeBlock.Text = "当前选择管理接口：" + DataBrige.SelectDeviceInfo.MportTag + ports + "端口";
                        Tag1Panel.ClearValue(HintAssist.HintProperty);
                        HintAssist.SetHint(Tag1Panel, "管理地址:");
                        break;
                }



            }



        }

        private DbClass dbClass;

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {

	        int enable;

			if (EnableBox.IsChecked == true)
			{
				enable = 1;
			}
			else
			{
				enable = 0;
			}


			string tableName = DataBrige.SelectDeviceInfo.TableName;


            if (DataBrige.portList.Count == 0)//单个分配
            {
	           
                string type = DataBrige.SelectDevicePortInfo.PortType;
                string typetemp = type;


                string port = DataBrige.SelectDevicePortInfo.PortNumber;
                string porttemp = port;


                Console.WriteLine(port);

                if (DataBrige.GraphicsMode == 1)
                {
	                string portTag="";
	                Console.WriteLine(type);

	                switch (type)
	                {
                        case "RJ45网口":
	                        portTag = DataBrige.SelectDeviceInfo.EportTag;
	                        type = "E";
                            break;

                        case "光纤网口":
	                        portTag = DataBrige.SelectDeviceInfo.FportTag;
	                        type = "F";
							break;
                            
                        case "硬盘插槽":
	                        portTag = DataBrige.SelectDeviceInfo.DportTag;
	                        type = "D";
							break;

                       case "管理网口":
	                        portTag = DataBrige.SelectDeviceInfo.MportTag;
	                        type = "M";
							break;

                        case "访问标签":
                            portTag = DataBrige.SelectDeviceInfo.MportTag;
                            type = "I";
                            break;
                    }

	                port = port.Replace(portTag, "");
                }




                string sql = string.Format(
                    $"UPDATE {tableName} SET \"PortStatus\" = '{enable}', \"PortTag1\" = '{Tag1TextBox.Text}'  , \"PortTag2\" = '{Tag2TextBox.Text}' , \"PortTag3\" = '{Tag3TextBox.Text}', \"Description\" = '{DescriptionTextBox.Text}' WHERE (PortType = '{type}' AND PortNumber = '{port}')");

                dbClass.ExecuteQuery(sql); //写入端口信息


                int index = 0;
                foreach (var item in DataBrige.DevicePortInfos)
                {
                    if (item.PortType == typetemp && item.PortNumber == porttemp)
                    {
                        DataBrige.DevicePortInfos[index].PortStatus = enable.ToString();
                        DataBrige.DevicePortInfos[index].PortTag1 = Tag1TextBox.Text;
                        DataBrige.DevicePortInfos[index].PortTag2 = Tag2TextBox.Text;
                        DataBrige.DevicePortInfos[index].PortTag3 = Tag3TextBox.Text;
                        DataBrige.DevicePortInfos[index].Description = DescriptionTextBox.Text;


                        break;
                    }

                    index++;
                }

            }
            else//多个分配
            {
                string type = DataBrige.SelectDevicePortType;

                if (DataBrige.portList.Count == 1)//多选模式只选了一个
                {

                    string port = DataBrige.portList[0];

                    string sql = string.Format(
                        $"UPDATE {tableName} SET \"PortStatus\" = '{enable}', \"PortTag1\" = '{Tag1TextBox.Text}'  , \"PortTag2\" = '{Tag2TextBox.Text}' , \"PortTag3\" = '{Tag3TextBox.Text}', \"Description\" = '{DescriptionTextBox.Text}' WHERE (PortType = '{type}' AND PortNumber = '{port}')");

                    dbClass.ExecuteQuery(sql); //写入端口信息


                    int index = 0;
                    foreach (var item in DataBrige.DevicePortInfos)
                    {
                        if (item.PortType == type && item.PortNumber == port)
                        {
                            DataBrige.DevicePortInfos[index].PortStatus = enable.ToString();
                            DataBrige.DevicePortInfos[index].PortTag1 = Tag1TextBox.Text;
                            DataBrige.DevicePortInfos[index].PortTag2 = Tag2TextBox.Text;
                            DataBrige.DevicePortInfos[index].PortTag3 = Tag3TextBox.Text;
                            DataBrige.DevicePortInfos[index].Description = DescriptionTextBox.Text;


                            break;
                        }

                        index++;
                    }

                }
                else//正常多选
                {
                    string port;

					if (DataBrige.SelectDevicePortStatus == "1")//多选状态下选择了已启用的端口准备释放
					{

						for (int i = 0; i < DataBrige.portList.Count; i++)
						{
							port = DataBrige.portList[i];

							string sql = string.Format(
								$"UPDATE {tableName} SET \"PortStatus\" = '{enable}' WHERE (PortType = '{type}' AND PortNumber = '{port}')");

							dbClass.ExecuteQuery(sql); //写入端口信息


							int index = 0;


							foreach (var item in DataBrige.DevicePortInfos)
							{
								if (item.PortType == type && item.PortNumber == port)
								{
									DataBrige.DevicePortInfos[index].PortStatus = enable.ToString();



									break;
								}

								index++;
							}

						}


					}
					else
                    {

						for (int i = 0; i < DataBrige.portList.Count; i++)
						{
							port = DataBrige.portList[i];

							string sql = string.Format(
								$"UPDATE {tableName} SET \"PortStatus\" = '{enable}', \"PortTag1\" = '{Tag1TextBox.Text}'  , \"PortTag2\" = '{Tag2TextBox.Text}' , \"PortTag3\" = '{Tag3TextBox.Text}', \"Description\" = '{DescriptionTextBox.Text}' WHERE (PortType = '{type}' AND PortNumber = '{port}')");

							dbClass.ExecuteQuery(sql); //写入端口信息


							int index = 0;


							foreach (var item in DataBrige.DevicePortInfos)
							{
								if (item.PortType == type && item.PortNumber == port)
								{
									DataBrige.DevicePortInfos[index].PortStatus = enable.ToString();
									DataBrige.DevicePortInfos[index].PortTag1 = Tag1TextBox.Text;
									DataBrige.DevicePortInfos[index].PortTag2 = Tag2TextBox.Text;
									DataBrige.DevicePortInfos[index].PortTag3 = Tag3TextBox.Text;
									DataBrige.DevicePortInfos[index].Description = DescriptionTextBox.Text;


									break;
								}

								index++;
							}

						}


					}


				}

            }





            this.Close();
        }

        private void PortAllocation_OnClosing(object sender, CancelEventArgs e)
        {
            this.DialogResult = true;
        }


        private void ReleaseButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("你正在删除该端口的分配信息，此操作不可逆！继续？", "注意", MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                string tableName = DataBrige.SelectDeviceInfo.TableName;


                if (DataBrige.portList.Count == 0) //单个释放
                {
                    string type = DataBrige.SelectDevicePortInfo.PortType;
                    string port = DataBrige.SelectDevicePortInfo.PortNumber;

                    string sql = string.Format(
                        $"UPDATE {tableName} SET \"PortStatus\" = '0', \"PortTag1\" = ''  , \"PortTag2\" = '' , \"PortTag3\" = '', \"Description\" = '' WHERE (PortType = '{type}' AND PortNumber = '{port}')");

                    dbClass.ExecuteQuery(sql); //写入端口信息


                    int index = 0;
                    foreach (var item in DataBrige.DevicePortInfos)
                    {
                        if (item.PortType == type && item.PortNumber == port)
                        {
                            DataBrige.DevicePortInfos[index].PortStatus = "0";
                            DataBrige.DevicePortInfos[index].PortTag1 = "";
                            DataBrige.DevicePortInfos[index].PortTag2 = "";
                            DataBrige.DevicePortInfos[index].PortTag3 = "";
                            DataBrige.DevicePortInfos[index].Description = "";


                            break;
                        }

                        index++;
                    }

                }
                else//多个释放
                {
                    string type = DataBrige.SelectDevicePortType;

                    if (DataBrige.portList.Count == 1) //多选模式只选了一个
                    {
                        string port = DataBrige.portList[0];

                        string sql = string.Format(
                            $"UPDATE {tableName} SET \"PortStatus\" = '0', \"PortTag1\" = ''  , \"PortTag2\" = '' , \"PortTag3\" = '', \"Description\" = '' WHERE (PortType = '{type}' AND PortNumber = '{port}')");

                        dbClass.ExecuteQuery(sql); //写入端口信息


                        int index = 0;
                        foreach (var item in DataBrige.DevicePortInfos)
                        {
                            if (item.PortType == type && item.PortNumber == port)
                            {
                                DataBrige.DevicePortInfos[index].PortStatus = "0";
                                DataBrige.DevicePortInfos[index].PortTag1 = "";
                                DataBrige.DevicePortInfos[index].PortTag2 = "";
                                DataBrige.DevicePortInfos[index].PortTag3 = "";
                                DataBrige.DevicePortInfos[index].Description = "";


                                break;
                            }

                            index++;
                        }


                    }
                    else //正常多选
                    {
                        string port;


                        for (int i = 0; i < DataBrige.portList.Count; i++)
                        {
                            port = DataBrige.portList[i];


                            string sql = string.Format(
                                $"UPDATE {tableName} SET \"PortStatus\" = '0', \"PortTag1\" = ''  , \"PortTag2\" = '' , \"PortTag3\" = '', \"Description\" = '' WHERE (PortType = '{type}' AND PortNumber = '{port}')");

                            dbClass.ExecuteQuery(sql); //写入端口信息


                            int index = 0;
                            foreach (var item in DataBrige.DevicePortInfos)
                            {
                                if (item.PortType == type && item.PortNumber == port)
                                {
                                    DataBrige.DevicePortInfos[index].PortStatus = "0";
                                    DataBrige.DevicePortInfos[index].PortTag1 = "";
                                    DataBrige.DevicePortInfos[index].PortTag2 = "";
                                    DataBrige.DevicePortInfos[index].PortTag3 = "";
                                    DataBrige.DevicePortInfos[index].Description = "";


                                    break;
                                }

                                index++;
                            }



                        }

                    }

                }





                this.Close();

            }



        }
    }
}
