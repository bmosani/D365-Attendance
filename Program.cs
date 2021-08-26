using log4net;
using log4net.Config;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace D365_Attendance
{
    class Program
    {
        static string AttendanceDate = ConfigurationManager.AppSettings["AttendanceDate"];
        static readonly string configFromDate = ConfigurationManager.AppSettings["FromDate"];
        static readonly string configToDate = ConfigurationManager.AppSettings["ToDate"];
        static readonly string date = ConfigurationManager.AppSettings["Date"];
        static readonly string ManualRun = ConfigurationManager.AppSettings["ManualRun"];
        static readonly string LogFilePath = ConfigurationManager.AppSettings["LogFilePath"];

        //static readonly string FromEmail = ConfigurationManager.AppSettings["FromEmail"];
        //static readonly string ToEmail = ConfigurationManager.AppSettings["ToEmail"];
        //static readonly string Username = ConfigurationManager.AppSettings["Username"];
        //static readonly string Password = ConfigurationManager.AppSettings["Password"];
        //static readonly string Port = ConfigurationManager.AppSettings["Port"];
        //static readonly string Host = ConfigurationManager.AppSettings["Host"];

        private static readonly ILog Log = LogManager.GetLogger("Program.cs");

        static void Main(string[] args)
        {
            //var result = Push.PostData(configFromDate, configToDate);
            GlobalContext.Properties["LogFileName"] = LogFilePath;
            XmlConfigurator.Configure();
            if (ManualRun.Equals("false"))
            {
                SingleDayAttendance();
            }
            else
            {
                MultipleDayAttendance();
            }
        }

        static void SingleDayAttendance()
        {
            if (AttendanceDate.Equals("Default"))
            {
                AttendanceDate = (DateTime.Today.AddDays(-1)).Date.ToString("yyyy-MM-dd");
            }
            try
            {
                //var attendanceData = Get.GetDefaultDateAttendance();
                var result = Push.PostData(date);
                //var count = attendanceData.Count;
                if (result != null)
                {
                    //Log.Info("Total Records Fetched: " + count + " - Record's Date: " + AttendanceDate);
                    //Console.WriteLine("Records fetched: " );
                    //Console.ReadLine();
                    Console.WriteLine("Date: " + AttendanceDate);
                    Console.WriteLine(result);
                    //Console.ReadLine();
                    //var json = JsonConvert.SerializeObject(attendanceData);
                    //SendEmail(json);
                    //Console.WriteLine("Pushing attendance data to MSD.");
                    //var result = Push.PostData(attendanceData);
                    Log.Info("Response from MSD");
                    Log.Info(result);
                    Log.Info("******************************************************************************************");

                }
                else
                {
                    Log.Info(DateTime.Now + " Error occurs");
                    Log.Info("No record fetched for the Date: " + AttendanceDate);
                }

            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        static void MultipleDayAttendance()
        {
            try
            {
                //var attendanceData = Get.GetSpecificDateAttendance();
                //var count = attendanceData.Count;
                var result = Push.PostData(configFromDate, configToDate);
                if (result != null)
                {
                    //Log.Info("Total Records Fetched: " + count + " - From Date: " + configFromDate + " To Date: " + configToDate);
                    //Console.WriteLine("Records fetched: " );
                    //Console.ReadLine();
                    Console.WriteLine("From Date: " + configFromDate + " To Date: " + configToDate);
                    Console.WriteLine("Pushing attendance data to MSD.");
                    Console.WriteLine(result);
                    //Console.ReadLine();
                    //var result = Push.PostData(configFromDate, configToDate);
                    Log.Info("Response from MSD");
                    Log.Info(result);
                    Log.Info("******************************************************************************************");
                }
                else
                {
                    Log.Info(DateTime.Now + " Error occurs");
                    Log.Info("No record fetched for the Date: " + "From Date: " + configFromDate + " To Date: " + configToDate);
                }

            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        static void SendEmail(string body)
        {

            var subject = "Attendance Integration Log";

            var FromEmail = ConfigurationManager.AppSettings["FromEmail"];
            var ToEmail = ConfigurationManager.AppSettings["ToEmail"];
            var Username = ConfigurationManager.AppSettings["Username"];
            var Password = ConfigurationManager.AppSettings["Password"];
            var Port = ConfigurationManager.AppSettings["Port"];
            var Host = ConfigurationManager.AppSettings["Host"];

            using (var mm = new MailMessage(FromEmail, ToEmail))
            {
                mm.Subject = subject;
                mm.Body = body;
                mm.IsBodyHtml = false;
                var smtp = new SmtpClient { Host = Host, EnableSsl = true };
                var NetworkCred = new NetworkCredential(Username, Password);
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = int.Parse(Port);
                Console.WriteLine("Sending Email......");
                smtp.Send(mm);
            }
        }
    }
}
