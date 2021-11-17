using System.Collections.Generic;

namespace kroniiapi.DB.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public ICollection<ClassModule> ClassModules { get; set; }
        public ICollection<Exam> Exams { get; set; }
    }
}