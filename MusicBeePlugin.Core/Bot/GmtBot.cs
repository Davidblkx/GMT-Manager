using System;
using System.Collections.Generic;
using System.IO;
using AllMusicApi;
using System.Linq;
using AllMusicApi.Model;
using System.Threading.Tasks;
using System.Windows.Threading;
using MusicBeePlugin.Core.Tools;

namespace MusicBeePlugin.Core.Bot
{
    public class GmtBot
    {
        private List<TrackFile> _files;
        private List<TrackFile> _filesToUpdate;
        private string _cacheFile;
        private bool _cancelProgress;
        private Dispatcher _uiDispatcher;

        public CacheBot Cache { get; private set; }
        public LogBot Logger { get; private set; }
        public GmtBotOptions Options { get; private set; }

        public void CancelProgress()
        {
            _cancelProgress = true;
            Logger.Add(new LogBotEntry
            {
                Level = LogBotEntryLevel.Warning,
                Message = "Cancellation request receive",
                Time = DateTime.Now
            });
        }

        public IEnumerable<TrackFile> Files
        {
            get
            {
                return _files;
            }
        }

        public GmtBot(List<TrackFile> fileList, GmtBotOptions options)
        {
            _files = fileList;
            Options = options;
            _cacheFile = CoreVars.GetFilePath(PluginSettings.Folder, "cache.bin");

            Logger = new LogBot();

            if (Options.UsePersistentCache)
                LoadCache();
            else
                Cache = new CacheBot();
        }

        private void LoadCache()
        {
            Cache = null;
            var file = new FileInfo(_cacheFile);

            //Delete file if exceeds limit size
            if (file.Exists && Options.LimitCacheSize)
            {
                var limitSizeBytes = Options.CacheMaxSize * 1024 * 1024;
                if (file.Length > limitSizeBytes)
                    file.Delete();
            }

            Cache = CacheBot.LoadFile(_cacheFile) ?? new CacheBot();
        }
        private void SaveCache()
        {
            CacheBot.Serialize(Cache, _cacheFile);
        }

        private IGmtMedia GetCache(TrackFile file, CacheType type)
        {
            if (Options.LimitCacheAge)
                return Cache.Get(file, type, TimeSpan.FromDays(Options.CacheMaxAge));

            return Cache.Get(file, type);
        }
        private async Task<IGmtMedia> GetTags(TrackFile file, SearchResultType type)
        {
            AllMusicApiAgent agent = new AllMusicApiAgent();

            IEnumerable<ISearchResult> results;
            if (type == SearchResultType.Album)
                results = await agent.Search<AlbumResult>(file.Album, 100);
            else
                results = await agent.Search<ArtistResult>(file.Artist, 100);

            ISearchResult mainResult = results.GetBestResult(file, Options.AlgorithmTolerance);
            if (mainResult == null) return null;

            if (mainResult.ResultType == SearchResultType.Artist)
                return await agent.GetArtist(mainResult.ID);
            else
                return await agent.GetAlbum(mainResult.ID);
        }
        private SearchResultType[] GetSearchOrder()
        {
            List<SearchResultType> order = new List<SearchResultType>();

            if(Options.UseAlbumTags && Options.UseArtistTags)
            {
                switch (Options.TagPriority)
                {
                    case 1:
                        order.Add(SearchResultType.Artist);
                        order.Add(SearchResultType.Album);
                        return order.ToArray();
                    default:
                        order.Add(SearchResultType.Album);
                        order.Add(SearchResultType.Artist);
                        return order.ToArray();
                }
            }
            else if (Options.UseArtistTags)
            {
                order.Add(SearchResultType.Artist);
            }
            else
            {
                order.Add(SearchResultType.Album);

            }

            return order.ToArray();
        }

        public delegate void ProgressHandler(TrackFile current, int currentIndex, int total);
        public event ProgressHandler OnProgress;

        public delegate void CompleteHandler(List<TrackFile> Files);
        public event CompleteHandler OnComplete;

