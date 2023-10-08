using System;
using System.Collections.Generic;
using System.Linq;

namespace DimaDevi.Libs.Extensions
{
    public static class ArrayExt
    {
        public static bool IsEqual<T>(this T[] a, T[] b) where T : struct
        {
            if (a.Length != b.Length)
                return false;
            for (int i = 0; i < a.Length; i++)
                if ((object)a[i] != (object)b[i])
                    return false;
            return true;
        }
        public static bool IsEqual(this byte[] f, byte[] s)
        {
            if (f.Length != s.Length)
                return false;
            for (int i = 0; i < f.Length; i++)
                if (f[i] != s[i])
                    return false;
            return true;
        }

        public static byte[] Combine(this byte[] first, byte[] second)
        {
            byte[] bytes = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            return bytes;
        }

        public static IEnumerable<Enum> GetFlags(this Enum e)
        {
            return Enum.GetValues(e.GetType()).Cast<Enum>().Where(e.HasFlag);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
                if (seenKeys.Add(keySelector(element)))
                    yield return element;
        }
    }
}
