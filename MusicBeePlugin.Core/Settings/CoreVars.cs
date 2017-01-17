using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Core
{
    public static class CoreVars
    {

        public static string PluginFolder { get{ return "GMTManager";} }
        public static string SettingsFile { get { return "Settings.json"; } }
        public static string CacheFile { get { return "Cache.bin"; } }
        public static string BotLogFile { get { return "Bot_Log.txt"; } }

        public static string GetPluginFolderPath(string rootFolder)
        {
            var path = Path.Combine(rootFolder, PluginFolder);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
        public static string GetFilePath(string rootFolder, string fileName)
        {
            return Path.Combine(GetPluginFolderPath(rootFolder), fileName);
        }
    }
}
