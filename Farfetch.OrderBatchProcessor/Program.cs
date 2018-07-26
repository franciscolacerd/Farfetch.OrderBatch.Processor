using Farfetch.OrderBatchProcessor.DomainModel.Order;
using Farfetch.OrderBatchProcessor.Dtos.Structs;
using Farfetch.OrderBatchProcessor.Instrumentation.Logging;
using Ninject;
using System;
using System.Text;
using System.Threading;

namespace Farfetch.OrderBatchProcessor
{
    internal static class Program
    {
        private static StandardKernel _kernel;

        private static void Main()
        {
            _kernel = NinjectBootstrapper.Get();

            Console.Title = typeof(Program).Name;

            WriteToConsole(Common.Contants.UI.Farfetch);
            WriteToConsole(Environment.NewLine);
            WriteToConsole(Environment.NewLine);
            WriteToConsole(Common.Contants.UI.Welcome);
            WriteToConsole(Common.Contants.UI.Help);

            Run();
        }

        private static void Run()
        {
            while (true)
            {
                var consoleInput = ReadFromConsole();
                if (string.IsNullOrWhiteSpace(consoleInput)) continue;

                try
                {
                    string result = Execute(consoleInput);

                    WriteToConsole(result);

                    if (consoleInput == ConsoleCommands.END)
                    {
                        Thread.Sleep(3000);
                        Environment.Exit(0);
                    }
                }
                catch (Exception ex)
                {
                    var loggingManager = _kernel.Get<ILoggingManager>();
                    loggingManager.LogException(ex);
                    WriteToConsole(ex.Message);
                }
            }
        }

        private static string Execute(string command)
        {
            string path = string.Empty;

            if (command.StartsWith(ConsoleCommands.OrderBatch, StringComparison.CurrentCulture))
            {
                path = command.Split(' ')[1];

                command = ConsoleCommands.OrderBatch;
            }

            switch (command)
            {
                case ConsoleCommands.HELP:
                    return Common.Contants.UI.HelpCommands;

                case ConsoleCommands.LOVE:
                    return Common.Contants.UI.Love;

                case ConsoleCommands.END:
                    return Common.Contants.UI.Close;

                case ConsoleCommands.OrderBatch:
                    var orderDomainModel = _kernel.Get<IOrderDomainModel>();

                    var boutiquesOrdersWithCommissions = orderDomainModel.CalculateBoutiquesOrdersCommissions(path, commissionPercentage: 10);

                    var stringBuilder = new StringBuilder();

                    foreach (var boutique in boutiquesOrdersWithCommissions)
                    {
                        stringBuilder.Append(boutique.BoutiqueId).Append(",").Append(boutique.TotalOrdersCommission).Append(Environment.NewLine);
                    }

                    return stringBuilder.ToString();

                default:
                    return Common.Contants.UI.UnknownCommand;
            }
        }

        public static void WriteToConsole(string message = "")
        {
            if (message.Length > 0)
            {
                Console.WriteLine(message);
            }
        }

        private const string ReadPrompt = "console> ";

        public static string ReadFromConsole(string promptMessage = "")
        {
            Console.Write(ReadPrompt + promptMessage);
            return Console.ReadLine();
        }
    }
}