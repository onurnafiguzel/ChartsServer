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

        public DatabaseSubscription(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        SqlTableDependency<T> tableDependency;

        public void Configure(string tableName)
        {
            tableDependency = new SqlTableDependency<T>(configuration.GetConnectionString("SQL"), tableName);
            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();
        }

        ~DatabaseSubscription()
        {
            tableDependency.Stop();
        }

        private void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<T> e)
        {
            throw new NotImplementedException();
        }


        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
