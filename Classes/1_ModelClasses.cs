using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Annotations;
using System.Windows.Controls;
//-----------------------------------------------------------------------КОНВЕНЦИЯ СТИЛЯ--------------------------------------------------------------------------------------
//---------------------------------------------------------------ВСЕ ПЕРЕМЕННЫЕ - С МАЛЕНЬКОЙ БУКВЫ!--------------------------------------------------------------------------
//-------------------------------------------------------------------МЕТОДЫ И КЛАССЫ - С БОЛЬШОЙ!-----------------------------------------------------------------------------
namespace Cluster_Analisys
{
    ///<summary>
    /// Класс, представляющий график, который выводится в каждую из проекций матрицы
    /// Содержит как настройку самих графиков, так и настройку манипуляций с ними
    ///</summary>
    class Matrix_Model : PlotModel
    {
        public int index;
        private bool model_pressed = false;

        public Matrix_Model
        (
            int plot_index, 
            string[] op_tit,
            List<ScatterSeries> list_matrix_series, 
            List<ScatterSeries> list_work_series, 
            OxyPlot.Wpf.PlotView matrix2D_work_plot,
            Work_Model work_model
        )
        {
            Title = op_tit[plot_index];
            index = plot_index;
            
            IsLegendVisible = false;
            PlotAreaBorderThickness = new OxyThickness(1, 0, 0, 1);
            PlotMargins = new OxyThickness(0, 0, 0, 0);
            Padding = new OxyThickness(5);

            Background = OxyColors.White;
            SelectionColor = OxyColors.Crimson;
            
            Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                TickStyle = OxyPlot.Axes.TickStyle.None,
                //MajorGridlineStyle = LineStyle.Dash,
                //MinorGridlineStyle = LineStyle.Dash,
                MaximumPadding = 0.1,
                MinimumPadding = 0.1,
                IsPanEnabled = false,
                IsZoomEnabled = false,
                FontSize = 0.1,
                SelectionMode = OxyPlot.SelectionMode.Multiple
            });
            Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                TickStyle = OxyPlot.Axes.TickStyle.None,
                //MajorGridlineStyle = LineStyle.Dash,
                //MinorGridlineStyle = LineStyle.Dash,
                MaximumPadding = 0.1,
                MinimumPadding = 0.1,
                IsPanEnabled = false,
                IsZoomEnabled = false,
                FontSize = 0.1,
                SelectionMode = OxyPlot.SelectionMode.Multiple
            });

            Load_Mouse_Events(list_matrix_series, list_work_series, work_model, matrix2D_work_plot);
        }

        private void Load_Mouse_Events
        (
            List<ScatterSeries> list_matrix_series, 
            List<ScatterSeries> list_work_series,
            Work_Model work_model,
            OxyPlot.Wpf.PlotView matrix2D_work_plot
        )
        {
            Series series = null;
            this.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    Tab_Workspace.id = index;

                    if (model_pressed == false)
                    {
                        work_model.Series.Add(list_work_series[Tab_Workspace.id]);
                        model_pressed = true;
                    }

                    matrix2D_work_plot.Model = work_model;
                    series = work_model.Series[0];
                    series.SelectionMode = OxyPlot.SelectionMode.Multiple;

                    for (int i = 0; i < list_matrix_series[Tab_Workspace.id].Points.Count; i++)
                    {
                        if (list_matrix_series[Tab_Workspace.id].IsItemSelected(i))
                        {
                            list_work_series[Tab_Workspace.id].SelectItem(i);
                        }
                    }
                    work_model.Background = OxyColors.White;

                    InvalidatePlot(false);
                    e.Handled = true;
                }
            };

            this.MouseUp += (s, e) =>
            {
                Background = OxyColors.White;

                InvalidatePlot(false);
                e.Handled = true;
            };

            this.MouseEnter += (s, e) =>
            {
                PlotAreaBorderColor = OxyColors.Crimson;

                InvalidatePlot(false);
                e.Handled = true;
            };

            this.MouseLeave += (s, e) =>
            {
                Background = OxyColors.White;
                PlotAreaBorderColor = OxyColors.Black;

                InvalidatePlot(false);
                e.Handled = true;
            };
        }
    }

    /// <summary>
    /// Класс, представляющий график, который выводится выводится в рабочую область
    /// </summary>
    class Work_Model : PlotModel
    {
        public int index;
        public List<int> selected_points = new List<int>();

        /// <summary>
        /// Позволяет получить доступ к списку индексов выделенных точек из самого класса
        /// Дублирует список выделенных точек, чтобы этот можно было спокойно из
        /// </summary>
        public List<int> Selected_points
        {
            get { return selected_points; }
        }

        public Work_Model
        (
            int plot_index,
            string[] op_tit,
            string[] x_tit, 
            string[] y_tit, 
            List<ScatterSeries> list_work_series, 
            List<ScatterSeries> list_matrix_series, 
            List<Matrix_Model> list_matrix_models
        )
        {
            Title = op_tit[plot_index];
            index = plot_index;
            PlotAreaBorderThickness = new OxyThickness(1, 0, 0, 1);
            PlotMargins = new OxyThickness(30, 0, 0, 30);
            IsLegendVisible = false;
            SelectionColor = OxyColors.Crimson;

            Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                TickStyle = OxyPlot.Axes.TickStyle.None,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dash,
                MaximumPadding = 0,
                MinimumPadding = 0,
                Title = x_tit[plot_index],
                SelectionMode = OxyPlot.SelectionMode.Multiple
            });
            Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                TickStyle = OxyPlot.Axes.TickStyle.None,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dash,
                MaximumPadding = 0,
                MinimumPadding = 0,
                Title = y_tit[plot_index],
                SelectionMode = OxyPlot.SelectionMode.Multiple
            });
            
            Load_Mouse_Events
            (
                list_work_series, 
                list_matrix_series, 
                list_matrix_models
            );
        }

        private void Load_Mouse_Events
        (
            List<ScatterSeries> list_work_series, 
            List<ScatterSeries> list_matrix_series, 
            List<Matrix_Model> list_matrix_models
        )
        {
            var pressed_button = "none";
            //Selection
            LineSeries l_series = null;
            PolygonAnnotation selection_annotation = null;
            this.MouseDown += (s, e) =>
            {
                Tab_Workspace.id = this.index;
                //DEBUG:Id Tracking
                //work_model.Subtitle = main_form.id.ToString(); 
                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    pressed_button = "left";
                    selection_annotation = new PolygonAnnotation();
                    selection_annotation.Layer = AnnotationLayer.BelowSeries;

                    l_series = new LineSeries
                    {
                        Color = OxyColors.Black,
                        StrokeThickness = 1.5,
                        LineStyle = OxyPlot.LineStyle.LongDashDot,
                        MinimumSegmentLength = 0.1,
                        CanTrackerInterpolatePoints = true,
                    };
                    this.Series.Add(l_series);
                    this.InvalidatePlot(true);
                    e.Handled = true;
                }
            };

            this.MouseMove += (s, e) =>
            {
                if (l_series != null && l_series.XAxis != null)
                {
                    l_series.Points.Add(l_series.InverseTransform(e.Position));
                    this.InvalidatePlot(false);
                }
            }; 
            
            this.MouseUp += (s, e) =>
            {        
                switch (pressed_button)
                {
                    case "left":
                        //Список выделенных точек
                        //Специально сделан локально, т.к. здесь его трогать НИ В КОЕМ СЛУЧАЕ нельзя
                        //Для этого есть специльный отдельный список (ВЫШЕ /\)
                        List<int> points = new List<int>();
                        selection_annotation.Points.AddRange(l_series.Points);
                        this.Annotations.Add(selection_annotation);

                        foreach (ScatterPoint scatter_point in list_work_series[Tab_Workspace.id].Points)
                        {
                            bool inside = IsPointInPolygon(l_series.Points, scatter_point);
                            if (inside == true)
                            {
                                int point_index = list_work_series[Tab_Workspace.id].Points.FindIndex(a => a == scatter_point);
                                if (list_work_series[Tab_Workspace.id].IsItemSelected(point_index) == false)
                                {
                                    list_work_series[Tab_Workspace.id].SelectItem(point_index);
                                    points.Add(point_index);
                                }
                            }
                        }

                        if (points.Count != 0)
                        {
                            for (int i = 0; i < list_matrix_series.Count; i++)
                            {
                                foreach (ScatterPoint s_point in list_matrix_series[i].Points)
                                {
                                    foreach (int index in points)
                                    {
                                        list_matrix_series[i].SelectItem(index);
                                    }
                                }
                            }
                        }

                        foreach (Matrix_Model plot in list_matrix_models)
                        {
                            plot.InvalidatePlot(true);
                        }

                        this.Series.Remove(l_series);
                        this.Annotations.Remove(selection_annotation);

                        this.InvalidatePlot(true);
                        e.Handled = true;

                        selected_points = points;
                        pressed_button = "none";
                        break;

                    case "right":
                        list_work_series[Tab_Workspace.id].XAxis.Pan(e.Position, e.Position);
                        list_work_series[Tab_Workspace.id].YAxis.Pan(e.Position, e.Position);

                        pressed_button = "none";
                        break;
                }
            };

            this.MouseEnter += (s, e) =>
            {
                Tab_Workspace.id = this.index;
                this.PlotAreaBorderColor = OxyColors.Red;
                this.InvalidatePlot(false);
                e.Handled = true;
            };

            this.MouseLeave += (s, e) =>
            {
                this.PlotAreaBorderColor = OxyColors.Black;
                this.InvalidatePlot(false);
                e.Handled = true;
            };
        }

        // Алгоритм поиска точек внутри многоугольника (во время выделения области)
        private static bool IsPointInPolygon(List<DataPoint> polygon, ScatterPoint point_to_check)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < point_to_check.Y && polygon[j].Y >= point_to_check.Y || polygon[j].Y < point_to_check.Y && polygon[i].Y >= point_to_check.Y)
                {
                    if (polygon[i].X + (point_to_check.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < point_to_check.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }
    }
}
