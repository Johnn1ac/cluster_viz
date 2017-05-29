using System;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//<------------------Для Excel--------------------->
using System.Reflection;
using ExcelObj = Microsoft.Office.Interop.Excel;
//<-----------------Для OxyPlot-------------------->
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Annotations;

namespace Cluster_Analisys
{
    /// <summary>
    /// Логика для окна выбора цвета для кластера
    /// Возможно ещё настройка кластера (форма, размер)
    /// </summary>
    partial class ColorPickerDialog : Window
    {
        private Color? selected_color;
        private string marker_type;

        /// <summary>
        /// Выбранный цвет
        /// </summary>
        public Color? Selected_color
        {
            get { return selected_color; }
        }

        /// <summary>
        /// Выбранный тип маркера
        /// </summary>
        public string Marker_type
        {
            get { return marker_type; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ColorPickerDialog.ColorPickerDialog()'
        protected internal ColorPickerDialog()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ColorPickerDialog.ColorPickerDialog()'
        {
            InitializeComponent();
            this.Type_Selection.ItemsSource = LoadListBoxData();
            Set_Default_Selection();
        }

        /// <summary>
        /// Список для ListBox
        /// </summary>
        /// <returns>Данные для ListBox'a</returns>
        private List<string> LoadListBoxData()
        {
            List<string> itemsList = new List<string>();

            itemsList.Add("Circle");
            itemsList.Add("Square");
            itemsList.Add("Diamond");
            itemsList.Add("Triangle");

            return itemsList;
        }

        /// <summary>
        /// Установка выбора по умолчанию
        /// </summary>
        private void Set_Default_Selection()
        {
            Random rand = new Random();
            var r = Convert.ToByte(rand.Next(256));
            var g = Convert.ToByte(rand.Next(256));
            var b = Convert.ToByte(rand.Next(256));

            Color_Canvas.SelectedColor = Color.FromRgb(r, g, b);
            Color_Picker.SelectedColor = Color.FromRgb(r, g, b);

            Type_Selection.SelectedIndex = 0;
            marker_type = (Type_Selection.SelectedValue).ToString();
        }

        #region Events
        private void Button_Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        private void Color_Canvas_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Color_Picker.SelectedColor = Color_Canvas.SelectedColor;
            selected_color = Color_Canvas.SelectedColor;
        }
        private void Color_Picker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Color_Canvas.SelectedColor = Color_Picker.SelectedColor;
            selected_color = Color_Canvas.SelectedColor;
        }
        private void Type_Selection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            marker_type = (Type_Selection.SelectedValue).ToString();
        }
        #endregion
    }
}
