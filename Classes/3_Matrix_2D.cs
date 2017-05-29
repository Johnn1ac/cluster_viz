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

namespace Cluster_Analisys
{
    class Matrix_2D
    {
        private OxyPlot.Wpf.PlotView matrix2D_matrix_plot;
        private OxyPlot.Wpf.PlotView matrix2D_work_plot;

        // Аналагоичные список и для моделей (графиков)
        List<Matrix_Model> list_matrix_models = new List<Matrix_Model>();
        List<Work_Model> list_work_models = new List<Work_Model>();

        public List<Matrix_Model> list_m_m { get { return list_matrix_models; } }
        public List<Work_Model> list_w_m { get { return list_work_models; } }

        string[] x_axis;
        string[] y_axis;
        string[] plot_title;

        string[] columnNames_r;


        protected internal Matrix_2D()
        {
            
        }

        protected internal void Test_Builder(
            DataTable excel_table_data, 
            string[] x_axis,
            string[] y_axis,
            string[] plot_title,
            List<ScatterSeries> list_matrix_series,
            List<ScatterSeries> list_work_series,
            MDS_Matrix form)
        {
            int plot_index = 0; // Индексация графиков

            int _columns = excel_table_data.Columns.Count; // Количество столбцов в таблице Excel

            int n = _columns * (_columns - 1); // Количество графиков

            var _height = (form.Matrix2D_Scroll.Height / _columns) - 10;
            var _width = (form.Matrix2D_Scroll.Width / _columns) - 10;

            // Инициализация рабочей области
            form.Matrix_WorkPanel.Children.Add(matrix2D_work_plot = new OxyPlot.Wpf.PlotView());
            matrix2D_work_plot.Height = form.Matrix_WorkPanel.Height;
            matrix2D_work_plot.Width = form.Matrix_WorkPanel.Width;

            for (int i = 0; i < n; i++)
            {
                form.Matrix2D_MatrixPanel.Children.Add(matrix2D_matrix_plot = new OxyPlot.Wpf.PlotView());
                matrix2D_matrix_plot.Height = _height;
                matrix2D_matrix_plot.Width = _width;
                matrix2D_matrix_plot.Padding = new Thickness(0.5, 0.5, 0.5, 0.5);
                matrix2D_matrix_plot.Margin = new Thickness(0.5, 0.5, 0.5, 0.5);

                Create_Model(plot_index, x_axis, y_axis, plot_title, list_matrix_series, list_work_series);

                plot_index = plot_index + 1;
            }
        }

        protected internal void Build_MainMatrixControls
        (
            DataTable excel_table_data, 
            string[] x_axis,
            string[] y_axis,
            string[] plot_title,
            List<ScatterSeries> list_matrix_series,
            List<ScatterSeries> list_work_series,
            Tab_Workspace layout_form
        )
        {
            Tab_Workspace form = layout_form;

            int plot_index = 0; // Индексация графиков

            int _columns = excel_table_data.Columns.Count; // Количество столбцов в таблице Excel

            int n = _columns * (_columns - 1); // Количество графиков

            var _height = (form.Matrix2D_Scroll.Height / _columns) - 4;
            var _width = (form.Matrix2D_Scroll.Width / _columns) - 4;

            // Инициализация рабочей области
            form.Matrix2D_WorkPanel.Children.Add(matrix2D_work_plot = new OxyPlot.Wpf.PlotView());
            matrix2D_work_plot.Height = form.Matrix2D_WorkPanel.Height;
            matrix2D_work_plot.Width = form.Matrix2D_WorkPanel.Width;

            for (int i = 0; i < n; i++)
            {
                form.Matrix2D_MatrixPanel.Children.Add(matrix2D_matrix_plot = new OxyPlot.Wpf.PlotView());
                matrix2D_matrix_plot.Height = _height;
                matrix2D_matrix_plot.Width = _width;
                matrix2D_matrix_plot.Padding = new Thickness(0.5, 0.5, 0.5, 0.5);
                matrix2D_matrix_plot.Margin = new Thickness(0.5, 0.5, 0.5, 0.5);

                Create_Model(plot_index, x_axis, y_axis, plot_title, list_matrix_series, list_work_series);

                plot_index = plot_index + 1;
            }
        }

        private void Create_Model
        (
            int plot_index,
            string[] x_axis,
            string[] y_axis,
            string[] plot_title,
            List<ScatterSeries> list_matrix_series,
            List<ScatterSeries> list_work_series
        )
        {
            Work_Model work_model = new Work_Model
            (
                plot_index,
                plot_title,
                x_axis,
                y_axis,
                list_work_series,
                list_matrix_series,
                list_matrix_models
            );

            Matrix_Model matrix_model = new Matrix_Model
            (
                plot_index,
                plot_title,
                list_matrix_series,
                list_work_series,
                matrix2D_work_plot,
                work_model
            );

            matrix_model.Series.Add(list_matrix_series[plot_index]);

            list_matrix_models.Add(matrix_model);
            list_work_models.Add(work_model);

            matrix2D_matrix_plot.Model = matrix_model;
        }
      

    }
}
