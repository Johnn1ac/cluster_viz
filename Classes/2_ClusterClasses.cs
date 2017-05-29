using OxyPlot;
using OxyPlot.Series;
//-----------------------------------------------------------------------КОНВЕНЦИЯ СТИЛЯ--------------------------------------------------------------------------------------
//---------------------------------------------------------------ВСЕ ПЕРЕМЕННЫЕ - С МАЛЕНЬКОЙ БУКВЫ!--------------------------------------------------------------------------
//-------------------------------------------------------------------МЕТОДЫ И КЛАССЫ - С БОЛЬШОЙ!-----------------------------------------------------------------------------
namespace Cluster_Analisys
{
    /// <summary>
    /// Представляет класс для кластера, который будет визуально отображаться на матрице 2D проекций
    /// </summary>
    class Matrix_Cluster : ScatterSeries
    {
        /// <summary>
        /// Индекс матричного кластера
        /// </summary>
        public int index;

        /// <summary>
        /// Название матричного кластера
        /// </summary>
        public string cluster_name;

        protected internal Matrix_Cluster(string marker_color, string marker_type)
        {
            MarkerType = (MarkerType) MarkerType.Parse(typeof(MarkerType), marker_type);
            MarkerSize = 3;
            Selectable = false;
            MarkerFill = OxyColor.Parse(marker_color);   
        }
    }

    /// <summary>
    /// Представляет класс для кластера, который будет визуально отображаться на рабочей области
    /// </summary>
    class Work_Cluster : ScatterSeries
    {
        /// <summary>
        /// Индекс рабочего кластера
        /// </summary>
        public int index;

        /// <summary>
        /// Название рабочего кластера
        /// </summary>
        public string cluster_name;


        protected internal Work_Cluster(string marker_color, string marker_type)
        {
            MarkerType = (MarkerType)MarkerType.Parse(typeof(MarkerType), marker_type);
            MarkerSize = 3;
            Selectable = false;
            MarkerFill = OxyColor.Parse(marker_color);
        }
    }
}
