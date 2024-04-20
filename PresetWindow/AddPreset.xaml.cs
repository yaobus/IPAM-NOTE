using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
using IPAM_NOTE.DatabaseOperation;

namespace IPAM_NOTE.PresetWindow
{
    /// <summary>
    /// AddPreset.xaml 的交互逻辑
    /// </summary>
    public partial class AddPreset : Window
    {
        public AddPreset()
        {
            InitializeComponent();
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


        private DbClass dbClass;

        private void DiskSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DiskBox.Text = DiskSlider.Value.ToString();
        }


        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (CheckInput() == 0)
            {

                //检查设备是否存在

                //检查设备编号是否存在
                string sqlTemp = $"SELECT COUNT(*) FROM ModelPreset WHERE `ModelType` = '{ModelBox.Text}' AND `Brand` = '{BrandBox.Text}' AND `Model` = '{ModelBox.Text}'";


                int num = dbClass.ExecuteScalarTableNum(sqlTemp, dbClass.connection);

                if (num == 0)
                {
                    //插入设备信息总表的数据
                    string sql = string.Format($"INSERT INTO \"main\".\"ModelPreset\" (\"ModelType\", \"Brand\", \"Model\", \"Ethernet\", \"Fiber\", \"Disk\", \"Manage\") VALUES ( '{NameBox.Text}', '{BrandBox.Text}', '{ModelBox.Text}', '{EthernetPort.Text}', '{FiberPort.Text}', '{DiskBox.Text}', '{ManagePort.Text}')");
                    dbClass.ExecuteQuery(sql);

                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("已存在同类型，同品牌，同型号预设", "预设重复", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            if (NameBox.Text.Length < 1)
            {
                index += 1;
                message += index + ":请输入设备类型\r";

            }

            //1.校验设备名
            if (BrandBox.Text.Length < 1)
            {
                index += 1;
                message += index + ":请输入设备品牌\r";

            }

            //1.校验设备名
            if (BrandBox.Text.Length < 1)
            {
                index += 1;
                message += index + ":请输入设备型号\r";

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

        private void AddPreset_OnLoaded(object sender, RoutedEventArgs e)
        {
            string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";
            string dbName = "Address_database.db";

            dbFilePath = dbFilePath + dbName;

            dbClass = new DbClass(dbFilePath);
            dbClass.OpenConnection();

            LoadPreset();
        }

        List<string>typeList=new List<string>();
        List<string>brandList=new List<string>();


        /// <summary>
        /// 加载品牌和类型预设
        /// </summary>
        private void LoadPreset()
        {
            typeList.Clear();
            brandList.Clear();
            string query = "SELECT DISTINCT ModelType FROM ModelPreset;";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();


            while (reader.Read())
            {
                typeList.Add(reader["ModelType"].ToString());
            }

            NameBox.ItemsSource = typeList;

            query = "SELECT DISTINCT Brand FROM ModelPreset;";
            command = new SQLiteCommand(query, dbClass.connection);

             reader = command.ExecuteReader();


            while (reader.Read())
            {
                brandList.Add(reader["Brand"].ToString());
            }

            BrandBox.ItemsSource= brandList;
        }

    }
}
