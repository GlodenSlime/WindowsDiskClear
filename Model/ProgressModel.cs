using System;

namespace WindowsDiskClear.Model
{
    //执行进度
    public class ProgressModel
    {
        public string DiskName { set; get; }//盘名称
        public string TmpFilePath { set; get; }//临时文件路径
        public long TotalBytes { set; get; }//总空间
        public long RemainBytes { set; get; }//剩余空间
        public bool Completed { set; get; } = false;//是否完成清理

        public string CalculateProgress()
        {
            if (TotalBytes == 0)
            {
                return "100%";
            }

            float tmp = 1 - RemainBytes * 1.0f / TotalBytes;

            return $"{Math.Round(tmp, 4) * 100:00.00}%";
        }
    }
}
