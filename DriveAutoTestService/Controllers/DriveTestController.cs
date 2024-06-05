using DriveAutoTestService.Logs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DriveAutoTestService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class DriveTestController : ControllerBase
    {
        [HttpGet]
        public JsonResult BookTestRack(string ip, string user)
        {
            string command = "api/home/booktestrack?user=" + user;
            return HttpService(ip, command);
        }

        [HttpGet]
        public JsonResult DownloadEquipConfig(string ip, string user, string equipjson)
        {
            string command = "api/home/DownloadEquipConfig?equipjson=" + equipjson + "&user=" + user;
            return HttpService(ip, command);
        }

        [HttpGet]
        public JsonResult GetEquipConfig(string ip, string user)
        {
            string command = "api/home/GetEquipConfig?user=" + user;
            return HttpService(ip, command);
        }

        [HttpGet]
        public JsonResult ConnectEquip(string ip, string user)
        {
            string command = "api/home/ConnectEquip?user=" + user;
            return HttpService(ip, command);
        }

        [HttpGet]
        public JsonResult StartTest(string ip, string user, string testcondition)
        {
            string command = "api/home/StartTest?user=" + user + "&testcondition=" + testcondition;
            return HttpService(ip, command);
        }

        [HttpGet]
        public JsonResult StopTest(string ip, string user)
        {
            string command = "api/home/StopTest?user=" + user;
            return HttpService(ip, command);
        }

        [HttpGet]
        public JsonResult MonitorStatus(string ip, string user)
        {
            string command = "api/home/MonitorStatus?user=" + user;
            return HttpService(ip, command);
        }

        [HttpGet]
        public JsonResult UploadRecord(string ip, string user)
        {
            string command = "api/home/MonitorStatus?user=" + user;
            return HttpServiceSaveFile(ip, command);
        }

        private static JsonResult HttpServiceSaveFile(string ip, string command)
        {
            //后台client方式GET提交
            HttpClient myHttpClient = new HttpClient();
            //提交当前地址的webapi
            string url = "http://" + ip + ":8806";
            Logger.Info("Url: " + url);
            Logger.Info("Command: " + command);
            myHttpClient.BaseAddress = new Uri(url);
            string result = "";
            string folderName = @"D:\"; 
            try
            {
                //GET提交 返回string
                HttpResponseMessage response = myHttpClient.GetAsync(command).Result;
                if (response.IsSuccessStatusCode)
                {
                    string fileName = response.Content.Headers.ContentDisposition.FileName;
                    Stream streamFromService = response.Content.ReadAsStreamAsync().Result;
                    FileStream fsDes = new FileStream(folderName+fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    streamFromService.CopyTo(fsDes);
                    fsDes.Close();
                    Logger.Info("Save file: "+folderName + fileName, "Success");
                }
            }
            catch (Exception ex)
            {
                result = "No connection";
                Logger.Error(ex.Message, "error");
            }
            JsonResult res = new JsonResult(result);
            return res;
        }

        private static JsonResult HttpService(string ip, string command)
        {
            //后台client方式GET提交
            HttpClient myHttpClient = new HttpClient();
            //提交当前地址的webapi
            string url = "http://" + ip + ":8806";
            Logger.Info("Url: " + url);
            Logger.Info("Command: "+ command);
            myHttpClient.BaseAddress = new Uri(url);
            string result = "";
            try
            {
                //GET提交 返回string
                HttpResponseMessage response = myHttpClient.GetAsync(command).Result;
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                    Logger.Info(result,"Success");
                }
            }
            catch(Exception ex)
            {
                result = "No connection";
                Logger.Error(ex.Message,"error");
            }           
            JsonResult res = new JsonResult(result);        
            return res;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage result = null;
            try
            {
                //Screen scr = Screen.PrimaryScreen;
                //Rectangle rc = scr.Bounds;
                //int iWidth = rc.Width;
                //int iHeight = rc.Height;
                //Image image = new Bitmap(iWidth, iHeight);
                //Graphics graph = Graphics.FromImage(image);
                //graph.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(iWidth, iHeight));
                //image.Save("D:\\3333j.jpeg");
                string path = @"D:\Trace_2022-07-28-13-51-49.png";
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(fs);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("Image/png");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = Path.GetFileName("");
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }
    }
}
