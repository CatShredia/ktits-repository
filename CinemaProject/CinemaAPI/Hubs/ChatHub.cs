using Microsoft.AspNetCore.SignalR;
using System.Linq;

namespace TestSignalR320.Hubs
{
    public class ChatHub : Hub
    {
        private static Dictionary<string, string> _users = new Dictionary<string, string>();

        public async Task Register(string userName)
        {
            if (!_users.ContainsKey(userName))
            {
                _users[userName] = Context.ConnectionId;
                await Clients.All.SendAsync("UpdateUsers", _users.Keys.ToList());
            }
        }

        public async Task SendMessage(string fromUser, string toUser, string message)
        {
            await Clients.All.SendAsync("NewMessage", fromUser, toUser, message, DateTime.Now);
        }
    }
}
