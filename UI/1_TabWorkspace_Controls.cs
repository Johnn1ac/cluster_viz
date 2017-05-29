using System;
using System.Windows;
using System.IO;
using System.Collections;
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
        bool m_eu_tab = false;
        bool m_mink_tab = false;
        bool nm_krusk_tab = false;
        bool nm_sam_tab = false;

        /// <summary>
        /// Общий метод для настройки интерфейса
        /// </summary>
        private void Setup_Controls()
        {
            textBox_dim.Text = columnNames_r.Length.ToString();

            comboBox_method.ItemsSource = Load_MDS_Methods();
            comboBox_method.SelectedIndex = 0;

            textBox_nm_dim.Text = columnNames_r.Length.ToString();

            comboBox_nm_method.ItemsSource = Load_NMMDS_Methods();
            comboBox_nm_method.SelectedIndex = 0;

            textBox_nm_p.Text = "2";
        }

        /// <summary>
        /// Список методов MDS для ListBox
        /// </summary>
        /// <returns></returns>
        private List<string> Load_MDS_Methods()
        {
            List<string> itemsList = new List<string>();

            itemsList.Add("euclidean");
            itemsList.Add("minkowski");

            return itemsList;
        }

        private List<string> Load_NMMDS_Methods()
        {
            List<string> itemsList = new List<string>();

            itemsList.Add("kruskal");
            itemsList.Add("sammon");

            return itemsList;
        }

        /// <summary>
        /// Обработка кнопки для добавления кластера:
        /// 1. Получаем доступ к массиву выделенных точек из класса (описание внутри него)
        /// 2. Делаем реверс этого массива, т.к. точки нужно удалять с конца
        /// 3. Проходимся по всем графикам, как матричным, так и рабочим, и удаляем точки
        /// 4. Затем эти же точки добавляем в новый набор точек, кластеры
        /// 5. Из этого набора формируем/собираем кластер
        /// !!!!------------------------!!!!ОПТИМИЗАЦИ(?) мб foreach!!!!------------------------!!!!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_create_cluster_Click(object sender, RoutedEventArgs e)
        {
            if (list_work_models[id].selected_points.Count != 0)
            {
                ColorPickerDialog ColorDialog = new ColorPickerDialog();// Открытие окна выбора цветов (и параметров)
                if (ColorDialog.ShowDialog() == true)
                {
                    var cluster_color = ColorDialog.Selected_color.ToString(); // Получаем цвет
                    var marker_type = ColorDialog.Marker_type;
                    list_work_models[id].selected_points.Reverse();

                    for (int i = 0; i < list_matrix_models.Count; i++)
                    {
                        Matrix_Cluster matrix_cluster = new Matrix_Cluster(cluster_color, marker_type);
                        Work_Cluster work_cluster = new Work_Cluster(cluster_color, marker_type);
                        for (int j = 0; j < list_work_models[id].selected_points.Count; j++)
                        {
                            //----------------------------WORK MODELS----------------------------//
                            var x = list_work_series[i].Points[list_work_models[id].selected_points[j]].X;
                            var y = list_work_series[i].Points[list_work_models[id].selected_points[j]].Y;
                            work_cluster.Points.Add(new ScatterPoint(x, y));

                            list_work_series[i].Points.RemoveAt(list_work_models[id].selected_points[j]);
                            //----------------------------WORK MODELS----------------------------//

                            //----------------------------MATRIX MODELS----------------------------//
                            x = list_matrix_series[i].Points[list_work_models[id].selected_points[j]].X;
                            y = list_matrix_series[i].Points[list_work_models[id].selected_points[j]].Y;
                            matrix_cluster.Points.Add(new ScatterPoint(x, y));

                            list_matrix_series[i].Points.RemoveAt(list_work_models[id].selected_points[j]);
                            //----------------------------MATRIX MODELS----------------------------//
                        }
                        list_work_models[i].Series.Add(work_cluster);
                        list_matrix_models[i].Series.Add(matrix_cluster);
                    }

                    foreach (Matrix_Model model in list_matrix_models)
                    {
                        model.InvalidatePlot(true);
                    }

                    list_work_models[id].InvalidatePlot(true);
                    Clear_Selection();
                    listView.Items.Add(new Cluster { Title = "123", Size = list_work_models[id].selected_points.Count });
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Не выделено ни одной точки", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        /// <summary>
        /// Снятие выделения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_clear_selection_Click(object sender, RoutedEventArgs e)
        {
            Clear_Selection();
        }

        /// <summary>
        /// Снятие выделения
        /// </summary>
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

        /// <summary>
        /// Показать\скрыть координаты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_show_coordinates_Click(object sender, RoutedEventArgs e)
        {
            if (coordinates_shown == false)
            {
                foreach (ScatterSeries series in list_work_series)
                {
                    series.LabelFormatString = "{0}, {1}";
                }
                list_work_models[id].InvalidatePlot(true);
                coordinates_shown = true;
            }
            else
            {
                foreach (ScatterSeries series in list_work_series)
                {
                    series.LabelFormatString = "";
                }
                list_work_models[id].InvalidatePlot(true);
                coordinates_shown = false;
            }
        }

        /// <summary>
        /// Кнопка для многомерного шкалирования (MDS)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_mds_Click(object sender, RoutedEventArgs e)
        {
            string selection = comboBox_method.SelectedValue.ToString();
            int k_dim = Int32.Parse(textBox_dim.Text);
            double p = double.NaN;

            try
            {
                if (selection == "euclidean")
                {
                    string file_path = new R_framework().Perform_MDS(k_dim, file_name, columnNames_r);

                    Build_MDS_Matrix(selection, k_dim, p, file_path);

                    MessageBox.Show(
                        "MDS выполнено успешно \n Метод - Евклидовый \n Размерность - " + k_dim,
                        "Выполнение MDS",
                        MessageBoxButton.OK, MessageBoxImage.Information);        
                }
                else if (selection == "minkowski")
                {        
                    p = Double.Parse(textBox_p.Text);
                    string file_path = new R_framework().Perform_MDS(k_dim, p, file_name, columnNames_r);

                    Build_MDS_Matrix(selection, k_dim, p, file_path);

                    MessageBox.Show(
                        "MDS выполнено успешно \n Метод - Минковского \n Размерность - " + k_dim + "\n Сила P - " + p,
                        "Выполнение MDS",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Не удалось выполнить многомерное шкалирование. \n" +
                                "Причина: " + exc.Message, "Ошибка выполнения многомерного шкалирования", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void button_nmmds_Click(object sender, RoutedEventArgs e)
        {
            string selection = comboBox_nm_method.SelectedValue.ToString();
            int k_dim = Int32.Parse(textBox_nm_dim.Text);
            double p = double.NaN;

            try
            {
                if (selection == "kruskal")
                {
                    string file_path = new R_framework().Perform_NM_MDS(k_dim, file_name, columnNames_r);

                    Build_NM_MDS_Matrix(selection, k_dim, p, file_path);
                    MessageBox.Show(
                        "NM MDS выполнено успешно \n Метод - Евклидовый \n Размерность - " + k_dim,
                        "Выполнение MDS",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (selection == "sammon")
                {
                    p = Double.Parse(textBox_nm_p.Text);
                    string file_path = new R_framework().Perform_NM_MDS(k_dim, p, file_name, columnNames_r);

                    Build_NM_MDS_Matrix(selection, k_dim, p, file_path);
                    MessageBox.Show(
                        "NM MDS выполнено успешно \n Метод - Минковского \n Размерность - " + k_dim + "\n Сила P - " + p,
                        "Выполнение MDS",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Не удалось выполнить многомерное шкалирование. \n" +
                                "Причина: " + exc.Message, "Ошибка выполнения многомерного шкалирования", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        List<Tab_MDS> mds_tabs = new List<Tab_MDS>() { null, null, null, null };

        private void Build_MDS_Matrix(string selection, int k_dim, double p, string file)
        {         
            if (selection == "euclidean")
            {        
                if (m_eu_tab == false)
                {
                    Tab_MDS tab_content = new Tab_MDS(file, selection, k_dim);
                    this.work_tab.Items.Add(new TabItem
                    {
                        Header = "MDS Euclidian",
                        Content = tab_content,
                        IsSelected = true,
                        TabIndex = 1
                        
                    });
                    m_eu_tab = true;
                    mds_tabs[0]=(tab_content);
                    
                }
                else
                {
                    mds_tabs[0].BuildMatrix(file, selection, k_dim);
                    //work_tab.SelectedItem = work_tab.Items[1]; Доделать
                }
            }

            if (selection == "minkowski")
            {           
                if (m_mink_tab == false)
                {
                    Tab_MDS tab_content = new Tab_MDS(file, selection, k_dim, p);
                    this.work_tab.Items.Add(new TabItem
                    {
                        Header = "MDS minkowski",
                        Content = tab_content,
                        IsSelected = true,
                        TabIndex = 2

                    });
                    m_mink_tab = true;
                    mds_tabs[1] = (tab_content);
                }
                else
                {
                    mds_tabs[1].BuildMatrix(file, selection, k_dim);
                    //work_tab.SelectedItem = work_tab.Items[2]; Доделать
                }
            }
        }

        private void Build_NM_MDS_Matrix(string selection, int k_dim, double p, string file)
        {
            if (selection == "kruskal")
            {
                if (nm_krusk_tab == false)
                {
                    Tab_MDS tab_content = new Tab_MDS(file, selection, k_dim);
                    this.work_tab.Items.Add(new TabItem
                    {
                        Header = "MDS Kruskal",
                        Content = tab_content,
                        IsSelected = true,
                        TabIndex = 3

                    });
                    nm_krusk_tab = true;
                    mds_tabs[2] = (tab_content);

                }
                else
                {
                    mds_tabs[2].BuildMatrix(file, selection, k_dim);
                    //work_tab.SelectedItem = work_tab.Items[1]; Доделать
                }
            }

            if (selection == "sammon")
            {
                if (nm_sam_tab == false)
                {
                    Tab_MDS tab_content = new Tab_MDS(file, selection, k_dim, p);
                    this.work_tab.Items.Add(new TabItem
                    {
                        Header = "MDS Sammon",
                        Content = tab_content,
                        IsSelected = true,
                        TabIndex = 4

                    });
                    nm_sam_tab = true;
                    mds_tabs[3] = (tab_content);
                }
                else
                {
                    mds_tabs[3].BuildMatrix(file, selection, k_dim);
                    //work_tab.SelectedItem = work_tab.Items[2]; Доделать
                }
            }
        }

        private void comboBox_method_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_method.SelectedIndex == 0)
            {
                textBox_p.Text = null;
                textBox_p.IsEnabled = false;
            }
            if (comboBox_method.SelectedIndex == 1)
            {
                textBox_p.Text = "1";
                textBox_p.IsEnabled = true;
            }
        }
    }
}