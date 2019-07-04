using System;
using System.Text;
using System.Threading.Tasks; // 添加表格保存数据
using System.Data;    
using System.Text.RegularExpressions;   // 添加正则引用
using System.IO;                        // 添加IO写入写出
namespace spider_test
{
    class Progran
    {
        static void Main(string[] args)
        {
            //html下载 
            /* https://music.douban.com/tag/%E6%91%87%E6%BB%9A?start=0&type=T  */
            byte[] buffer = null;
            var webclient = new System.Net.WebClient();
            buffer = webclient.DownloadData("https://music.douban.com/tag/%E6%91%87%E6%BB%9A?start=0&type=T");
            // utf-8, gb2312, gbk, utf-1...... 
            string html = System.Text.Encoding.GetEncoding("utf-8").GetString(buffer);

            // Console.WriteLine(html);

            // html分析 
            // 通过正则获取到需要的数据导出到csv

            // <p class="pl">刺猬 / 2018-01-08 / 单曲 / 数字(Digital) / 摇滚</p>
            MatchCollection matches  = new Regex("<p class=\"pl\">([\\s\\S]*?)</p>").Matches(html);

            int count = matches.Count;
            string[] input = new string[count];
            for (int i = 0; i < count; i++)
            {
                input[i] = matches[i].Result("$1");
            }

            DataTable dt = new DataTable();
            DataColumnCollection col = dt.Columns;
            col.Add("tag信息", typeof(string));
            for (int i = 0; i < input.Length; i++)
            {
                DataRow row = dt.NewRow();
                row["tag信息"] = input[i];
                dt.Rows.Add(row);

            }

            // html导出csv

        } 
    }
}
