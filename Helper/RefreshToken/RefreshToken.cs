using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DTO.Token;

namespace kroniiapi.Helper
{
    public class RefreshToken : IRefreshToken
    {
        private Dictionary<string, Token> _tokenList;

        public RefreshToken()
        {
            _tokenList = new Dictionary<string, Token>();
        }

        /// <summary>
        /// Create new refresh token then set it to tokenlist
        /// </summary>
        /// <param name="email">email of account</param>
        /// <returns>refresh token</returns>
        public string CreateRefreshToken(string email)
        {
            removeExpiredToken();

            // Generate new random hash string as token
            string refreshToken = Guid.NewGuid().ToString();

            // Set expire time 2 week
            DateTime expire = DateTime.Now.AddDays(12);

            // Create new token
            Token token = new Token(refreshToken, expire);

            // string key = accountId.ToString() + role;

            if (_tokenList.ContainsKey(email))
            {
                _tokenList.Remove(refreshToken);
            }
            _tokenList.Add(refreshToken, token);

            return refreshToken;
        }

        /// <summary>
        /// Get payload data (email) in refresh token
        /// </summary>
        /// <param name="token">Refresh token</param>
        /// <returns>email / null (not found token)</returns>
        public string GetEmailByToken(string token)
        {
            foreach (var entry in _tokenList)
            {
                if (entry.Value.RefreshToken == token)
                {
                    if (DateTime.Compare(entry.Value.Expire, DateTime.Now) < 0)
                    {
                        _tokenList.Remove(entry.Key);
                        return null;
                    }
                    return entry.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// Remove expired refresh token
        /// </summary>
        private void removeExpiredToken()
        {
            foreach (var entry in _tokenList)
            {
                if (DateTime.Compare(entry.Value.Expire, DateTime.Now) < 0)
                {
                    _tokenList.Remove(entry.Key);
                }
            }
        }

        /// <summary>
        /// Remove refresh token using email
        /// </summary>
        /// <param name="email">email contain token</param>
        public void RemoveTokenByEmail(string email)
        {
            _tokenList.Remove(email);
        }
    }
}