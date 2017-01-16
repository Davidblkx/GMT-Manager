using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Core.Bot
{
    public class LogBotEntry
    {
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public LogBotEntryLevel Level { get; set; }

        public LogBotEntry()
        {
            Time = DateTime.Now;
            Message = "Nothing to Show";
            Level = LogBotEntryLevel.Debug;
        }
        public LogBotEntry(string message)
        {
            Message = message;
            Time = DateTime.Now;
            Level = LogBotEntryLevel.Info;
        }
        public LogBotEntry(string message, LogBotEntryLevel level)
        {
            Message = message;
            Time = DateTime.Now;
            Level = level;

        }

        public override string ToString()
        {
            string time = Time.ToString("dd-MM-yyyy|hh:mm:ss");
            return $"[{Level.ToString()}][{time}]{Message}";
        }
    }
}
