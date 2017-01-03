using System;
using System.Collections;
using System.Collections.Generic;
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

namespace MusicBeePlugin.Core.Tools
{
    /// <summary>
    /// Interaction logic for ComboBoxEditor.xaml
    /// </summary>
    public partial class ComboBoxEditor : UserControl
    {
        public ComboBoxEditor()
        {
            InitializeComponent();
            btn_addItem.Click += ButtonAddClicked;
            comboBox.SelectionChanged += ComboBoxSelectionChanged;
            txt_Item.TextChanged += OnTextChanged;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            btn_addItem.Visibility = txt_Item.Text.Length > 0 ?
                Visibility.Visible : Visibility.Hidden;
        }

        private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(comboBox.SelectedIndex != -1)
            {
                InputText = comboBox.SelectedItem as string;
                comboBox.SelectedIndex = -1;
            }
        }

        private void ButtonAddClicked(object sender, RoutedEventArgs e)
        {
            OnAddItem?.Invoke(this, e);
        }

        public IEnumerable ItemsSource
        {
            get { return comboBox.ItemsSource; }
            set { comboBox.ItemsSource = value; }
        }

        public string InputText
        {
            get { return txt_Item.Text; }
            set { txt_Item.Text = value; }
        }

        public event RoutedEventHandler OnAddItem;
    }
}
