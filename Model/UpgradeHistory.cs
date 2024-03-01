using System.Collections.Generic;

namespace WindowsDiskClear.Model
{
    //更新记录
    public class UpgradeHistory
    {
        public string Date { set; get; }//日期
        
        public string Version { set; get; }//软件版本

        public List<string> Contents { set; get; }//更新内容
    }
}
