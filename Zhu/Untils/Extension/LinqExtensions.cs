using System.Collections.Generic;

namespace Zhu.Untils.Extension
{
    public static class LinqExtensions
    {
        public static ICollection<T> AddRange<T>(this ICollection<T> source, IEnumerable<T> addSource)
        {
            foreach (T item in addSource)
            {
                source.Add(item);
            }
            return source;
        }
    }

}
