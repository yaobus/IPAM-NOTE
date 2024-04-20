using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
using ControlzEx.Standard;
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

            LoadPreset();

            if (DataBrige.DeviceAddStatus == 1)//编辑设备
            {
                TypeBox.Text = DataBrige.SelectDeviceInfo.Name;



                //string extractedText;
                //// 匹配方括号内的文本
                //Match match = Regex.Match(DataBrige.SelectDeviceInfo.Model, @"\[(.*?)\]");
                //if (match.Success)
                //{
                //   extractedText = match.Groups[1].Value;
                   
                //}
                //else
                //{
                //    extractedText = null;
                //}

                //if (extractedText != null)
                //{
                    
                //}
                //else
                //{
                //    ModelBox.Text = DataBrige.SelectDeviceInfo.Model;
                //}




                ModelBox.Text = Regex.Replace(DataBrige.SelectDeviceInfo.Model, @"\[.*?\]", "");



                NumberBox.Text = DataBrige.SelectDeviceInfo.Number;
                PeopleBox.Text = DataBrige.SelectDeviceInfo.People;

                DateBox.Text = DataBrige.SelectDeviceInfo.Date;

                DescriptionBox.Text = DataBrige.SelectDeviceInfo.Description;


                EPlan.Visibility= Visibility.Collapsed;
                FPlan.Visibility = Visibility.Collapsed;
                DPlan.Visibility = Visibility.Collapsed;
                MPlan.Visibility = Visibility.Collapsed;

                BrandBox.Visibility= Visibility.Collapsed;
                BrandLabel.Visibility= Visibility.Collapsed;

                ECheckBox.IsEnabled = false;
                ECheckBox.IsChecked = false;

                SerialBox.IsEnabled=false;
                SerialBox.IsChecked=false;

                FCheckBox.IsEnabled = false;
                FCheckBox.IsChecked = false;

                MCheckBox.IsEnabled = false;
                MCheckBox.IsChecked = false;

                DCheckBox.IsEnabled = false;
                DCheckBox.IsChecked = false;
            }



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




        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {


            if (DataBrige.DeviceAddStatus == 0)//添加
            {

                //检查设备编号是否存在
                string sqlTemp = $"SELECT COUNT(*) FROM Devices WHERE `Number` = '{NumberBox.Text}'";

                


                int num = dbClass.ExecuteScalarTableNum(sqlTemp, dbClass.connection);


                if (num == 0)
                {

                    //1、添加设备到设备总表
                    if (CheckInput() == 0)
                    {

                        string tableName = CreateTableName();


                        string model = "[" + BrandBox.Text + "]" + ModelBox.Text;

                        //插入设备信息总表的数据
                        string sql = string.Format(
                            $"INSERT INTO Devices (TableName,Name,Model,Number,People,Date,Description,Eport,EportTag,Fport,FportTag,Mport,MportTag,Dport,DportTag) " +
                            $"VALUES ('{tableName}','{TypeBox.Text}','{model}','{NumberBox.Text}','{PeopleBox.Text}','{DateBox.Text}','{DescriptionBox.Text}'," +
                            $"{Convert.ToInt32(EthernetPort.Text)},'{EComboBox.Text}',{Convert.ToInt32(FiberPort.Text)},'{FiberComboBox.Text}',{Convert.ToInt32(ManagePort.Text)},'{MgmtComboBox.Text}',{Convert.ToInt32(DiskBox.Text)},'{DiskComboBox.Text}')");
                        dbClass.ExecuteQuery(sql);

                        //创建表
                        CreateTable(tableName);

                        //初始化设备详细信息表
                        InitializedData(tableName);

                        //关闭窗口
                        CloseParentWindowRequested?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    MessageBox.Show("已存在相同编号的设备，请检查", "编号重复", MessageBoxButton.OK, MessageBoxImage.Warning);

                }







            }
            else //编辑
            {


                if (TypeBox.Text.Length > 0)
                {


                    //检查设备编号是否存在
                    string sqlTemp = $"SELECT COUNT(*) FROM Devices WHERE `Number` = '{NumberBox.Text}'";




                    int num = dbClass.ExecuteScalarTableNum(sqlTemp, dbClass.connection);


                    if (num == 0 || NumberBox.Text == DataBrige.SelectDeviceInfo.Number)
                    {
                        string tableName = DataBrige.SelectDeviceInfo.TableName;


                        string model = DataBrige.SelectDeviceInfo.Model;//数据库中的型号

                        string brand;//品牌

                        Match match = Regex.Match(model, @"\[(.*?)\]");

                        if (match.Success)
                        {
                            brand = match.Groups[1].Value;
                           
                        }
                        else
                        {
                            brand = null;
                        }

                        string newModel;

                        if (brand != null)
                        {
                            newModel = "[" + brand + "]" + ModelBox.Text;//新的型号
                        }
                        else
                        {
                            newModel =  ModelBox.Text;//新的型号
                        }

                       


                        string sql = string.Format(
                            $"UPDATE Devices SET \"Name\" = '{TypeBox.Text}', \"Model\" = '{newModel}'  , \"Number\" = '{NumberBox.Text}' , \"People\" = '{PeopleBox.Text}', \"Description\" = '{DescriptionBox.Text}' WHERE TableName = '{tableName}'");
                        Console.WriteLine(sql);

                        dbClass.ExecuteQuery(sql); //写入端口信息

                        //关闭窗口
                        CloseParentWindowRequested?.Invoke(this, EventArgs.Empty);

                    }
                    else
                    {
                        MessageBox.Show("已存在相同编号的设备，请检查", "编号重复", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }



                }
                else
                {
                    MessageBox.Show("设备名称不得为空", "必要信息不完整", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

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
            if (TypeBox.Text.Length < 1)
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


        List<string> typeList = new List<string>();



        /// <summary>
        /// 加载类型预设
        /// </summary>
        private void LoadPreset()
        {
            typeList.Clear();

            string query = "SELECT DISTINCT ModelType FROM ModelPreset;";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();


            while (reader.Read())
            {
                typeList.Add(reader["ModelType"].ToString());
            }

            TypeBox.ItemsSource = null;
            TypeBox.ItemsSource = typeList;



        }


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
                    sql = $"INSERT INTO \"{tableName}\" (\"PortType\", \"PortNumber\", \"PortStatus\", \"PortTag1\", \"PortTag2\", \"PortTag3\", \"Description\") VALUES ('E', '{i + 1}', 0, '', '', '', '')";
                    dbClass.ExecuteQuery(sql);
                }
            }

            //添加光口
            x = Convert.ToInt32(FiberPort.Text);

            if (x > 0)
            {
                int num = Convert.ToInt32(EthernetPort.Text) + 1;

                for (int i = 0; i < x; i++)
                {
                    
                    if (SerialBox.IsChecked == true)//连续编号
                    {
                        
                        //PortStatus为0时表示未用端口，1表示已用端口
                        sql = $"INSERT INTO \"{tableName}\" (\"PortType\", \"PortNumber\", \"PortStatus\", \"PortTag1\", \"PortTag2\", \"PortTag3\", \"Description\") VALUES ('F', '{num + i}', 0, '', '', '', '')";
                    }
                    else
                    {
                        
                        //PortStatus为0时表示未用端口，1表示已用端口
                        sql = $"INSERT INTO \"{tableName}\" (\"PortType\", \"PortNumber\", \"PortStatus\", \"PortTag1\", \"PortTag2\", \"PortTag3\", \"Description\") VALUES ('F', '{i + 1}', 0, '', '', '', '')";
                    }


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
                    sql = $"INSERT INTO \"{tableName}\" (\"PortType\", \"PortNumber\", \"PortStatus\", \"PortTag1\", \"PortTag2\", \"PortTag3\", \"Description\") VALUES ('M', '{i + 1}', 0, '', '', '', '')";
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
                    sql = $"INSERT INTO \"{tableName}\" (\"PortType\", \"PortNumber\", \"PortStatus\", \"PortTag1\", \"PortTag2\", \"PortTag3\", \"Description\") VALUES ('D', '{i + 1}', 0, '', '', '', '')";
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

        private void DatePicker_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateBox.Text = DatePicker.Text;
        }


        /// <summary>
        /// 选择类别，加载品牌
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TypeBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            brandList.Clear();
            modeList.Clear();
            presetInfos.Clear();
            ModelBox.Text = null;
            EthernetSlider.Value = 0;
            FiberSlider.Value = 0;
            DiskSlider.Value = 0;
            ManageSlider.Value = 0;

            if (TypeBox.SelectedIndex != -1)
            {
                string type = typeList[TypeBox.SelectedIndex];

                Console.WriteLine(type);

                LoadBrandPreset(type);

            }

        }

        /// <summary>
        /// 品牌列表
        /// </summary>
        private List<string> brandList = new List<string>();

        /// <summary>
        /// 加载品牌预设
        /// </summary>
        private void LoadBrandPreset(string type)
        {
            brandList.Clear();

            string query = $"SELECT DISTINCT Brand FROM ModelPreset WHERE ModelType ='{type}';";

            Console.WriteLine(query);

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();


            while (reader.Read())
            {
                brandList.Add(reader["Brand"].ToString());
            }

            BrandBox.ItemsSource = null;
            BrandBox.ItemsSource = brandList;
        }


        /// <summary>
        /// 选择品牌预设
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrandBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            modeList.Clear();
            presetInfos.Clear();
            ModelBox.Text = null;
            EthernetSlider.Value = 0;
            FiberSlider.Value = 0;
            DiskSlider.Value = 0;
            ManageSlider.Value = 0;

            if (BrandBox.SelectedIndex != -1)
            {
                string type = typeList[TypeBox.SelectedIndex];

                string brand = brandList[BrandBox.SelectedIndex];



                LoadModelPreset(type, brand);

            }

        }

        private List<string> modeList = new List<string>();

        private List<ViewMode.ModelPresetInfo> presetInfos = new List<ViewMode.ModelPresetInfo>();


        /// <summary>
        /// 加载品牌预设
        /// </summary>
        private void LoadModelPreset(string type, string brand)
        {
            modeList.Clear();

            string query = $"SELECT * FROM ModelPreset WHERE ModelType ='{type}' AND Brand='{brand}';";

            Console.WriteLine(query);

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int id = 0;
            while (reader.Read())
            {

                id++;
                string model = reader["Model"].ToString();

                int ethernet = Convert.ToInt32(reader["Ethernet"]);

                int fiber = Convert.ToInt32(reader["Fiber"]);

                int disk = Convert.ToInt32(reader["Disk"]);

                int manage = Convert.ToInt32(reader["Manage"]);

                ViewMode.ModelPresetInfo info =
                    new ViewMode.ModelPresetInfo(id, type, brand, model, ethernet, fiber, disk, manage);
                presetInfos.Add(info);
                modeList.Add(model);

            }

            ModelBox.ItemsSource = null;
            ModelBox.ItemsSource = modeList;
        }



        /// <summary>
        /// 加载预设
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModelBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataBrige.DeviceAddStatus != 1)
            {
                int index = ModelBox.SelectedIndex;
                if (index != -1)
                {
                    int ethernet = presetInfos[index].Ethernet;

                    if (ethernet > 0)
                    {
                        ECheckBox.IsChecked = true;
                        EPlan.Visibility = Visibility.Visible;
                        EthernetSlider.Value = ethernet;
                    }



                    int fiber = presetInfos[index].Fiber;

                    if (fiber > 0)
                    {
                        FCheckBox.IsChecked = true;
                        FPlan.Visibility = Visibility.Visible;
                        FiberSlider.Value = fiber;
                    }


                    int disk = presetInfos[index].Disk;

                    if (disk > 0)
                    {
                        DCheckBox.IsChecked = true;
                        DPlan.Visibility = Visibility.Visible;
                        DiskSlider.Value = disk;
                    }


                    int manage = presetInfos[index].Manage;

                    if (manage > 0)
                    {
                        MCheckBox.IsChecked = true;
                        MPlan.Visibility = Visibility.Visible;
                        ManageSlider.Value = manage;
                    }




                }

            }



        }
    }
}
