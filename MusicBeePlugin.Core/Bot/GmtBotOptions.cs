using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MusicBeePlugin.Core.Bot
{
    public class GmtBotOptions
    {
        /// <summary>
        /// Mode to save tags: 0 - TestMode, 1 - OnlyEmptyFields, 
        ///                    2 - Add     , 3 - ReplaceAllTags
        /// </summary>
        public int FillTagsMode { get; set; }

        public bool UseAlbumTags { get; set; }
        public bool UseArtistTags { get; set; }

        /// <summary>
        /// Search priority:
        /// 0 - Album, 1 - Artist, 2 - Both
        /// </summary>
        public int TagPriority { get; set; }

        public bool UsePersistentCache { get; set; }

        public bool LimitCacheAge { get; set; }
        public double CacheMaxAge { get; set; }

        public bool LimitCacheSize { get; set; }
        public long CacheMaxSize { get; set; }

        public int AlgorithmTolerance { get; set; }

        public override string ToString()
        {
            string output = "";

            var properties = typeof(GmtBotOptions).GetProperties();
            foreach (PropertyInfo p in properties)
                output += $"{p.Name}:{p.GetValue(this, null)}\n";

            return output;
        }
    }
}
