using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
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
using IPAM_NOTE.DatabaseOperation;
using MaterialDesignThemes.Wpf;
using static IPAM_NOTE.ViewMode;

namespace IPAM_NOTE.UserPages
{
    /// <summary>
    /// IndexPage.xaml 的交互逻辑
    /// </summary>
    public partial class IndexPage : UserControl
    {
        public IndexPage()
        {
            InitializeComponent();
        }

        private DbClass dbClass;

        private void IndexPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";

            string dbName = "Address_database.db";


            dbFilePath = dbFilePath + dbName;

            dbClass = new DbClass(dbFilePath);
            dbClass.OpenConnection();

            //加载所有的设备名称，表名，型号，编号
            LoadDevicesInfo(dbClass.connection);



            if (DataBrige.DeviceInfos != null)
            {
                int index = 0;

                foreach (var info in DataBrige.DeviceInfos)//查询所有的表
                {
                   

                    string tableName = info.TableName;


                    ////查询每个设备对应的标签
                    string sql = $"SELECT * FROM {tableName} WHERE PortType ='I' and PortStatus = 1";

                    SQLiteCommand command = new SQLiteCommand(sql, dbClass.connection);
                    SQLiteDataReader reader = command.ExecuteReader();

                    

                    int rowCount = 0;

                    while (reader.Read())
                    {
                        rowCount++;
                    }

                   

                    if (rowCount != 0)
                    {
                        index += 1;

						Grid grid = new Grid();


						string name = info.Name;
						string model = info.Model;
						string number = info.Number;
						string note = info.Description;


						// 设置 Grid 行定义
						grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto }); // 最小高度为 20
						grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

						// 设置 Grid 列定义
						grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40) }); // 左边宽度为 40
						grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(260) }); // 左边宽度为 260
						grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // 右边宽度自适应



						//索引
						TextBlock indexTextBlock = new TextBlock();
						indexTextBlock.Text = index.ToString();
						indexTextBlock.Style = (Style)this.FindResource("MaterialDesignHeadline6TextBlock");
						indexTextBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
						indexTextBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
						indexTextBlock.FontWeight = FontWeights.Black;
						Grid.SetColumn(indexTextBlock, 0); // 设置左边 TextBlock 的列位置
						Grid.SetRow(indexTextBlock, 0); // 设置左边 TextBlock 的行位置
						Grid.SetRowSpan(indexTextBlock, 2);


						StackPanel stack = new StackPanel();
						stack.Orientation = Orientation.Horizontal;
						Grid.SetColumn(stack, 1); // 设置左边 TextBlock 的列位置
						Grid.SetRow(stack, 0); // 设置左边 TextBlock 的行位置


						TextBlock nameTextBlock = new TextBlock();
						nameTextBlock.Text = name;
						nameTextBlock.Style = (Style)this.FindResource("MaterialDesignHeadline6TextBlock");
						nameTextBlock.Width = 80;
						nameTextBlock.Margin = new Thickness(0);
						nameTextBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
						nameTextBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;



						TextBlock nameTextBlock2 = new TextBlock();
						nameTextBlock2.Text = number;
						nameTextBlock2.Style = (Style)this.FindResource("MaterialDesignHeadline6TextBlock");
						nameTextBlock2.Width = 220;
						nameTextBlock2.Margin = new Thickness(0);
						nameTextBlock2.VerticalAlignment = System.Windows.VerticalAlignment.Center;
						nameTextBlock2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;




						stack.Children.Add(nameTextBlock);
						stack.Children.Add(nameTextBlock2);
						stack.VerticalAlignment = VerticalAlignment.Bottom;


						StackPanel stack2 = new StackPanel();
						stack2.VerticalAlignment = VerticalAlignment.Top;
						stack2.HorizontalAlignment = HorizontalAlignment.Left;
						stack2.Orientation = Orientation.Horizontal;
						Grid.SetColumn(stack2, 1); // 设置左边 TextBlock 的列位置
						Grid.SetRow(stack2, 1); // 设置左边 TextBlock 的行位置


						//型号信息
						TextBlock modelTextBlock = new TextBlock();
						modelTextBlock.Text = model;
						modelTextBlock.Width = 150;
						modelTextBlock.Style = (Style)this.FindResource("MaterialDesignCaptionTextBlock");
						modelTextBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
						modelTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
						modelTextBlock.Opacity = 0.3;


						//注释信息
						TextBlock noteBlock = new TextBlock();
						noteBlock.Text = note;
						noteBlock.Style = (Style)this.FindResource("MaterialDesignCaptionTextBlock");
						noteBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
						noteBlock.HorizontalAlignment = HorizontalAlignment.Left;
						noteBlock.Opacity = 0.3;


						stack2.Children.Add(modelTextBlock);
						stack2.Children.Add(noteBlock);


						WrapPanel chiPanel = new WrapPanel();

						Grid.SetColumn(chiPanel, 2); // 设置左边 TextBlock 的列位置
						Grid.SetRow(chiPanel, 0); // 设置左边 TextBlock 的行位置
						Grid.SetRowSpan(chiPanel, 2);

						grid.Children.Add(indexTextBlock);//索引

						grid.Children.Add(stack);//名称+设备编号
						grid.Children.Add(stack2);//型号+注释

						grid.Children.Add(chiPanel);//快速访问标签


						////查询每个设备对应的标签
						string sql2 = $"SELECT * FROM {tableName} WHERE PortType ='I' and PortStatus = 1";
                        SQLiteCommand command2 = new SQLiteCommand(sql, dbClass.connection);
						reader = command2.ExecuteReader();


						while (reader.Read())
						{
							string portTag1 = reader["PortTag1"].ToString();
							string portTag2 = reader["PortTag2"].ToString();

							Console.WriteLine(portTag1);

							Button button = new Button();
							button.Height = 60;
							button.Width = 130;
							button.Content = CreatButtonPanel(portTag1, portTag2);
							button.Margin = new Thickness(5);
							button.Click += Button_Click;
							button.Tag = portTag2;
							button.Foreground = (Brush)FindResource("MaterialDesignDarkForeground");
							chiPanel.Children.Add(button);

						}


						Graphics.Children.Add(grid);

						Separator separator = new Separator();
						separator.Width = 10000; // 设置横线的宽度，根据需要调整
						separator.Opacity = 0.3;
						Graphics.Children.Add(separator);
					}

                }
            }









        }
        /// <summary>
        /// 点击按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string url = button.Tag.ToString();

                string pattern = @"\[\d+\]";
                url = Regex.Replace(url, pattern, "");


                if (!url.Contains("https://") && !url.Contains("http://"))
                {
                    // 如果字符串中不存在 "https://" 或 "http://", 添加 "https://"
                    url = "http://" + url;
                }

                Process.Start(url);
            }
        }




        /// <summary>
        /// 创建一个Button的样式
        /// </summary>
        /// <returns></returns>
        private Grid CreatButtonPanel(string name,string url)
        {

            Grid grid = new Grid();
            ColumnDefinition column1 = new ColumnDefinition();
            ColumnDefinition column2 = new ColumnDefinition();
            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();

            column1.Width = new GridLength(20);
            column2.Width = new GridLength(100);

            row1.Height = new GridLength(60, GridUnitType.Star);
            row2.Height = new GridLength(40, GridUnitType.Star);

            grid.ColumnDefinitions.Add(column1);
            grid.ColumnDefinitions.Add(column2);
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            grid.VerticalAlignment = VerticalAlignment.Center;
            

            // 添加PackIcon到第一列
            PackIcon packIcon = new PackIcon();
            packIcon.Kind = PackIconKind.GoogleChrome; // 设置PackIcon的图标类型
            packIcon.Margin = new Thickness(0);
            packIcon.VerticalAlignment = VerticalAlignment.Center;

            Grid.SetColumn(packIcon, 0);
            Grid.SetRow(packIcon, 0);
            Grid.SetRowSpan(packIcon, 2);

            TextBlock textBlock1 = new TextBlock();
            textBlock1.Text = name;
            textBlock1.FontWeight = FontWeights.Bold;
            textBlock1.FontSize = 16;
            textBlock1.Foreground= (Brush)FindResource("MaterialDesignDarkForeground");

            TextBlock textBlock2 = new TextBlock();
            textBlock2.Text = url;
            textBlock2.FontWeight = FontWeights.Bold;
            textBlock2.FontSize = 12;
            textBlock2.Opacity = 0.5;
            textBlock2.Foreground = (Brush)FindResource("MaterialDesignDarkForeground");

            Grid.SetColumn(textBlock1, 1);
            Grid.SetRow(textBlock1, 0);

            Grid.SetColumn(textBlock2, 1);
            Grid.SetRow(textBlock2, 1);


            grid.Children.Add(packIcon);
            grid.Children.Add(textBlock1);
            grid.Children.Add(textBlock2);


            return grid;

        }




        /// <summary>
        /// 加载设备信息列表
        /// </summary>
        /// <param name="connection"></param>
        public void LoadDevicesInfo(SQLiteConnection connection)
        {
            DataBrige.DeviceInfos.Clear();
            Graphics.Children.Clear(); //
            try
            {
                string query = "SELECT * FROM Devices";

                SQLiteCommand command = new SQLiteCommand(query, connection);
                SQLiteDataReader reader = command.ExecuteReader();

                int i = 0;


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


                }


                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
