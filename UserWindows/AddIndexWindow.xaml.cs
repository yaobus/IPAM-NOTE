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




            //判断是编辑还是新建标签
            if (DataBrige.EditMode == 1) //编辑模式
            {
                DeviceBlock.Text = DataBrige.SelectDeviceInfo.Name + " " + DataBrige.SelectDeviceInfo.Number;
                ModelTextBlock.Text = "型号:" + DataBrige.SelectDeviceInfo.Model;
                DescriptionBlock.Text = "备注:" + DataBrige.SelectDeviceInfo.Description;

                TypeBlock.Text = "当前选中导航标签:" + DataBrige.SelectIndexTag.ToString();

                string sql =
                    $"SELECT * FROM {DataBrige.SelectDeviceInfo.TableName} WHERE PortType='I' AND PortNumber ={DataBrige.SelectIndexTag}";

                SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);
                SQLiteDataReader reader = command.ExecuteReader();

                ViewMode.DevicePortInfo info = null;

                while (reader.Read())
                {
                    string portType = reader["PortType"].ToString();
                    string portNumber = reader["PortNumber"].ToString();
                    int portStatus = Convert.ToInt32(reader["PortStatus"]);
                    string portTag1 = reader["PortTag1"].ToString();
                    string portTag2 = reader["PortTag2"].ToString();
                    string portTag3 = reader["PortTag3"].ToString();
                    string description = reader["Description"].ToString();


                    NameBox.Text = portTag1;
                    UrlBox.Text = portTag2;
                    DescriptionBox.Text = description;

                    if (portStatus == 0)
                    {
                        EnableBox.IsChecked = false;
                        
                    }
                    else
                    {
                        EnableBox.IsChecked = true;
                    }


                }
            }
            else
            {


                int num = CalculateIndexId(DataBrige.SelectDeviceInfo.TableName);

                TypeBlock.Text = "当前新建快速访问标签第" + (num) + "个";
            }


        }


        /// <summary>
        /// 保存导航标签配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


            if (NameBox.Text != "" && UrlBox.Text != "")
            {
                string sql;

                if (DataBrige.EditMode == 0)//新建
                {
                    //计算已有导航标签数量

                    string sqlTemp = string.Format($"SELECT COUNT(*) FROM {tableName} WHERE `PortType` = 'I'");


                    int num = CalculateIndexId(tableName);

                    Console.WriteLine("num:"+num);

                    sql = $"INSERT INTO \"{tableName}\" (\"PortType\", \"PortNumber\", \"PortStatus\", \"PortTag1\", \"PortTag2\", \"Description\") VALUES ('I', '{num}',{enable}, '{NameBox.Text}', '{UrlBox.Text}', '{DescriptionBox.Text}')";

                    Console.WriteLine(sql);
                }
                else
                {

                     sql = $"UPDATE {tableName} SET \"PortStatus\" = '{enable}', \"PortTag1\" = '{NameBox.Text}'  , \"PortTag2\" = '{UrlBox.Text}' , \"PortTag3\" = '', \"Description\" = '{DescriptionBox.Text}' WHERE (PortType = 'I' AND PortNumber = '{DataBrige.SelectIndexTag}')";

                }


                dbClass.ExecuteQuery(sql); //写入快速访问标签信息

                //DataBrige.DevicePortInfos.Add(new ViewMode.DevicePortInfo("I",(num+1).ToString(),enable,NameBox.Text,UrlBox.Text,"",DescriptionBox.Text));
               
                
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("请完整填写标签信息", "必要信息不完整", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            
        }


        /// <summary>
        /// 
        /// </summary>
        private int CalculateIndexId(string tableName)
        {
            //计算已有导航标签数量

            string sqlTemp = string.Format($"SELECT PortNumber FROM {tableName} WHERE PortType ='I'");
            SQLiteCommand command = new SQLiteCommand(sqlTemp,dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            List<int> idList = new List<int>();

            while (reader.Read())
            {
                idList.Add(Convert.ToInt32(reader["PortNumber"]));
            }


            return FindMissingNumber(idList);
        }


        /// <summary>
        /// 寻找遗失的数值
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static int FindMissingNumber(List<int> numbers)
        {
            if (numbers == null || numbers.Count == 0)
            {
               Console.WriteLine("List cannot be null or empty");
            
            }
            else
            {
                int max = numbers.Max();

                for (int i = 1; i <= max; i++)
                {
                    if (!numbers.Contains(i))
                    {
                        return i;
                        
                    }
                }

            }

           
            return numbers.Count + 1;
        }


        /// <summary>
        /// 自动添加http://
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UrlBox_OnMouseLeave(object sender, MouseEventArgs e)
        {
            UrlBox.Text = AddHttpIfNeeded(UrlBox.Text);
        }

        static string AddHttpIfNeeded(string input)
        {
            if (!input.Contains("https://") && !input.Contains("http://"))
            {
                // 如果字符串中不存在 "https://" 或 "http://", 添加 "https://"
                input = "http://" + input;
            }
            return input;
        }


        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReleaseButton_OnClick(object sender, RoutedEventArgs e)
        {
            string sql = $"DELETE FROM {DataBrige.SelectDeviceTableName} WHERE (PortType = 'I' AND PortNumber = {DataBrige.SelectIndexTag})";
            dbClass.ExecuteQuery(sql); //写入快速访问标签信息
            this.DialogResult=true;
            this.Close();
        }
    }
}
