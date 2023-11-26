using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotTest.Services;
using static System.Net.WebRequestMethods;

namespace TelegramBotTest
{
    public class TelegramT
    {
        public readonly ITelegramBotClient bot = new TelegramBotClient("6746153930:AAFwNdo8EFHLpGYvS9u5PvoqGNV-Kyxa3NU");

        ReplyKeyboardMarkup Keyboard = new ReplyKeyboardMarkup(new[]
        {
            new[]
            {
                new KeyboardButton("Все фильмы"),
                new KeyboardButton("Рекомендуемые")
            },
            new[]
            {
                new KeyboardButton("Непросмотренные"),
                new KeyboardButton("Дополнительные команды")
            }
        });

        public TelegramT()
        {

        }

        public ObservableCollection<Film> Films { get; set; } = new ObservableCollection<Film>();
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                string command = string.Empty;
                var message = update.Message;
                int probel = 0;

                try
                {
                    command = message.Text.ToLower();
                    probel = command.IndexOf(" ");
                    if (probel > 0)
                    {
                        command = command.Substring(0, probel);
                    }
                }
                catch (Exception ex)
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Неизвестная команда");
                }

                switch (command)
                {
                    case "/start":
                        await botClient.SendTextMessageAsync(message.Chat, "Это тестовая версия бота - v 0.0.3. Ошибки возможны.\n");
                        await Command(botClient, message);
                        break;
                    case "все":
                        await GetListFilms(botClient, message);
                        break;
                    case "непросмотренные":
                        await GetUnChecked(botClient, message);
                        break;
                    case "+":
                        await AddItem(botClient, message, probel);
                        break;
                    case "-":
                        await RemoveItem(botClient, message, probel);
                        break;
                    case ">":
                        await Cheack(botClient, message, probel);
                        break;
                    case "!":
                        await Like(botClient, message, probel);
                        break;
                    case "<":
                        await UnCheack(botClient, message, probel);
                        break;
                    case "описание":
                        await GetDiscription(botClient, message, probel);
                        break;
                    case "трейлер":
                        await GetTrailer(botClient, message, probel);
                        break;
                    case "remove":
                        await ClearAll(botClient, message);
                        break;
                    case "/command":
                        await Command(botClient, message);
                        break;
                    case "дополнительные":
                        await Command(botClient, message);
                        break;
                    case "рекомендуемые":
                        await GetRecomended(botClient, message);
                        break;
                    case "скучаю":
                        await botClient.SendTextMessageAsync(message.Chat, "Ты у меня самая лучшая <3\n");
                        break;
                    case "егор":
                        await botClient.SendTextMessageAsync(message.Chat, "Где журнал!?\n");
                        break;
                    default:
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Неизвестная команда");
                            break;
                        }
                }
            }
        }

        private async Task Command(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendTextMessageAsync(message.Chat,
                            "Список доступных команд: \n" +
                            "+ \"название фильма\" - Добавить фильм, \n" +
                            "- \"название фильма\" - Удалить фильм, \n" +
                            "> \"название фильма\" - Отметить фильм как просмотренный \n" +
                            "< \"название фильма\" - Отметить фильм как непросмотренный \n" +
                            "! \"название фильма\" - Отметить фильм как рекомендуемый \n" +
                            "Описание \"название фильма\" - Показать описание фильма \n" +
                            "Трейлер \"название фильма\" - Найти трейлер фильма \n" +
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
            await botClient.SendTextMessageAsync(message.Chat, string.Format("Фильм \"{0}\" добавлен в список", newFilm.Name), replyMarkup: Keyboard);
            return;
        }

        private async Task RemoveItem(ITelegramBotClient botClient, Message message, int probel)
        {
            //Добавить проверку на наличие фильма в БД
            Film newFilm = new Film();
            newFilm.Name = message.Text.Substring(probel + 1).Trim();
            Films.Remove(newFilm);

            DBService.Instance.Remove(message.Text.Substring(probel + 1).Trim());
            await botClient.SendTextMessageAsync(message.Chat, string.Format("Фильм \"{0}\" удален из списка", newFilm.Name), replyMarkup: Keyboard);
            return;
        }

        private async Task Cheack(ITelegramBotClient botClient, Message message, int probel)
        {
            string name = message.Text.Substring(probel + 1).Trim();

            DBService.Instance.CheckFilm(name);
            await botClient.SendTextMessageAsync(message.Chat, string.Format("Фильм \"{0}\" отмечен как просмотренный", name), replyMarkup: Keyboard);
            return;
        }

        private async Task Like(ITelegramBotClient botClient, Message message, int probel)
        {
            string name = message.Text.Substring(probel + 1).Trim();

            DBService.Instance.LikeFilm(name);
            await botClient.SendTextMessageAsync(message.Chat, string.Format("Фильм \"{0}\" отмечен как рекомендуемый", name), replyMarkup: Keyboard);
            return;
        }

        private async Task UnCheack(ITelegramBotClient botClient, Message message, int probel)
        {
            string name = message.Text.Substring(probel + 1).Trim();

            DBService.Instance.UnCheckFilm(name);
            await botClient.SendTextMessageAsync(message.Chat, string.Format("Фильм \"{0}\" отмечен как непросмотренный", name), replyMarkup: Keyboard);
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
                    string isRecomended = string.Empty;
                    if (f.IsChecked) isChecked = "✅"; else isChecked = "❌";
                    if (f.IsRecomended) isRecomended = "❤️";;

                    filmsList += ( isRecomended + f.Name + " " + (isChecked + " " + f.Owner) + "\n");
                }
                await botClient.SendTextMessageAsync(message.Chat, filmsList, replyMarkup: Keyboard);
            }
            return;
        }

        private async Task GetUnChecked(ITelegramBotClient botClient, Message message)
        {

            string filmsList = string.Empty;
            var films = DBService.Instance.GetNotChecked();

            if (films.Count < 1) await botClient.SendTextMessageAsync(message.Chat, "У вас нет непросмотренных фильмов", replyMarkup: Keyboard);
            else
            {
                foreach (var f in films)
                {
                    filmsList += (f + "\n");
                }
                await botClient.SendTextMessageAsync(message.Chat, filmsList, replyMarkup: Keyboard);
            }
            return;
        }

        private async Task GetRecomended(ITelegramBotClient botClient, Message message)
        {

            string filmsList = string.Empty;
            var films = DBService.Instance.GetRecomended();

            if (films.Count < 1) await botClient.SendTextMessageAsync(message.Chat, "У вас нет рекомендуемых фильмов", replyMarkup: Keyboard);
            else
            {
                foreach (var f in films)
                {
                    filmsList += (f + "\n");
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

        private async Task GetDiscription(ITelegramBotClient botClient, Message message, int probel)
        {
            string name = message.Text.Substring(probel + 1, message.Text.Length - (probel + 1)).Trim();
            string link = "https://ya.ru/search/?text=" + "описание фильма " + name;
            link = link.Replace(" ", "+");
            await botClient.SendTextMessageAsync(message.Chat, string.Format("Вот описание фильма \"{0}\" \n {1} ", name, link), replyMarkup: Keyboard);
        }

        private async Task GetTrailer(ITelegramBotClient botClient, Message message, int probel)
        {
            string name = message.Text.Substring(probel + 1, message.Text.Length - (probel + 1)).Trim();
            string link = "https://www.youtube.com/results?search_query=" + name + " трейлер";
            link = link.Replace(" ", "+");
            await botClient.SendTextMessageAsync(message.Chat, string.Format("Вот трейлер фильма \"{0}\" \n {1} ", name, link), replyMarkup: Keyboard);
        }



        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
    }
}
