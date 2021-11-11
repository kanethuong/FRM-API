using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.Services.Attendance
{
    public static class AttendanceStatus
    {
        public enum _attendanceStatus
        {
            A,
            P,
            L,
            Ln,
            E,
            En,
            An,
            Ob
        }
    }
}