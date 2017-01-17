using MusicBeePlugin.Core;
using MusicBeePlugin.Core.Bot;
using MusicBeePlugin.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;

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
            if (files.Count() == 0) return;

            foreach (var f in files)
            {
                if (f == null) continue;

                _mbApiInterface.Library_SetFileTag(f.FilePath,
                    GetMetaDataTypeByName(PluginSettings.LocalSettings.GenresTagField),
                   string.Join(";", f.Genres));

                _mbApiInterface.Library_SetFileTag(f.FilePath,
                    GetMetaDataTypeByName(PluginSettings.LocalSettings.MoodsTagField),
                   string.Join(";", f.Moods));

                _mbApiInterface.Library_SetFileTag(f.FilePath,
                    GetMetaDataTypeByName(PluginSettings.LocalSettings.ThemesTagField),
                   string.Join(";", f.Themes));

                _mbApiInterface.Library_CommitTagsToFile(f.FilePath);
            }

            PluginSettings.LocalSettings.Genres.AddRange(files.First().Genres);
            PluginSettings.LocalSettings.Genres = PluginSettings.LocalSettings.Genres.Distinct().ToList();

            PluginSettings.LocalSettings.Moods.AddRange(files.First().Moods);
            PluginSettings.LocalSettings.Moods = PluginSettings.LocalSettings.Moods.Distinct().ToList();

            PluginSettings.LocalSettings.Themes.AddRange(files.First().Themes);
            PluginSettings.LocalSettings.Themes = PluginSettings.LocalSettings.Themes.Distinct().ToList();

            PluginSettings.LocalSettings.Save();

            _mbApiInterface.MB_RefreshPanels();
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
    }
}
