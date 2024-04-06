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
            dbClass.CreateTableIfNotExists("Devices");

            LoadDevicesInfo(dbClass.connection);
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
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
                    int fPort = Convert.ToInt32(reader["Fport"]);
                    int mPort = Convert.ToInt32(reader["Mport"]);

                    DataBrige.DeviceInfos.Add(new DeviceInfo(id, tableName, name, model, number, people, date, description, ePort, fPort, mPort));
                    
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
                ViewMode.DeviceInfo info = DevicesView.SelectedItem as ViewMode.DeviceInfo;


                //加载所选设备信息
                GetDeviceInfo(info.TableName);

               

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

                    StackPanel panel = CreateButtonContent(i.ToString(), PackIconKind.Ethernet);

                    Button newButton = new Button()
                    {
                        Width = 60,
                        Height = 60,
                        Background = colorBrush,
                        Foreground = fontBrush,
                        FontWeight = FontWeights.ExtraBold,
                        Content = panel,
                        ToolTip = description,
                        Tag = i.ToString(),
                        Style = (Style)this.FindResource("MaterialDesignFlatSecondaryDarkBgButton"),
                        BorderThickness = new Thickness(0),
                        Margin = new Thickness(5),

                    };


                    //显示到图形化区域
                    subitemPanel.Children.Add(newButton);

                }



                Graphics.Children.Add(subitemPanel);

                Separator separator = new Separator();
                separator.Width = 10000; // 设置横线的宽度，根据需要调整

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

             if (x>0)
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

                    StackPanel panel = CreateButtonContent(i.ToString(), PackIconKind.ViewStreamOutline);

                    Button newButton = new Button()
                    {
                        Width = 60,
                        Height = 60,
                        Background = colorBrush,
                        Foreground = fontBrush,
                        FontWeight = FontWeights.ExtraBold,
                        Content = panel,
                        ToolTip = description,
                        Tag = i.ToString(),
                        Style = (Style)this.FindResource("MaterialDesignFlatSecondaryDarkBgButton"),
                        BorderThickness = new Thickness(0),
                        Margin = new Thickness(5),

                    };


                    //显示到图形化区域
                    fSubitemPanel.Children.Add(newButton);

                }

                Graphics.Children.Add(fSubitemPanel);

                Separator fiberseparator = new Separator();
                fiberseparator.Width = 10000; // 设置横线的宽度，根据需要调整

                //fiberseparator.Loaded += (sender, e) =>
                //{
                //    // 设置横线的宽度为父容器的宽度
                //    fiberseparator.Width = Graphics.ActualWidth;
                //};

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

                    StackPanel panel = CreateButtonContent(i.ToString(), PackIconKind.Hdd);

                    Button newButton = new Button()
                    {
                        Width = 60,
                        Height = 60,
                        Background = colorBrush,
                        Foreground = fontBrush,
                        FontWeight = FontWeights.ExtraBold,
                        Content = panel,
                        ToolTip = description,
                        Tag = i.ToString(),
                        Style = (Style)this.FindResource("MaterialDesignFlatSecondaryDarkBgButton"),
                        BorderThickness = new Thickness(0),
                        Margin = new Thickness(5),

                    };


                    //显示到图形化区域
                    dSubitemPanel.Children.Add(newButton);

                }

                Graphics.Children.Add(dSubitemPanel);

                Separator diskSeparator = new Separator();
                diskSeparator.Width = 10000; // 设置横线的宽度，根据需要调整



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

                    StackPanel panel = CreateButtonContent(i.ToString(), PackIconKind.Ethernet);

                    Button newButton = new Button()
                    {
                        Width = 60,
                        Height = 60,
                        Background = colorBrush,
                        Foreground = fontBrush,
                        FontWeight = FontWeights.ExtraBold,
                        Content = panel,
                        ToolTip = description,
                        Tag = i.ToString(),
                        Style = (Style)this.FindResource("MaterialDesignFlatSecondaryDarkBgButton"),
                        BorderThickness = new Thickness(0),
                        Margin = new Thickness(5),

                    };


                    //显示到图形化区域
                    mSubitemPanel.Children.Add(newButton);

                }

                Graphics.Children.Add(mSubitemPanel);

                Separator miberseparator = new Separator();
                miberseparator.Width = 10000; // 设置横线的宽度，根据需要调整

                //miberseparator.Loaded += (sender, e) =>
                //{
                //    // 设置横线的宽度为父容器的宽度
                //    miberseparator.Width = Graphics.ActualWidth;
                //};

                Graphics.Children.Add(miberseparator);
            }



            #endregion



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
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;

            // 创建图标
            PackIcon icon = new PackIcon();
            icon.Kind = iconKind;
            icon.Width = 32;
            icon.Height = 32;
            icon.HorizontalAlignment = HorizontalAlignment.Center;
            
            icon.Margin = new Thickness(0, 0, 0, 0);

            // 创建文本块
            TextBlock textBlock = new TextBlock();
            textBlock.Text = buttonText;
            //textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
           
            // 将图标和文本块添加到 StackPanel 中
            stackPanel.Children.Add(icon);
            stackPanel.Children.Add(textBlock);

            return stackPanel;
        }
    }
}
