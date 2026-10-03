using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Hubs
{
    public class UserHub : Hub
    {
        public static int TotalViews { get; private set; } = 0;

        public async Task NewWindowLoaded()
        {
            TotalViews++;
            // Broadcast the updated user count to all connected clients
            await Clients.All.SendAsync("UpdateUserCount", TotalViews);
        }
    }
}
