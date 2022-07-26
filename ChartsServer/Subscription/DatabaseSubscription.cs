using ChartsServer.Hubs;
using ChartsServer.Models;
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
                SatisDbContext context = new SatisDbContext();
                var data = (from personel in context.Personellers
                            join satis in context.Satislars
                            on personel.Id equals satis.PersonelId
                            select new { personel, satis }).ToList();

                List<object> datas = new List<object>();
                var personelIsimleri = data.Select(d => d.personel.Adi).Distinct().ToList();

                personelIsimleri.ForEach(p =>
                {
                    datas.Add(new
                    {
                        name = p,
                        data = data.Where(s => s.personel.Adi == p).Select(s => s.satis.Fiyat).ToList()
                    });
                });

                await hubContext.Clients.All.SendAsync("receiveMessage", datas);
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
