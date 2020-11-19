using System.Collections.Generic;

namespace Memegenbot
{
    public static class Extensions
    {
        public static IEnumerable<T> Dequeue<T>(this Queue<T> queue, int count)
        {
            var i = 0;
            while (i < count && queue.TryDequeue(out var item))
            {
                i++;
                yield return item;
            }
        }
    }
}