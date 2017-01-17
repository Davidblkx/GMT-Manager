using AllMusicApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicBeePlugin.Core.Tools
{
    public static class Extensions
    {
        public static double ToDouble(this string str, bool throwError = false)
        {
            double val = -1;

            if (!double.TryParse(str, out val) && throwError)
                throw new InvalidCastException($"Can't convert {str} to number");

            return val;
        }
        public static long ToLong(this string str, bool throwError = false)
        {
            long val = -1;

            if (!long.TryParse(str, out val) && throwError)
                throw new InvalidCastException($"Can't convert {str} to number");

            return val;
        }
        public static int ToInt(this string str, bool throwError = false)
        {
            int val = -1;

            if (!int.TryParse(str, out val) && throwError)
                throw new InvalidCastException($"Can't convert {str} to number");

            return val;
        }

        public static IEnumerable<FrameworkElement> GetChildren(this Panel parent, bool recurse = true)
        {
            if (parent != null)
            {
                foreach (FrameworkElement elem in parent.Children)
                {
                    yield return elem;

                    if (recurse && elem is Panel)
                        foreach (var grandChild in ((Panel)elem).GetChildren(true))
                            yield return grandChild;

                    if (recurse && elem is GroupBox)
                        foreach (var grandChild in ((Panel)((GroupBox)elem).Content).GetChildren(true))
                            yield return grandChild;
                }
            }
        }

        public static int Count(this IGmtMedia media)
        {
            return
                (media.Genres?.Count ?? 0) +
                (media.Moods?.Count ?? 0) +
                (media.Themes?.Count ?? 0);
        }
    }
}
