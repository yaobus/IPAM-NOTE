using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IPAM_NOTE.DatabaseOperation;
using IPAM_NOTE.DevicePage;
using MaterialDesignThemes.Wpf;
using static IPAM_NOTE.ViewMode;
using UserControl = System.Windows.Controls.UserControl;

namespace IPAM_NOTE.UserPages
{
    /// <summary>
    /// DevicesPage.xaml 的交互逻辑
    /// </summary>
    public partial class DevicesPage : UserControl
    {
        public DevicesPage()
        {
            InitializeComponent();
           
        }


        private DbClass dbClass;

        

        private void DevicesPage_OnLoaded(object sender, RoutedEventArgs e)
        {
           

            DevicesView.ItemsSource = DataBrige.DeviceInfos;

            string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";

            string dbName = "Address_database.db";


            dbFilePath = dbFilePath + dbName;



            dbClass = new DbClass(dbFilePath);
            dbClass.OpenConnection();

            dbClass.CreateTableIfNotExists("Devices");//检查表单是否创建

            LoadDevicesInfo(dbClass.connection);
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            DataBrige.DeviceAddStatus = 0;
            
            
            AddDeviceWindow addWindow = new AddDeviceWindow();

            if (addWindow.ShowDialog() == true)
            {
                DevicesView.ItemsSource = null;
                DataBrige.DeviceInfos.Clear();
                
                // 当子窗口关闭后执行这里的代码
                LoadDevicesInfo(dbClass.connection);
               // DevicesView.ItemsSource = DataBrige.DeviceInfos;
            }
        }


        /// <summary>
        /// 加载设备信息列表
        /// </summary>
        /// <param name="connection"></param>
        public void LoadDevicesInfo(SQLiteConnection connection)
        {
            DataBrige.DeviceInfos.Clear();
            Graphics.Children.Clear();
            try
            {
                string query = "SELECT * FROM Devices";
               
                SQLiteCommand command = new SQLiteCommand(query, connection);
                SQLiteDataReader reader = command.ExecuteReader();

                int i = 0;

                DevicesView.ItemsSource = null;

                while (reader.Read())
                {
                    i++;
                    // 读取数据行中的每一列
                    int id = i;
                    string tableName = reader["TableName"].ToString();
                    string name = reader["Name"].ToString();
                    string model = reader["Model"].ToString();
                    string number = reader["Number"].ToString();
                    string people = reader["People"].ToString();
                    string date = reader["Date"].ToString();
                    string description = reader["Description"].ToString();
                    int ePort=Convert.ToInt32( reader["Eport"]);
                    string ePortTag= reader["EportTag"].ToString();
                    int fPort = Convert.ToInt32(reader["Fport"]);
                    string fPortTag = reader["FportTag"].ToString();
                    int mPort = Convert.ToInt32(reader["Mport"]);
                    string mPortTag = reader["MportTag"].ToString();
                    int dPort = Convert.ToInt32(reader["Mport"]);
                    string dPortTag = reader["DportTag"].ToString();

                    DataBrige.DeviceInfos.Add(new DeviceInfo(id, tableName, name, model, number, people, date, description, ePort, ePortTag, fPort, fPortTag, mPort, mPortTag, dPort, dPortTag));

                }

                DevicesView.ItemsSource = DataBrige.DeviceInfos;

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private void DevicesView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DevicesView.SelectedIndex != -1)
            {
                if (DevicesView.SelectedItem is DeviceInfo info)
                {
                    //将所选设备信息存到临时区域
                    DataBrige.SelectDeviceInfo = info;

                    //加载所选设备信息
                    GetDeviceInfo(info.TableName);


                    
                    //启用删除按钮
                    MinusButton.IsEnabled = true;

                    //启用编辑按钮
                    EditButton.IsEnabled = true;

                    //状态栏显示设备信息
                    DeviceName.Text = info.Name;
                    DeviceModel.Text = info.Model;
                    DeviceNumber.Text = info.Number;
                    People.Text = info.People;
                    DeviceDate.Text = info.Date;
                    DeviceDescription.Text = info.Description;


                }


            }
            else
            {
                MinusButton.IsEnabled = false;
                EditButton.IsEnabled = false;
                DeviceName.Text = "";
                DeviceModel.Text = "";
                DeviceNumber.Text = "";
                People.Text = "";
                DeviceDate.Text = "";
                DeviceDescription.Text = "";
            }

            
		}


        /// <summary>
        /// 加载设备端口信息
        /// </summary>
        private void GetDeviceInfo(string tableName)
        {

            string sql = string.Format("SELECT * FROM {0} ORDER BY PortType ASC", tableName);


            DataBrige.DevicePortInfos.Clear();

            SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);

