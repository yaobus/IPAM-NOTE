using System;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using IPAM_NOTE.DatabaseOperation;
using IPAM_NOTE.PresetWindow;
using static IPAM_NOTE.ViewMode;

namespace IPAM_NOTE.UserPages
{
    /// <summary>
    /// ModelPreset.xaml 的交互逻辑
    /// </summary>
    public partial class PeoplePreset : UserControl
    {
        public PeoplePreset()
        {
            InitializeComponent();
        }
        private DbClass dbClass;
        private void ModelPreset_OnLoaded(object sender, RoutedEventArgs e)
        {
            PeoplePresetList.ItemsSource = DataBrige.ModelPresetInfos;

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
            PeoplePresetList.ItemsSource = null;
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

                    int id = i;
                    string modelType = reader["ModelType"].ToString();
                    string brand = reader["Brand"].ToString();
                    string model = reader["Model"].ToString();
                    int ethernet = Convert.ToInt32(reader["Ethernet"].ToString());
                    int fiber = Convert.ToInt32(reader["Fiber"].ToString());
                    int disk = Convert.ToInt32(reader["Disk"].ToString());
                    int Manage = Convert.ToInt32(reader["Manage"].ToString());
                    DataBrige.ModelPresetInfos.Add(new ModelPresetInfo(id, modelType, brand, model, ethernet, fiber, disk, Manage));


                }

                PeoplePresetList.ItemsSource = DataBrige.ModelPresetInfos;

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }



        private void PeoplePresetList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PeoplePresetList.SelectedIndex != -1)
            {
                MinusButton.IsEnabled = true;
            }
        }


        private void MinusButton_OnClick(object sender, RoutedEventArgs e)
        {
            string type = DataBrige.ModelPresetInfos[PeoplePresetList.SelectedIndex].ModelType;
            string brand= DataBrige.ModelPresetInfos[PeoplePresetList.SelectedIndex].Brand;
            string model = DataBrige.ModelPresetInfos[PeoplePresetList.SelectedIndex].Model;

            string sql = $"DELETE FROM \"ModelPreset\" WHERE ModelType = '{type}' AND Brand = '{brand}' AND Model = '{model}'";

            dbClass.ExecuteQuery(sql);
            LoadModelPreset(dbClass.connection);
        }
    }
}
