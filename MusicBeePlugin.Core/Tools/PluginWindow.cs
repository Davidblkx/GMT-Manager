using MusicBeePlugin.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms.Integration;
using System.ComponentModel;

namespace MusicBeePlugin.Core.Tools
{
    public partial class PluginWindow : Window
    {
        private bool _cancelClose = true;

        public PluginWindow()
        {
            Loaded += OnWindowLoaded;
            Closing += OnWindowClosing;
        }

        public void TryToShow()
        {
            if (!IsVisible)
            {
                //Accept keyboard input
                ElementHost.EnableModelessKeyboardInterop(this);
                Show();
            }
        }
        public void ForceClose()
        {
            _cancelClose = false;
            Close();
        }

        public WindowSettings Measures
        {
            get
            {
                return new WindowSettings
                {
                    Width = Width,
                    Height = Height,
                    LeftPosition = Left,
                    TopPosition = Top
                };
            }
        }

        public virtual void Reset() { }
        public virtual void Initialize(params object[] init_params) { }
        public virtual Type GetImplementedType()
        {
            return GetType();
        }

        protected void InvokeOnSave(object sender, IEnumerable<TrackFile> files)
        {
            OnSave?.Invoke(sender, files);
        }

        public event SaveChangesHandler OnSave;
        public delegate void SaveChangesHandler(object sender, IEnumerable<TrackFile> files);

        #region Events
        protected virtual void OnWindowClosing(object sender, CancelEventArgs e)
        {
            PluginSettings.LocalSettings.SetWindowSetting(
                GetImplementedType().Name,
                Measures
                );

            e.Cancel = _cancelClose;
            if (_cancelClose)
                Hide();
        }
        protected virtual void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            var m = PluginSettings.LocalSettings.GetWindowSetting(GetImplementedType().Name);
            Left = m.LeftPosition;
            Top = m.TopPosition;
            Height = m.Height;
            Width = m.Width;
        }
        #endregion
    }
}
