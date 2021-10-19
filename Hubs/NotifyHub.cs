using System.Collections.Generic;
using System.Threading.Tasks;
using kroniiapi.DTO.NotificationDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.SignalR;

namespace kroniiapi.Hubs
{
    public class NotifyHub : Hub
    {
        private readonly ICacheProvider _cacheProvider;

        public NotifyHub(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }
        /// <summary>
        /// call to name connection base on email
        /// </summary>
        /// <param name="email">email of user(trainee, admin) who want to connect</param>
        /// <returns></returns>
        public async Task CreateName(string email)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, email);
        }
    }
}