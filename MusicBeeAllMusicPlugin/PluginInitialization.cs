using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MusicBeePlugin
{
    //Plugin initialization methods
    public partial class Plugin
    {
        private void InitPlugin()
        {
            InitContextMenu();
        }

        private void InitContextMenu()
        {
            _mbApiInterface.MB_AddMenuItem("context.Main/GMT Manager", "", OpenGMTManager);
        }
    }
}
