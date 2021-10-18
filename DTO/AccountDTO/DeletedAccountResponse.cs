using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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