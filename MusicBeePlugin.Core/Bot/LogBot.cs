using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace MusicBeePlugin.Core.Bot
{
    public class LogBot
    {
        public ObservableCollection<LogBotEntry> LogEntries { get; set; }

        public LogBot()
        {
            LogEntries = new ObservableCollection<LogBotEntry>();
        }

        public void AddAsync(LogBotEntry entry, Dispatcher uiDispatcher)
        {
            uiDispatcher.BeginInvoke(new Action(() =>
            {
                LogEntries.Add(entry);
            }));
        }
        public void Add(LogBotEntry entry)
        {
            LogEntries.Add(entry);
            OnNewEntry?.Invoke(this, entry);
        }
        public void Clear()
        {
            LogEntries.Clear();
        }

        public delegate void NewEntryHandler(LogBot sender, LogBotEntry newEntry);
        public event NewEntryHandler OnNewEntry;

        public List<string> GetStringEntries()
        {
            return LogEntries.Select(x => x.ToString()).ToList();
        }
        public void SaveToFile(string filePath)
        {
            File.WriteAllLines(filePath, GetStringEntries());
        }
    }
}
