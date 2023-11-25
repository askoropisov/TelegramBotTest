
using DryIoc;
using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Polling;
using TelegramBotTest.Services;

namespace TelegramBotTest
{
    public class Program
    {
        private static void InitializeDI()
        {
            var container = new Container();

            container.Register<DBService>(Reuse.Singleton);
            container.Register<TelegramT>(Reuse.Singleton);

        }

        static void Main(string[] args)
        {
            InitializeDI();

            TelegramT t = new TelegramT();

            Console.WriteLine("Запущен бот " + t.bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { },
            };

            t.bot.StartReceiving(
                t.HandleUpdateAsync,
                t.HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );

            Console.ReadLine();
        }
    }
}
