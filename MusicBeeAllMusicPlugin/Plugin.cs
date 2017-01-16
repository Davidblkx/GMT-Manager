using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MusicBeePlugin.Core;
using System.Reflection;
using MusicBeePlugin.Core.Manager;
using MusicBeePlugin.Core.Tools;

namespace MusicBeePlugin
{
    //Main plugin implementation
    public partial class Plugin
    {
        // TODO: Allow multiple windows or make it a panel?
        private WindowManager _windows;
        private MusicBeeApiInterface _mbApiInterface;
        private SettingsControl _control;

        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            _mbApiInterface = new MusicBeeApiInterface();
            _mbApiInterface.Initialise(apiInterfacePtr);

            //Get persistent folder
            string rootFolder = _mbApiInterface.Setting_GetPersistentStoragePath();
            PluginSettings.InitSettings(rootFolder);

            return BuildPluginInfo();
        }

        public bool Configure(IntPtr panelHandle)
        {
            if(_control == null)
            {
                _control = new SettingsControl();
                _control.SetTagFields(Enum.GetNames(typeof(MetaDataType)).ToList());
            }

            if (panelHandle != IntPtr.Zero)
            {
                Panel configPanel = (Panel)Panel.FromHandle(panelHandle);
                configPanel.Controls.Add(_control);
            }

            return true;
        }
        public void SaveSettings()
        {
            _control.SaveSettings();
        }

        public void Close(PluginCloseReason reason)
        {
            PluginSettings.LocalSettings.Save();
            _windows?.CloseAll();
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