        public async void Run(Dispatcher uiDispatcher)
        {
            //Reset vars and logs initial messages
            await StartupBot(uiDispatcher);

            //Create a task for each call
            var tasks = BuildTaskArray();

            await TaskEx.WhenAll(tasks);
            
            if (Options.UsePersistentCache)
            {
                SaveCache();
            }

            uiDispatcher.BeginInvoke(new Action(() =>
            {
                OnComplete?.Invoke(_filesToUpdate);
            }));
        }

        /// <summary>
        /// Reset process vars and write initial log messages
        /// </summary>
        /// <param name="uiDispatcher"></param>
        private async Task StartupBot(Dispatcher uiDispatcher)
        {
            _cancelProgress = false;
            _filesToUpdate = new List<TrackFile>();
            _uiDispatcher = uiDispatcher;

            await LogMessagesInitialBotRun();
        }
        /// <summary>
        /// Return available tags for specified file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task<IGmtMedia> GetTagsAsync(TrackFile file)
        {
            //Object to store all retrieved tags
            var gmtContainer = new TrackFile();

            //Start looking for tags based on specified order in settings
            foreach(var tagType in GetSearchOrder())
            {
                //Get the tag type that has priority
                var searchCacheType = tagType == SearchResultType.Album ? CacheType.Album : CacheType.Artist;

                //Search tags from cache or if result is null return it from web
                IGmtMedia tags = GetCache(file, searchCacheType) ?? (await GetTagsFromWebAsync(file, tagType));

                //If no tracks are found go to next search type
                if (tags == null) continue;

                //Log progress
                await LogMessageTagSearchResult(tagType.ToString(), tags.Count());

                //Add retrieved tags to container
                gmtContainer.AddGmtMedia(tags);

                //If we got what we want, stop
                if (tags?.Count() > 0 && Options.TagPriority != 2) break;
            }

            return gmtContainer;
        }
        private async Task<IGmtMedia> GetTagsFromWebAsync(TrackFile file, SearchResultType tagType)
        {
            //Load tags from web
            IGmtMedia tags = await GetTags(file, tagType);

            //save it to cache
            if (tagType == SearchResultType.Album)
                Cache.Set(new CacheObject(file.GetAlbumCacheId(), tags));
            else
                Cache.Set(new CacheObject(file.GetArtistCacheId(), tags));

            return tags;
        }
        private async Task LoadTagsForGroup(IEnumerable<TrackFile> files)
        {
            foreach(var file in files)
            {
                if (_cancelProgress) break;

                IGmtMedia tags = await GetTagsAsync(file);

                ReportProgress(file);

                if (tags?.Count() > 0)
                {
                    await LogMessageTagSearchCompleted(tags.Count(), file.Title, file.Artist);
                    _filesToUpdate.Add(file.SetGmtMedia(tags, Options));
                }
            }
        }
        private Task[] BuildTaskArray()
        {
            List<Task> taskList = new List<Task>();

            var groupedFiles = _files.GroupBy(x => x.Artist);

            foreach(var group in groupedFiles)
            {
                taskList.Add(LoadTagsForGroup(group));
            }

            return taskList.ToArray();
        }


        private async Task LogMessage(string message, LogBotEntryLevel level)
        {
            await Task.Factory.StartNew(() =>
            {
                Logger.AddAsync(
                    new LogBotEntry(
                        message,
                        level)
                    , _uiDispatcher);
            });
        }
        private async Task LogMessagesInitialBotRun()
        {
            await LogMessage($"Starting bot for {_files.Count} files", LogBotEntryLevel.Info);
            await LogMessage($"Options: {Options.ToString()}", LogBotEntryLevel.Debug);
        }
        private async Task LogMessageTagSearchResult(string searchName, int resultCount)
        {
            await LogMessage($"Getting tags for {searchName} completed. Result: {resultCount}", LogBotEntryLevel.Debug);
        }
        private async Task LogMessageTagSearchCompleted(int tagCount, string trackTitle, string artist)
        {
            await LogMessage($"Saved {tagCount} tags for {trackTitle} by {artist}", LogBotEntryLevel.Info);
        }

        private void ReportProgress(TrackFile file)
        {
            _uiDispatcher.BeginInvoke(new Action(() =>
            {
                OnProgress?.Invoke(file, 1, _files.Count);
            }));
        }
    }
}