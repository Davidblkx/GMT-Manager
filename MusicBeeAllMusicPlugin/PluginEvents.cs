using MusicBeePlugin.Core;
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
            if (_gmtManager == null || !_gmtManager.IsLoaded)
                _gmtManager = new Window_GmtManager();

            string[] files = new string[1];
            _mbApiInterface.Library_QueryFilesEx("domain=SelectedFiles", ref files);

            List<TrackFile> source = new List<TrackFile>();
            var settings = GetSettings();

            //TODO: load items into GMT Manager window
            foreach(var f in files)
            {
                MetaDataType[] tagTypes = new[] { MetaDataType.TrackTitle,
                    MetaDataType.Artist, MetaDataType.Album,
                    GetMetaDataTypeByName(settings.GenresTagField),
                    GetMetaDataTypeByName(settings.MoodsTagField),
                    GetMetaDataTypeByName(settings.ThemesTagField)
                };
                string[] tags = new string[0];
                _mbApiInterface.Library_GetFileTags(f, tagTypes,ref tags);
                source.Add(new TrackFile
                {
                    FilePath = f,
                    Title = tags[0],
                    Artist = tags[1],
                    Album = tags[2],
                    Genres = tags[3].Split(';').Where(x=> !string.IsNullOrEmpty(x)).ToList(),
                    Moods = tags[4].Split(';').Where(x=> !string.IsNullOrEmpty(x)).ToList(),
                    Themes = tags[5].Split(';').Where(x=> !string.IsNullOrEmpty(x)).ToList()
                });
            }

            _gmtManager.AddTrackToHandle(source);
            _gmtManager.TryToShow();
        }
    }
}
