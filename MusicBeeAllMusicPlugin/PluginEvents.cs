using MusicBeePlugin.Core;
using MusicBeePlugin.Core.Bot;
using MusicBeePlugin.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicBeePlugin
{
    //Where all plugin events are called
    public partial class Plugin
    {
        private void OpenGMTManager(object sender, EventArgs e)
        {
            //Get selected tracks
            var tracks = GetTracks("domain=SelectedFiles");

            //Initialize and show window
            _windows.ShowNew<Window_GmtManager>(tracks.ToList());
        }
        private void OpenGMTBot(object sender, EventArgs e)
        {
            //Get displayed tracks
            var tracks = GetTracks("domain=DisplayedFiles");

            //Initialize and show window
            _windows.ShowNew<Window_LaunchBot>(tracks.ToList());
        }

        /// <summary>
        /// Update GMT tags from specified file list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="files"></param>
        private void UpdateTags(object sender, IEnumerable<TrackFile> files)
        {
            UpdateFileTagsAsync(files);
        }

        private IEnumerable<TrackFile> GetTracks(string domain)
        {
            //Load selected files
            string[] files = new string[0];
            _mbApiInterface.Library_QueryFilesEx(domain, ref files);

            foreach (var f in files)
            {
                //Array of required tag fields
                MetaDataType[] tagTypes = new[] { MetaDataType.TrackTitle,
                    MetaDataType.Artist, MetaDataType.Album,
                    GetMetaDataTypeByName(PluginSettings.LocalSettings.GenresTagField),
                    GetMetaDataTypeByName(PluginSettings.LocalSettings.MoodsTagField),
                    GetMetaDataTypeByName(PluginSettings.LocalSettings.ThemesTagField)
                };

                //Array to hold tag values
                string[] tags = new string[0];

                //Load tags from library
                _mbApiInterface.Library_GetFileTags(f, tagTypes, ref tags);

                //return new TrackFile
                yield return new TrackFile{
                    FilePath = f,
                    Title = tags[0],
                    Artist = tags[1],
                    Album = tags[2],
                    Genres = tags[3].Split(';').Where(x => !string.IsNullOrEmpty(x)).ToList(),
                    Moods = tags[4].Split(';').Where(x => !string.IsNullOrEmpty(x)).ToList(),
                    Themes = tags[5].Split(';').Where(x => !string.IsNullOrEmpty(x)).ToList()
                };
            }
        }

        private async Task SetMusicBeeProrgessMessageAsync(int current, int max, string task)
        {
            var message = $"Saving: {current}/{max} [{task}]";
            await Task.Factory.StartNew(() => { _mbApiInterface.MB_SetBackgroundTaskMessage(task); });
        }

        private async Task SaveFileTagAsync(string filePath, MetaDataType fieldType, string fieldValue)
        {
            await Task.Factory.StartNew(() =>
            {
                _mbApiInterface.Library_SetFileTag(filePath, fieldType, fieldValue);
            });
        }

        private async Task SaveFileTagAsync(string filePath, TagType fieldType, IEnumerable<string> values)
        {
            if (!HandleTagEnabled(fieldType)) return;

            var field = GetMetaDataType(fieldType);
            var value = string.Join(";", values.Select(x => x.Trim()));

            await SaveFileTagAsync(filePath, field, value);
        }

        /// <summary>
        /// Updates current source of genres, moods and themes with a new one
        /// </summary>
        /// <param name="source"></param>
        private void UpdateTagContainer(TrackFile source)
        {
            PluginSettings.LocalSettings.Genres.AddRange(source.Genres);
            PluginSettings.LocalSettings.Genres = PluginSettings.LocalSettings.Genres.Distinct().ToList();

            PluginSettings.LocalSettings.Moods.AddRange(source.Moods);
            PluginSettings.LocalSettings.Moods = PluginSettings.LocalSettings.Moods.Distinct().ToList();

            PluginSettings.LocalSettings.Themes.AddRange(source.Themes);
            PluginSettings.LocalSettings.Themes = PluginSettings.LocalSettings.Themes.Distinct().ToList();

            PluginSettings.LocalSettings.Save();
        }

        private async void UpdateFileTagsAsync(IEnumerable<TrackFile> fileList)
        {
            if (fileList.Count(x => x != null) == 0) return;

            var progressCount = 0;
            var max = fileList.Count();

            foreach(var file in fileList)
            {
                progressCount++;
                await SetMusicBeeProrgessMessageAsync(progressCount, max, file.Title);

                if (file == null) continue;

                await SaveFileTagAsync(file.FilePath, TagType.Genres, file.Genres);
                await SaveFileTagAsync(file.FilePath, TagType.Moods, file.Moods);
                await SaveFileTagAsync(file.FilePath, TagType.Themes, file.Themes);

                await Task.Factory.StartNew(
                    () => { _mbApiInterface.Library_CommitTagsToFile(file.FilePath); });
            }

            UpdateTagContainer(fileList.First(x => x != null));

            _mbApiInterface.MB_RefreshPanels();
            _mbApiInterface.MB_SetBackgroundTaskMessage($"{max} files were successfully updated!");
        }
    }
}
