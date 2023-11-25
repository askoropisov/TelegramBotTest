using System;
using System.IO;

namespace TelegramBotTest
{
    public class Paths
    {
        private static readonly string Root = Path.Combine(new[]
        {
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create),
            "TelegrammBot"
        });


        static Paths()
        {
            if (!Directory.Exists(Root))
                Directory.CreateDirectory(Root);
        }

        public static string Database = GetPath("Database.sqlite");

        private static string GetPath(string file)
        {
            return Path.Combine(new[] { Root, file });
        }

        public static string View(string view)
        {
            return "Amplituda.RaspberryPi.Celandine.Views." + view;
        }
    }
}
