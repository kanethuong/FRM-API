using System.Collections.Generic;
using System.Threading.Tasks;
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
    }
}