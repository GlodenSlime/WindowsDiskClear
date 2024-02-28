using System;

namespace WindowsDiskClear.Model
{
    //执行进度模型
    public class ProgressModel
    {
        public string Name { set; get; }//盘名称
        public string FilePath { set; get; }//文件路径
        public long Total { set; get; }//总空间
        public long Remain { set; get; }//剩余空间
        public bool IsOk { set; get; } = false;//是否完成清理

        public string CalculateProgress()
        {
            if (Total == 0)
            {
                return "100%";
            }

            float tmp = 1 - Remain * 1.0f / Total;

            return $"{(Math.Round(tmp, 4) * 100).ToString("00.00")}%";
        }
    }
}
