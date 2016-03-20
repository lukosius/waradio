using System;
using System.Data.SQLite;
using System.IO;

namespace WARadio
{
    class Database
    {
        /// <summary>
        /// SQLite database file
        /// </summary>
        public string DatabaseFilename
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WARadio\\waradio.sqlite");
            }
        }

        /// <summary>
        /// SQLite connection string
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return "Data Source=" + DatabaseFilename + ";Version=3;";
            }
        }

        /// <summary>
        /// SQLite connection object
        /// </summary>
        public SQLiteConnection Connection
        {
            get
            {
                return _db;
            }
        }

        /// <summary>
        /// SQLite connection object
        /// </summary>
        private SQLiteConnection _db;

        /// <summary>
        /// Class constructor
        /// </summary>
        public Database()
        {
            if (!DatabaseExists())
                CreateDatabase();

            _db = new SQLiteConnection(ConnectionString);
            _db.Open();
        }

        /// <summary>
        /// Determines if SQLite database file exists
        /// </summary>
        /// <returns>true if DB exists, false otherwise</returns>
        public bool DatabaseExists()
        {
            return File.Exists(DatabaseFilename);
        }

        /// <summary>
        /// Creates SQLite database
        /// </summary>
        private void CreateDatabase()
        {
            string path = Path.GetDirectoryName(DatabaseFilename);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            SQLiteConnection.CreateFile(DatabaseFilename);
            CreateTables();
        }

        /// <summary>
        /// Create SQLite database tables
        /// </summary>
        private void CreateTables()
        {
            using (SQLiteConnection db = new SQLiteConnection(ConnectionString))
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
    }
}
