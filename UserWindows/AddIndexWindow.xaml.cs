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
using System.Windows.Shapes;
using IPAM_NOTE.DatabaseOperation;

namespace IPAM_NOTE.UserWindows
{
    /// <summary>
    /// AddIndexWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddIndexWindow : Window
    {
        public AddIndexWindow()
        {
            InitializeComponent();
        }
       private DbClass dbClass;

        private void AddIndexWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";
            string dbName = "Address_database.db";

            dbFilePath = dbFilePath + dbName;

            dbClass = new DbClass(dbFilePath);

            dbClass.OpenConnection();


            DeviceBlock.Text = DataBrige.SelectDeviceInfo.Name + " " + DataBrige.SelectDeviceInfo.Number;
            ModelTextBlock.Text = "型号:" + DataBrige.SelectDeviceInfo.Model;
            DescriptionBlock.Text = "备注:" + DataBrige.SelectDeviceInfo.Description;

        }


        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
