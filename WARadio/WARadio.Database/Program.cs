using Newtonsoft.Json;
using System;
using System.Data.SQLite;
using System.IO;
using System.Net;

namespace WARadio.Database
{
    class Program
    {
        /// <summary>
        /// API token
        /// </summary>
        private static readonly string token = "ac97c22adda845f8329a221197";

        static void Main(string[] args)
        {
            Console.WriteLine("Downloading data from Dirble API...");

            using (SQLiteConnection db = new SQLiteConnection("Data Source=waradio.db;Version=3;"))
            {
                db.Open();
                
                using (WebClient Client = new WebClient())
                {
                    using (SQLiteTransaction tr = db.BeginTransaction())
                    {
                        using (SQLiteCommand cmd = db.CreateCommand())
                        {
                            cmd.Transaction = tr;

                            Console.Write(" - countries");
                            cmd.CommandText = @"CREATE TABLE `countries` (
     `country_code` TEXT NOT NULL,
     `name` TEXT NOT NULL,
     `region` TEXT NOT NULL,
     `subregion` TEXT NOT NULL,
     PRIMARY KEY(country_code)
 ) WITHOUT ROWID";
                            cmd.ExecuteNonQuery();

                            string data = Client.DownloadString("http://api.dirble.com/v2/countries?token=" + token);
                            dynamic countries = JsonConvert.DeserializeObject(data);

                            foreach (dynamic country in countries)
                            {
                                cmd.CommandText = "INSERT INTO countries VALUES (@code, @name, @region, @subregion)";
                                cmd.Parameters.AddWithValue("code", country.country_code);
                                cmd.Parameters.AddWithValue("name", country.name);
                                cmd.Parameters.AddWithValue("region", country.region);
                                cmd.Parameters.AddWithValue("subregion", country.subregion);
                                cmd.ExecuteNonQuery();
                            }

                            Console.WriteLine("\r + countries");


                            Console.Write(" - genres");
                            cmd.CommandText = @"CREATE TABLE `genres` (
     `id` INTEGER NOT NULL,
     `title` TEXT NOT NULL,
     `description`   TEXT,
     `slug`  TEXT NOT NULL,
     `ancestry`  TEXT,
     PRIMARY KEY(id)
 ) WITHOUT ROWID";
                            cmd.ExecuteNonQuery();

                            data = Client.DownloadString("http://api.dirble.com/v2/categories?token=" + token);
                            dynamic genres = JsonConvert.DeserializeObject(data);

                            foreach (dynamic genre in genres)
                            {
                                cmd.CommandText = "INSERT INTO genres VALUES (@id, @title, @description, @slug, @ancestry)";
                                cmd.Parameters.AddWithValue("id", genre.id);
                                cmd.Parameters.AddWithValue("title", genre.title);
                                cmd.Parameters.AddWithValue("description", genre.description);
                                cmd.Parameters.AddWithValue("slug", genre.slug);
                                cmd.Parameters.AddWithValue("ancestry", genre.ancestry);
                                cmd.ExecuteNonQuery();
                            }

                            Console.WriteLine("\r + genres");




                            Console.Write(" - stations");
                            cmd.CommandText = @"CREATE TABLE `stations` (
    `id` INTEGER NOT NULL,
    `name` TEXT NOT NULL,
    `country` TEXT NOT NULL,
    `image` TEXT,
    `slug` TEXT NOT NULL,
    `website` TEXT,
     PRIMARY KEY(id)
) WITHOUT ROWID";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = @"CREATE TABLE `streams` (
    `station_id` INTEGER NOT NULL,
    `stream` TEXT NOT NULL,
    `bitrate` INTEGER NOT NULL,
    `content_type` TEXT
)";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = @"CREATE TABLE `station_genres` (
    `station_id` INTEGER NOT NULL,
    `genre_id` INTEGER NOT NULL
)";
                            cmd.ExecuteNonQuery();

                            int page = 1;
                            while (true)
                            {
                                data = Client.DownloadString("http://api.dirble.com/v2/stations?token=" + token + "&per_page=30&page=" + page);

                                if (data.Equals("[]") || page > 500)
                                    break;
                                
                                dynamic stations = JsonConvert.DeserializeObject(data);

                                foreach (dynamic station in stations)
                                {
                                    cmd.CommandText = "INSERT INTO stations VALUES (@id, @name, @country, @image, @slug, @website)";
                                    cmd.Parameters.AddWithValue("id", station.id);
                                    cmd.Parameters.AddWithValue("name", station.name);
                                    cmd.Parameters.AddWithValue("country", station.country);
                                    cmd.Parameters.AddWithValue("image", station.image.url);
                                    cmd.Parameters.AddWithValue("slug", station.slug);
                                    cmd.Parameters.AddWithValue("website", station.website);
                                    cmd.ExecuteNonQuery();

                                    foreach (dynamic streams in station.streams)
                                    {
                                        if (streams.status == 0)
                                            continue;

                                        cmd.CommandText = "INSERT INTO streams VALUES (@id, @stream, @bitrate, @content_type)";
                                        cmd.Parameters.AddWithValue("id", station.id);
                                        cmd.Parameters.AddWithValue("stream", streams.stream);
                                        cmd.Parameters.AddWithValue("bitrate", streams.bitrate);
                                        cmd.Parameters.AddWithValue("content_type", streams.content_type);
                                        cmd.ExecuteNonQuery();
                                    }

                                    foreach (dynamic sgenres in station.categories)
                                    {
                                        cmd.CommandText = "INSERT INTO station_genres VALUES (@station, @genre)";
                                        cmd.Parameters.AddWithValue("station", station.id);
                                        cmd.Parameters.AddWithValue("genre", sgenres.id);
                                        cmd.ExecuteNonQuery();
                                    }
                                }

                                ++page;
                            }

                            Console.WriteLine("\r + stations");
                        }

                        tr.Commit();
                    }
                }
            }
        }
    }
}
