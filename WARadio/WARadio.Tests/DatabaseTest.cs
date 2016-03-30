using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Data.SQLite;
using System.Data;

namespace WARadio.Tests
{
    [TestClass]
    public class DatabaseTest
    {
        [TestCleanup]
        public void Cleanup()
        {
            File.Delete("test.sqlite");
        }

        [TestMethod]
        public void TestNewDatabase()
        {
            Database db = new Database();

            Assert.IsNotNull(db.Connection);
        }

        [TestMethod]
        public void TestDefaultConnectionString()
        {
            Database db = new Database();

            Assert.AreEqual("Data Source=" + db.GetDBFilename() + ";Version=3;", db.Connection.ConnectionString);
        }

        [TestMethod]
        public void TestConnectionString()
        {
            Database db = new Database("test.sqlite");

            Assert.AreEqual("Data Source=test.sqlite;Version=3;", db.Connection.ConnectionString);
        }

        [TestMethod]
        public void TestNotExists()
        {
            Database db = new Database("test.sqlite");

            Assert.IsFalse(db.Exists());
        }

        [TestMethod]
        public void TestCreateDB()
        {
            Database db = new Database("test.sqlite");

            db.Create();

            Assert.IsTrue(db.Exists());

            db.Start();

            using (SQLiteCommand cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'stations'", db.Connection))
            {
                using (SQLiteDataReader r = cmd.ExecuteReader())
                {
                    Assert.IsTrue(r.HasRows);
                }
            }

            db.Stop();
        }

        [TestMethod]
        public void TestDelete()
        {
            Database db = new Database("test.sqlite");

            db.Create();

            Assert.IsTrue(db.Exists());

            db.Delete();

            Assert.IsFalse(db.Exists());
        }

        [TestMethod]
        public void TestStart()
        {
            Database db = new Database("test.sqlite");

            db.Start();

            Assert.AreEqual(ConnectionState.Open, db.Connection.State);

            db.Stop();
        }

        [TestMethod]
        public void TestStop()
        {
            Database db = new Database("test.sqlite");

            db.Start();

            Assert.AreEqual(ConnectionState.Open, db.Connection.State);

            db.Stop();

            Assert.AreEqual(ConnectionState.Closed, db.Connection.State);
        }
    }
}
