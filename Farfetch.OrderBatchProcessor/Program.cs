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
    using System.Linq;

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
                    string result = await ExecuteAsync(consoleInput).ConfigureAwait(false);

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
            string path = string.Empty;

            if (command.StartsWith(ConsoleCommands.OrderBatch, StringComparison.Ordinal))
            {
                path = command.Replace(ConsoleCommands.OrderBatch, string.Empty).Trim();

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

                    var orderLines = await orderDomainModel.GetOrderLinesFromDocumentAsync(path).ConfigureAwait(false);

                    var orders = orderDomainModel.GetOrders(orderLines);

                    var boutiquesOrdersWithCommissions = orderDomainModel.CalculateBoutiquesOrdersCommissions(orders.ToList(), commissionPercentage: 10);

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