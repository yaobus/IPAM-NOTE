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
using System.Windows.Navigation;
using System.Windows.Shapes;
using IPAM_NOTE.DatabaseOperation;
using IPAM_NOTE.PresetWindow;
using static IPAM_NOTE.ViewMode;

namespace IPAM_NOTE.UserPages
{
    /// <summary>
    /// ModelPreset.xaml 的交互逻辑
    /// </summary>
    public partial class ModelPreset : UserControl
    {
        public ModelPreset()
        {
            InitializeComponent();
        }
        private DbClass dbClass;
        private void ModelPreset_OnLoaded(object sender, RoutedEventArgs e)
        {
            ModelPresetList.ItemsSource = DataBrige.ModelPresetInfos;

            string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";

            string dbName = "Address_database.db";


            dbFilePath = dbFilePath + dbName;



            dbClass = new DbClass(dbFilePath);
            dbClass.OpenConnection();

            dbClass.CreateModelPresetIfNotExists("ModelPreset"); //检查表单是否创建


            LoadModelPreset(dbClass.connection);

        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {


            AddPreset addPreset = new AddPreset();

            if (addPreset.ShowDialog() == true)
            {

                // 当子窗口关闭后执行这里的代码
                LoadModelPreset(dbClass.connection);

            }

        }

        /// <summary>
        /// 加载设备信息列表
        /// </summary>
        /// <param name="connection"></param>
        public void LoadModelPreset(SQLiteConnection connection)
        {
            DataBrige.ModelPresetInfos.Clear();
            ModelPresetList.ItemsSource = null;
            try
            {
                string query = "SELECT * FROM ModelPreSet";

                SQLiteCommand command = new SQLiteCommand(query, connection);
                SQLiteDataReader reader = command.ExecuteReader();

                int i = 0;



                while (reader.Read())
                {
                    i++;
                    // 读取数据行中的每一列

                    int id = Convert.ToInt32(reader["Id"].ToString());
                    string modelType = reader["ModelType"].ToString();
                    string brand = reader["Brand"].ToString();
                    string model = reader["Model"].ToString();
                    int ethernet = Convert.ToInt32(reader["Ethernet"].ToString());
                    int fiber = Convert.ToInt32(reader["Fiber"].ToString());
                    int disk = Convert.ToInt32(reader["Disk"].ToString());
                    int Manage = Convert.ToInt32(reader["Manage"].ToString());
                    DataBrige.ModelPresetInfos.Add(new ModelPresetInfo(id, modelType, brand, model, ethernet, fiber, disk, Manage));


                }

                ModelPresetList.ItemsSource = DataBrige.ModelPresetInfos;

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }



        private void ModelPresetList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModelPresetList.SelectedIndex != -1)
            {
                MinusButton.IsEnabled = true;
            }
        }


        private void MinusButton_OnClick(object sender, RoutedEventArgs e)
        {
            string type = DataBrige.ModelPresetInfos[ModelPresetList.SelectedIndex].ModelType;
            string brand= DataBrige.ModelPresetInfos[ModelPresetList.SelectedIndex].Brand;
            string model = DataBrige.ModelPresetInfos[ModelPresetList.SelectedIndex].Model;

            string sql = $"DELETE FROM \"ModelPreset\" WHERE ModelType = '{type}' AND Brand = '{brand}' AND Model = '{model}'";

            dbClass.ExecuteQuery(sql);

        }
    }
}
