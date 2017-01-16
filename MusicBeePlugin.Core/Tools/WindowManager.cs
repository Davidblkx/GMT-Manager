using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Core.Tools
{
    public class WindowManager
    {
        private Dictionary<Type, PluginWindow> _windows;

        public WindowManager()
        {
            _windows = new Dictionary<Type, PluginWindow>();
        }

        public PluginWindow this[Type windowType]
        {
            get
            {
                if (!_windows.ContainsKey(windowType))
                {
                    _windows.Add(windowType, Initialize(windowType));
                    _windows[windowType].Reset();
                }

                return _windows[windowType];
            }
        }

        private PluginWindow Initialize(Type windowType)
        {
            var instance = Activator.CreateInstance(windowType) as PluginWindow;
            instance.OnSave += OnSaveInvoker;
            return instance;
        }

        private void OnSaveInvoker(object sender, IEnumerable<TrackFile> files)
        {
            OnSave?.Invoke(sender, files);
        }

        public event SaveChangesHandler OnSave;
        public delegate void SaveChangesHandler(object sender, IEnumerable<TrackFile> files);

        public void Show<T>()
            where T : PluginWindow
        {
            this[typeof(T)].TryToShow();
        }
        public void ShowNew<T>(params object [] init_params)
            where T : PluginWindow
        {
            Type t = typeof(T);

            this[t].Reset();
            this[t].Initialize(init_params);

            Show<T>();
        }
        public T Get<T>()
            where T : PluginWindow
        {
            return this[typeof(T)] as T;
        }

        public void CloseAll()
        {
            foreach (var win in _windows.Values)
                win.ForceClose();
        }
    }
}
