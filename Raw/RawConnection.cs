using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace SparxoChat.Raw
{
    public class RawConnection : PersistentConnection
    {
        private static readonly ConcurrentDictionary<string, string> _users = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, string> _clients = new ConcurrentDictionary<string, string>();

        protected override async Task OnConnected(HttpRequest request, string connectionId)
        {
            string clientIp = request.HttpContext.Connection.RemoteIpAddress?.ToString();
            
            string user = GetUser(connectionId);

        }

        private string GetUser(string connectionId)
        {
            string user;
            if (!_clients.TryGetValue(connectionId, out user))
            {
                return connectionId;
            }
            return user;
        }

        private string GetClient(string user){
            string connectionId;
            if(_users.TryGetValue(user, out connectionId)){
                return connectionId;
            }
            return null;
        }
    }
}