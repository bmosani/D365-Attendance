using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace D365_Attendance
{
    public static class Push
    {

        private static readonly string ApiUrl = ConfigurationManager.AppSettings["ApiUrl"];
        private static readonly string DataUrl = ConfigurationManager.AppSettings["DataUrl"];




        //static HttpClient client = new HttpClient();

        //public static async Task<Uri> PostData(List<DAPAttendanceList> @params)
        //{
        //    client.BaseAddress = new Uri(ApiUrl);
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    var json = JsonConvert.SerializeObject(@params);

        //    var response = await client.PostAsJsonAsync("Attendance/Post", json);
        //    response.EnsureSuccessStatusCode();

        //    var resultString = await response.Content.ReadAsStringAsync();

        //    return response.Headers.Location;
        //}

        public static string PostData(string date)
        {
            string message;
            string data;

            var dataUrl = "http://172.20.1.99:8012/biotime/json.php?date=" + date;
            var req = WebRequest.Create(dataUrl);
            req.Method = "Get";
            req.GetResponse();

            WebResponse webResponse = null;
            try
            {
                using (webResponse = req.GetResponse())
                {
                    using (var str = webResponse.GetResponseStream())
                    {
                        using (var sr = new StreamReader(str))
                        {
                            //return JsonConvert.DeserializeObject<DAPAttendanceList>(sr.ReadToEnd());
                            data = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException e)
            {
                data = e.Message;
            }



            //string message;
            var request = WebRequest.Create(ApiUrl);

            request.Method = "POST";
            request.ContentType = "application/json";

            //var json = JsonConvert.SerializeObject(data);

            var byteArray = Encoding.UTF8.GetBytes(data);
            request.ContentLength = byteArray.Length;
            try
            {
                using (var dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
            }
            catch (WebException e)
            {
                message = e.Message;
                return message;
            }
            //WebResponse webResponse = null;
            try
            {
                using (webResponse = request.GetResponse())
                {
                    using (var str = webResponse.GetResponseStream())
                    {
                        using (var sr = new StreamReader(str))
                        {
                            //return JsonConvert.DeserializeObject<DAPAttendanceList>(sr.ReadToEnd());
                            message = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException e)
            {
                message = e.Message;
            }

            return message;
        }

        public static string PostData(string fromDate,string toDate)
        {
            string message;
            string data;

            var dataUrl = DataUrl +"?from=" + fromDate + "&to=" + toDate;
            var req = WebRequest.Create(dataUrl);
            req.Method = "Get";
            req.GetResponse();

            WebResponse webResponse = null;
            try
            {
                using (webResponse = req.GetResponse())
                {
                    using (var str = webResponse.GetResponseStream())
                    {
                        using (var sr = new StreamReader(str))
                        {
                            //return JsonConvert.DeserializeObject<DAPAttendanceList>(sr.ReadToEnd());
                            data = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException e)
            {
                data = e.Message;
            }



            //string message;
            var request = WebRequest.Create(ApiUrl);

            request.Method = "POST";
            request.ContentType = "application/json";

            //var json = JsonConvert.SerializeObject(data);
            //var finaldata = "["+ data + "]";
            var byteArray = Encoding.UTF8.GetBytes(data);
            request.ContentLength = byteArray.Length;
            try
            {
                using (var dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
            }
            catch (WebException e)
            {
                message = e.Message;
                return message;
            }
            //WebResponse webResponse = null;
            try
            {
                using (webResponse = request.GetResponse())
                {
                    using (var str = webResponse.GetResponseStream())
                    {
                        using (var sr = new StreamReader(str))
                        {
                            //return JsonConvert.DeserializeObject<DAPAttendanceList>(sr.ReadToEnd());
                            message = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException e)
            {
                message = e.Message;
            }

            return message;
        }


        //public static string PostData(List<DAPAttendanceList> @params)
        //{

        //    string message;
        //    var request = WebRequest.Create(ApiUrl);

        //    request.Method = "POST";
        //    request.ContentType = "application/json";

        //    var json = JsonConvert.SerializeObject(@params);

        //    var byteArray = Encoding.UTF8.GetBytes(json);
        //    request.ContentLength = byteArray.Length;
        //    try
        //    {
        //        using (var dataStream = request.GetRequestStream())
        //        {
        //            dataStream.Write(byteArray, 0, byteArray.Length);
        //        }
        //    }
        //    catch (WebException e)
        //    {
        //        message = e.Message;
        //        return message;
        //    }
        //    WebResponse webResponse = null;
        //    try
        //    {
        //        using (webResponse = request.GetResponse())
        //        {
        //            using (var str = webResponse.GetResponseStream())
        //            {
        //                using (var sr = new StreamReader(str))
        //                {
        //                    //return JsonConvert.DeserializeObject<DAPAttendanceList>(sr.ReadToEnd());
        //                    message = sr.ReadToEnd();
        //                }
        //            }
        //        }
        //    }
        //    catch (WebException e)
        //    {
        //        message = e.Message;
        //    }

        //    return message;
        //}

    }
}
