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
//-----------------------------------------------------------------------КОНВЕНЦИЯ СТИЛЯ--------------------------------------------------------------------------------------
//---------------------------------------------------------------ВСЕ ПЕРЕМЕННЫЕ - С МАЛЕНЬКОЙ БУКВЫ!--------------------------------------------------------------------------
//-------------------------------------------------------------------МЕТОДЫ И КЛАССЫ - С БОЛЬШОЙ!-----------------------------------------------------------------------------
namespace Cluster_Analisys
{
    /// <summary>
    /// Класс, содержащий логику взаимодействия для Tab_Workspace.xaml
    /// Определяет структуру разметки на TabPage. Вся разметка в XAML документе
    /// </summary>
    public partial class Tab_Workspace : UserControl
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

        /// <summary>
        /// Конструктор - только в нём происходит инициализация всего
        /// При вызове класса, ему автоматически передают данные из Excel
        /// Далее эти данные остаются только в этом классе
        /// </summary>
        /// <param name="excel_temp_data"></param>
        /// <param name="file"></param>
        protected internal Tab_Workspace(DataTable excel_temp_data, string file)
        {
            file_name = file;
            InitializeComponent();
            // 1. Инициализируем DataGrid, загружаем туда эти данные
            InitializeDataGrid(excel_temp_data); 
            // 2. Строим матрицу 2D
            Build_Matrix();
            // 3. Настройка элементов управления
            Setup_Controls();
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
            Matrix_2D.Build_MainMatrixControls(excel_table_data, x_axis, y_axis, plot_title, list_matrix_series, list_work_series, this);

            list_matrix_models = Matrix_2D.list_m_m;
            list_work_models = Matrix_2D.list_w_m;           
        }
    }

    /// <summary>
    /// Кластер для отображения в списке кластеров
    /// </summary>
    public class Cluster
    {
        /// <summary>
        /// Название кластера
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Размерность кластера
        /// </summary>
        public int Size { get; set; }
    }
}
