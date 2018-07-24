namespace Farfetch.OrderBatchProcessor.Common.Helpers
{
    public static class StringHelpers
    {
        public static string SanitizeTextFile(string text)
        {
            if (text.Contains("\r\n"))
            {
                text = text.Replace("\r\n", "\n");
            }

            return text;
        }
    }
}