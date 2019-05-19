using JiebaNet.Segmenter;
using JiebaNet.Segmenter.Common;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace Trac_DailyReportAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            var names = Settings.Names;
            var nameArr = names.Split(',');
            if (nameArr.Length > 0)
            {
                foreach (var item in nameArr)
                {
                    Console.WriteLine($"{item} 开始爬取日志");
                    var html = GetDailyReport(item);
                    Console.WriteLine($"{item} 爬取日志结束");
                    var mostCommon = GetMostCommon(html);
                    html = html + mostCommon;

                    WriteFile(html,item);
                    Console.WriteLine($"{item} 分析日志结束");
                }
            }
            else
            {
                Console.WriteLine("请配置Name");
            }
        }

        

        static string GetDailyReport(string name)
        {
            var beginDate = DateTime.Parse(Settings.BeginDate);
            var endDate = String.IsNullOrEmpty(Settings.EndData)? 
                DateTime.Now : DateTime.Parse(Settings.EndData);
            var tracDailyReportUrl = Settings.TracDailyReportUrl;
            var credentials = GetCredentials(tracDailyReportUrl);
            var certificate = GetX509Certificate();
            var dailyReports = $"<h1>{name} {beginDate.ToShortDateString()} - {endDate.ToShortDateString()} 的日志</h1>";

            for (var date = beginDate; date <= endDate; date = date.AddDays(1))
            {
                var url = $"{tracDailyReportUrl}{name}/{date.ToString("yyyyMMdd")}";
                var request = WebRequest.CreateHttp(url);

                if (credentials != null)
                {
                    request.Credentials = credentials;
                }

                if (certificate != null)
                {
                    request.ClientCertificates.Add(certificate);
                }

                var response = request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                var html = reader.ReadToEnd().Replace("\n","");

                if(Regex.IsMatch(html, "<strong>今日工作任务</strong>：</p>(.*)</div>\\s*<div class=\"" + "trac-modifiedby\"" + ">"))
                {
                    dailyReports = dailyReports + "\n" + date.ToShortDateString() + "\n" + Regex.Match(html, "<strong>今日工作任务</strong>：</p>(.*)</div>\\s*<div class=\"" + "trac-modifiedby\"" + ">").Groups[1].Value;
                    Console.WriteLine($"{name} {date.Date} 写了日志 ");

                }
                else
                {
                    Console.WriteLine($"{name} {date.Date} 没有写日志 ");
                }
            }
            return dailyReports;
        }

        static string GetMostCommon(string html)
        {
            var result = "<h2>词频统计</h2>";
            var seg = new JiebaSegmenter();
            var freqs = new Counter<string>(seg.Cut(html));

            foreach (var item in freqs.MostCommon(100))
            {
                result = result + $"{item.Key}:{item.Value} <br/>";
            }

            return result;
        }

        static void WriteFile(string html,string name)
        {
            var fs = new FileStream($@"{name}.html", FileMode.OpenOrCreate, FileAccess.Write);
            var streamWriter = new StreamWriter(fs);

            streamWriter.BaseStream.Seek(0, SeekOrigin.End);
            streamWriter.WriteLine(html);
            streamWriter.Flush();
            streamWriter.Close();
        }

        static CredentialCache GetCredentials(string tracDailyReportUrl)
        {
            var userName = Settings.UserName;
            var userPassword = Settings.UserPassword;
            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(userPassword))
            {
                return null;
            }
            else
            {

                var cache = new CredentialCache();
                cache.Add(new Uri(tracDailyReportUrl), "Basic", new NetworkCredential(userName, userPassword));
                return cache;
            }
        }

        static X509Certificate2 GetX509Certificate()
        {
            var path = Settings.CertificatePath;
            var password = Settings.CertificatePassword;

            if(String.IsNullOrEmpty(path) || String.IsNullOrEmpty(password))
            {
                return null;
            }
            else
            {
                return new X509Certificate2(path, password);
            }
        }
    }
}
