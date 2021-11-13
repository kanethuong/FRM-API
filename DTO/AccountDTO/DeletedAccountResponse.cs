using System;

namespace kroniiapi.DTO.AccountDTO
{
    public class DeletedAccountResponse
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public DateTime DeactivatedAt { get; set; }
        public DeleteBy DeleteBy { get; set; }
    }
}