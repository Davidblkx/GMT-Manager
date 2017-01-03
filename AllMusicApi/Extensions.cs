using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllMusicApi.Extensions
{
    public static class Extensions
    {
        public static string SubstringFromLastIndex(this string str, char value, int Offset = 0)
        {
            int index = str?.LastIndexOf(value) ?? -1;
            if (index == -1 || index + 1 >= str.Length) return str;

            return str.Substring(index + Offset);
        }

        public static int ToInt(this string str, bool throwException = false)
        {
            int x = -1;

            if (!int.TryParse(str, out x) && throwException)
                throw new InvalidCastException();

            return x;
        }

        public static string Join(this IEnumerable<char> charList)
        {
            return string.Concat(charList);
        }
    }
}
