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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace IPAM_NOTE.UserPages
{
    /// <summary>
    /// DatabaseBackup.xaml 的交互逻辑
    /// </summary>
    public partial class DatabaseBackup : UserControl
    {
        public DatabaseBackup()
        {
            InitializeComponent();
        }

        List<ViewMode.BackupInfo> backupList = new List<ViewMode.BackupInfo>();

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

        private void DatabaseBackup_OnLoaded(object sender, RoutedEventArgs e)
        {
            backupList.Clear();

            string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\";

            string backupDirectoryPath = System.IO.Path.Combine(dbFilePath, "Backup");


            string[] backupFiles = Directory.GetFiles(backupDirectoryPath, "*.bak")
                .OrderByDescending(f => File.GetLastWriteTime(f))
                .Take(20)
                .ToArray();


            for (int i = 0; i < backupFiles.Length; i++)
            {
                string fileName = System.IO.Path.GetFileName(backupFiles[i]);
                DateTime backupTime = File.GetLastWriteTime(backupFiles[i]);
                backupList.Add(new ViewMode.BackupInfo
                {
                    Index = i + 1,
                    FileName = fileName,
                    BackupTime = backupTime.ToString("yyyy年M月d日HH:mm")
                });
            }

           
            BackupListView.ItemsSource = backupList;
        }



    }
}
