using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Telegram.Bot.Types;
using DryIoc.ImTools;

namespace TelegramBotTest.Models
{
    public class Db
    {
        private SqliteConnection _connection;

        public void Open()
        {
            if (!System.IO.File.Exists(Paths.Database))
                CreateDatabase();

            _connection = new SqliteConnection(GetConnectionString());
            _connection.Open();
        }


        private string GetConnectionString()
        {
            return $"Data Source={Paths.Database};";
        }

        public bool IsOpen => _connection?.State == ConnectionState.Open;

        public void Add(Film film)
        {
            using (var transact = _connection.BeginTransaction())
            {
                var query = new SqliteCommand("insert into films " +
                                              "(filmName, discription, link, owner, status) " +
                                              "values (@filmName, @discription, @link, @owner, @status)", _connection, transact);

                query.Parameters.Add(new SqliteParameter("@filmName", film.Name));
                query.Parameters.Add(new SqliteParameter("@discription", film.Discription)) ;
                query.Parameters.Add(new SqliteParameter("@link", film.URL));
                query.Parameters.Add(new SqliteParameter("@owner", film.Owner));
                query.Parameters.Add(new SqliteParameter("@status", film.IsChecked));

                query.ExecuteNonQuery();
                transact.Commit();
            }
        }

        public void CheckFilm(string film)
        {

            var command = new SqliteCommand("update films " +
                                            "set status = @status " +
                                            "where filmName = @filmName", _connection);

            command.Parameters.Add(new SqliteParameter("@status", true));
            command.Parameters.Add(new SqliteParameter("@filmName", film));

            var reader = command.ExecuteReader();
        }

        public void UnCheckFilm(string film)
        {

            var command = new SqliteCommand("update films " +
                                            "set status = @status " +
                                            "where filmName = @filmName", _connection);

            command.Parameters.Add(new SqliteParameter("@status", false));
            command.Parameters.Add(new SqliteParameter("@filmName", film));

            var reader = command.ExecuteReader();
        }

        public List<Film> GetAll()
        {
            var list = new List<Film>();

            var command = new SqliteCommand("select * from films;", _connection);

            var reader = command.ExecuteReader();
            foreach (DbDataRecord record in reader)
            {
                var film = new Film();

                film.Name = (string)(record["filmName"] ?? "");
                film.Discription = (string)(record["discription"] ?? "");
                film.URL = (string)(record["link"] ?? "");
                film.Owner = (string)(record["owner"] ?? "");
                film.IsChecked = Convert.ToBoolean(record["status"]);

                list.Add(film);
            }

            return list;
        }

        public void Clear()
        {
            var command = new SqliteCommand("delete from films", _connection);
            command.ExecuteNonQuery();
        }

        private void CreateDatabase()
        {
            var connection = new SqliteConnection(GetConnectionString());
            
            connection.Open();

            var command = new SqliteCommand("CREATE TABLE films " +
                                            "(" +
                                            "id INTEGER PRIMARY KEY," +
                                            "filmName TEXT," +
                                            "discription TEXT," +
                                            "link TEXT," +
                                            "owner TEXT," +
                                            "status INTEGER" +
                                            ")", connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        private static DateTime FromEpoch(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddMilliseconds(timestamp);
        }
    }
}
