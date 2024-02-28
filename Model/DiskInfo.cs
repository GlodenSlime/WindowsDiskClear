using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace WindowsDiskClear.Model
{
    //硬盘信息模型
    public class DiskInfo : INotifyPropertyChanged
    {
        public DriveInfo DriveInfo { get; set; }
        public string Name { get; set; }

        private string _desc;
        public string Desc
        {
            get
            {
                return _desc;
            }
            set
            {
                _desc = value;
                OnPropertyChanged();
            }
        }

        public void RefreshDesc()
        {
            Desc = "类型: " + DriveInfo.DriveType + " 空闲:" + (Math.Round(DriveInfo.AvailableFreeSpace / 1024f / 1024f / 1024f, 1)) + " GB";
        }

        private bool _isSelected;
        public bool IsSelected
        {
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
            get
            {
                return _isSelected;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
