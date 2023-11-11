using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotTest.Services
{
    public class PathsService
    {
        private readonly string _rootDirectory;
        private readonly string _configDirectory;

        public PathsService(string app = "TelegramBotTest")
        {
            _rootDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Amplituda", app);
            _configDirectory = Path.Combine(_rootDirectory, "Configs");
        }

        public string ConfigDirectory => _configDirectory;
        public string DbDirectory => _rootDirectory;

        public Task StartAsync()
        {
            Directory.CreateDirectory(_rootDirectory);
            Directory.CreateDirectory(_configDirectory);

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            return Task.CompletedTask;
        }
    }
}
