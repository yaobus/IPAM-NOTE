using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IPAM_NOTE.DatabaseOperation;
using IPAM_NOTE.DevicePage;
using IPAM_NOTE.UserWindows;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using OfficeOpenXml;
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

            dbClass.CreateTableIfNotExists("Devices"); //检查表单是否创建

            LoadDevicesInfo(dbClass.connection);


            //CreatDomainList();
            //DomainComboBox.ItemsSource = domainList;
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


        //private List<string> domainList = new List<string>();

        ///// <summary>
        ///// 创建搜索范围表项
        ///// </summary>
        //private void CreatDomainList()
        //{
        //	domainList.Add("用户");
        //	domainList.Add("备注");
        //	domainList.Add("主机名");
        //	domainList.Add("MAC");
        //}



        

        /// <summary>
        /// 加载设备信息列表
        /// </summary>
        /// <param name="connection"></param>
        public void LoadDevicesInfo(SQLiteConnection connection)
        {
            DataBrige.DeviceInfos.Clear();
            GraphicsPlan.Children.Clear(); //
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
                    int ePort = Convert.ToInt32(reader["Eport"]);
                    string ePortTag = reader["EportTag"].ToString();
                    int fPort = Convert.ToInt32(reader["Fport"]);
                    string fPortTag = reader["FportTag"].ToString();
                    int mPort = Convert.ToInt32(reader["Mport"]);
                    string mPortTag = reader["MportTag"].ToString();
                    int dPort = Convert.ToInt32(reader["Mport"]);
                    string dPortTag = reader["DportTag"].ToString();

                    DataBrige.DeviceInfos.Add(new DeviceInfo(id, tableName, name, model, number, people, date,
                        description, ePort, ePortTag, fPort, fPortTag, mPort, mPortTag, dPort, dPortTag));

                    DataBrige.DevicesList.Add($"{id}-{name}-{number}-{model}");
                }

                DevicesView.ItemsSource = DataBrige.DeviceInfos;
                DeviceBox.ItemsSource = DataBrige.DevicesList;
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private void DevicesView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
	        DataBrige.DeviceLoadType = 0;

            GraphicsButton.IsEnabled = true;

            if (DevicesView.SelectedIndex != -1)
            {
                ExportButton.IsEnabled=true;

                if (DevicesView.SelectedItem is DeviceInfo info)
                {
                    //将所选设备信息存到临时区域
                    DataBrige.SelectDeviceInfo = info;

                    DataBrige.SelectDeviceTableName = info.TableName;

                    

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
	            ExportButton.IsEnabled = false;
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



                DevicePortInfo portInfo = new DevicePortInfo(portType, portNumber, portStatus, portTag1, portTag2,
                    portTag3, description);

                DataBrige.DevicePortInfos.Add(portInfo);
            }



            reader.Dispose();

            if (DataBrige.DevicePortInfos != null)
            {

                if (DataBrige.GraphicsMode == 0)
                {
                    MultipleSelect.IsEnabled = true;
                    WriteDeviceConfig(DataBrige.DevicePortInfos);
                    //图形加载
                }
                else
                {
                    ListButton_Click(null, null);
                    MultipleSelect.IsChecked = false;
                    MultipleSelect.IsEnabled = false;
                }



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
            ListPlan.Children.Clear();

            var eList = devicePortInfos.Where(data => data.PortType == "E").ToList(); //电口
            var fList = devicePortInfos.Where(data => data.PortType == "F").ToList(); //光口
            var mList = devicePortInfos.Where(data => data.PortType == "M").ToList(); //管理口
            var dList = devicePortInfos.Where(data => data.PortType == "D").ToList(); //硬盘位

            string etag = "";
            string ftag = "";
            string mtag = "";
            string dtag = "";



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

            if (x > 0)
            {
                WrapPanel subitemPanel = new WrapPanel();



                for (int i = 0; i < x; i++)
                {
                    string description = null;
                    Brush colorBrush = null;
                    string status = eList[i].PortStatus.ToString();

                    Brush fontBrush = null;

                    switch (status)
                    {
                        case "0":
                            colorBrush = Brushes.YellowGreen;
                            description = "类型：RJ45网口\r未使用端口";

                            break;

                        case "1":
                            colorBrush = Brushes.Coral;
                            description = "类型：RJ45网口\r已使用端口" + "\r" + eList[i].PortTag1 + "\r" + eList[i].PortTag2 +
                                          "\r" + eList[i].PortTag3 + "\r" + eList[i].Description;

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



                GraphicsPlan.Children.Add(subitemPanel); //

                Separator separator = new Separator();
                separator.Width = 10000; // 设置横线的宽度，根据需要调整
                separator.Opacity = 0.3;
                //separator.Loaded += (sender, e) =>
                //{
                //    // 设置横线的宽度为父容器的宽度
                //    separator.Width = Graphics.ActualWidth;
                //};

                GraphicsPlan.Children.Add(separator);


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
                    string status = fList[i].PortStatus;

                    Brush fontBrush = null;

                    switch (status)
                    {
                        case "0":
                            colorBrush = Brushes.DarkTurquoise;
                            description = "类型：光纤网口\r未使用端口";

                            break;

                        case "1":
                            colorBrush = Brushes.Coral;
                            description = "类型：光纤网口\r已使用端口" + "\r" + fList[i].PortTag1 + "\r" + fList[i].PortTag2 +
                                          "\r" + fList[i].PortTag3 + "\r" + fList[i].Description;

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

                GraphicsPlan.Children.Add(fSubitemPanel);

                Separator fiberseparator = new Separator();
                fiberseparator.Width = 10000; // 设置横线的宽度，根据需要调整
                fiberseparator.Opacity = 0.3;


                GraphicsPlan.Children.Add(fiberseparator);
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
                    string status = dList[i].PortStatus;

                    Brush fontBrush = null;

                    switch (status)
                    {
                        case "0":
                            colorBrush = Brushes.ForestGreen;
                            description = "类型：硬盘插槽\r未使用插槽";

                            break;

                        case "1":
                            colorBrush = Brushes.Coral;
                            description = "类型：硬盘插槽\r已使用插槽" + "\r" + dList[i].PortTag1 + "\r" + dList[i].PortTag2 +
                                          "\r" + dList[i].PortTag3 + "\r" + dList[i].Description;

                            break;


                    }

                    fontBrush = Brushes.AliceBlue;


                    string content = dList[i].PortNumber;

                    if (dtag != "")
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

                GraphicsPlan.Children.Add(dSubitemPanel);

                Separator diskSeparator = new Separator();
                diskSeparator.Width = 10000; // 设置横线的宽度，根据需要调整
                diskSeparator.Opacity = 0.3;


                GraphicsPlan.Children.Add(diskSeparator);
            }



            #endregion

            #region 添加管理口


            x = mList.Count;

            if (x > 0)
            {
                WrapPanel mSubitemPanel = new WrapPanel();



                for (int i = 0; i < x; i++)
                {
                    string description = null;
                    Brush colorBrush = null;
                    string status = mList[i].PortStatus;

                    Brush fontBrush = null;

                    switch (status)
                    {
                        case "0":
                            colorBrush = Brushes.DarkRed;
                            description = "类型：管理网口\r未使用端口";

                            break;

                        case "1":
                            colorBrush = Brushes.Coral;
                            description = "类型：管理网口\r已使用端口" + "\r" + mList[i].PortTag1 + "\r" + mList[i].PortTag2 +
                                          "\r" + mList[i].PortTag3 + "\r" + mList[i].Description;

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
                        Tag = "M" + mList[i].PortNumber,
                        Style = (Style)this.FindResource("MaterialDesignFlatSecondaryDarkBgButton"),
                        BorderThickness = new Thickness(0),
                        Margin = new Thickness(5),

                    };

                    newButton.Click += NewButton_Click;
                    //显示到图形化区域
                    mSubitemPanel.Children.Add(newButton);

                }

                GraphicsPlan.Children.Add(mSubitemPanel);

                Separator mSeparator = new Separator();
                mSeparator.Width = 10000; // 设置横线的宽度，根据需要调整
                mSeparator.Opacity = 0.3;
                //miberseparator.Loaded += (sender, e) =>
                //{
                //    // 设置横线的宽度为父容器的宽度
                //    miberseparator.Width = Graphics.ActualWidth;
                //};

                GraphicsPlan.Children.Add(mSeparator);
            }



            #endregion



        }




        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                DataBrige.SelectDeviceButtonTag = button.Tag.ToString();

                string port = DataBrige.SelectDeviceButtonTag.Substring(1); //端口号


                char firstCharacter = DataBrige.SelectDeviceButtonTag[0];


                // 将第一个字符转换为字符串
                string type = firstCharacter.ToString(); //端口类型



                if (MultipleSelect.IsChecked == false) //如果是单选
                {


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
                else //多选
                {


                    if (DataBrige.SelectDevicePortMode == 0) //初次选择
                    {

                        //记录首次选择的端口类型
                        DataBrige.SelectDevicePortType = type;
                        DataBrige.SelectDevicePortMode = 1;

                        //记录首次选择的端口是已分配还是未分配
                        if (button.Background == Brushes.Coral)
                        {
                            DataBrige.SelectDevicePortStatus = "1";
                        }
                        else
                        {
                            DataBrige.SelectDevicePortStatus = "0";
                        }

                        button.Background = Brushes.HotPink;
                        DataBrige.portList.Add(port);
                    }
                    else //再次选择
                    {
                        string status = "-1";
                        //判断端口是已分配还是未分配

                        var list = DataBrige.DevicePortInfos.Where(data => data.PortType == type).ToList(); //电口

                        foreach (var item in list)
                        {
                            if (item.PortNumber == port)
                            {
                                status = item.PortStatus;
                                break;
                            }
                        }

                        //如果端口类型和状态一样则下一步验证
                        if (type == DataBrige.SelectDevicePortType && status == DataBrige.SelectDevicePortStatus)
                        {

                            //地址不存在则添加
                            if (!DataBrige.portList.Contains(port))
                            {

                                button.Background = Brushes.HotPink;

                                DataBrige.portList.Add(port);
                            }
                            else //存在则删除
                            {
                                Brush brush = null;
                                if (status == "1")
                                {
                                    brush = Brushes.Coral;
                                }
                                else
                                {
                                    switch (type)
                                    {
                                        case "E":

                                            brush = Brushes.YellowGreen;
                                            break;

                                        case "F":
                                            brush = Brushes.DarkTurquoise;

                                            break;
                                        case "D":

                                            brush = Brushes.ForestGreen;

                                            break;

                                        case "M":

                                            brush = Brushes.DarkRed;
                                            break;
                                    }

                                }


                                button.Background = brush;

                                Icon.Kind = PackIconKind.CogOutline;

                                DataBrige.portList.Remove(port);



                            }


                        }



                    }



                }



                if (DataBrige.portList.Count > 0)
                {
                    MultipleSelectStatus.Visibility = Visibility.Visible;
                    //GraphicsPlan.Height = GraphicsPlan.Height - 30;
                    CountBox.Text = DataBrige.portList.Count.ToString();
                }
                else
                {
                    MultipleSelectStatus.Visibility = Visibility.Collapsed;
                    //GraphicsPlan.Height = GraphicsPlan.Height + 30;
                    DataBrige.SelectDevicePortMode = 0;
                    DataBrige.SelectDevicePortType = "";
                    DataBrige.SelectDevicePortStatus = "-1";

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
            stackPanel.Margin = new Thickness(-10, -5, -10, -5);

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
            textBlock.TextAlignment = TextAlignment.Center;
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

                dbClass.ExecuteQuery(sql); //删除设备详表

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


        /// <summary>
        /// 启用多选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MultipleSelect_Click(object sender, RoutedEventArgs e)
        {

            DataBrige.SelectDevicePortMode = 0;
            DataBrige.SelectDevicePortType = "";
            DataBrige.SelectDevicePortStatus = "-1";
            DataBrige.portList.Clear();

            //if (LoadMode == 0)
            //{
            WriteDeviceConfig(DataBrige.DevicePortInfos);
            //}
            //else
            //{

            //    ListLoad();

            //}

        }

        private void DistributionButton_OnClick(object sender, RoutedEventArgs e)
        {
            Window allocation = new PortAllocation();

            if (allocation.ShowDialog() == true)
            {
                MultipleSelectStatus.Visibility = Visibility.Hidden; //隐藏多选按钮
                DataBrige.SelectDevicePortMode = 0;
                DataBrige.SelectDevicePortType = "";
                DataBrige.SelectDevicePortStatus = "-1";
                DataBrige.portList.Clear();

                //if (LoadMode == 0)
                //{
                WriteDeviceConfig(DataBrige.DevicePortInfos);
                //}
                //else
                //{
                //    ListLoad();

                //}
            }

        }


        /// <summary>
        /// 列表加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListButton_Click(object sender, RoutedEventArgs e)
        {
            MultipleSelect.IsEnabled = false;
            DataBrige.GraphicsMode = 1;
            GraphicsPlan.Children.Clear();

            List<DevicePortInfo> infos = new List<DevicePortInfo>();

            foreach (var item in DataBrige.DevicePortInfos)
            {
                var info = item;
                switch (info.PortType)
                {
                    case "E":
                        info.PortType = "RJ45网口";
                        info.PortNumber = DataBrige.SelectDeviceInfo.EportTag + info.PortNumber;
                        break;

                    case "F":
                        info.PortType = "光纤网口";
                        info.PortNumber = DataBrige.SelectDeviceInfo.FportTag + info.PortNumber;
                        break;
                    case "M":
                        info.PortType = "管理网口";
                        info.PortNumber = DataBrige.SelectDeviceInfo.MportTag + info.PortNumber;
                        break;
                    case "D":
                        info.PortType = "硬盘插槽";
                        info.PortNumber = DataBrige.SelectDeviceInfo.DportTag + info.PortNumber;
                        break;
                    case "I":
                        info.PortType = "访问标签";
                        break;

                }

                switch (info.PortStatus)
                {
                    case "0": //未分配

                        info.PortStatus = "";
                        break;

                    case "1":

                        info.PortStatus = "已分配";

                        break;
                }


                infos.Add(info);

            }


            //清空图表
            ListPlan.Children.Clear();
            //Graphics.Children.Clear();

            // 创建一个 ListView
            ListView listView = new ListView();

            // 设置 ListView 的垂直对齐方式为拉伸
            listView.VerticalAlignment = VerticalAlignment.Stretch;

            // 设置 ListView 的内容垂直对齐方式为顶部对齐
            listView.VerticalContentAlignment = VerticalAlignment.Stretch;

            ScrollViewer.SetCanContentScroll(listView, false);

            // 创建 ScrollViewer 控件
            ScrollViewer scrollViewer = new ScrollViewer();

            // 设置垂直滚动条的可见性为自动
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;


            // 创建一个 GridView
            GridView gridView = new GridView();


            // 添加列到 GridView
            // 添加列到 GridView
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "端口类型",
                DisplayMemberBinding = new Binding("PortType")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "端口编号",
                DisplayMemberBinding = new Binding("PortNumber")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "端口状态",
                DisplayMemberBinding = new Binding("PortStatus")
            });
            gridView.Columns.Add(new GridViewColumn { Header = "端口标记", DisplayMemberBinding = new Binding("PortTag1") });
            gridView.Columns.Add(new GridViewColumn { Header = "端口标记", DisplayMemberBinding = new Binding("PortTag2") });
            gridView.Columns.Add(new GridViewColumn { Header = "端口标记", DisplayMemberBinding = new Binding("PortTag3") });
            gridView.Columns.Add(new GridViewColumn { Header = "端口注释", DisplayMemberBinding = new Binding("Description") });
            // 将 GridView 设置为 ListView 的 View
            listView.View = gridView;



            // 设置 ListView 的属性

            Binding bindingWidth = new Binding("ActualWidth");
            bindingWidth.Source = GraphicsPlan;
            listView.SetBinding(ListView.WidthProperty, bindingWidth);

            // 设置 ListView 的高度为 Auto
            listView.Height = double.NaN;

            listView.Margin = new Thickness(10);


            listView.ItemsSource = infos;
            listView.SelectionChanged += ListView_SelectionChanged;
            listView.MouseDoubleClick += ListView_MouseDoubleClick;




            scrollViewer.Content = listView;
            scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;


            ListPlan.Children.Add(scrollViewer);


        }


        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 获取当前的 ScrollViewer 控件
            ScrollViewer currentScrollViewer = sender as ScrollViewer;

            if (currentScrollViewer != null)
            {
                // 计算滚动的增量
                double delta = e.Delta;

                // 计算新的滚动位置
                double newOffset = currentScrollViewer.VerticalOffset - delta;

                // 限制滚动位置在合理范围内
                if (newOffset < 0)
                {
                    newOffset = 0;
                }
                else if (newOffset > currentScrollViewer.ScrollableHeight)
                {
                    newOffset = currentScrollViewer.ScrollableHeight;
                }

                // 设置新的滚动位置
                currentScrollViewer.ScrollToVerticalOffset(newOffset);
            }

            // 标记事件为已处理，以阻止默认的滚动行为
            e.Handled = true;

        }


        /// <summary>
        /// 表项被选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView)
            {
                ////Console.WriteLine("当前表项INDEX：" + listView.SelectedIndex);
                //if (listView.SelectedIndex != -1)
                //{
                //	IpAddressInfo ipAddressInfo = listView.SelectedItem as IpAddressInfo;
                //	int ip = ipAddressInfo.Address;
                //	Console.WriteLine("当前表项IP：" + ip);


                //	//计算广播IP
                //	int broadcast = 0;
                //	IPAddress ipAddress;
                //	IPAddress mask = IPAddress.Parse(DataBrige.TempAddress.NetMask);

                //	MaskText.Text = DataBrige.TempAddress.NetMask;



                //	//如果网段IP中有*则替换为0
                //	string tempNetwork = DataBrige.TempAddress.Network;

                //	if (tempNetwork.IndexOf("*") != -1)
                //	{
                //		tempNetwork = tempNetwork.Replace("*", "0");

                //	}


                //	Network.Text = tempNetwork;

                //	//计算IP地址
                //	UpdateIPCalculations();

                //	if (IPAddress.TryParse(tempNetwork, out ipAddress))
                //	{
                //		IPAddress networkAddress = ipAddress.GetNetworkAddress(mask);
                //		int maskLength = IPAddressCalculations.CalculateSubnetMaskLength(mask);
                //		IPAddress broadcastAddress = networkAddress.GetBroadcastAddress(maskLength);

                //		string[] parts = broadcastAddress.ToString().Split('.');

                //		//取出广播IP
                //		broadcast = Convert.ToInt32(parts[3]);
                //		Console.WriteLine(broadcast);
                //	}


                //	//取出网段IP
                //	string[] parts2 = tempNetwork.Split('.');


                //	int firstIp = Convert.ToInt32(parts2[3]);



                //	if (ip != firstIp && ip != broadcast)
                //	{
                //		DataBrige.SelectIp = ip.ToString();
                //		DataBrige.SelectIndex = listView.SelectedIndex;
                //		Console.WriteLine("DataBrige.SelectIndex=" + DataBrige.SelectIndex);
                //		DataBrige.IpAddress.HostName = DataBrige.ipAddressInfos[listView.SelectedIndex].HostName;
                //		DataBrige.IpAddress.MacAddress = DataBrige.ipAddressInfos[listView.SelectedIndex].MacAddress;


                //	}
                //}
            }
        }

        /// <summary>
        /// 表项被双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (sender is ListView listView)
            {
                if (listView.SelectedIndex != -1)
                {
                    DevicePortInfo devicePortInfo = listView.SelectedItem as DevicePortInfo;

                    DataBrige.SelectDevicePortInfo = devicePortInfo;


                    Console.WriteLine(devicePortInfo.PortStatus);

                    Window portAllocation = new PortAllocation();


                    if (portAllocation.ShowDialog() == true)
                    {

                        if (DataBrige.GraphicsMode == 0)
                        {

                            WriteDeviceConfig(DataBrige.DevicePortInfos);
                            //图形加载
                        }
                        else
                        {
                            ListButton_Click(null, null);
                        }

                    }
                }




            }



        }



        private void GraphicsButton_Click(object sender, RoutedEventArgs e)
        {
            MultipleSelect.IsEnabled = true;
            DataBrige.GraphicsMode = 0;
            DevicesView_OnSelectionChanged(null, null);


        }



        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            //列表显示
            DataBrige.GraphicsMode = 1;

            //加载方式
            DataBrige.DeviceLoadType = 1;

			GraphicsButton.IsEnabled=false;
            ListButton.IsEnabled=true;
            ListButton.IsChecked = true;


            //清空工作区
            GraphicsPlan.Children.Clear();
            ListPlan.Children.Clear();

            //禁用多选功能
            MultipleSelectStatus.Visibility = Visibility.Hidden;
            MultipleSelect.IsChecked = false;


            //启用清空搜索按钮
            SearchClear.IsEnabled = true;

            //模式为列表显示
            DataBrige.GraphicsMode = 1;

            //启用导出按钮
            ExportButton.IsEnabled = true;



            //索引为-1的时候搜索全部设备，不为-1的时候搜索指定设备
            if (DeviceBox.SelectedIndex != -1)
            {

	            //进行了指定搜索的标记
	            DataBrige.DeviceSearchType = 1;

				DataBrige.SearchDeviceTableName = DataBrige.DeviceInfos[DeviceBox.SelectedIndex].TableName + "_Search_" + KeyWord.Text;

                MultipleSelect.IsEnabled = false;
                

                //--------------------------------------------------

                string sql = ConstructQueryForTable(DataBrige.DeviceInfos[DeviceBox.SelectedIndex].TableName);


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



                    DevicePortInfo portInfo = new DevicePortInfo(portType, portNumber, portStatus, portTag1, portTag2,
                        portTag3, description);

                    DataBrige.DevicePortInfos.Add(portInfo);
                }





                List<DevicePortInfo> infos = new List<DevicePortInfo>();


                string ePortTag = null;
                string fPortTag = null;
                string dPortTag = null;
                string mPortTag = null;
                string tableName = null;

               
                foreach (var item in DataBrige.DevicePortInfos)
                {

                    if (DataBrige.SearchDeviceTableName != tableName)
                    {
                        ePortTag = GetDevicePortTag(DataBrige.SearchDeviceTableName, "EportTag");
                        fPortTag = GetDevicePortTag(DataBrige.SearchDeviceTableName, "FportTag");
                        dPortTag = GetDevicePortTag(DataBrige.SearchDeviceTableName, "DportTag");
                        mPortTag = GetDevicePortTag(DataBrige.SearchDeviceTableName, "MportTag");
                        tableName = DataBrige.SearchDeviceTableName;
                    }

                    var info = item;



                    switch (info.PortType)
                    {
                        case "E":
                            info.PortType = "RJ45网口";
                            info.PortNumber = ePortTag + info.PortNumber;
                            break;

                        case "F":
                            info.PortType = "光纤网口";
                            info.PortNumber =fPortTag + info.PortNumber;
                            break;
                        case "M":
                            info.PortType = "管理网口";
                            info.PortNumber = mPortTag + info.PortNumber;
                            break;
                        case "D":
                            info.PortType = "硬盘插槽";
                            info.PortNumber = dPortTag + info.PortNumber;
                            break;
                        case "I":
                            info.PortType = "访问标签";
                            break;

                    }

                    switch (info.PortStatus)
                    {
                        case "0": //未分配

                            info.PortStatus = "";
                            break;

                        case "1":

                            info.PortStatus = "已分配";

                            break;
                    }


                    infos.Add(info);

                }


                //清空图表
                ListPlan.Children.Clear();
                //Graphics.Children.Clear();

                // 创建一个 ListView
                ListView listView = new ListView();

                // 设置 ListView 的垂直对齐方式为拉伸
                listView.VerticalAlignment = VerticalAlignment.Stretch;

                // 设置 ListView 的内容垂直对齐方式为顶部对齐
                listView.VerticalContentAlignment = VerticalAlignment.Stretch;

                ScrollViewer.SetCanContentScroll(listView, false);

                // 创建 ScrollViewer 控件
                ScrollViewer scrollViewer = new ScrollViewer();

                // 设置垂直滚动条的可见性为自动
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;


                // 创建一个 GridView
                GridView gridView = new GridView();


                // 添加列到 GridView
                gridView.Columns.Add(new GridViewColumn
                {
                    Header = "端口类型",
                    DisplayMemberBinding = new Binding("PortType")
                });
                gridView.Columns.Add(new GridViewColumn
                {
                    Header = "端口编号",
                    DisplayMemberBinding = new Binding("PortNumber")
                });
                gridView.Columns.Add(new GridViewColumn
                {
                    Header = "端口状态",
                    DisplayMemberBinding = new Binding("PortStatus")
                });
                gridView.Columns.Add(new GridViewColumn { Header = "端口标记", DisplayMemberBinding = new Binding("PortTag1") });
                gridView.Columns.Add(new GridViewColumn { Header = "端口标记", DisplayMemberBinding = new Binding("PortTag1") });
                gridView.Columns.Add(new GridViewColumn { Header = "端口标记", DisplayMemberBinding = new Binding("PortTag1") });
                gridView.Columns.Add(new GridViewColumn { Header = "端口注释", DisplayMemberBinding = new Binding("Description") });
                // 将 GridView 设置为 ListView 的 View
                listView.View = gridView;



                // 设置 ListView 的属性

                Binding bindingWidth = new Binding("ActualWidth");
                bindingWidth.Source = GraphicsPlan;
                listView.SetBinding(ListView.WidthProperty, bindingWidth);

                // 设置 ListView 的高度为 Auto
                listView.Height = double.NaN;

                listView.Margin = new Thickness(10);


                listView.ItemsSource = infos;
                listView.SelectionChanged += ListView_SelectionChanged;
                listView.MouseDoubleClick += ListView_MouseDoubleClick;




                scrollViewer.Content = listView;
                scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;


                ListPlan.Children.Add(scrollViewer);



            }
            else//搜索全部设备
            {
                //进行了全局搜索的标记
	            DataBrige.DeviceSearchType = 0;


				DataBrige.SearchDeviceInfos.Clear();

                DataBrige.SearchDeviceTableName = "Search_All_Devices_Table_" + KeyWord.Text;


                // 创建一个 ListView
                ListView listView = new ListView();

                // 创建 ScrollViewer 控件
                ScrollViewer scrollViewer = new ScrollViewer();
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto; // 设置垂直滚动条的可见性为自动
                listView.VerticalAlignment = VerticalAlignment.Top; // 设置 ListView 的垂直对齐方式为顶部对齐
                scrollViewer.Content = listView;


                // 创建一个 GridView
                GridView gridView = new GridView();


                // 添加列到 GridView
                gridView.Columns.Add(new GridViewColumn
                {
                    Header = "设备名称",
                    DisplayMemberBinding = new Binding("Name")
                });
                gridView.Columns.Add(
                    new GridViewColumn { Header = "设备型号", DisplayMemberBinding = new Binding("Model") });
                gridView.Columns.Add(
                    new GridViewColumn { Header = "设备编号", DisplayMemberBinding = new Binding("Number") });
                gridView.Columns.Add(new GridViewColumn
                {
                    Header = "端口类型",
                    DisplayMemberBinding = new Binding("PortType")
                });
                gridView.Columns.Add(new GridViewColumn
                {
                    Header = "端口编号",
                    DisplayMemberBinding = new Binding("PortNumber")
                });
                gridView.Columns.Add(new GridViewColumn
                {
                    Header = "端口状态",
                    DisplayMemberBinding = new Binding("PortStatus")
                });
                gridView.Columns.Add(new GridViewColumn
                {
                    Header = "端口标记",
                    DisplayMemberBinding = new Binding("PortTag1")
                });
                gridView.Columns.Add(new GridViewColumn
                {
                    Header = "端口标记",
                    DisplayMemberBinding = new Binding("PortTag2")
                });
                gridView.Columns.Add(new GridViewColumn
                {
                    Header = "端口标记",
                    DisplayMemberBinding = new Binding("PortTag3")
                });
                gridView.Columns.Add(new GridViewColumn
                {
                    Header = "端口注释",
                    DisplayMemberBinding = new Binding("Description")
                });
                // 将 GridView 设置为 ListView 的 View
                listView.View = gridView;



                // 设置 ListView 的属性

                Binding bindingWidth = new Binding("ActualWidth");
                bindingWidth.Source = GraphicsPlan;
                listView.SetBinding(ListView.WidthProperty, bindingWidth);



                listView.Margin = new Thickness(10);

                

                listView.SelectionChanged += ListView_SelectionChanged;
                listView.MouseDoubleClick += ListView_MouseDoubleClick;

                scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;

                ListPlan.Children.Add(scrollViewer);

               var infos = SearchInTables();


               string ePortTag = null;
               string fPortTag = null;
               string dPortTag = null;
               string mPortTag = null;
               string tableName=null;

                foreach (var item in infos)
                {
                    var info = item;

                    if (item.TableName != tableName)
                    {
                        ePortTag = GetDevicePortTag(item.TableName, "EportTag");
                        fPortTag = GetDevicePortTag(item.TableName, "FportTag");
                        dPortTag = GetDevicePortTag(item.TableName, "DportTag");
                        mPortTag = GetDevicePortTag(item.TableName, "MportTag");
                        tableName = info.TableName;
                    }


                    switch (info.PortType)
                    {
                        case "E":
                            info.PortType = "RJ45网口";

                            info.PortNumber = ePortTag + info.PortNumber;

                            break;

                        case "F":
                            info.PortType = "光纤网口";
                            info.PortNumber = fPortTag+ info.PortNumber;
                            break;
                        case "M":
                            info.PortType = "管理网口";
                            info.PortNumber = mPortTag + info.PortNumber;
                            break;
                        case "D":
                            info.PortType = "硬盘插槽";
                            info.PortNumber = dPortTag+ info.PortNumber;
                            break;
                        case "I":
                            info.PortType = "访问标签";
                            break;

                    }


                    switch (info.PortStatus)
                    {
                        case "0": //未分配

                            info.PortStatus = "";
                            break;

                        case "1":

                            info.PortStatus = "已分配";

                            break;
                    }


                    DataBrige.SearchDeviceInfos.Add(info);

                }







                listView.ItemsSource = DataBrige.SearchDeviceInfos;
                
                ListButton.IsChecked = true;
                GraphicsButton.IsEnabled = false;


            }
        }

        /// <summary>
        /// 获取设备槽位标签
        /// </summary>
        /// <param name="tableName">设备表名</param>
        /// <param name="tag">设备槽位类型</param>
        /// <returns></returns>
        private string GetDevicePortTag(string tableName, string tag)
        {

            string sql = $"SELECT {tag} FROM Devices WHERE TableName = '{tableName}'";
            SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                return reader[$"{tag}"].ToString();

                break;
            }

            return "";

        }

        public List<SearchDeviceInfo> SearchInTables()
        {
            List<SearchDeviceInfo> searchResults = new List<SearchDeviceInfo>();

            using (SQLiteCommand cmd = dbClass.connection.CreateCommand())
            {
                // 获取Devices表中的所有表名
                cmd.CommandText = "SELECT * FROM Devices";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tableName = reader["TableName"].ToString();
                        string name = reader["Name"].ToString();
                        Console.WriteLine(name);
                        string model = reader["Model"].ToString();
                        string number = reader["Number"].ToString();

                        List<SearchDeviceInfo> tableSearchResults = SearchInTable(tableName, name, model, number);

                        searchResults.AddRange(tableSearchResults);
                    }
                }
            }

            return searchResults;
        }

        private List<SearchDeviceInfo> SearchInTable(string tableName, string name, string model, string number)
        {
            List<SearchDeviceInfo> tableSearchResults = new List<SearchDeviceInfo>();

            // 构造针对特定表的查询语句
            string query = ConstructQueryForTable(tableName);

            Console.WriteLine(query);

            using (SQLiteCommand cmd = dbClass.connection.CreateCommand())
            {
                cmd.CommandText = query;
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // 将查询结果映射到SearchInfo对象
                        SearchDeviceInfo searchInfo = MapToSearchInfo(reader, tableName, name, model, number);

                        tableSearchResults.Add(searchInfo);
                    }
                }
            }

            return tableSearchResults;
        }

        /// <summary>
        /// 构建查询语句
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string ConstructQueryForTable(string tableName)
        {


            string keyword = KeyWord.Text;

            string sql;


            //判断是精确搜索还是模糊搜索,选中为精确，非选中为模糊
            if (Accurate.IsChecked == true)
            {

                sql = string.Format(" = '{0}'", keyword);
            }
            else
            {
                sql = string.Format("LIKE '%{0}%'", keyword);
            }


            return string.Format($"SELECT * FROM `{tableName}` WHERE  (`PortTag1` {sql} OR `PortTag2` {sql}  OR `PortTag3` {sql}  OR Description {sql} ) ");




        }

        /// <summary>
        /// 映射搜索结果
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="tableName"></param>
        /// <param name="network"></param>
        /// <param name="netmask"></param>
        /// <returns></returns>
        private SearchDeviceInfo MapToSearchInfo(SQLiteDataReader reader, string tableName, string name, string model, string number)
        {
            // 将查询结果映射到SearchInfo对象的方法，根据数据库字段和SearchInfo属性的映射自定义
            SearchDeviceInfo searchInfo = new SearchDeviceInfo()
            {
                TableName = tableName,
                Name = name,
                Model = model,
                Number = number,
                PortType = reader["PortType"].ToString(),
                PortNumber = reader["PortNumber"].ToString(),
                PortStatus = reader["PortStatus"].ToString(),
                PortTag1 = reader["PortTag1"].ToString(),
                PortTag2 = reader["PortTag2"].ToString(),
                PortTag3 = reader["PortTag3"].ToString(),
                Description = reader["Description"].ToString(),
            };

            return searchInfo;
        }


        private void SearchClear_OnClick(object sender, RoutedEventArgs e)
        {
            GraphicsPlan.Children.Clear();
            ListPlan.Children.Clear();
            DevicesView_OnSelectionChanged(null,null);
            SearchClear.IsEnabled=false;

        }

        /// <summary>
        /// 导出内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportButton_OnClick(object sender, RoutedEventArgs e)
        {
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


			// 创建 SaveFileDialog 实例
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
			saveFileDialog.FilterIndex = 1;
			saveFileDialog.RestoreDirectory = true;


			if (DataBrige.DeviceLoadType == 0)
			{

				// 设置默认文件名
				saveFileDialog.FileName = DataBrige.SelectDeviceTableName + ".xlsx";
			}
			else
			{
				// 设置默认文件名
				saveFileDialog.FileName = DataBrige.SearchDeviceTableName +"_"+ KeyWord.Text + ".xlsx";
			}




			// 显示 SaveFileDialog
			bool? result = saveFileDialog.ShowDialog();

			if (result == true)
			{
				// 获取用户选择的文件路径
				string filePath = saveFileDialog.FileName;


				if (DataBrige.DeviceSearchType == 0)
				{
					Console.WriteLine("s-0");
					ListViewExportToExcel(DataBrige.SearchDeviceInfos, filePath);
				}
				else
				{
					Console.WriteLine("s-1");
					ExportToExcel(DataBrige.DevicePortInfos, filePath);
				}



			}
		}

        /// <summary>
        /// 全局搜索导出
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="filePath"></param>
        public void ListViewExportToExcel(List<SearchDeviceInfo> dataList, string filePath)
        {


	        using (var package = new ExcelPackage())
	        {
		        var sheet = package.Workbook.Worksheets.Add("SearchAllTable");



				// 写入标题行

				sheet.Cells[1, 1].Value = "设备表名";
				sheet.Cells[1, 2].Value = "设备名称";
				sheet.Cells[1, 3].Value = "设备型号";
				sheet.Cells[1, 4].Value = "设备编号";
				sheet.Cells[1, 5].Value = "端口类型";
		        sheet.Cells[1, 6].Value = "端口编号";
		        sheet.Cells[1, 7].Value = "端口状态";
		        sheet.Cells[1, 8].Value = "端口标记";
		        sheet.Cells[1, 9].Value = "端口标记";
		        sheet.Cells[1, 10].Value = "端口标记";
		        sheet.Cells[1, 11].Value = "端口注释";


		        // 写入数据
		        int rowIndex = 2;
		        foreach (var info in dataList)
		        {
			        sheet.Cells[rowIndex, 1].Value = info.TableName;
			        sheet.Cells[rowIndex, 2].Value = info.Name;
			        sheet.Cells[rowIndex, 3].Value = info.Model;
			        sheet.Cells[rowIndex, 4].Value = info.Number;
					sheet.Cells[rowIndex, 5].Value = info.PortType;
			        sheet.Cells[rowIndex, 6].Value = info.PortNumber;
			        sheet.Cells[rowIndex, 7].Value = info.PortStatus;
			        sheet.Cells[rowIndex, 8].Value = info.PortTag1;
			        sheet.Cells[rowIndex, 9].Value = info.PortTag2;
			        sheet.Cells[rowIndex, 10].Value = info.PortTag3; ;
			        sheet.Cells[rowIndex, 11].Value = info.Description; ;

			        rowIndex++;
		        }


		        // 保存Excel文件
		        FileInfo excelFile = new FileInfo(filePath);



		        package.SaveAs(excelFile);

	        }


        }




		public void ExportToExcel(List<DevicePortInfo> dataList, string filePath)
		{

			if (DataBrige.LoadType == 1)
			{
				

				using (var package = new ExcelPackage())
				{
					var sheet = package.Workbook.Worksheets.Add(DataBrige.SearchDeviceTableName);


					// 写入标题行
					sheet.Cells[1, 1].Value = "PortType";
					sheet.Cells[1, 2].Value = "PortNumber";
					sheet.Cells[1, 3].Value = "PortStatus";
					sheet.Cells[1, 4].Value = "PortTag1";
					sheet.Cells[1, 5].Value = "PortTag2";
					sheet.Cells[1, 6].Value = "PortTag3";
					sheet.Cells[1, 7].Value = "Description";

					// 写入数据
					int rowIndex = 2;
					foreach (var info in dataList)
					{
						sheet.Cells[rowIndex, 1].Value = info.PortType;
						sheet.Cells[rowIndex, 2].Value = info.PortNumber;
						sheet.Cells[rowIndex, 3].Value = info.PortStatus;
						sheet.Cells[rowIndex, 4].Value = info.PortTag1;
						sheet.Cells[rowIndex, 5].Value = info.PortTag2;
						sheet.Cells[rowIndex, 6].Value = info.PortTag3; ;
						sheet.Cells[rowIndex, 7].Value = info.Description; ;
						rowIndex++;
					}


					// 保存Excel文件
					FileInfo excelFile = new FileInfo(filePath);



					package.SaveAs(excelFile);
				}
			}
			else
			{

				
				using (var package = new ExcelPackage())
				{
					var sheet = package.Workbook.Worksheets.Add(DataBrige.SelectDeviceTableName);

					// 写入标题行
					sheet.Cells[1, 1].Value = "PortType";
					sheet.Cells[1, 2].Value = "PortNumber";
					sheet.Cells[1, 3].Value = "PortStatus";
					sheet.Cells[1, 4].Value = "PortTag1";
					sheet.Cells[1, 5].Value = "PortTag2";
					sheet.Cells[1, 6].Value = "PortTag3";
					sheet.Cells[1, 7].Value = "Description";

					int rowIndex = 2;
					foreach (var info in dataList)
					{
						sheet.Cells[rowIndex, 1].Value = info.PortType;
						sheet.Cells[rowIndex, 2].Value = info.PortNumber;
						sheet.Cells[rowIndex, 3].Value = info.PortStatus;
						sheet.Cells[rowIndex, 4].Value = info.PortTag1;
						sheet.Cells[rowIndex, 5].Value = info.PortTag2;
						sheet.Cells[rowIndex, 6].Value = info.PortTag3; ;
						sheet.Cells[rowIndex, 7].Value = info.Description; ;
						rowIndex++;
					}

					// 保存Excel文件
					FileInfo excelFile = new FileInfo(filePath);

					package.SaveAs(excelFile);
				}
			}


		}

		private void DeviceBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (DeviceBox.SelectedIndex != -1)
			{
				DataBrige.SearchDeviceTableName = DataBrige.DeviceInfos[DeviceBox.SelectedIndex].TableName;
			}

			
		}

		private void ImportButton_OnClick(object sender, RoutedEventArgs e)
		{
			DeviceImportWindow deviceImportWindow = new DeviceImportWindow();



            if (deviceImportWindow.ShowDialog() == true)
            {

                DevicesView_OnSelectionChanged(null, null);
                // 当子窗口关闭后执行这里的代码
                //LoadNetworkInfo(dbClass.connection);

            }
        }
    }
}
