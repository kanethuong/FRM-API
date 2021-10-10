using System.Collections.Generic;

namespace kroniiapi.DB.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public ICollection<Class> Classes { get; set; }
    }
}