            SQLiteDataReader reader = command.ExecuteReader();

            


            while (reader.Read())
            {
                string portType = reader["PortType"].ToString();
                string portNumber = reader["PortNumber"].ToString();
                int portStatus = Convert.ToInt32(reader["PortStatus"]);
                string portTag1 = reader["PortTag1"].ToString();
                string portTag2 = reader["PortTag2"].ToString();
                string portTag3 = reader["PortTag3"].ToString();
                string description = reader["Description"].ToString();



                DevicePortInfo portInfo = new DevicePortInfo(portType, portNumber, portStatus, portTag1, portTag2, portTag3, description);

                DataBrige.DevicePortInfos.Add(portInfo);
            }



            reader.Dispose();

            if (DataBrige.DevicePortInfos != null)
            {
                WriteDeviceConfig(DataBrige.DevicePortInfos);
                //图形加载
            }


            //列表加载

        }


        /// <summary>
        /// 设备信息配置图形化显示
        /// </summary>
        /// <param name="ipAddressInfos"></param>
        private void WriteDeviceConfig(List<DevicePortInfo> devicePortInfos)
        {

           

            GraphicsPlan.Children.Clear();
            Graphics.Children.Clear();

            var eList = devicePortInfos.Where(data => data.PortType == "E").ToList();//电口
            var fList = devicePortInfos.Where(data => data.PortType == "F").ToList();//光口
            var mList = devicePortInfos.Where(data => data.PortType == "M").ToList();//管理口
            var dList = devicePortInfos.Where(data => data.PortType == "D").ToList();//硬盘位

            string etag = "";
            string ftag = "";
            string mtag = "";
            string dtag = "";


            Console.WriteLine(DataBrige.SelectDeviceInfo.Id);

            try
            {
                if (DataBrige.SelectDeviceInfo.EportTag != null)
                {
                    etag = DataBrige.SelectDeviceInfo.EportTag;
                }

                if (DataBrige.SelectDeviceInfo.FportTag != null)
                {
                    ftag = DataBrige.SelectDeviceInfo.FportTag;
                }

                if (DataBrige.SelectDeviceInfo.DportTag != null)
                {
                    dtag = DataBrige.SelectDeviceInfo.DportTag;
                }

                if (DataBrige.SelectDeviceInfo.MportTag != null)
                {
                    mtag = DataBrige.SelectDeviceInfo.MportTag;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }



            #region 添加电口


            //添加电口
            int x = eList.Count;

            if (x>0)
            {
                WrapPanel subitemPanel = new WrapPanel();

               

                for (int i = 0; i < x; i++)
                {
                    string description = null;
                    Brush colorBrush = null;
                    int status = eList[i].PortStatus;

                    Brush fontBrush = null;

                    switch (status)
                    {
                        case 0:
                            colorBrush = Brushes.YellowGreen;
                            description = "类型：RJ45网口\r未使用端口";

                            break;

                        case 1:
                            colorBrush = Brushes.Coral;
                            description = "类型：RJ45网口\r已使用端口" + "\r" + eList[i].PortTag1 + "\r" + eList[i].PortTag2 + "\r" + eList[i].PortTag3 + "\r" + eList[i].Description;

                            break;


                    }

                    fontBrush = Brushes.AliceBlue;


                    string content = eList[i].PortNumber;

                    if (etag != "")
                    {
                        content = etag + content;
                    }

                    StackPanel panel = CreateButtonContent(content, PackIconKind.Ethernet);



					Button newButton = new Button()
                    {
                        Width = 60,
                        Height = 40,
                        Background = colorBrush,
                        Foreground = fontBrush,
                        FontWeight = FontWeights.ExtraBold,
                        Content = panel,
                        ToolTip = description,
                        Tag = "E" + eList[i].PortNumber,
						Style = (Style)this.FindResource("MaterialDesignFlatSecondaryDarkBgButton"),
                        BorderThickness = new Thickness(0),
                        Margin = new Thickness(5),

                    };

					newButton.Click += NewButton_Click;
                    //显示到图形化区域
                    subitemPanel.Children.Add(newButton);

                }



                Graphics.Children.Add(subitemPanel);

                Separator separator = new Separator();
                separator.Width = 10000; // 设置横线的宽度，根据需要调整
                separator.Opacity = 0.3;
                //separator.Loaded += (sender, e) =>
                //{
                //    // 设置横线的宽度为父容器的宽度
                //    separator.Width = Graphics.ActualWidth;
                //};

                Graphics.Children.Add(separator);


            }

            

            #endregion


            #region 添加光口

            //添加光口
             x = fList.Count;

			if (x > 0)
			{
				WrapPanel fSubitemPanel = new WrapPanel();

              

                for (int i = 0; i < x; i++)
                {
                    string description = null;
                    Brush colorBrush = null;
                    int status = fList[i].PortStatus;

                    Brush fontBrush = null;

                    switch (status)
                    {
                        case 0:
                            colorBrush = Brushes.DarkTurquoise;
                            description = "类型：光纤网口\r未使用端口";

                            break;

                        case 1:
                            colorBrush = Brushes.Coral;
                            description = "类型：光纤网口\r已使用端口" + "\r" + eList[i].PortTag1 + "\r" + eList[i].PortTag2 + "\r" + eList[i].PortTag3 + "\r" + eList[i].Description;

                            break;


                    }

                    fontBrush = Brushes.AliceBlue;


                    string content = fList[i].PortNumber;

                    if (ftag != "")
                    {
                        content = ftag + content;
                    }


                    StackPanel panel = CreateButtonContent(content, PackIconKind.ViewStreamOutline);

                    Button newButton = new Button()
                    {
                        Width = 60,
                        Height = 40,
                        Background = colorBrush,
                        Foreground = fontBrush,
                        FontWeight = FontWeights.ExtraBold,
                        Content = panel,
                        ToolTip = description,
						Tag = "F" + fList[i].PortNumber,
						Style = (Style)this.FindResource("MaterialDesignFlatSecondaryDarkBgButton"),
                        BorderThickness = new Thickness(0),
                        Margin = new Thickness(5),

                    };

                    newButton.Click += NewButton_Click;
					//显示到图形化区域
					fSubitemPanel.Children.Add(newButton);

                }

                Graphics.Children.Add(fSubitemPanel);

                Separator fiberseparator = new Separator();
                fiberseparator.Width = 10000; // 设置横线的宽度，根据需要调整
                fiberseparator.Opacity = 0.3;


				Graphics.Children.Add(fiberseparator);
            }



            #endregion

            #region 添加硬盘位


            x = dList.Count;

            if (x > 0)
            {
                WrapPanel dSubitemPanel = new WrapPanel();



                for (int i = 0; i < x; i++)
                {
                    string description = null;
                    Brush colorBrush = null;
                    int status = dList[i].PortStatus;

                    Brush fontBrush = null;

                    switch (status)
                    {
                        case 0:
                            colorBrush = Brushes.ForestGreen;
                            description = "类型：硬盘插槽\r未使用插槽";

                            break;

                        case 1:
                            colorBrush = Brushes.Coral;
                            description = "类型：硬盘插槽\r已使用插槽" + "\r" + eList[i].PortTag1 + "\r" + eList[i].PortTag2 + "\r" + eList[i].PortTag3 + "\r" + eList[i].Description;

                            break;


                    }

                    fontBrush = Brushes.AliceBlue;


                    string content = dList[i].PortNumber;

                    if (dtag!= "")
                    {
                        content = dtag + content;
                    }


                    StackPanel panel = CreateButtonContent(content, PackIconKind.Hdd);

                    Button newButton = new Button()
                    {
                        Width = 60,
                        Height = 40,
                        Background = colorBrush,
                        Foreground = fontBrush,
                        FontWeight = FontWeights.ExtraBold,
                        Content = panel,
                        ToolTip = description,
						Tag = "D" + dList[i].PortNumber,
						Style = (Style)this.FindResource("MaterialDesignFlatSecondaryDarkBgButton"),
                        BorderThickness = new Thickness(0),
                        Margin = new Thickness(5),

                    };

                    newButton.Click += NewButton_Click;

					//显示到图形化区域
					dSubitemPanel.Children.Add(newButton);

                }

                Graphics.Children.Add(dSubitemPanel);

                Separator diskSeparator = new Separator();
                diskSeparator.Width = 10000; // 设置横线的宽度，根据需要调整
                diskSeparator.Opacity = 0.3;


				Graphics.Children.Add(diskSeparator);
            }



            #endregion

            #region 添加管理口


            x = mList.Count;

            if (x>0)
            {
                WrapPanel mSubitemPanel = new WrapPanel();

               

                for (int i = 0; i < x; i++)
                {
                    string description = null;
                    Brush colorBrush = null;
                    int status = mList[i].PortStatus;

                    Brush fontBrush = null;

                    switch (status)
                    {
                        case 0:
                            colorBrush = Brushes.DarkRed;
                            description = "类型：管理网口\r未使用端口";

                            break;

                        case 1:
                            colorBrush = Brushes.Coral;
                            description = "类型：管理网口\r已使用端口" + "\r" + eList[i].PortTag1 + "\r" + eList[i].PortTag2 + "\r" + eList[i].PortTag3 + "\r" + eList[i].Description;

                            break;


                    }

                    fontBrush = Brushes.AliceBlue;

                    string content = mList[i].PortNumber;

                    if (mtag != "")
                    {
                        content = mtag + content;
                    }



                    StackPanel panel = CreateButtonContent(content, PackIconKind.Ethernet);

                    Button newButton = new Button()
                    {
                        Width = 60,
                        Height = 40,
                        Background = colorBrush,
                        Foreground = fontBrush,
                        FontWeight = FontWeights.ExtraBold,
                        Content = panel,
                        ToolTip = description,
						Tag = "M" + mList[i].PortStatus,
						Style = (Style)this.FindResource("MaterialDesignFlatSecondaryDarkBgButton"),
                        BorderThickness = new Thickness(0),
                        Margin = new Thickness(5),

                    };

					newButton.Click += NewButton_Click;
					//显示到图形化区域
					mSubitemPanel.Children.Add(newButton);

                }

                Graphics.Children.Add(mSubitemPanel);

                Separator mSeparator = new Separator();
                mSeparator.Width = 10000; // 设置横线的宽度，根据需要调整
                mSeparator.Opacity = 0.3;
				//miberseparator.Loaded += (sender, e) =>
				//{
				//    // 设置横线的宽度为父容器的宽度
				//    miberseparator.Width = Graphics.ActualWidth;
				//};

				Graphics.Children.Add(mSeparator);
            }



            #endregion



        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
	        if (sender is Button button)
	        {
		        DataBrige.SelectDeviceButtonTag = button.Tag.ToString();

		        string port = DataBrige.SelectDeviceButtonTag.Substring(1);//端口号


				char firstCharacter = DataBrige.SelectDeviceButtonTag[0];

				// 将第一个字符转换为字符串
				string type = firstCharacter.ToString();//端口类型

				var list = DataBrige.DevicePortInfos.Where(data => data.PortType == type).ToList();

				

				foreach (var info in list)
				{
					if (info.PortNumber == port)
					{

						DataBrige.SelectDevicePortInfo = info;

						break;
					}
				}


				Window allocation = new PortAllocation();

				if (allocation.ShowDialog() == true)
				{
					
					//if (LoadMode == 0)
					//{
						WriteDeviceConfig(DataBrige.DevicePortInfos);
					//}
					//else
					//{
					//	ListLoad();

					//}
				}




			}
        }






        /// <summary>
        /// 创建按钮内容
        /// </summary>
        /// <param name="buttonText"></param>
        /// <param name="iconKind"></param>
        /// <returns></returns>
        private StackPanel CreateButtonContent(string buttonText, PackIconKind iconKind)
			 {
            
                 // 创建包含图标和文本的 StackPanel
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.HorizontalAlignment = HorizontalAlignment.Right;
            stackPanel.VerticalAlignment = VerticalAlignment.Top;
            stackPanel.Margin=new Thickness(-10,-5,-10,-5);
            
            // 创建图标
            PackIcon icon = new PackIcon();
            icon.Kind = iconKind;
            icon.Width = 24;
            icon.Height = 24;
            icon.HorizontalAlignment = HorizontalAlignment.Center;
            icon.VerticalAlignment = VerticalAlignment.Top;
            icon.Margin = new Thickness(0, 0, 0, 0);

            // 创建文本块
            TextBlock textBlock = new TextBlock();
            
            textBlock.Text = buttonText;
            textBlock.FontSize = 12;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
			textBlock.TextAlignment= TextAlignment.Center;
            icon.Margin = new Thickness(0, 0, 0, 0);

			// 将图标和文本块添加到 StackPanel 中
			stackPanel.Children.Add(icon);
            stackPanel.Children.Add(textBlock);

            return stackPanel;
        }



        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinusButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("注意！你正在删除一个设备，是否继续？", "注意", MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                string tableName = DataBrige.SelectDeviceInfo.TableName;
                string sql = string.Format("DELETE FROM Devices WHERE  TableName = '{0}'", tableName);

                dbClass.ExecuteQuery(sql); //写入端口信息

                sql = string.Format("DROP TABLE '{0}'", tableName);

                dbClass.ExecuteQuery(sql);//删除设备详表

                LoadDevicesInfo(dbClass.connection);
            }
        }


        /// <summary>
        /// 编辑设备信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            DataBrige.DeviceAddStatus = 1;

            AddDeviceWindow addWindow = new AddDeviceWindow();

            if (addWindow.ShowDialog() == true)
            {
                // 当子窗口关闭后执行这里的代码
                LoadDevicesInfo(dbClass.connection);
            }

        }
    }
}
