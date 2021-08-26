using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace D365_Attendance
{
    public static class Get
    {
        private static readonly string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private static readonly string shift = ConfigurationManager.AppSettings["Shift"];
        private static readonly string AttendanceDate = ConfigurationManager.AppSettings["AttendanceDate"];

        public static List<DAPAttendanceList> GetSpecificDateAttendance()
        {
            var DAPAttendanceListObj = new List<DAPAttendanceList>();
            var staffId = string.Empty;
            var totalBreak = string.Empty;
            var cmdText = "GetAttendanceData";
            var staffList = new List<string>();

            var employeeId = ConfigurationManager.AppSettings["employeeId"];
            var configFromDate = ConfigurationManager.AppSettings["FromDate"];
            var configToDate = ConfigurationManager.AppSettings["ToDate"];

            staffList = employeeId.Equals("Default") ? GetStaffList() : employeeId.Split(',').ToList();

            if (staffList.Count != 0)
            {
                foreach (var employee in staffList)
                {
                    try
                    {
                        var FromDate = DateTime.ParseExact(configFromDate, "yyyy-MM-dd", null);
                        var ToDate = DateTime.ParseExact(configToDate, "yyyy-MM-dd", null);

                        var dates = GetDateRange(FromDate, ToDate);

                        if (dates != null && dates.Count > 0)
                        {
                            foreach (var date in dates)
                            {
                                var AttendanceDataObj = new List<AttendanceData>();

                                using (_connection = new SqlConnection(_connectionString))
                                {
                                    var cmd = new SqlCommand { Connection = _connection };
                                    _connection.Open();
                                    cmd.Parameters.Clear();
                                    cmd.CommandText = cmdText;
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    SqlParameter[] sqlParams =
                                    {
                                        new SqlParameter("@LogDt", SqlDbType.DateTime),
                                        new SqlParameter("@staffId", SqlDbType.NVarChar),
                                    };
                                    sqlParams[0].Value = date;
                                    sqlParams[1].Value = employee;
                                    foreach (var parm in sqlParams)
                                    {
                                        cmd.Parameters.Add(parm);
                                    }

                                    var reader = cmd.ExecuteReader();


                                    using (reader)
                                    {

                                        while (reader.Read())
                                        {
                                            var eventLogDt = Convert.ToDateTime(reader["eventlog_dt"].ToString().Trim());
                                            var eventName = reader["event"].ToString().Trim();

                                            switch (employee.Length)
                                            {
                                                case 1:
                                                    staffId = "000" + employee;
                                                    break;
                                                case 2:
                                                    staffId = "00" + employee;
                                                    break;
                                                case 3:
                                                    staffId = "0" + employee;
                                                    break;
                                            }

                                            var recordAttendanceList = new AttendanceData
                                            {
                                                staff_id = staffId,
                                                eventlog_dt = eventLogDt,
                                                eventName = eventName
                                            };
                                            AttendanceDataObj.Add(recordAttendanceList);
                                        }
                                    }
                                }

                                if (AttendanceDataObj.Count != 0)
                                {
                                    string firstRecord;
                                    string lastRecord;
                                    double InTimeSeconds = 0;
                                    double OutTimeSeconds = 0;
                                    //var temp = new DateTime();
                                    var pattern = "yyyy-MM-dd HH:mm";
                                    var InDateTime = DateTime.Now;
                                    var OutDateTime = DateTime.Now;

                                    if (AttendanceDataObj.FirstOrDefault().eventName.Trim().Equals("20001"))
                                    {
                                        firstRecord = AttendanceDataObj.FirstOrDefault().eventlog_dt.ToString("yyyy-MM-dd HH:mm");
                                        DateTime.TryParseExact(firstRecord, pattern, null, DateTimeStyles.None, out InDateTime);

                                        //temp = Convert.ToDateTime(firstRecord);
                                        //var InTime = Convert.ToDateTime(firstRecord);
                                        var InTimeString = InDateTime.ToString("HH:mm");
                                        InTimeSeconds = TimeSpan.Parse(InTimeString).TotalSeconds;
                                    }

                                    if (AttendanceDataObj.LastOrDefault().eventName.Trim().Equals("20003"))
                                    {
                                        lastRecord = AttendanceDataObj.LastOrDefault().eventlog_dt.ToString("yyyy-MM-dd HH:mm");
                                        DateTime.TryParseExact(lastRecord, pattern, null, DateTimeStyles.None, out OutDateTime);

                                        //var OutTime = Convert.ToDateTime(lastRecord);
                                        var OutTimeString = OutDateTime.ToString("HH:mm");
                                        OutTimeSeconds = TimeSpan.Parse(OutTimeString).TotalSeconds;
                                    }

                                    if (AttendanceDataObj.Count > 1)
                                    {
                                        totalBreak = CalculateBreakTime(AttendanceDataObj);
                                    }

                                    var recordAttendanceList = new DAPAttendanceList
                                    {
                                        HcmPersonnelNumberId = staffId,
                                        ShiftNum = shift,
                                        TransDate = InDateTime.Date,
                                        MachineFrom = InTimeSeconds.ToString(CultureInfo.InvariantCulture),
                                        MachineTo = OutTimeSeconds.ToString(CultureInfo.InvariantCulture),
                                        BreakTime = totalBreak
                                    };
                                    DAPAttendanceListObj.Add(recordAttendanceList);
                                }

                            }

                        }


                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }


            return DAPAttendanceListObj;
        }

        public static List<DAPAttendanceList> GetDefaultDateAttendance()
        {

            var DAPAttendanceListObj = new List<DAPAttendanceList>();
            string totalBreak = string.Empty;
            var staffId = string.Empty;

            var cmdText = "GetAttendanceData";
            var LogDt = GetDate();
            try
            {
                var staffList = GetStaffList();
                foreach (var staff in staffList)
                {
                    var AttendanceDataObj = new List<AttendanceData>();

                    using (_connection = new SqlConnection(_connectionString))
                    {
                        var cmd = new SqlCommand { Connection = _connection };
                        _connection.Open();
                        cmd.Parameters.Clear();
                        cmd.CommandText = cmdText;
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter[] sqlParams =
                        {
                            new SqlParameter("@LogDt", SqlDbType.DateTime),
                            new SqlParameter("@staffId", SqlDbType.NVarChar),
                        };
                        sqlParams[0].Value = LogDt;
                        sqlParams[1].Value = staff;
                        foreach (var parm in sqlParams)
                        {
                            cmd.Parameters.Add(parm);
                        }

                        var reader = cmd.ExecuteReader();


                        using (reader)
                        {

                            while (reader.Read())
                            {
                                var eventLogDt = Convert.ToDateTime(reader["eventlog_dt"].ToString().Trim());
                                var eventName = reader["event"].ToString().Trim();

                                switch (staff.Length)
                                {
                                    case 1:
                                        staffId = "000" + staff;
                                        break;
                                    case 2:
                                        staffId = "00" + staff;
                                        break;
                                    case 3:
                                        staffId = "0" + staff;
                                        break;
                                }

                                var recordAttendanceList = new AttendanceData
                                {
                                    staff_id = staffId,
                                    eventlog_dt = eventLogDt,
                                    eventName = eventName
                                };
                                AttendanceDataObj.Add(recordAttendanceList);
                            }
                        }
                    }

                    if (AttendanceDataObj.Count != 0)
                    {
                        string firstRecord;
                        string lastRecord;
                        double InTimeSeconds = 0;
                        double OutTimeSeconds = 0;
                        //var temp = new DateTime();
                        var pattern = "yyyy-MM-dd HH:mm";
                        var InDateTime = DateTime.Now;
                        var OutDateTime = DateTime.Now;

                        if (AttendanceDataObj.FirstOrDefault().eventName.Trim().Equals("20001"))
                        {
                            firstRecord = AttendanceDataObj.FirstOrDefault().eventlog_dt.ToString("yyyy-MM-dd HH:mm");
                            DateTime.TryParseExact(firstRecord, pattern, null, DateTimeStyles.None, out InDateTime);

                            //temp = Convert.ToDateTime(firstRecord);
                            //var InTime = Convert.ToDateTime(firstRecord);
                            var InTimeString = InDateTime.ToString("HH:mm");
                            InTimeSeconds = TimeSpan.Parse(InTimeString).TotalSeconds;
                        }

                        if (AttendanceDataObj.LastOrDefault().eventName.Trim().Equals("20003"))
                        {
                            lastRecord = AttendanceDataObj.LastOrDefault().eventlog_dt.ToString("yyyy-MM-dd HH:mm");
                            DateTime.TryParseExact(lastRecord, pattern, null, DateTimeStyles.None, out OutDateTime);

                            //var OutTime = Convert.ToDateTime(lastRecord);
                            var OutTimeString = OutDateTime.ToString("HH:mm");
                            OutTimeSeconds = TimeSpan.Parse(OutTimeString).TotalSeconds;
                        }
                        if (AttendanceDataObj.Count > 1)
                        {
                            totalBreak = CalculateBreakTime(AttendanceDataObj);
                        }


                        var recordAttendanceList = new DAPAttendanceList
                        {
                            HcmPersonnelNumberId = staffId,
                            ShiftNum = shift,
                            TransDate = InDateTime.Date,
                            MachineFrom = InTimeSeconds.ToString(CultureInfo.InvariantCulture),
                            MachineTo = OutTimeSeconds.ToString(CultureInfo.InvariantCulture),
                            BreakTime = totalBreak
                        };
                        DAPAttendanceListObj.Add(recordAttendanceList);
                    }

                }
            }


            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return DAPAttendanceListObj;
        }

        #region Private Methods

        private static string GetDate()
        {
            var LogDt = AttendanceDate.Equals("Default") ? DateTime.Today.AddDays(-1).Date.ToString("yyyy-MM-dd") : AttendanceDate;

            return LogDt;
        }

        private static SqlConnection _connection;

        private static List<string> GetStaffList()
        {
            var staffList = new List<string>();

            var cmdText = "GetStaffList";
            //var LogDt = GetDate();
            try
            {
                using (_connection = new SqlConnection(_connectionString))
                {
                    var cmd = new SqlCommand { Connection = _connection };
                    _connection.Open();
                    // cmd.Parameters.Clear();
                    cmd.CommandText = cmdText;
                    cmd.CommandType = CommandType.StoredProcedure;

                    var reader = cmd.ExecuteReader();

                    using (reader)
                    {

                        while (reader.Read())
                        {
                            var staffId = reader["staff_id"].ToString().Trim();
                            staffList.Add(staffId);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return staffList;
        }

        private static string CalculateBreakTime(List<AttendanceData> times)
        {
            double totalBreaksMinutes = 0;
            var timesCount = times.Count;

            for (var i = 0; i < timesCount - 1; i++)
            {
                if (times[i].eventName.Trim().Equals("20003") && times[i + 1].eventName.Trim().Equals("20001"))
                {
                    totalBreaksMinutes += (times[i + 1].eventlog_dt - times[i].eventlog_dt).TotalHours;
                }
            }

            var convertedMinutes = totalBreaksMinutes.ToString(CultureInfo.InvariantCulture);

            return convertedMinutes;

            /*var time_deltas = new List<TimeSpan>();

            var breakResult = 0;

            var prev = times.First();
            foreach (var t in times.Skip(1))
            {
                var result = t - prev;
                breakResult += (int)result.TotalSeconds;
                time_deltas.Add(result);
                prev = t;
            }

            var sum = time_deltas.Sum(span => span.Minutes);

            return time_deltas;*/
        }

        private static List<DateTime> GetDateRange(DateTime FromDate, DateTime ToDate)
        {
            var dateList = new List<DateTime>();
            try
            {
                if (FromDate > ToDate)
                {
                    return null;
                }

                var tmpDate = FromDate;
                do
                {
                    dateList.Add(tmpDate);
                    tmpDate = tmpDate.AddDays(1);
                } while (tmpDate <= ToDate);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return dateList;
        }



        #endregion
    }
}
