using System;
using System.Text;

namespace Farfetch.OrderBatchProcessor.Common
{
    public class Contants
    {
        public class ApplicationName
        {
            public const string Name = "Farfetch.OrderBatchProcessor";
        }

        public class TraceSwitch
        {
            public const string Name = "Farfetch.OrderBatchProcessor.TraceSwitch";

            public const string Description = "TraceSwitch for Logging with configuration at system.diagnostics";

            public const string Source = "Farfetch.OrderBatchProcessor";

            public const string Log = "Farfetch.OrderBatchProcessor.Log";

            public const string Application = "Application";
        }

        public class Path
        {
            public const string Debug = "\\bin\\Debug";

            public const string CsvFile = "\\Files\\sample.orders.csv";

            public const string CsvFileWithOneLine = "\\Files\\sample.orders.oneline.csv";

            public const string CsvFileWithInvalidLine = "\\Files\\sample.orders.invalidline.csv";

            public const string CsvFileWithInvalidAmountFormat = "\\Files\\sample.orders.invalidamountformat.csv";

            public const string CsvFileWithNoLines = "\\Files\\sample.orders.nolines.csv";

            public const string InvalidFileFormat = "\\Files\\sample.orders.xpto";
        }

        public class Exceptions
        {
            public const string MustBeCsv = "File must be a CSV.";

            public const string FileNotFound = "File not found. Use path like C:\\temp\\sample.orders.csv";

            public const string FileMustHaveLines = "File must have lines.";

            public const string InvalidAmountFormat = "Amount is in invalid format: ";

            public const string NoValidOrderFormat = "Order must be in <Boutique_ID>,<Order_ID>,<TotalOrderPrice> format at Line: ";
        }

        public class UI
        {
            public static string Farfetch
            {
                get
                {
                    var stringBuilder = new StringBuilder();

                    stringBuilder.Append("                   ");
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append("   ______          __     _       _      ");
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append("  |  ____|        / _|   | |     | |     ");
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append("  | |__ __ _ _ __| |_ ___| |_ ___| |__   ");
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append("  |  __/ _` | '__|  _/ _ \\ __/ __| '_ \\  ");
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append("  | | | (_| | |  | ||  __/ || (__| | | | ");
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append("  |_|  \\__,_|_|  |_| \\___|\\__\\___|_| |_| ");
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append("                                         ");
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append("                                         ");

                    return stringBuilder.ToString();
                }
            }

            public const string Welcome = "Welcome to Farfetch Order Batch Processor.";

            public const string UnknownCommand = "Unknown Command. For more information on a specific command, type HELP command-name";

            public const string Help = "For more information on a specific command, type HELP command-name";

            public const string Love = "<3 <3 <3 <3";

            public static string Close = "See you next time. Farfetch @ " + DateTime.Now.Year;

            public static string HelpCommands
            {
                get
                {
                    var stringBuilder = new StringBuilder();

                    stringBuilder.Append("HELP                               Provides Help information for app commands.");
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append("OrderBatch <Path_to_orders_file>   Processes Orders Batch File.");
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append("LOVE                               Show developer some love.");
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append("END                                End session and close console.");
                    stringBuilder.Append(Environment.NewLine);

                    return stringBuilder.ToString();
                }
            }
        }
    }
}