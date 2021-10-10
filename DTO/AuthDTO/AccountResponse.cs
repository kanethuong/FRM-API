using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.AuthDTO
{
    public class AccountResponse
    {
        public int AccountId { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string AvatarURL { get; set; }
        public string Role { get; set; }
        public string AccessToken { get; set; }
    }
}