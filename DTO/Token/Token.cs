using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.Token
{
    public class Token
    {
        public Token(string refreshToken, DateTime expire)
        {
            RefreshToken = refreshToken;
            Expire = expire;
        }

        public string RefreshToken { get; set; }
        public DateTime Expire { get; set; }
    }
}