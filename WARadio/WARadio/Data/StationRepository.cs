using Dapper;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using WARadio.Model;

namespace WARadio.Data
{
    class StationRepository : BaseRepository
    {
        public Station GetStation(int id)
        {
            using (SQLiteConnection c = Connect())
            {
                c.Open();

                return c.Query<Station>("SELECT * FROM stations WHERE id=@id", new { id }).FirstOrDefault();
            }
        }

        public Stream GetStream(int id)
        {
            using (SQLiteConnection c = Connect())
            {
                c.Open();

                return c.Query<Stream>("SELECT * FROM streams WHERE station_id=@id", new { id }).FirstOrDefault();
            }
        }

        public List<Station> GetStationsByCountry(string country_code)
        {
            using (SQLiteConnection c = Connect())
            {
                c.Open();

                return c.Query<Station>("SELECT * FROM stations WHERE country=@country_code", new { country_code }).AsList();
            }
        }

        public List<Station> GetStationsByGenre(int id)
        {
            using (SQLiteConnection c = Connect())
            {
                c.Open();

                return c.Query<Station>("SELECT stations.* FROM stations LEFT JOIN station_genres ON stations.id=station_genres.station_id WHERE station_genres.genre_id=@id", new { id }).AsList();
            }
        }

        public List<Station> GetStationsByName(string name)
        {
            using (SQLiteConnection c = Connect())
            {
                c.Open();

                return c.Query<Station>(
                    "SELECT * FROM stations WHERE name LIKE @name",
                    new { name = "%" + name + "%" }).ToList();
            }
        }
    }
}
