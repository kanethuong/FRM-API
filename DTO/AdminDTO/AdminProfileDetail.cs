using System;

namespace kroniiapi.DTO.AdminDTO
{
    public class AdminProfileDetail
    {
        public string Fullname { get; set; }
        public string AvatarURL { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DOB { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Facebook { get; set; }
    }
}