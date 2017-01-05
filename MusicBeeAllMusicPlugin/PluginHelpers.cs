using MusicBeePlugin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin
{
    public partial class Plugin
    {
        private MetaDataType GetMetaDataTypeByName(string name)
        {
            MetaDataType metaData = MetaDataType.Custom16;
            Enum.TryParse<MetaDataType>(name, out metaData);
            return metaData;
        }
    }
}
