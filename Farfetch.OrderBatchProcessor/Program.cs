using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Farfetch.OrderBatchProcessor.DomainModel.Order;
using Farfetch.OrderBatchProcessor.Dtos.Structs;
using Farfetch.OrderBatchProcessor.Instrumentation.Logging;
using Ninject;

namespace Farfetch.OrderBatchProcessor
{
    internal class Program
    {
        private static StandardKernel _kernel;

        private static void Main(string[] args)
        {
            _kernel = NinjectBootstrapper.Get();

            Console.Title = typeof(Program).Name;

            WriteToConsole(Common.Contants.UI.Farfetch);
            WriteToConsole(Environment.NewLine);
            WriteToConsole(Environment.NewLine);
            WriteToConsole(Common.Contants.UI.Welcome);
            WriteToConsole(Common.Contants.UI.Help);

            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            while (true)
            {
                var consoleInput = ReadFromConsole();
                if (string.IsNullOrWhiteSpace(consoleInput)) continue;

                try
                {
                    string result = await ExecuteAsync(consoleInput);

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

        private static async Task<string> ExecuteAsync(string command)
        {
            string message;

            string path = string.Empty;

            if (command.StartsWith(ConsoleCommands.OrderBatch))
            {
                path = command.Replace(ConsoleCommands.OrderBatch, string.Empty).Trim();

                command = ConsoleCommands.OrderBatch;
            }

            switch (command)
            {
                case ConsoleCommands.HELP:
                    message = Common.Contants.UI.HelpCommands;
                    break;

                case ConsoleCommands.LOVE:
                    message = Common.Contants.UI.Love;
                    break;

                case ConsoleCommands.END:
                    message = Common.Contants.UI.Close;
                    break;

                case ConsoleCommands.OrderBatch:
                    var orderDomainModel = _kernel.Get<IOrderDomainModel>();

                    var orderLines = await orderDomainModel.GetOrderLinesFromDocumentAsync(path);

                    var orders = orderDomainModel.GetOrders(orderLines);

                    var boutiquesOrdersWithCommissions = orderDomainModel.CalculateBoutiquesOrdersCommissions(orders, commissionPercentage: 10);

                    var stringBuilder = new StringBuilder();

                    foreach (var boutique in boutiquesOrdersWithCommissions)
                    {
                        stringBuilder.Append($"{boutique.BoutiqueId},{boutique.TotalOrdersCommission}");
                        stringBuilder.Append(Environment.NewLine);
                    }

                    message = stringBuilder.ToString();

                    break;

                default:
                    message = Common.Contants.UI.UnknownCommand;
                    break;
            }

            return message;
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