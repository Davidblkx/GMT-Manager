using System;
using System.Collections.Generic;
using System.IO;
using AllMusicApi;
using System.Linq;
using System.Text;
using AllMusicApi.Model;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MusicBeePlugin.Core.Bot
{
    public class GmtBot
    {
        private List<TrackFile> _files;
        private string _cacheFile;
        private bool _cancelProgress;

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

        /// <summary>
        /// Asynchronous get tags for current selected files
        /// </summary>
        /// <param name="uiDispatcher"></param>
        /// <returns></returns>
        public async Task Run(Dispatcher uiDispatcher)
        {
            _cancelProgress = false;

            Logger.AddAsync(new LogBotEntry(
                $"Starting bot for {_files.Count} files",
                LogBotEntryLevel.Info)
                , uiDispatcher);

            Logger.AddAsync(new LogBotEntry(
                $"Options: {Options.ToString()}",
                LogBotEntryLevel.Debug)
                , uiDispatcher);

            for(int i = 0; i < _files.Count; i++)
            {
                if (_cancelProgress) break;

                uiDispatcher.BeginInvoke(new Action(() =>
                {
                    if (i >= _files.Count) i = _files.Count - 1;
                    OnProgress?.Invoke(_files[i], i + 1, _files.Count);
                }));

                Logger.AddAsync(new LogBotEntry(
                $"Getting tags for: {i+1} of {_files.Count}",
                LogBotEntryLevel.Info)
                , uiDispatcher);

                Logger.AddAsync(new LogBotEntry(
                $"Getting tags for: {_files[i].FilePath}",
                LogBotEntryLevel.Debug)
                , uiDispatcher);

                //Shadow from current TrackFile to store tags during the filter process
                TrackFile gmtHolder = new TrackFile()
                {
                    Title = _files[i].Title,
                    Album = _files[i].Album,
                    Artist = _files[i].Artist,
                    FilePath = _files[i].FilePath
                };

                foreach(var type in GetSearchOrder())
                {
                    IGmtMedia tags = null;

                    Logger.AddAsync(new LogBotEntry(
                        $"Trying to get tags from cache...",
                        LogBotEntryLevel.Debug)
                        , uiDispatcher);

                    tags = GetCache(_files[i], type == SearchResultType.Album
                        ? CacheType.Album : CacheType.Artist);

                    //If no Data exists in cache, retrieve it from the web
                    if (tags == null)
                    {
                        Logger.AddAsync(new LogBotEntry(
                            $"Nothing in cache, loading from web...",
                            LogBotEntryLevel.Debug)
                            , uiDispatcher);

                        tags = await GetTags(_files[i], type);

                        //TODO: fix null tags
                        //Add tags to cache
                        if (type == SearchResultType.Album)
                            Cache.Set(new CacheObject(_files[i].GetAlbumCacheId(), tags));
                        else
                            Cache.Set(new CacheObject(_files[i].GetArtistCacheId(), tags));
                    }

                    //Count number of tag items retrieved in current process
                    int processCount = tags?.Genres.Count + tags?.Moods.Count + tags?.Themes.Count ?? 0;

                    if (tags == null) continue;

                    Logger.AddAsync(new LogBotEntry(
                        $"Getting tags for {type.ToString()} completed. Result: {processCount}",
                        LogBotEntryLevel.Debug)
                        , uiDispatcher);

                    gmtHolder.AddGmtMedia(tags);

                    //If process is completed, break cycle
                    if (tags != null && Options.TagPriority != 2) break;

                }//END FOREACH

                _files[i] = _files[i].SetGmtMedia(gmtHolder, Options);

            }//END FOR

            Logger.AddAsync(new LogBotEntry(
                $"Getting tags completed for {_files.Count} files",
                LogBotEntryLevel.Info)
                , uiDispatcher);

            if (Options.UsePersistentCache)
            {
                Logger.AddAsync(new LogBotEntry(
                    "Updating cache in disk",
                    LogBotEntryLevel.Debug)
                    , uiDispatcher);

                SaveCache();
            }

            uiDispatcher.BeginInvoke(new Action(() =>
            {
                OnComplete?.Invoke(_files);
            }));
        }
    }
}
