using DriveAutoTestService.Database;
using DriveAutoTestService.Logs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveAutoTestService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //string createtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //string r1 = GenerateRandomLong();
            //string str = "insert into matestresult set testcaseid='gggg', testresultid='"+r1;
            //Sql.Execute(str);
            Logger.InitLog("Service Log");
            Logger.Info("Service started");
            CreateHostBuilder(args).Build().Run();
        }

        private static char[] constant = {
        '0','1','2','3','4','5','6','7','8','9',
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'};

        public static string GenerateRandomCode(int Length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(constant.Length);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(constant.Length)]);
            }
            return newRandom.ToString();
        }

        public static string GenerateRandomLong()
        {
            //ffe00b16 - a0d2 - 43a5 - 9215 - 03965f6d77e2
            return GenerateRandomCode(8) + "-" + GenerateRandomCode(4) + "-" + GenerateRandomCode(4) + "-" + GenerateRandomCode(4) + "-" + GenerateRandomCode(12);
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://labnow.snc.siemens.com.cn:3344").UseStartup<Startup>();
                });
    }
}
