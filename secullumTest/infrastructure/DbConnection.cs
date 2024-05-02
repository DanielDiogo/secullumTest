using Npgsql;

namespace secullumTest.infrastructure {

    public class DbConnection : IDisposable {
        public DbConnection() {
            Connection = new NpgsqlConnection("Server=localhost;Port=5432;Database=pointrecord;Username=postgres;Password=12;timeout=1000;");
            Connection.Open();
        }

        public NpgsqlConnection Connection { get; set; }
        public void Dispose() {
            Connection.Dispose();
        }
    }
}