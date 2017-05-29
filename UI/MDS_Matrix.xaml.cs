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
using System.Reflection;
//<------------------Для Excel--------------------->
using ExcelObj = Microsoft.Office.Interop.Excel;
//<-----------------Для OxyPlot-------------------->
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Annotations;
using ConverterLibrary;

namespace Cluster_Analisys
{
    /// <summary>
    /// Interaction logic for MDS_Matrix.xaml
    /// </summary>
    public partial class MDS_Matrix : UserControl
    {
        #region Data
        /// <summary>
        /// Локальная для класса статическая переменная для определния id графика
        /// </summary>
        public static int id;

        /// <summary>
        /// Имя файла
        /// </summary>
        public static string file_name;

        bool coordinates_shown = false; // Определяет, показаны ли координаты

        DataTable excel_table_data = new DataTable();// Данные Excel в виде таблицы

        string[] x_axis;
        string[] y_axis;
        string[] plot_title;

        string[] columnNames_r;

        // В этих списках хранятся все возможные наборы точек для графика
        // Для матричного и рабочего графиков соответственно
        // Списки глобальны и могут исользоваться везде (на то и расчёт)
        List<ScatterSeries> list_matrix_series = new List<ScatterSeries>();
        List<ScatterSeries> list_work_series = new List<ScatterSeries>();
        // Аналагоичные список и для моделей (графиков)
        List<Matrix_Model> list_matrix_models = new List<Matrix_Model>();
        List<Work_Model> list_work_models = new List<Work_Model>();
        // Список кластеров - тут сложнее
        List<Matrix_Cluster> list_matrix_cluster = new List<Matrix_Cluster>();
        List<Work_Cluster> work_matrix_cluster = new List<Work_Cluster>();
        #endregion

        public MDS_Matrix(DataTable excel_temp_data, string file)
        {
            file_name = file;
            InitializeComponent();
            // 1. Инициализируем DataGrid, загружаем туда эти данные
            InitializeDataGrid(excel_temp_data);
            // 2. Строим матрицу 2D
            Build_Matrix();
        }

        /// <summary>
        /// Инициализация данных в DataGrid
        /// </summary>
        /// <param name="excel_temp_data"></param>
        private void InitializeDataGrid(DataTable excel_temp_data)
        {
            excel_table_data = excel_temp_data;
            this.DataGrid_Excel.ItemsSource = excel_temp_data.DefaultView;
        }

        /// <summary>
        /// Построение основной матрицы при загрузки формы
        /// </summary>
        private void Build_Matrix()
        {
            Data_Converter converter = new Data_Converter();
            converter.Convert_Excel_to_Series(excel_table_data);

            list_matrix_series = converter.list_m_s;
            list_work_series = converter.list_w_s;

            x_axis = converter.x_a;
            y_axis = converter.y_a;

            plot_title = converter.plot_t;

            columnNames_r = converter.column_r;

            Matrix_2D Matrix_2D = new Matrix_2D();
            Matrix_2D.Test_Builder(excel_table_data, x_axis, y_axis, plot_title, list_matrix_series, list_work_series, this);

            list_matrix_models = Matrix_2D.list_m_m;
            list_work_models = Matrix_2D.list_w_m;
        }

        private void button_clear_Click(object sender, RoutedEventArgs e)
        {
            Clear_Selection();
        }

        private void Clear_Selection()
        {
            for (int i = 0; i < list_matrix_series.Count; i++)
            {
                list_work_series[i].ClearSelection();
                list_matrix_series[i].ClearSelection();
            }
            foreach (Matrix_Model plot in list_matrix_models)
            {
                plot.InvalidatePlot(true);
            }
            list_work_models[id].InvalidatePlot(true);
        }
    }
}
