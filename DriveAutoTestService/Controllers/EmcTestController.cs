using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DriveAutoTestService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class EmcTestController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<string> GetburstequipsStatus(string IPaddress)
        {
            using (HttpClient client = new HttpClient())
            {
                // 发送请求获取仪器状态
                HttpResponseMessage getequipstatus = await client.PostAsync("http://" + IPaddress + ":5005/api/v1/EmcEquipment", null);

                string equipname = "";
                // 检查响应是否成功
                if (getequipstatus.IsSuccessStatusCode)
                {
                    // 读取仪器状态
                    string equipstatus = await getequipstatus.Content.ReadAsStringAsync();
                    Console.WriteLine(equipstatus);
                    if (equipstatus == "Connected")
                    {
                        //发送请求获取仪器名称和耦合仪器名称
                        HttpResponseMessage getequipname = await client.GetAsync("http://" + IPaddress + ":5005/api/v1/EmcEquipment/details");
                        if (getequipname.IsSuccessStatusCode)
                        {
                            //读取响应内容，仪器名称
                            equipname = await getequipname.Content.ReadAsStringAsync();
                            //判断错误情况
                            if (equipname.Contains("IDLE,0004")) { equipname = "TEST ON button on generator is OFF"; }
                            else if (equipname.Contains("IDLE,0008")) { equipname = "TEST ON button on CDN is OFF"; }
                            else if (equipname.Contains("IDLE,000C")) { equipname = "Both TEST ON buttons are off"; }
                            else if (equipname.Contains("3000,8009")) { equipname = "Connect AC mains to EUT mains"; }
                            Console.WriteLine(equipname);
                        }
                        else
                        {
                            equipname = "Error";
                            Console.WriteLine("获取Burst/Surge仪器名称失败： " + getequipname.StatusCode);
                        }
                    }
                }
                else
                {
                    equipname = "Error";
                    Console.WriteLine("连接Burst/Surge仪器失败 " + getequipstatus.StatusCode);
                }
                return equipname;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<string> Sendcontrolcmd(string IPaddress, string LDMDcommand)
        {
            using (HttpClient client = new HttpClient())
            {
                //构造请求内容
                var jsonContent = new { LDMD = LDMDcommand };
                var json = JsonConvert.SerializeObject(jsonContent);

                //发送LDMD指令的请求
                HttpResponseMessage ControlEquip = await client.PostAsync("http://" + IPaddress + ":5005/api/v1/EmcTestCase/execute", new StringContent($"\"{LDMDcommand}\"", Encoding.UTF8, "application/json"));

                if (ControlEquip.IsSuccessStatusCode)
                {
                    //获取控制指令是否开始
                    string ExecuteResult = await ControlEquip.Content.ReadAsStringAsync();
                    Console.WriteLine(ExecuteResult);
                    return ExecuteResult;
                }
                else
                {
                    Console.WriteLine("发送LDMD指令失败： " + ControlEquip.StatusCode);
                    return "Error";
                }
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<string> Sendstartcmd(string IPaddress, string CPLDcommand, string startcommand)
        {
            using (HttpClient client = new HttpClient())
            {
                //构造请求内容
                var jsonContent = new { CPLD = CPLDcommand };
                var json = JsonConvert.SerializeObject(jsonContent);

                //发送CPLD指令的请求
                HttpResponseMessage ControlEquip = await client.PostAsync("http://" + IPaddress + ":5005/api/v1/EmcTestCase/execute", new StringContent($"\"{CPLDcommand}\"", Encoding.UTF8, "application/json"));
                ControlEquip.Content = new StringContent(json, Encoding.UTF8, "application/json");

                if (ControlEquip.IsSuccessStatusCode)
                {
                    //获取控制指令是否开始
                    string ExecuteResult = await ControlEquip.Content.ReadAsStringAsync();
                    Console.WriteLine(ExecuteResult);
                    //开始执行测试
                    using (HttpClient client2 = new HttpClient())
                    {
                        //构造请求内容
                        var jsonContent2 = new { start = startcommand };
                        var json2 = JsonConvert.SerializeObject(jsonContent);

                        //发送开始测试的请求
                        HttpResponseMessage StartTest = await client2.PostAsync($"http://{IPaddress}:5005/api/v1/EmcTestCase/start?time={DateTime.Now}", new StringContent($"\"{startcommand}\"", Encoding.UTF8, "application/json"));
                        StartTest.Content = new StringContent(json, Encoding.UTF8, "application/json");

                        if (StartTest.IsSuccessStatusCode)
                        {
                            //获取控制指令是否开始
                            string StartResult = await StartTest.Content.ReadAsStringAsync();
                            //判断.... command error
                            if (StartResult.Contains("error")) { return "Error"; }
                            Console.WriteLine(StartResult);
                            return StartResult;
                        }
                        else
                        {
                            Console.WriteLine("发送测试指令失败： " + StartTest.StatusCode);
                            return "Error";
                        }
                    }
                }
                else
                {
                    Console.WriteLine("发送CPLD指令失败： " + ControlEquip.StatusCode);
                    return "Error";
                }
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<string> Getburstsurgelog(string IPaddress)
        {
            string loginfo = "";
            using (HttpClient client = new HttpClient())
            {
                //发送获取log的请求
                HttpResponseMessage ControlEquip = await client.GetAsync("http://" + IPaddress + ":5005/api/v1/EmcEquipment/logs");
                if (ControlEquip.IsSuccessStatusCode)
                {
                    //获取log内容
                    string data = await ControlEquip.Content.ReadAsStringAsync();
                    Console.WriteLine(data);
                    loginfo = data;
                }
                else
                {
                    Console.WriteLine("获取log信息失败： " + ControlEquip.StatusCode);
                    return "Get Log Info Error";
                }
            }
            return loginfo;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<string> Gettestduration(string IPaddress) 
        {
            string testdata = "";
            using (HttpClient client = new HttpClient())
            {
                //发送获取测试时长的请求
                HttpResponseMessage ControlEquip = await client.GetAsync("http://" + IPaddress + ":5005/api/v1/EmcTestCase/result");
                if (ControlEquip.IsSuccessStatusCode)
                {
                    //获取测试时长
                    testdata = await ControlEquip.Content.ReadAsStringAsync();
                    Console.WriteLine(testdata);                    
                    
                }
                else
                {
                    Console.WriteLine("获取测试时长失败： " + ControlEquip.StatusCode);
                }
            }
            return testdata;
        }

    }
}
