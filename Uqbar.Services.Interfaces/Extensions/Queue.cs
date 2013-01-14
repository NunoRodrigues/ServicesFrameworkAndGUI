using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uqbar.Services.Framework
{
    public static partial class Extensions
    {
        public static void FixLength<T>(this ConcurrentQueue<T> queue, int max)
        {
            for (int i = queue.Count; i < max; i--)
            {
                T item;
                queue.TryDequeue(out item);
                item = default(T);
            }
        }

        public static T[] PeekRange<T>(this ConcurrentQueue<T> queue, int start, int length)
        {
            T[] result = new T[length];

            int begin = (queue.Count - 1) - start;
            for (int i = begin; (i >= 0 && i > begin - length); i--)
            {
                result[begin - i] = queue.ElementAt(i);
            }

            return result;
        }
    }
}
