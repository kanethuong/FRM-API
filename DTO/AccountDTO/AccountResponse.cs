using System;

namespace kroniiapi.DTO.AccountDTO
{
    public class AccountResponse
    {
        public int AccountId { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}