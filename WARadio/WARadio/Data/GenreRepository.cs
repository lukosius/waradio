using Dapper;
using System.Collections.Generic;
using System.Data.SQLite;
using WARadio.Model;

namespace WARadio.Data
{
    class GenreRepository : BaseRepository
    {
        public List<Genre> GetGenres()
        {
            using (SQLiteConnection c = Connect())
            {
                c.Open();

                return c.Query<Genre>("SELECT * FROM genres ORDER BY title ASC").AsList();
            }
        }
    }
}
