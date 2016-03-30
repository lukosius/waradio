using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace WARadio
{
    public class Database
    {
        /// <summary>
        /// SQLite connection object
        /// </summary>
        public SQLiteConnection Connection;

        /// <summary>
        /// SQLite database file
        /// </summary>
        public string SqliteFile;

        /// <summary>
        /// Class constructor
        /// 
        /// Initializes connection with the given connection string
        /// </summary>
        public Database(string file = null)
        {
            if (file != null)
            {
                SqliteFile = file;
            }
            else
            {
                SqliteFile = GetDBFilename();
            }

            Connection = new SQLiteConnection("Data Source=" + SqliteFile + ";Version=3;");
        }

        /// <summary>
        /// Checks whether SQLite database file exists
        /// </summary>
        /// <returns>true if database file exists, false otherwise</returns>
        public bool Exists()
        {
            return File.Exists(SqliteFile);
        }

        /// <summary>
        /// Creates SQLite database file
        /// </summary>
        public void Create()
        {
            using (SQLiteConnection db = new SQLiteConnection(Connection.ConnectionString))
            {
                db.Open();

                using (SQLiteTransaction tr = db.BeginTransaction())
                {
                    using (SQLiteCommand cmd = db.CreateCommand())
                    {
                        cmd.Transaction = tr;
                        cmd.CommandText = Properties.Resources.tables;
                        cmd.ExecuteNonQuery();
                    }

                    tr.Commit();
                }

                db.Close();
            }
        }

        /// <summary>
        /// Deletes database file
        /// </summary>
        public void Delete()
        {
            if (File.Exists(SqliteFile))
            {
                File.Delete(SqliteFile);
            }
        }

        /// <summary>
        /// Starts the connection with the database
        /// </summary>
        public void Start()
        {
            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }

            if (Exists() == false)
            {
                Create();
            }

            Connection.Open();
        }

        /// <summary>
        /// Stops the connection with the database
        /// </summary>
        public void Stop()
        {
            Connection.Close();
        }

        public string GetDBFilename()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WARadio\\waradio.sqlite");
        }
    }
}
