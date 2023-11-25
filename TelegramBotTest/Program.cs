
using DryIoc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
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
                AllowedUpdates = { }, // receive all update types
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
