using System;
using System.Collections.Generic;
using System.Data;
using System.IO;                        // 添加IO写入写出
using System.Linq;
using System.Text.RegularExpressions;   // 添加正则引用
using CsvHelper;

namespace CsSpider
{
    class Program
    {
        private static string _filename = @"./豆瓣音乐数据导出Sample.csv";
        private static List<Record> _records = new List<Record>();

        static void Main(string[] args)
        {
            if (File.Exists(_filename))
            {
                File.Delete(_filename);
            }

            var webclient = new System.Net.WebClient();
            int totalPageCount = GetTotalCount(webclient, 0);

            for (int i = 0; i < totalPageCount; i++)
            {
                GetCurrentPageRecords(webclient, 20 * i);
                Console.WriteLine($"完成第{i + 1}页tag信息采集.");
            }

            if (_records.Count > 0)
            {
                var writer = new StreamWriter(_filename, false, System.Text.Encoding.UTF8);  // 设置Encoding，防止乱码

                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteRecords(_records);
                    Console.WriteLine("导出完成.");
                }
            }
            else Console.WriteLine("未登录，数据抓取失败.");

        }

        private static int GetTotalCount(System.Net.WebClient webclient, int startIdx)
        {
            //html下载 
            /* https://music.douban.com/tag/%E6%91%87%E6%BB%9A?start=0&type=T  */

            byte[] buffer = webclient.DownloadData("https://music.douban.com/tag/%E6%91%87%E6%BB%9A?start=" + startIdx + "&type=T");
            // utf-8, gb2312, gbk, utf-1...... 
            string html = System.Text.Encoding.GetEncoding("utf-8").GetString(buffer);

            MatchCollection pageCount_matches = new Regex(">[0-9][0-9]{0,}</a>").Matches(html);

            if (pageCount_matches.Count == 0)
                return 0;

            string tempPageNum = pageCount_matches[pageCount_matches.Count - 1].Value;
            int.TryParse(tempPageNum.Replace("</a>", string.Empty).Substring(1), out int lastPageNum);

            return lastPageNum;
        }

        private static void GetCurrentPageRecords(System.Net.WebClient webclient, int startIdx) 
        {
            byte[] buffer = webclient.DownloadData("https://music.douban.com/tag/%E6%91%87%E6%BB%9A?start=" + startIdx + "&type=T");
            // utf-8, gb2312, gbk, utf-1...... 
            string html = System.Text.Encoding.GetEncoding("utf-8").GetString(buffer);

            // Console.WriteLine(html);

            // html分析 
            // 通过正则获取到需要的数据
            // <p class="pl">刺猬 / 2018-01-08 / 单曲 / 数字(Digital) / 摇滚</p>
            MatchCollection musicItem_matches = new Regex("<p class=\"pl\">([\\s\\S]*?)</p>").Matches(html);

            int count = musicItem_matches.Count;

            for (int i = 0; i < count; i++)
            {
                string item = musicItem_matches[i].Result("$1");
                string[] strArr = item.Trim().Split('/');

                Record record = new Record
                {
                    SongName = strArr[0],
                    Date = strArr.ElementAtOrDefault(1),
                    Album = strArr.ElementAtOrDefault(2),
                    Type = strArr.ElementAtOrDefault(3),
                    Topic = strArr.ElementAtOrDefault(4)
                };

                _records.Add(record);
            }
        }
    }
}
