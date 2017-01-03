using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MusicBeePlugin.Core;
using System.Reflection;
using MusicBeePlugin.Core.Manager;

namespace MusicBeePlugin
{
    //Main plugin implementation
    public partial class Plugin
    {
        private Window_GmtManager _gmtManager;
        private MusicBeeApiInterface _mbApiInterface;
        private SettingsControl _settingsControl;

        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            _mbApiInterface = new MusicBeeApiInterface();
            _mbApiInterface.Initialise(apiInterfacePtr);

            _settingsControl = null;

            return BuildPluginInfo();
        }

        public bool Configure(IntPtr panelHandle)
        {
            // save any persistent settings in a sub-folder of this path
            string dataPath = _mbApiInterface.Setting_GetPersistentStoragePath();

            _settingsControl = new SettingsControl();
            _settingsControl.LoadSettings(PluginSettings.LoadSettings(dataPath));
            _settingsControl.SetTagFields(Enum.GetNames(typeof(MetaDataType)).ToList());

            if (panelHandle != IntPtr.Zero)
            {
                Panel configPanel = (Panel)Panel.FromHandle(panelHandle);
                configPanel.Controls.Add(_settingsControl);
            }

            return true;
        }
        public void SaveSettings()
        {
            // save any persistent settings in a sub-folder of this path
            string dataPath = _mbApiInterface.Setting_GetPersistentStoragePath();
            _settingsControl?.GetSettings().Save(dataPath);
        }

        public void Close(PluginCloseReason reason)
        {
            _gmtManager?.ForceClose();
        }

        /// <summary>
        /// Deletes all files created by this plugin
        /// </summary>
        public void Uninstall()
        {
            string dataPath = _mbApiInterface.Setting_GetPersistentStoragePath();
            string pluginPath = System.IO.Path.Combine(dataPath, CoreVars.PluginFolder);

            if (System.IO.Directory.Exists(pluginPath) && pluginPath != dataPath)
                System.IO.Directory.Delete(pluginPath, true);
        }

        /// <summary>
        /// Called by MusicBee every time one of the notifications defined at initialization occur
        /// </summary>
        /// <param name="sourceFileUrl"></param>
        /// <param name="type"></param>
        public void ReceiveNotification(string sourceFileUrl, NotificationType type)
        {
            switch (type)
            {
                case NotificationType.PluginStartup:
                    InitPlugin();
                    break;
            }
        }

        private static PluginInfo BuildPluginInfo()
        {
            PluginInfo _about = new PluginInfo();
            _about.PluginInfoVersion = PluginInfoVersion;
            _about.Name = "GMT Manager";
            _about.Description = "Add various tools to make it easier to filter by genre, mood and themes";
            _about.Author = "David Pires";
            _about.TargetApplication = string.Empty;
            _about.Type = PluginType.General;
            _about.VersionMajor = 1;
            _about.VersionMinor = 0;
            _about.Revision = 1;
            _about.MinInterfaceVersion = MinInterfaceVersion;
            _about.MinApiRevision = MinApiRevision;
            _about.ReceiveNotifications = (ReceiveNotificationFlags.PlayerEvents | ReceiveNotificationFlags.TagEvents);
            _about.ConfigurationPanelHeight = 200;
            return _about;
        }
    }
}
