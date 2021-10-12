using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.Helper
{
    public interface IRefreshToken
    {
        string CreateRefreshToken(string email);
        string GetEmailByToken(string token);
        void RemoveTokenByEmail(string email);
    }
}