using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotTest
{
    class Program
    {
        static readonly ITelegramBotClient bot = new TelegramBotClient("6746153930:AAFwNdo8EFHLpGYvS9u5PvoqGNV-Kyxa3NU");

        public static ObservableCollection<Film> Films { get; set; } = new ObservableCollection<Film>();


        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                string command = message.Text.ToLower();
                int probel = command.IndexOf(" ");
                if (probel > 0)
                {
                    command = command.Substring(0, probel);
                }

                switch (command)
                {
                    case "/start":
                        await botClient.SendTextMessageAsync(message.Chat, "Это тестовая версия бота. Ошибки возможны.\n");
                        await Command(botClient, message);
                        break;
                    case "/getlist":
                        await GetListFilms(botClient, message);
                        break;
                    case "+":
                        await AddItem(botClient, message, probel);
                        break;
                    case "-":
                        await RemoveItem(botClient, message, probel);
                        break;
                    case "check":
                        await Cheack(botClient, message, probel);
                        break;
                    case "/clear":
                        await ClearAll(botClient, message);
                        break;
                    case "/command":
                        await Command(botClient, message);
                        break;
                    case "/secret":
                        await botClient.SendTextMessageAsync(message.Chat, "Ты у меня самая лучшая\n");
                        break;
                    default:
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Неизвестная команда");
                        break;
                    }
                }
            }
        }

        private static async Task Command(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendTextMessageAsync(message.Chat,
                            "Список доступных команд: \n" +
                            "/GetList - Получить список фильмов, \n" +
                            "+ \"название фильма\" - Добавить фильм, \n" +
                            "- \"название фильма\" - Удалить фильм, \n" +
                            "Check \"название фильма\" - Отметить фильм как просмотренный \n" +
                            "/Clear - Удалить все \n" +
                            "Secret - <3 \n" +
                            "/Command - Получить список доступных команд \n");
            return;
        }

        private static async Task AddItem(ITelegramBotClient botClient, Message message, int probel)
        {
            Film newFilm = new Film();
            newFilm.Name = message.Text.Substring(probel + 1);
            newFilm.Owner = message.From.FirstName;
            Films.Add(newFilm);
            return;
        }

        private static async Task RemoveItem(ITelegramBotClient botClient, Message message, int probel)
        {
            Film newFilm = new Film();
            newFilm.Name = message.Text.Substring(probel + 1);
            Films.Remove(newFilm);
            return;
        }

        private static async Task Cheack(ITelegramBotClient botClient, Message message, int probel)
        {
            string name = message.Text.Substring(probel + 1);

            foreach (var f in Films)
            {
                if (f.Name == name) f.IsChecked = true;
            }
            return;
        }

        private static async Task GetListFilms(ITelegramBotClient botClient, Message message)
        {
            string films = string.Empty;
            if (Films.Count < 1) await botClient.SendTextMessageAsync(message.Chat, "Список фильмов пока пуст =(");
            else
            {
                foreach (var f in Films)
                {
                    string isChecked = string.Empty;
                    if (f.IsChecked) isChecked = "✓"; else isChecked = "✕";

                    films += (f.Name +" " + (isChecked + " " + f.Owner) + "\n");
                }
                await botClient.SendTextMessageAsync(message.Chat, films);
            }
            return;
        }

        private static async Task ClearAll(ITelegramBotClient botClient, Message message)
        {
            Films.Clear();
            await botClient.SendTextMessageAsync(message.Chat, "Список фильмов очищен, скорее добавь новые!");
            return;
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}
