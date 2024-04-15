using ControlzEx.Standard;
using IPAM_NOTE.DatabaseOperation;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IPAM_NOTE.UserPages;
using static IPAM_NOTE.ViewMode;
using Button = System.Windows.Controls.Button;
using GridView = System.Windows.Controls.GridView;
using ListView = System.Windows.Controls.ListView;
using Path = System.IO.Path;
using Style = System.Windows.Style;

namespace IPAM_NOTE
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			WindowsShow();
		}


		private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			try
			{
				CheckVer();
			}
			catch (Exception exception)
			{
				Console.WriteLine("无法查询新版本!");
			}

            string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";

            //创建数据库备份
            CreatBackup(dbFilePath);

            BottomControl.SelectedIndex = -1;

            //加载初始页面
            FunctionPanel.Children.Clear();
            NetworkManage networkManage = new NetworkManage();
            networkManage.Style = (Style)FindResource("UserControlStyle");
            FunctionPanel.Children.Add(networkManage);

            TopControl.SelectedIndex = 0;

        }


		private async void CheckVer()
		{

			var versionChecker = new GitHubVersionChecker("yaobus", "IPAM-NOTE");
			string latestVersion = await versionChecker.GetLatestVersionAsync();
			

			

			// 定义匹配版本号的正则表达式
			Regex regex = new Regex(@"\d+\.\d+");

            Match match;

            try
            {
                // 在输入字符串中搜索匹配的版本号
                 match = regex.Match(latestVersion);

            }
            catch (Exception e)
            {
               
                match = Match.Empty;
            }



			// 如果找到匹配的版本号，则输出
			if (match.Success)
			{
				double version = Convert.ToDouble( match.Value);

				if (version > Convert.ToDouble(DataBrige.Ver))
				{

                    //发现新版本则闪烁提示
                    ButtonProgressAssist.SetIsIndeterminate(AboutButton, true);
                    AboutButton.ToolTip = "发现新版本:" + latestVersion;

                    DataBrige.LatestVersion = version.ToString();


                    //获取下载地址
                    DataBrige.DownloadUrl = await versionChecker.GetDownloadUrlAsync();
				}
				else
				{
					ButtonProgressAssist.SetIsIndeterminate(AboutButton, false);
					

					//清空下载地址
					DataBrige.DownloadUrl = "";
				}

                DataBrige.LatestVersion = version.ToString();
            }


            DataBrige.UpdateInfos = await versionChecker.GetReleaseBodyAsync();

        }


		/// <summary>
		/// 设置窗口显示方式，分辨率小于1080P就全屏显示，否则居中显示
		/// </summary>
		private void WindowsShow()
		{

			// 获取屏幕的宽度和高度
			double screenWidth = SystemParameters.PrimaryScreenWidth;
			double screenHeight = SystemParameters.PrimaryScreenHeight;

			// 设定最大分辨率
			double maxResolutionWidth = 1920;
			double maxResolutionHeight = 1080;

			// 如果分辨率小于等于1920x1080，则最大化窗口
			if (screenWidth <= maxResolutionWidth && screenHeight <= maxResolutionHeight)
			{
				WindowState = WindowState.Maximized;
			}
			else // 否则，居中显示窗口
			{
				WindowStartupLocation = WindowStartupLocation.CenterScreen;
			}
		}




		private void CreatBackup(string sourceFilePath)
		{

			string dbFileName = "Address_database.db";
			string dbFilePath = Path.Combine(sourceFilePath, dbFileName);


			// 如果源文件不存在，则无需备份
			if (!File.Exists(dbFilePath))
			{
				Console.WriteLine("源文件不存在，无法备份。");
				return;
			}

			string backupDirectoryPath = Path.Combine(sourceFilePath, "Backup");

			// 创建备份目录（如果不存在）
			if (!Directory.Exists(backupDirectoryPath))
			{
				Directory.CreateDirectory(backupDirectoryPath);
			}

			// 获取备份文件夹内的所有备份文件
			string[] backupFiles = Directory.GetFiles(backupDirectoryPath, "*.bak");

			// 如果存在备份文件，则选择最新的一个进行比较
			if (backupFiles.Length > 0)
			{
				string latestBackupFilePath = backupFiles.OrderByDescending(f => File.GetLastWriteTime(f)).First();

				// 如果最新备份文件与源文件相同，则无需备份
				if (FilesAreIdentical(dbFilePath, latestBackupFilePath))
				{
					Console.WriteLine("最新备份和源文件相同，无需备份。");
					return;
				}
			}


			// 构建备份文件名
			string backupFileName = $"Address_database.db_{DateTime.Now:yyyy年M月d日HH.mm}.bak";
			string backupFilePath = Path.Combine(backupDirectoryPath, backupFileName);


			try
			{
				// 进行备份
				File.Copy(dbFilePath, backupFilePath, true);
				Console.WriteLine("备份已创建：" + backupFilePath);
			}
			catch (DirectoryNotFoundException ex)
			{
				Console.WriteLine("备份目录未找到：" + ex.Message);
			}

		}

		/// <summary>
		/// 检查两个文件是否相同
		/// </summary>
		/// <param name="file1"></param>
		/// <param name="file2"></param>
		/// <returns></returns>
		private bool FilesAreIdentical(string file1, string file2)
		{
			using (var hashAlgorithm = SHA256.Create())
			{

				using (var stream1 = File.OpenRead(file1))
				using (var stream2 = File.OpenRead(file2))
				{
					var hash1 = hashAlgorithm.ComputeHash(stream1);


					var hash2 = hashAlgorithm.ComputeHash(stream2);

					return StructuralComparisons.StructuralEqualityComparer.Equals(hash1, hash2);
				}



			}
		}





        /// <summary>
        /// 加载对应模块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TopControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataBrige. DevicesList.Clear();
            
            DataBrige.GraphicsMode = 0;

            int index = TopControl.SelectedIndex;

            if (index != -1)
            {
                BottomControl.SelectedIndex = -1;

                FunctionPanel.Children.Clear();


                switch (index)
                {
                    case 0://网段管理
                        
                        NetworkManage networkManage = new NetworkManage();

                        networkManage.Style = (Style)FindResource("UserControlStyle");

                        FunctionPanel.Children.Add(networkManage);
                        break;

                    case 1://设备管理
                        DevicesPage devicesPage = new DevicesPage();

                        devicesPage.Style = (Style)FindResource("DevicesStyle");

                        FunctionPanel.Children.Add(devicesPage);

                        break;
                    case 2: 
                        IndexPage indexPage = new IndexPage();


                        indexPage.Style = (Style)FindResource("IndexStyle");
                        FunctionPanel.Children.Add(indexPage);

                        break;

                    case 3:
                        AssetManage asset = new AssetManage();


                        asset.Style = (Style)FindResource("AssetStyle");
                        FunctionPanel.Children.Add(asset);

                        break;
                }

            }






        }

        private void BottomControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           

            int index = BottomControl.SelectedIndex;

            if (index != -1)
            {
                TopControl.SelectedIndex = -1;
                FunctionPanel.Children.Clear();
                switch (index)
                {
                    case 0:


                        break;


                    case 1:

                        DatabaseBackup databaseBackup = new DatabaseBackup();

                        databaseBackup.Style = (Style)FindResource("DatabaseBackupStyle");

                        FunctionPanel.Children.Add(databaseBackup);
                        break;

                    case 2:


                       
                        HelpPage helpPage = new HelpPage();

                        helpPage.Style = (Style)FindResource("HelpStyle");

                        FunctionPanel.Children.Add(helpPage);


                        break;

                    case 3:
                        DonationPage donationPage = new DonationPage();

                        donationPage.Style = (Style)FindResource("DonationStyle");

                        FunctionPanel.Children.Add(donationPage);

                        break;


                    case 4:


                       
                        About aboutPage = new About();

                        aboutPage.Style = (Style)FindResource("AboutPageStyle");

                        FunctionPanel.Children.Add(aboutPage);


                        break;
                }

            }


            

        }

        private void AboutButton_OnClick(object sender, RoutedEventArgs e)
        {
            FunctionPanel.Children.Clear();
            TopControl.SelectedIndex = -1;

            About aboutPage = new About();

            aboutPage.Style = (Style)FindResource("AboutPageStyle");

            FunctionPanel.Children.Add(aboutPage);

        }



		/// <summary>
		/// 内测邀请
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void HumanGreeting_OnClick(object sender, RoutedEventArgs e)
        {
			MessageBox.Show("如果你希望参与到本软件的成长过程\r可以加入BUG反馈企鹅群957648723", "哦？", MessageBoxButton.YesNo,
				MessageBoxImage.Information);
		}
	}
}
