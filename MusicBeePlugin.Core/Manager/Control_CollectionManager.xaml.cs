using System;
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
        private ObservableCollection<string> _items;

        public Control_CollectionManager()
        {
            InitializeComponent();
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

        public void AddItem(string newItem)
        {
            if(_items == null)
            {
                _items = new ObservableCollection<string>();
                _listView.ItemsSource = _items;
            }

            if (!_items.Contains(newItem)) _items.Add(newItem);
        }

        public void RemoveItem(string itemToRemove)
        {
            if (_items != null)
                _items.Remove(itemToRemove);
        }
    }
}
