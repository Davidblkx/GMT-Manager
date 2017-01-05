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
            if (ElementButtonEnabled)
                btn_addItem.Visibility = txt_Item.Text.Length > 0 ?
                Visibility.Visible : Visibility.Hidden;
            else if (btn_addItem.Visibility != Visibility.Hidden)
                btn_addItem.Visibility = Visibility.Hidden;
        }

        private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(comboBox.SelectedIndex != -1)
            {
                string item = comboBox.SelectedItem as string;
                comboBox.SelectedIndex = -1;
                if (!string.IsNullOrEmpty(item))
                {
                    OnItemSelected?.Invoke(this, item);

                    if (AddOnSelection) txt_Item.Text = item;
                }
            }
        }

        private void ButtonAddClicked(object sender, RoutedEventArgs e)
        {
            if (!ElementButtonEnabled) return;

            OnAddButtonClick?.Invoke(this, e);

            if (InputText.Length > 0)
                OnItemSelected?.Invoke(this, InputText);
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

        public event RoutedEventHandler OnAddButtonClick;
        public event ItemSelectedHandler OnItemSelected;

        public Button ElementButton { get { return btn_addItem; } }
        public TextBox ElementTextBox { get { return txt_Item; } }
        public ComboBox ElementComboBox { get { return comboBox; } }

        public bool ElementButtonEnabled { get; set; } = true;
        /// <summary>
        /// Add selected item to TextBox
        /// </summary>
        public bool AddOnSelection { get; set; } = false;
    }

    public delegate void ItemSelectedHandler(object sender, string item);
}
