using System;
using System.ComponentModel.DataAnnotations;
using static kroniiapi.Services.Attendance.AttendanceStatus;

namespace kroniiapi.DTO.AttendanceDTO
{
    public class AttendanceInput
    {
        public int Id { get; set; }
        private string _status;
        [Required(ErrorMessage = "Accept A, An, E, En, L, Ln, P, Ob Only")]
        public string Status
        {
            get { return _status; }
            set
            {
                switch (value)
                {
                    case nameof(_attendanceStatus.A):
                        _status = _attendanceStatus.A.ToString();
                        break;
                    case nameof(_attendanceStatus.An):
                        _status = _attendanceStatus.An.ToString();
                        break;
                    case nameof(_attendanceStatus.E):
                        _status = _attendanceStatus.E.ToString();
                        break;
                    case nameof(_attendanceStatus.En):
                        _status = _attendanceStatus.En.ToString();
                        break;
                    case nameof(_attendanceStatus.L):
                        _status = _attendanceStatus.L.ToString();
                        break;
                    case nameof(_attendanceStatus.Ln):
                        _status = _attendanceStatus.Ln.ToString();
                        break;
                    case nameof(_attendanceStatus.P):
                        _status = _attendanceStatus.P.ToString();
                        break;
                    case nameof(_attendanceStatus.Ob):
                        _status = _attendanceStatus.Ob.ToString();
                        break;
                    default:
                        _status = null;
                        break;
                }
            }
        }

        public string Reason { get; set; }
        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (value == default(DateTime))
                {
                    _date = DateTime.Now;
                }
                else
                {
                    _date = value;
                }
            }
        }

    }
}