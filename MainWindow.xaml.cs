using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WindowsDiskClear.Model;

namespace WindowsDiskClear
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<DiskInfo> _data;

        public List<DiskInfo> Data
        {
            get
            {
                if (_data == null)
                {
                    _data = new List<DiskInfo>();
                }
                return _data;
            }
            set
            {
                _data = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            Title = Title.Replace("_VERSION_", App.APP_VERSION);

            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo driveInfo in allDrives)
            {
                if (driveInfo.DriveType != DriveType.Fixed)
                {
                    continue;
                }
                //Console.WriteLine("Drive {0}", driveInfo.Name);
                //Console.WriteLine("Drive type: {0}", driveInfo.DriveType);
                //if (driveInfo.IsReady == true)
                //{
                //    Console.WriteLine("  Volume label: {0}", driveInfo.VolumeLabel);
                //    Console.WriteLine("  File system: {0}", driveInfo.DriveFormat);
                //    Console.WriteLine("  Total size: {0} bytes", driveInfo.TotalSize);
                //    Console.WriteLine("  Available space: {0} bytes", driveInfo.AvailableFreeSpace);
                //}

                Data.Add(new DiskInfo()
                {
                    DriveInfo = driveInfo,
                    Name = (driveInfo.Name.ElementAt(0) + "").ToUpper(),
                    IsSelected = false
                });
            }

            DataContext = this;

            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(500);
                    foreach (DiskInfo diskInfo in Data)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            diskInfo.RefreshDesc();
                        });
                    }
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        bool isRunning = false;

        private void StartClear(object sender, RoutedEventArgs e)
        {
            if (Data.Where(data => data.IsSelected).Count() == 0)
            {
                MessageBox.Show("请先选择磁盘");
                return;
            }

            if (isRunning)
            {
                MessageBox.Show("请不要重复执行");
                return;
            }

            isRunning = true;
            List<ProgressModel> progresses = new List<ProgressModel>();
            int index = 0;
            foreach (DiskInfo diskInfo in Data)
            {
                if (diskInfo.IsSelected)
                {
                    DriveInfo drive = new DriveInfo(diskInfo.Name);
                    if (!drive.IsReady)
                    {
                        MessageBox.Show(diskInfo.Name + " 盘未就绪。");
                        continue;
                    }

                    long freeSpace = drive.AvailableFreeSpace;
                    if (freeSpace == 0)
                    {
                        MessageBox.Show(diskInfo.Name + " 盘空间已满。");
                        continue;
                    }

                    ProgressModel progressModel = new ProgressModel()
                    {
                        Name = diskInfo.Name,
                        Total = freeSpace,
                        Remain = freeSpace,
                        IsOk = false
                    };
                    progresses.Add(progressModel);

                    int dataIndex = index;
                    Task.Run(() =>
                    {
                        byte[] buffer = new byte[4096]; // 每次写入4KB

                        Random random = new Random();

                        string fileName = "random." + random.Next(1000, 9999) + ".bin";
                        string filePath = Path.Combine(diskInfo.Name.ElementAt(0).ToString() + ":\\", fileName);

                        if (diskInfo.Name.ToUpper().StartsWith("C"))
                        {
                            filePath = Path.Combine(
                                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                                , fileName
                            );
                        }

                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        progressModel.FilePath = filePath;

                        try
                        {
                            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                            {
                                while (freeSpace > 0 && isRunning)
                                {
                                    random.NextBytes(buffer);//随机数据

                                    fileStream.Write(buffer, 0, buffer.Length);
                                    fileStream.Flush();

                                    freeSpace -= buffer.Length;
                                    progressModel.Remain = freeSpace;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        finally
                        {
                            if (isAutoDeleteFile)//自动删除文件
                            {
                                if (File.Exists(progressModel.FilePath))
                                {
                                    File.Delete(progressModel.FilePath);
                                }
                            }
                        }
                        progressModel.IsOk = isRunning;
                    });

                    index += 1;
                }
            }

            Task.Run(() =>
            {
                DateTime start = DateTime.Now;
                string infoText = "";
                while (isRunning)
                {
                    Thread.Sleep(300);

                    infoText = "";

                    for (int i = 0; i < progresses.Count; i++)
                    {
                        infoText += $"{progresses[i].Name}盘进度: " + progresses[i].CalculateProgress() + $" 临时文件: {progresses[i].FilePath}\n";
                    }

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        info.Text = infoText;
                    });

                    if (progresses.Where(p => p.IsOk).Count() == progresses.Count)
                    {
                        //完成
                        break;
                    }
                }

                if (isRunning)//不是手动停止的情况下
                {
                    App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        if (isAutoShutdown)
                        {
                            //30秒后自动关机
                            Process.Start("shutdown", "/s /t 30");
                        }

                        info.Text = "恭喜全部清理完成!";
                        MessageBox.Show("恭喜全部清理完成!");
                    });
                }

                isRunning = false;
            });
        }

        private void StopClear(object sender, RoutedEventArgs e)
        {
            isRunning = false;

            MessageBox.Show("停止清理");
        }

        private void ViewUpdateHistory(object sender, RoutedEventArgs e)
        {
            string info = "";
            foreach (UpdateHistory updateHistory in App.updateHistories)
            {
                info += updateHistory.Date + " v" + updateHistory.Version + " \n";
                for (int i = 0; i < updateHistory.Contents.Count; i++)
                {
                    info += $"{i + 1}. {updateHistory.Contents[i]}\n";
                }
                info += "\n";
            }

            MessageBox.Show(info);
        }

        bool isAutoDeleteFile = true;
        private void OnAutoDeleteFileClick(object sender, RoutedEventArgs e)
        {
            isAutoDeleteFile = cb_auto_delete_file.IsChecked == true;
        }
        bool isAutoShutdown = false;

        private void OnAutoShutdownClick(object sender, RoutedEventArgs e)
        {
            isAutoShutdown = cb_auto_shutdown.IsChecked == true;
        }
    }
}
