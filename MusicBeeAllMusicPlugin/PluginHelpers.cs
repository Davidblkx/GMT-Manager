using MusicBeePlugin.Core;
using System;

namespace MusicBeePlugin
{
    public partial class Plugin
    {
        /// <summary>
        /// Get MetaDataType based on name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private MetaDataType GetMetaDataTypeByName(string name)
        {
            MetaDataType metaData = MetaDataType.Custom16;
            if(Enum.TryParse<MetaDataType>(name, out metaData))
                return metaData;

            throw new Exception("Value not found in Metadata list: " + name);
        }

        /// <summary>
        /// Get MetaData field based on TagType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private MetaDataType GetMetaDataType(TagType type)
        {
            switch (type)
            {
                case TagType.Genres:
                    return GetMetaDataTypeByName(PluginSettings.LocalSettings.GenresTagField);

                case TagType.Moods:
                    return GetMetaDataTypeByName(PluginSettings.LocalSettings.MoodsTagField);

                case TagType.Themes:
                    return GetMetaDataTypeByName(PluginSettings.LocalSettings.ThemesTagField);
            }

            throw new Exception("Invalid tag type, plugin can only handle Genres, Moods and themes");
        }

        /// <summary>
        /// Checks if management is enabled for current TagType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool HandleTagEnabled(TagType type)
        {
            switch (type)
            {
                case TagType.Genres:
                    return PluginSettings.LocalSettings.HandleGenres;

                case TagType.Moods:
                    return PluginSettings.LocalSettings.HandleMoods;

                case TagType.Themes:
                    return PluginSettings.LocalSettings.HandleThemes;
            }

            throw new Exception("Invalid tag type, plugin can only handle Genres, Moods and themes");
        }
    }
}
