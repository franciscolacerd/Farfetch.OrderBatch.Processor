using System.Collections.Generic;
using System.Linq;

namespace Farfetch.OrderBatchProcessor.Common.Helpers
{
    public static class CollectionsHelpers
    {
        public static IEnumerable<T> DropLast<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Take(enumerable.Count() - 1);
        }
    }
}