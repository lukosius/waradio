using Dapper;
using System.Collections.Generic;
using System.Data.SQLite;
using WARadio.Model;

namespace WARadio.Data
{
    class CountryRepository : BaseRepository
    {
        public List<Country> GetCountries()
        {
            using (SQLiteConnection c = Connect())
            {
                c.Open();

                return c.Query<Country>("SELECT * FROM countries").AsList();
            }
        }
    }
}
