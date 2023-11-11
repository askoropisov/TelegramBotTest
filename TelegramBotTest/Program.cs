using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TelegramBotTest
{
    class Program
    {
        static readonly ITelegramBotClient bot = new TelegramBotClient("6746153930:AAFwNdo8EFHLpGYvS9u5PvoqGNV-Kyxa3NU");


        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                string command = message.Text.ToLower();

                switch (command)
                {
                    case "/start":
                        await botClient.SendTextMessageAsync(message.Chat, "Это тестовая версия бота. Ошибки возможны.\n");
                        await Command(botClient, message);
                        break;
                    case "/getlist":
                        break;
                    case "/additem":
                        break;
                    case "/removeitem":
                        break;
                    case "/checkitem":
                        break;
                    case "/command":
                        await Command(botClient, message);
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
                            "/AddItem - Добавить фильм, \n" +
                            "/RemoveItem - Удалить фильм, \n" +
                            "/CheckItem - Отметить фильм как просмотренный \n" +
                            "/Command - Получить список доступных команд \n");
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
