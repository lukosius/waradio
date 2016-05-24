using System.Data.SQLite;

namespace WARadio.Data
{
    public class BaseRepository
    {
        public static string ConnectionString
        {
            get { return "Data source=waradio.db;Version=3"; }
        }

        public static SQLiteConnection Connect()
        {
            return new SQLiteConnection(ConnectionString);
        }
    }
}
