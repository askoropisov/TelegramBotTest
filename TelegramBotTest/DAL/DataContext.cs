using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using TelegramBotTest.Services;

namespace TelegramBotTest.DAL
{
    public class DataContext : DbContext
    {
        private readonly PathsService _paths;

        public DataContext(PathsService paths)
        {
            _paths = paths;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = new SqliteConnectionStringBuilder { DataSource = Path.Combine(_paths.DbDirectory, "data.db") }.ToString();
                var connection = new SqliteConnection(connectionString);
                optionsBuilder.UseSqlite(connection);
                //optionsBuilder.LogTo(Console.WriteLine);
            }
        }

        public DbSet<Film> Operators => Set<Film>();
    }
}
