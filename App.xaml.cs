using System.Collections.Generic;
using System.Windows;
using WindowsDiskClear.Model;

namespace WindowsDiskClear
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public const string APP_VERSION = "1.0.2";

        public static List<UpgradeHistory> updateHistories = new List<UpgradeHistory>();

        static App()
        {
            updateHistories.Add(new UpgradeHistory()
            {
                Date = "20240228",
                Version = "1.0.2",
                Contents = new List<string>()
                {
                     "实时更新显示硬盘剩余空间",
                     "默认勾选完成后清理临时文件",
                     "UI界面优化",
                }
            });

            updateHistories.Add(new UpgradeHistory()
            {
                Date = "20240227",
                Version = "1.0.1",
                Contents = new List<string>()
                {
                     "UI界面优化",
                     "支持完成后是否自动清理文件选项",
                     "支持完成后是否自动关机选项",
                     "增加版本更新信息显示",
                }
            });

            updateHistories.Add(new UpgradeHistory()
            {
                Date = "20240226",
                Version = "1.0.0",
                Contents = new List<string>()
                {
                     "软件第一次发布"
                }
            });
        }
    }
}
