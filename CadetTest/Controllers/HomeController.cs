using CadetTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CadetTest.Controllers
{
    public class HomeController : Controller
    {
        public class auth
        {
            public int Id;
            public string Username;
            public string JwtToken;
            public DateTime ExpirationDate;
        }

        public class cont
        {
            public int id;
            public string type;
            public string recipient;
            public string status;
            public string recipientType;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ContentTable()
        {
            #region getToken
            auth serviceResponse = new auth();
            List<cont> serviceResponseCont = new List<cont>();
            var token = "";

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://localhost:44338/api/User/authenticate");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.Method = "POST";
            using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    username = "User-5E638711-2D64-47B0-A8F5-1C5A9EADA966",
                    password = "Pass-E4A679C7-2F4B-40AE-9DC8-C967EF7215AE"
                });
                streamWriter.Write(json);
            }

            HttpWebResponse httpResponse = null;
            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string message = streamReader.ReadToEnd();
                serviceResponse = JsonConvert.DeserializeObject<auth>(message);
                token = serviceResponse.JwtToken;
            }
            #endregion

            #region postContent
            // content metodu işlemleri.
            HttpWebRequest httpWebRequestContent = (HttpWebRequest)WebRequest.Create("https://localhost:44338/api/Consents");
            httpWebRequestContent.ContentType = "application/json";
            httpWebRequestContent.Accept = "application/json";
            httpWebRequestContent.PreAuthenticate = true;
            httpWebRequestContent.Headers.Add("Authorization", "Bearer " + token);
            httpWebRequestContent.Method = "POST";
            using (StreamWriter streamWriterContent = new StreamWriter(httpWebRequestContent.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    startId = 1,
                    count = 1000
                });
                streamWriterContent.Write(json);
            }

            HttpWebResponse httpResponseContent = null;
            httpResponseContent = (HttpWebResponse)httpWebRequestContent.GetResponse();

            using (StreamReader streamReaderContent = new StreamReader(httpResponseContent.GetResponseStream()))
            {
                string message = streamReaderContent.ReadToEnd();
                serviceResponseCont = JsonConvert.DeserializeObject<List<cont>>(message);
            }

            #endregion

            HttpContext.Session.SetString("sessionList", "");

            serviceResponseCont = serviceResponseCont.OrderByDescending(x => x.id).ToList();

            return View(serviceResponseCont);
        }

        [HttpGet]
        public PartialViewResult ProcessesPartial(string Process, string Type, string Recipient, string Status, string RecipientType, string Id)
        {
            #region getToken
            auth serviceResponse = new auth();
            List<cont> serviceResponseCont = new List<cont>();
            var token = "";

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://localhost:44338/api/User/authenticate");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.Method = "POST";
            using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    username = "User-5E638711-2D64-47B0-A8F5-1C5A9EADA966",
                    password = "Pass-E4A679C7-2F4B-40AE-9DC8-C967EF7215AE"
                });
                streamWriter.Write(json);
            }

            HttpWebResponse httpResponse = null;
            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string message = streamReader.ReadToEnd();
                serviceResponse = JsonConvert.DeserializeObject<auth>(message);
                token = serviceResponse.JwtToken;
            }
            #endregion

            #region postContent
            // content metodu işlemleri.
            HttpWebRequest httpWebRequestContent = (HttpWebRequest)WebRequest.Create("https://localhost:44338/api/Consents");
            httpWebRequestContent.ContentType = "application/json";
            httpWebRequestContent.Accept = "application/json";
            httpWebRequestContent.PreAuthenticate = true;
            httpWebRequestContent.Headers.Add("Authorization", "Bearer " + token);
            httpWebRequestContent.Method = "POST";
            using (StreamWriter streamWriterContent = new StreamWriter(httpWebRequestContent.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    startId = 1,
                    count = 1000
                });
                streamWriterContent.Write(json);
            }

            HttpWebResponse httpResponseContent = null;
            httpResponseContent = (HttpWebResponse)httpWebRequestContent.GetResponse();

            using (StreamReader streamReaderContent = new StreamReader(httpResponseContent.GetResponseStream()))
            {
                string message = streamReaderContent.ReadToEnd();
                serviceResponseCont = JsonConvert.DeserializeObject<List<cont>>(message);
            }

            #endregion

            #region takingCurrentList
            var value = HttpContext.Session.GetString("sessionList");
            cont[] arrayContent;

            if (value == "")
            {
                serviceResponseCont = serviceResponseCont.OrderByDescending(x => x.id).ToList();

                HttpContext.Session.SetString("sessionList", JsonConvert.SerializeObject(serviceResponseCont));

                value = HttpContext.Session.GetString("sessionList");
                arrayContent = JsonConvert.DeserializeObject<cont[]>(value);
            }
            else
            {
                value = HttpContext.Session.GetString("sessionList");
                arrayContent = JsonConvert.DeserializeObject<cont[]>(value);
            }

            var listContent = arrayContent.ToList();
            #endregion


            int idObj = Convert.ToInt32(Id);

            if (Process == "Insert")
            {
                var lastId = listContent[0].id;
                listContent.Add(new cont()
                {
                    id = (lastId + 1),
                    type = Type,
                    recipient = Recipient,
                    status = Status,
                    recipientType = RecipientType
                });
            }

            if (Process == "Edit")
            {
                var editObj = listContent.Where(x => x.id == idObj).FirstOrDefault();
                editObj.type = Type;
                editObj.recipient = Recipient;
                editObj.status = Status;
                editObj.recipientType = RecipientType;
            }

            if (Process == "Delete")
            {
                var editObj = listContent.Where(x => x.id == idObj).FirstOrDefault();
                listContent.Remove(editObj);
            }

            listContent = listContent.OrderByDescending(x => x.id).ToList();
            HttpContext.Session.SetString("sessionList", JsonConvert.SerializeObject(listContent));

            return PartialView(listContent);
        }

        public string GetDetailContent(int id)
        {
            string result = "";

            #region getToken
            auth serviceResponse = new auth();
            List<cont> serviceResponseCont = new List<cont>();
            var token = "";

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://localhost:44338/api/User/authenticate");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.Method = "POST";
            using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    username = "User-5E638711-2D64-47B0-A8F5-1C5A9EADA966",
                    password = "Pass-E4A679C7-2F4B-40AE-9DC8-C967EF7215AE"
                });
                streamWriter.Write(json);
            }

            HttpWebResponse httpResponse = null;
            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string message = streamReader.ReadToEnd();
                serviceResponse = JsonConvert.DeserializeObject<auth>(message);
                token = serviceResponse.JwtToken;
            }
            #endregion

            #region postContent
            // content metodu işlemleri.
            HttpWebRequest httpWebRequestContent = (HttpWebRequest)WebRequest.Create("https://localhost:44338/api/Consents");
            httpWebRequestContent.ContentType = "application/json";
            httpWebRequestContent.Accept = "application/json";
            httpWebRequestContent.PreAuthenticate = true;
            httpWebRequestContent.Headers.Add("Authorization", "Bearer " + token);
            httpWebRequestContent.Method = "POST";
            using (StreamWriter streamWriterContent = new StreamWriter(httpWebRequestContent.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    startId = 1,
                    count = 1000
                });
                streamWriterContent.Write(json);
            }

            HttpWebResponse httpResponseContent = null;
            httpResponseContent = (HttpWebResponse)httpWebRequestContent.GetResponse();

            using (StreamReader streamReaderContent = new StreamReader(httpResponseContent.GetResponseStream()))
            {
                string message = streamReaderContent.ReadToEnd();
                serviceResponseCont = JsonConvert.DeserializeObject<List<cont>>(message);
            }

            #endregion

            #region takingCurrentList
            var value = HttpContext.Session.GetString("sessionList");
            cont[] arrayContent;

            if (value == "")
            {
                serviceResponseCont = serviceResponseCont.OrderByDescending(x => x.id).ToList();

                HttpContext.Session.SetString("sessionList", JsonConvert.SerializeObject(serviceResponseCont));

                value = HttpContext.Session.GetString("sessionList");
                arrayContent = JsonConvert.DeserializeObject<cont[]>(value);
            }
            else
            {
                value = HttpContext.Session.GetString("sessionList");
                arrayContent = JsonConvert.DeserializeObject<cont[]>(value);
            }

            var listContent = arrayContent.ToList();
            #endregion

            var serviceModel = listContent.Where(x => x.id == id).FirstOrDefault();

            result = serviceModel.id + "~" + serviceModel.type + "~" + serviceModel.recipient + "~" +
                                serviceModel.status + "~" + serviceModel.recipientType;

            return result;
        }
    }
}
