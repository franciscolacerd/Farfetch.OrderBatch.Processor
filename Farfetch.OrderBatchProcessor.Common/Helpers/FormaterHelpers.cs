using System.Globalization;
using Farfetch.OrderBatchProcessor.Common.Exceptions;

namespace Farfetch.OrderBatchProcessor.Common.Helpers
{
    public static class FormaterHelpers
    {
        public static decimal ConvertFromStringToDecimal(string value)
        {
            var success = decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out var decimalValue);

            if (!success)
            {
                throw new InvalidAmountFormatException($"{Contants.Exceptions.InvalidAmountFormat}{value}");
            }

            return decimalValue;
        }
    }
}