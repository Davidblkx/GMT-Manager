using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MusicBeePlugin.Core.Settings;

namespace MusicBeePlugin.Core
{
    public partial class SettingsControl : UserControl
    {
    
        public SettingsControl()
        {
            InitializeComponent();
        }

        public void SetTagFields(List<string> fields)
        {
            controlSettings.InitializeTagFields(fields);
        }

        public void UpdateControls()
        {
            controlSettings.UpdateControls();
        }
        public void LoadSettings(PluginSettings settings)
        {
            controlSettings.LoadSettings(settings);

            UpdateControls();
        }
        public PluginSettings GetSettings()
        {
            return controlSettings.GetSettings();
        }
    }
}
