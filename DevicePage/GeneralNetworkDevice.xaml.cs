using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using IPAM_NOTE.DatabaseOperation;

namespace IPAM_NOTE.DevicePage
{
    /// <summary>
    /// GeneralNetworkDevice.xaml 的交互逻辑
    /// </summary>
    public partial class GeneralNetworkDevice : UserControl
    {
        public GeneralNetworkDevice()
        {
            InitializeComponent();
        }

        // 声明一个关闭窗口事件
        public static event EventHandler CloseParentWindowRequested;

        private DbClass dbClass;

        /// <summary>
        /// 页面加载完毕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GeneralNetworkDevice_OnLoaded(object sender, RoutedEventArgs e)
        {
            string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";


            string dbName = "Address_database.db";


            dbFilePath = dbFilePath + dbName;

            dbClass = new DbClass(dbFilePath);
            dbClass.OpenConnection();

            DeviceNamePreset.ItemsSource = deviceTypePresets;

            ModelPreset.ItemsSource = modelPresetInfos;

            LoadPreset();
        }

        #region 端口数字输入





        private void EthernetSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            EthernetPort.Text = EthernetSlider.Value.ToString();
        }

        private void FiberSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FiberPort.Text = FiberSlider.Value.ToString();
        }

        private void ManageSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ManagePort.Text = ManageSlider.Value.ToString();
        }

        private void E0_OnClick(object sender, RoutedEventArgs e)
        {

            int value = Convert.ToInt32(E0.Content);

            EthernetPort.Text = value.ToString();
            EthernetSlider.Value = value;
        }

        private void E4_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(E4.Content);

            EthernetPort.Text = value.ToString();
            EthernetSlider.Value = value;
        }

        private void E12_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(E12.Content);

            EthernetPort.Text = value.ToString();
            EthernetSlider.Value = value;
        }

        private void E24_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(E24.Content);

            EthernetPort.Text = value.ToString();
            EthernetSlider.Value = value;
        }

        private void E36_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(E36.Content);

            EthernetPort.Text = value.ToString();
            EthernetSlider.Value = value;
        }

        private void E48_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(E48.Content);

            EthernetPort.Text = value.ToString();
            EthernetSlider.Value = value;
        }

        private void F0_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(F0.Content);

            FiberPort.Text = value.ToString();
            FiberSlider.Value = value;
        }

        private void F4_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(F4.Content);

            FiberPort.Text = value.ToString();
            FiberSlider.Value = value;
        }

        private void F12_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(F12.Content);

            FiberPort.Text = value.ToString();
            FiberSlider.Value = value;
        }

        private void F24_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(F24.Content);

            FiberPort.Text = value.ToString();
            FiberSlider.Value = value;
        }

        private void F36_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(F36.Content);

            FiberPort.Text = value.ToString();
            FiberSlider.Value = value;
        }

        private void F48_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(F48.Content);

            FiberPort.Text = value.ToString();
            FiberSlider.Value = value;
        }

        private void M0_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(M0.Content);

            ManagePort.Text = value.ToString();
            ManageSlider.Value = value;
        }

        private void M1_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(M1.Content);

            ManagePort.Text = value.ToString();
            ManageSlider.Value = value;
        }

        private void M2_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(M2.Content);

            ManagePort.Text = value.ToString();
            ManageSlider.Value = value;
        }

        private void M3_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(M3.Content);

            ManagePort.Text = value.ToString();
            ManageSlider.Value = value;
        }

        private void M4_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(M4.Content);

            ManagePort.Text = value.ToString();
            ManageSlider.Value = value;
        }


        private void D0_OnClick(object sender, RoutedEventArgs e)
        {

            int value = Convert.ToInt32(D0.Content);

            DiskBox.Text = value.ToString();
            DiskSlider.Value = value;
        }

        private void D4_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(D4.Content);

            DiskBox.Text = value.ToString();
            DiskSlider.Value = value;
        }

        private void D8_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(D8.Content);

            DiskBox.Text = value.ToString();
            DiskSlider.Value = value;
        }

        private void D16_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(D16.Content);

            DiskBox.Text = value.ToString();
            DiskSlider.Value = value;
        }

        private void D32_OnClick(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(D32.Content);

            DiskBox.Text = value.ToString();
            DiskSlider.Value = value;
        }

        #endregion


        private List<string> deviceTypePresets = new List<string>();
        private List<string> brandPresets = new List<string>();
        private List<ViewMode.ModelPresetInfo> modelPresetInfos = new List<ViewMode.ModelPresetInfo>();

        /// <summary>
        /// 加载设备预设
        /// </summary>
        private void LoadPreset()
        {
            //设备类型预设
            deviceTypePresets.Add("交换机");
            deviceTypePresets.Add("服务器");
            deviceTypePresets.Add("通用设备");
        }


        private List<string> LoadBrandList(int typeIndex = 0)
        {
            List<string> brands = new List<string>();



            switch (typeIndex)
            {
                case 0://交换机
                    brands.Add("华为(HUAWEI)");
                    brands.Add("华三(H3C)");
                    brands.Add("锐捷(RUJIE)");
                    brands.Add("思科(CISCO)");
                    return brands;

                case 1://服务器
                    brands.Add("华为(HUAWEI)");
                    brands.Add("华三(H3C)");
                    brands.Add("锐捷(RUJIE)");
                    brands.Add("联想(LENOVO)");
                    brands.Add("惠普(HP)");
                    brands.Add("戴尔(DELL)");
                    brands.Add("IBM(IBM)");
                    return brands;





                default:

                    return null;
            }


        }


        private List<ViewMode.ModelPresetInfo> loadModelList(int typeIndex = 0, int brandIndex = 0)
        {


            List<ViewMode.ModelPresetInfo> modelInfo = new List<ViewMode.ModelPresetInfo>();

            switch (typeIndex)
            {
                case 0://交换机


                    switch (brandIndex)
                    {
                        case 0://华为

                            modelInfo.Add(new ViewMode.ModelPresetInfo("S5700-10P-LI-AC", 8, 2, 1));
                            modelInfo.Add(new ViewMode.ModelPresetInfo("S5720-14X-PWH-SI-AC", 12, 2, 1));
                            modelInfo.Add(new ViewMode.ModelPresetInfo("S5700-28P-LI-AC", 24, 4, 1));
                            modelInfo.Add(new ViewMode.ModelPresetInfo("S5700-52X-LI-AC", 48, 4, 1));
                            modelInfo.Add(new ViewMode.ModelPresetInfo("S5735-S32ST4X", 8, 28, 2));
                            modelInfo.Add(new ViewMode.ModelPresetInfo("S5735S-H24S4XC-A", 0, 28, 2));



                            return modelInfo;

                        case 1://H3C

                            return null;






                        default:

                            return null;
                    }




                    break;


                case 1://服务器


                    return null;


                default://通用设备


                    return null;
            }



        }




        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            //1、添加设备到设备总表


            if (CheckInput() == 0)
            {

                string tableName = CreateTableName();

                //插入设备信息总表的数据
                string sql = string.Format(
                    $"INSERT INTO Devices (TableName,Name,Model,Number,People,Date,Description,Eport,Fport,Mport) VALUES ('{tableName}','{NameBox.Text}','{ModelBox.Text}','{NumberBox.Text}','{PeopleBox.Text}','{DatePicker.Text}','{DescriptionBox.Text}',{Convert.ToInt32(EthernetPort.Text)},{Convert.ToInt32(FiberPort.Text)},{Convert.ToInt32(ManagePort.Text)})");
                dbClass.ExecuteQuery(sql);

                //创建表
                CreateTable(tableName);

                //初始化设备详细信息表
                InitializedData(tableName);

                //关闭窗口
                CloseParentWindowRequested?.Invoke(this, EventArgs.Empty);
            }


        }

        /// <summary>
        /// 检查用户输入是否合法
        /// </summary>
        /// <returns></returns>
        private int CheckInput()
        {

            string message = "";
            int index = 0;

            //1.校验设备名
            if (NameBox.Text.Length < 1)
            {
                index += 1;
                message += index + ":请输入设备名称\r";

            }


            int ePort = Convert.ToInt32(EthernetPort.Text);
            int fPort = Convert.ToInt32(FiberPort.Text);


            //1.校验端口
            if (ePort == 0 && fPort == 0)
            {
                index += 1;
                message += index + ":光口和电口数量不能都为0\r";

            }




            if (index > 0)
            {
                MessageBox.Show(message, "发现" + index + "项内容需要注意！", MessageBoxButton.OK, MessageBoxImage.Warning);
                return index;
            }
            else
            {
                return 0;
            }



        }



        /// <summary>
        /// 生成设备对应表名
        /// </summary>
        /// <returns></returns>
        private string CreateTableName()
        {
            string tableName;

            DateTime now = DateTime.Now;
            string formattedTime = now.ToString("yyyyMMddHHmmss");
            tableName = "Dev_" + formattedTime;

            return tableName;
        }


        #region 选择设备预设
        private void DeviceNamePreset_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeviceNamePreset.SelectedIndex != -1)
            {
                NameBox.Text = DeviceNamePreset.SelectedItem.ToString();
                brandPresets = LoadBrandList(DeviceNamePreset.SelectedIndex);
                BrandPreset.ItemsSource = brandPresets;
            }
        }

        private void BrandPreset_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BrandPreset.SelectedIndex != -1)
            {
                modelPresetInfos = loadModelList(DeviceNamePreset.SelectedIndex, BrandPreset.SelectedIndex);
                ModelPreset.ItemsSource = modelPresetInfos;
            }
        }

        private void ModelPreset_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModelPreset.SelectedIndex != -1)
            {

                ViewMode.ModelPresetInfo info = (ViewMode.ModelPresetInfo)ModelPreset.SelectedItem;

                ModelBox.Text = BrandPreset.SelectedItem.ToString() + info.Model;

                EthernetSlider.Value = info.Eport;
                FiberSlider.Value = info.Fport;
                ManageSlider.Value = info.Mport;

            }

        }

        #endregion


        /// <summary>
        /// 创建一张数据表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private void CreateTable(string tableName)
        {
            string sql = string.Format(
                "CREATE TABLE IF NOT EXISTS {0} (\r\n  \"PortType\" TEXT,\r\n  \"PortNumber\" TEXT,\r\n  \"PortStatus\" integer,\r\n  \"PortTag1\" TEXT,\r\n  \"PortTag2\" TEXT,\r\n  \"PortTag3\" TEXT,\r\n  \"Description\" TEXT\r\n);",
                tableName);

            dbClass.ExecuteQuery(sql);


        }

        /// <summary>
        /// 填充网段表单初始数据
        /// </summary>
        private void InitializedData(string tableName, int Eprot = 0, int Fport = 0, int Mport = 0, int Disk = 0)
        {
            string sql;
                

            //添加电口
            int x = Convert.ToInt32(EthernetPort.Text);
            if (x > 0)
            {
                for (int i = 0; i < x; i++)
                {
                    //PortStatus为0时表示未用端口，1表示已用端口
                    sql = $"INSERT INTO \"{tableName}\" (\"PortType\", \"PortNumber\", \"PortStatus\", \"PortTag1\", \"PortTag2\", \"PortTag3\", \"Description\") VALUES ('E', '{i}', 0, '', '', '', '')";
                    dbClass.ExecuteQuery(sql);
                }
            }

            //添加光口
            x = Convert.ToInt32(FiberPort.Text);
            if (x > 0)
            {
                for (int i = 0; i < x; i++)
                {
                    //PortStatus为0时表示未用端口，1表示已用端口
                    sql = $"INSERT INTO \"{tableName}\" (\"PortType\", \"PortNumber\", \"PortStatus\", \"PortTag1\", \"PortTag2\", \"PortTag3\", \"Description\") VALUES ('F', '{i}', 0, '', '', '', '')";
                    dbClass.ExecuteQuery(sql);
                }
            }

            //添加管理口
            x = Convert.ToInt32(ManagePort.Text);
            if (x > 0)
            {
                for (int i = 0; i < x; i++)
                {
                    //PortStatus为0时表示未用端口，1表示已用端口
                    sql = $"INSERT INTO \"{tableName}\" (\"PortType\", \"PortNumber\", \"PortStatus\", \"PortTag1\", \"PortTag2\", \"PortTag3\", \"Description\") VALUES ('M', '{i}', 0, '', '', '', '')";
                    dbClass.ExecuteQuery(sql);
                }
            }

            //添加硬盘位
            x = Convert.ToInt32(DiskBox.Text);
            if (x > 0)
            {
                for (int i = 0; i < x; i++)
                {
                    //PortStatus为0时表示未用端口，1表示已用端口
                    sql = $"INSERT INTO \"{tableName}\" (\"PortType\", \"PortNumber\", \"PortStatus\", \"PortTag1\", \"PortTag2\", \"PortTag3\", \"Description\") VALUES ('D', '{i}', 0, '', '', '', '')";
                    dbClass.ExecuteQuery(sql);
                }
            }
        }




        private void ECheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            EthernetSlider.Value = 0;
            if (ECheckBox.IsChecked == true)
            {
                
                EPlan.Visibility = Visibility.Visible;
            }
            else
            {
                EPlan.Visibility = Visibility.Collapsed;
            }
        }

        private void FCheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            FiberSlider.Value = 0;
            if (FCheckBox.IsChecked == true)
            {

                FPlan.Visibility = Visibility.Visible;
            }
            else
            {
               FPlan.Visibility = Visibility.Collapsed;
            }
        }

        private void MCheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            ManageSlider.Value = 0;
            if (MCheckBox.IsChecked == true)
            {

                MPlan.Visibility = Visibility.Visible;
            }
            else
            {
                MPlan.Visibility = Visibility.Collapsed;
            }
        }

        private void DCheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            DiskSlider.Value = 0;
            if (DCheckBox.IsChecked == true)
            {

                DPlan.Visibility = Visibility.Visible;
            }
            else
            {
                DPlan.Visibility = Visibility.Collapsed;
            }
        }

        private void DiskSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DiskBox.Text = DiskSlider.Value.ToString();
        }
    }
}
