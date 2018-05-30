using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using FileService.Configuration;
using resources = FileService.Resources.Resource;

namespace FileService
{
    class Program
    {
        private static ServiceConfigurationSection _configuration;
        private static int _count = 1;

        static void Main(string[] args)
        {
            _configuration = (ServiceConfigurationSection)ConfigurationManager.GetSection("serviceConfiguration");
            CultureInfo.CurrentCulture = _configuration.Culture.Culture;
            CultureInfo.CurrentUICulture = _configuration.Culture.Culture;
            CultureInfo.DefaultThreadCurrentCulture = _configuration.Culture.Culture;
            CultureInfo.DefaultThreadCurrentUICulture = _configuration.Culture.Culture;

            Console.WriteLine(resources.Greeting);

            CheckDirectories();

            Setup();

            while (true)
            {
                Thread.Sleep(10000);
            }

        }

        public static void Setup()
        {
            Console.CancelKeyPress += Console_CancelKeyPress;

            foreach (FolderElement folder in _configuration.Folders)
            {
                var fileSystemWatcher = new FileSystemWatcher(folder.Path)
                {
                    EnableRaisingEvents = true
                };

                fileSystemWatcher.Created += FileSystemWatcher_Created;
                Console.WriteLine(resources.FolderRegistration, folder.Path);
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine(resources.AskConfirmationOfExit);
            if (Console.ReadLine()?.ToLower() == "y")
            {
                Console.WriteLine(resources.EndOfWork);
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(100);
            if (!File.Exists(e.FullPath)) return;

            Console.WriteLine(resources.NewFileFound, DateTime.Now.ToLongTimeString(), e.FullPath);
            var processed = false;
            foreach (RuleElement rule in _configuration.Rules)
            {
                if (!Regex.IsMatch(e.FullPath, rule.Pattern)) continue;
                Console.WriteLine(resources.PatternMatch, rule.Pattern);

                var newPath = (rule.AddOrder ? _count++ + " - " : "") +
                              (rule.AddDate ? DateTime.Now.ToShortDateString() + " - " : "") +
                              e.Name;
                Console.WriteLine(resources.FileMoved, Path.Combine(rule.DestinationFolder + "\\" + newPath));
                try
                {
                    File.Move(e.FullPath, Path.Combine(rule.DestinationFolder + "\\" + newPath));
                }
                catch (IOException ex)
                {
                    Console.WriteLine(resources.MoveError, ex.Message);
                    return;
                }
                processed = true;
                break;
            }

            if (processed) return;
            Console.WriteLine(resources.PatternNotFound, e.FullPath);
            Console.WriteLine(resources.FileMoved, Path.Combine(_configuration.DefaultFolder.Path + "\\" + e.Name));

            try
            {
                File.Move(e.FullPath, Path.Combine(_configuration.DefaultFolder.Path + "\\" + e.Name));
            }
            catch (IOException ex)
            {
                Console.WriteLine(resources.MoveError, ex.Message);
            }
        }

        public static void CheckDirectories()
        {
            if (!Directory.Exists(_configuration.DefaultFolder.Path))
            {
                Directory.CreateDirectory(_configuration.DefaultFolder.Path);
            }

            foreach (FolderElement folder in _configuration.Folders)
            {
                if (!Directory.Exists(folder.Path))
                {
                    Directory.CreateDirectory(folder.Path);
                }
            }

            foreach (RuleElement rule in _configuration.Rules)
            {
                if (!Directory.Exists(rule.DestinationFolder))
                {
                    Directory.CreateDirectory(rule.DestinationFolder);
                }
            }
        }
    }
}
