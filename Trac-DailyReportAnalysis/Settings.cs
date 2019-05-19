using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trac_DailyReportAnalysis
{
    public static class Settings
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public static string Names = ConfigurationManager.AppSettings["Names"];

        /// <summary>
        /// 开始日期
        /// </summary>
        public static string BeginDate = ConfigurationManager.AppSettings["BeginDate"];
        
        /// <summary>
        /// 结束日期
        /// </summary>
        public static string EndData = ConfigurationManager.AppSettings["EndData"];
        
        /// <summary>
        /// 证书地址
        /// </summary>
        public static string CertificatePath = ConfigurationManager.AppSettings["CertificatePath"];

        /// <summary>
        /// 证书密码
        /// </summary>
        public static string CertificatePassword = ConfigurationManager.AppSettings["CertificatePassword"];
        /// <summary>
        /// Trac 系统登录用户名
        /// </summary>
        public static string UserName = ConfigurationManager.AppSettings["UserName"];

        /// <summary>
        /// Trac 系统登录密码
        /// </summary>
        public static string UserPassword = ConfigurationManager.AppSettings["UserPassword"];

        public static string TracDailyReportUrl = ConfigurationManager.AppSettings["TracDailyReportUrl"];
    }
}
