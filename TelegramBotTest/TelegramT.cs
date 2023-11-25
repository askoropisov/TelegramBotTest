﻿using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotTest.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TelegramBotTest
{
    public class TelegramT
    {
        public readonly ITelegramBotClient bot = new TelegramBotClient("6746153930:AAFwNdo8EFHLpGYvS9u5PvoqGNV-Kyxa3NU");
        private readonly DBService _dBService;

        ReplyKeyboardMarkup Keyboard = new ReplyKeyboardMarkup(new[]
        {
            new[]
            {
                new KeyboardButton("Все фильмы"),
                new KeyboardButton("Удалить все")
            },
            new[]
            {
                new KeyboardButton("Дополнительные команды"),
                //new KeyboardButton("Удалить все")
            }
        });

        public TelegramT()
        {
            
        }

        public  ObservableCollection<Film> Films { get; set; } = new ObservableCollection<Film>();
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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
                        await botClient.SendTextMessageAsync(message.Chat, "Это тестовая версия бота - v 0.0.2. Ошибки возможны.\n");
                        await Command(botClient, message);
                        break;
                    case "все":
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
                    case "удалить":
                        await ClearAll(botClient, message);
                        break;
                    case "/command":
                        await Command(botClient, message);
                        break;
                    case "дополнительные":
                        await Command(botClient, message);
                        break;
                    case "скучаю":
                        await botClient.SendTextMessageAsync(message.Chat, "Ты у меня самая лучшая <3\n");
                        break;
                    default:
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Неизвестная команда");
                            break;
                        }
                }
            }
        }

        private  async Task Command(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendTextMessageAsync(message.Chat,
                            "Список доступных команд: \n" +
                            "+ \"название фильма\" - Добавить фильм, \n" +
                            "- \"название фильма\" - Удалить фильм, \n" +
                            "Check \"название фильма\" - Отметить фильм как просмотренный \n" +
                            "/Command - Получить список доступных команд \n", replyMarkup: Keyboard);
            return; 
        }

        private async Task AddItem(ITelegramBotClient botClient, Message message, int probel)
        {
            Film newFilm = new Film();
            newFilm.Name = message.Text.Substring(probel + 1).Trim();
            newFilm.Owner = message.From.FirstName;
            Films.Add(newFilm);

            DBService.Instance.Add(newFilm);
            await botClient.SendTextMessageAsync(message.Chat, string.Format("Фильм \"{0}\" добавлен", newFilm.Name), replyMarkup: Keyboard);
            return;
        }

        private  async Task RemoveItem(ITelegramBotClient botClient, Message message, int probel)
        {
            Film newFilm = new Film();
            newFilm.Name = message.Text.Substring(probel + 1).Trim();
            Films.Remove(newFilm);
            return;
        }

        private async Task Cheack(ITelegramBotClient botClient, Message message, int probel)
        {
            string name = message.Text.Substring(probel + 1).Trim();

            foreach (var f in Films)
            {
                if (f.Name == name) f.IsChecked = true;
            }
            return;
        }

        private async Task GetListFilms(ITelegramBotClient botClient, Message message)
        {

            string filmsList = string.Empty;
            var films = DBService.Instance.GetAll();

            if (films.Count < 1) await botClient.SendTextMessageAsync(message.Chat, "Список фильмов пока пуст =(", replyMarkup: Keyboard);
            else
            {
                foreach (var f in films)
                {
                    string isChecked = string.Empty;
                    if (f.IsChecked) isChecked = "✓"; else isChecked = "✕";

                    filmsList += (f.Name + " " + (isChecked + " " + f.Owner) + "\n");
                }
                await botClient.SendTextMessageAsync(message.Chat, filmsList, replyMarkup: Keyboard);
            }
            return;
        }

        private async Task ClearAll(ITelegramBotClient botClient, Message message)
        {
            Films.Clear();
            await botClient.SendTextMessageAsync(message.Chat, "Список фильмов очищен, скорее добавь новые!", replyMarkup: Keyboard);

            DBService.Instance.Clear();

            return;
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
    }
}