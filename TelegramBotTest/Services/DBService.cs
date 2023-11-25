using TelegramBotTest.Models;

namespace TelegramBotTest.Services
{
    public class DBService
    {
        private static Db _database;

        public static Db Instance
        {
            get
            {
                if (_database == null)
                    _database = new Db();

                if (!_database.IsOpen)
                    _database.Open();

                return _database;
            }
        }

    }
}
