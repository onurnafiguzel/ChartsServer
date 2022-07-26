using ChartsServer.Hubs;
using Microsoft.AspNetCore.SignalR;
using TableDependency.SqlClient;

namespace ChartsServer.Subscription
{
    public interface IDatabaseSubscription
    {
        void Configure(string tableName);
    }

    public class DatabaseSubscription<T> : IDatabaseSubscription where T : class, new()
    {
        IConfiguration configuration;
        IHubContext<SatisHub> hubContext;

        public DatabaseSubscription(IConfiguration configuration, IHubContext<SatisHub> hubContext)
        {
            this.configuration = configuration;
            this.hubContext = hubContext;
        }

        SqlTableDependency<T> tableDependency;

        public void Configure(string tableName)
        {
            tableDependency = new SqlTableDependency<T>(configuration.GetConnectionString("SQL"), tableName);
            tableDependency.OnChanged += async (o, e) =>
            {
                await hubContext.Clients.All.SendAsync("receiveMessage", "Deneme");
            };
            tableDependency.OnError += (o, e) =>
            {

            };
            tableDependency.Start();
        }

        ~DatabaseSubscription()
        {
            tableDependency.Stop();
        }


    }
}
