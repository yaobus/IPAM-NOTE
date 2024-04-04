using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Path = System.IO.Path;

namespace IPAM_NOTE
{
	/// <summary>
	/// AboutWindow.xaml 的交互逻辑
	/// </summary>
	public partial class AboutWindow : Window
	{
		public AboutWindow()
		{
			InitializeComponent();
		}

		private void AboutWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			backupList.Clear();

			string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";

			string backupDirectoryPath = Path.Combine(dbFilePath, "Backup");


			string[] backupFiles = Directory.GetFiles(backupDirectoryPath, "*.bak")
				.OrderByDescending(f => File.GetLastWriteTime(f))
				.Take(20)
				.ToArray();


			for (int i = 0; i < backupFiles.Length; i++)
			{
				string fileName = Path.GetFileName(backupFiles[i]);
				DateTime backupTime = File.GetLastWriteTime(backupFiles[i]);
				backupList.Add(new ViewMode.BackupInfo
					{ Index = i + 1, FileName = fileName, BackupTime = backupTime.ToString("yyyy年M月d日HH:mm") });
			}

			VerCheck();	//版本检查
			BackupListView.ItemsSource = backupList;
		}


		/// <summary>
		/// 版本对比
		/// </summary>
		private void VerCheck()
		{
			NowVer.Text = "版本号:V" +Convert.ToString( DataBrige.Ver) + "-Beta";


			if (DataBrige.LatestVersion != 0)
			{
				NewVerPlan.Visibility= Visibility.Visible;
				NewVer.Text = "最新版:V" + Convert.ToString(DataBrige.LatestVersion) + "-Beta";
				NewVer.Foreground = Brushes.DarkOrange;
			}
			else
			{
				NewVerPlan.Visibility = Visibility.Collapsed;
			}



		}

		private void NowVer_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (DataBrige.DownloadUrl!="")
			{
				Process.Start(DataBrige.DownloadUrl);
			}

			
		}




		private void IpamNote_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			Process.Start("https://github.com/yaobus/IPAM-NOTE.git");
		}


		List<ViewMode.BackupInfo> backupList = new List<ViewMode.BackupInfo>();


		private void AboutTabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (AboutTabControl.SelectedIndex == 5)
			{



			}
			else
			{
				//RestoreBackup.IsEnabled = false;
			}
		}







		private void BackupListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			Console.WriteLine(BackupListView.SelectedIndex);

			if (BackupListView.SelectedIndex != -1)
			{
				string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\Backup";

				string fileName = backupList[BackupListView.SelectedIndex].FileName;

				BakPath.Text = Path.Combine(dbFilePath, fileName);

				RestoreBackup.IsEnabled = true;


			}
			else
			{
				RestoreBackup.IsEnabled = false;
			}



		}



		/// <summary>
		/// 回滚备份
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RestoreBackup_OnClick(object sender, RoutedEventArgs e)
		{

			string destinationFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\Address_database.db";

			string backupFilePath = BakPath.Text;

			RestoreDataBasseBackup(backupFilePath, destinationFilePath);


		}

		private void RestoreDataBasseBackup(string backupFilePath, string destinationFilePath)
		{
			try
			{
				File.Copy(backupFilePath, destinationFilePath, true); // 复制备份文件到目标位置并覆盖源文件
				MessageBox.Show("数据库恢复完成，程序即将重启", "注意", MessageBoxButton.OK, MessageBoxImage.Information);

				RestartApplication();
			}
			catch (Exception ex)
			{
				Console.WriteLine("文件恢复失败：" + ex.Message);
			}
		}

		private void RestartApplication()
		{
			// 获取当前应用程序的可执行文件路径
			string appPath = Process.GetCurrentProcess().MainModule.FileName;

			// 启动另一个实例你的软件
			Process.Start(appPath);

			// 关闭当前实例
			Environment.Exit(0);
		}


		private void OpenFolder(string folderPath)
		{
			try
			{
				Process.Start(folderPath);
			}
			catch (Exception ex)
			{
				Console.WriteLine("无法打开文件夹：" + ex.Message);
			}
		}

		private void BackupFile_OnClick(object sender, RoutedEventArgs e)
		{
			string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\Backup";
			OpenFolder(dbFilePath);
		}

		private void GithubDownloadButton_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(DataBrige.DownloadUrl);
		}

		private void LanzouDownloadButton_Click(object sender, RoutedEventArgs e)
		{

			MessageBoxResult result = MessageBox.Show("点击确定，将打开蓝奏云下载链接,提取密码为ab7k","蓝奏云下载",MessageBoxButton.OKCancel,MessageBoxImage.Information);
			if (result == MessageBoxResult.OK)
			{
				Process.Start("https://wwt.lanzout.com/b00sfauuj");
			}

		}
	}
}
