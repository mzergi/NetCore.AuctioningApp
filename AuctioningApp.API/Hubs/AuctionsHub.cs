using AuctioningApp.Domain.Models.DBM;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctioningApp.API.Hubs
{
    public class AuctionsHub : Hub
    {
        protected IHubContext<AuctionsHub> context;
        
        public AuctionsHub(IHubContext<AuctionsHub> context)
        {
            this.context = context;
        }

        public async Task SendBid(Bid bid)
        {
            await context.Clients.All.SendAsync("bidReceived", bid);
        }
    }
}
