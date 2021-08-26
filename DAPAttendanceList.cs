using System;

namespace D365_Attendance
{
    public class DAPAttendanceList
    {
        public string HcmPersonnelNumberId { get; set; }
        public string ShiftNum { get; set; }
        public DateTime TransDate { get; set; }
        public string MachineTo { get; set; }
        public string MachineFrom { get; set; }
        public string BreakTime { get; set; }
    }

    public class AttendanceData
    {
        public string staff_id { get; set; }
        public DateTime eventlog_dt { get; set; }
        public string eventName { get; set; }
    }

}
