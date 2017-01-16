using AllMusicApi;
using AllMusicApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Core.Bot
{
    public static class CacheBotExtensions
    {
        public static string GetAlbumCacheId(this TrackFile file)
        {
            return file.Album.ToLower() + file.Artist.ToLower();
        }

        public static string GetArtistCacheId(this TrackFile file)
        {
            return file.Artist.ToLower();
        }

        public static string GetCacheId(this TrackFile file, CacheType type)
        {
            return type == CacheType.Artist ? 
                file.GetArtistCacheId() : file.GetAlbumCacheId();
        }

        /// <summary>
        /// Add values of IGmtMedia if properties are empty
        /// </summary>
        /// <param name="file"></param>
        /// <param name="media"></param>
        /// <returns></returns>
        public static TrackFile SetGmtMediaIfEmpty(this TrackFile file, IGmtMedia media)
        {
            if (media == null) return file;

            if (file.Genres.Count == 0)
                file.Genres = media.Genres.Select(x=>x.Trim()).ToList();
            if (file.Moods.Count == 0)
                file.Moods = media.Moods.Select(x => x.Trim()).ToList();
            if (file.Themes.Count == 0)
                file.Themes = media.Themes.Select(x => x.Trim()).ToList();

            return file;
        }

        /// <summary>
        /// Add GMT values to existing values
        /// </summary>
        /// <param name="file"></param>
        /// <param name="media"></param>
        /// <returns></returns>
        public static TrackFile AddGmtMedia(this TrackFile file, IGmtMedia media)
        {
            if (media == null) return file;

            file.Genres.AddRange(media.Genres);
            file.Moods.AddRange(media.Moods);
            file.Themes.AddRange(media.Themes);

            file.Genres = file.Genres.Select(x => x.Trim()).Distinct().ToList();
            file.Moods = file.Moods.Select(x => x.Trim()).Distinct().ToList();
            file.Themes = file.Themes.Select(x => x.Trim()).Distinct().ToList();

            return file;
        }

        /// <summary>
        /// Replace properties with GMT values
        /// </summary>
        /// <param name="file"></param>
        /// <param name="media"></param>
        /// <returns></returns>
        public static TrackFile ForceSetGmt(this TrackFile file, IGmtMedia media)
        {
            if(media?.Genres?.Count > 0)
                file.Genres = media.Genres.Select(x => x.Trim()).Distinct().ToList();

            if(media?.Moods?.Count > 0)
                file.Moods = media.Moods.Select(x => x.Trim()).Distinct().ToList();

            if(media?.Themes?.Count > 0)
                file.Themes = media.Themes.Select(x => x.Trim()).Distinct().ToList();

            return file;
        }

        /// <summary>
        /// Trim and remove duplicates
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static TrackFile FixGmtTags(this TrackFile file)
        {
            file.Genres = file.Genres.Select(x => x.Trim()).Distinct().ToList();
            file.Moods = file.Moods.Select(x => x.Trim()).Distinct().ToList();
            file.Themes = file.Themes.Select(x => x.Trim()).Distinct().ToList();

            return file;
        }

        /// <summary>
        /// Add GMT tags according to bot options
        /// </summary>
        /// <param name="file"></param>
        /// <param name="media"></param>
        /// <param name="setOptions"></param>
        /// <returns></returns>
        public static TrackFile SetGmtMedia(this TrackFile file, IGmtMedia media, GmtBotOptions setOptions)
        {
            switch (setOptions.FillTagsMode)
            {
                case 1: //If empty
                    return file.SetGmtMediaIfEmpty(media);

                case 2: //Add to existing ones
                    return file.AddGmtMedia(media);

                case 3: // Replace all
                    return file.ForceSetGmt(media);

                default: // Test mode or invalid options return unchanged data
                    return file;
            }
        }

        public static ISearchResult GetBestResult(
            this IEnumerable<ISearchResult> enumList,  TrackFile file, int tolerance)
        {
            IEnumerable<ISearchResult> results = enumList
                .Where(x => x.Diference(file.Artist) <= tolerance)
                .OrderBy(x=>x.Diference(file.Artist));

            if(results.FirstOrDefault()?.ResultType == SearchResultType.Album)
            {
                results = results
                    .Where(x => x.Diference(file.Album, "Title") < tolerance)
                    .OrderBy(x=> (x.Diference(file.Album, "Title") + x.Diference(file.Artist)) / 2);
            }

            return results.FirstOrDefault();
        }
    }
}
