﻿using MusicBeePlugin.Core;
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
            _windows = new Core.Tools.WindowManager();
            _windows.OnSave += UpdateTags;

            InitContextMenu();
        }

        private void InitContextMenu()
        {
            _mbApiInterface.MB_AddMenuItem("context.Main/GMT Manager", "", OpenGMTManager);
            _mbApiInterface.MB_AddMenuItem("context.Main/GMT Bot", "", OpenGMTBot);
        }
    }
}
