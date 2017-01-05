using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicBeePlugin.Core.Manager
{
    /// <summary>
    /// Interaction logic for Control_CollectionManager.xaml
    /// </summary>
    public partial class Control_CollectionManager : UserControl
    {
        #region DependencyProperties
        public static readonly DependencyProperty 
            ShowCloseButtonProperty = DependencyProperty.Register("Title",
                typeof(string), typeof(Control_CollectionManager), new PropertyMetadata("Control Title"));

        public string Title
        {
            get { return (string)GetValue(ShowCloseButtonProperty); }
            set { SetValue(ShowCloseButtonProperty, value); }
        }
        #endregion

        private ObservableCollection<string> _items;

        public Control_CollectionManager()
        {
            InitializeComponent();
            cmb_add.OnItemSelected += OnItemAdd;
            btn_remove.Click += OnRemoveClick;
            _listView.SelectionChanged += _listView_SelectionChanged;
            _listView.MouseDoubleClick += _listView_MouseDoubleClick;
        }

        public void SetItemsSources(IEnumerable<string> itemsCollection)
        {
            _listView.ItemsSource = null;
            _items = new ObservableCollection<string>(itemsCollection.Distinct());
            _listView.ItemsSource = _items;
        }
        public void SetItemsSources(IEnumerable<IEnumerable<string>> itemsCollection)
        {
            var list = new List<string>();
            foreach (var collection in itemsCollection)
                list.AddRange(collection);
            SetItemsSources(list);
        }

        public List<string> GetItems()
        {
            return new List<string>(_items);
        }

        public void SetSelectionSource(IEnumerable enumeration)
        {
            cmb_add.ItemsSource = enumeration;
        }

        public void AddItem(string newItem)
        {
            if(_items == null)
            {
                _items = new ObservableCollection<string>();
                _listView.ItemsSource = _items;
            }

            if (!_items.Contains(newItem)) _items.Add(newItem);
        }

        public void AddRange(IEnumerable<string> itemsCollection)
        {
            foreach (var item in itemsCollection)
                AddItem(item);
        }

        public void RemoveItem(string itemToRemove)
        {
            if (_items != null)
                _items.Remove(itemToRemove);
        }

        public void RemoveRange(List<string> itemsCollectionToRemove)
        {
            foreach (var item in itemsCollectionToRemove)
                RemoveItem(item);
        }

        #region Events
        private void _listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = _listView.SelectedItem as string;
            if (!string.IsNullOrEmpty(item))
                RemoveItem(item);
        }

        private void OnItemAdd(object sender, string item)
        {
            AddItem(item);
            cmb_add.InputText = string.Empty;
        }

        private void _listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btn_remove.IsEnabled = _listView.SelectedIndex >= 0;
        }

        private void OnRemoveClick(object sender, RoutedEventArgs e)
        {
            var item = _listView.SelectedItem as string;
            if (!string.IsNullOrEmpty(item))
                RemoveItem(item);
        }
        #endregion
    }
}
