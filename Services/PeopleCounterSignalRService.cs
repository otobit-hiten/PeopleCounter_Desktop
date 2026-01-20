using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace PeopleCounterDesktop.Services
{
    public class PeopleCounterSignalRService
    {
        public HubConnection Connection { get; }

        public bool IsConnected =>
            Connection?.State == HubConnectionState.Connected;
        public PeopleCounterSignalRService()
        {
            Connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/peopleCounterHub")
                .WithAutomaticReconnect()
                .Build();
        }

        public async Task StartAsync()
        {
            if (Connection.State == HubConnectionState.Disconnected)
                await Connection.StartAsync();
        }

        public async Task JoinDashboard()
        {
            if (IsConnected)
                await Connection.InvokeAsync("JoinDashboard");
        }

        public async Task JoinBuilding(string building)
        {
            if (IsConnected)
                await Connection.InvokeAsync("JoinBuilding", building);
        }

        public async Task LeaveBuilding(string building)
        {
            if (IsConnected)
                await Connection.InvokeAsync("LeaveBuilding", building);
        }

        public async Task StopAsync()
        {
            if (Connection.State != HubConnectionState.Disconnected)
                await Connection.StopAsync();
        }
    }
}
