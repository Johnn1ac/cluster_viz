using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ConverterLibrary;
using System.IO;
using System.Data;
using System.Reflection;
using ExcelObj = Microsoft.Office.Interop.Excel;

namespace Cluster_Analisys
{
    /// <summary>
    /// Interaction logic for Tab_MDS.xaml
    /// </summary>
    public partial class Tab_MDS : UserControl
    {
        string file;
        string selection;

        int k_dim;
        double p;


        /// <summary>
        /// Конструктор - Метод Евклидовый
        /// </summary>
        /// <param name="file_value">Файл</param>
        /// <param name="selection_value">Метод</param>
        /// <param name="k_dim_value">Размерность</param>
        public Tab_MDS(string file_value, string selection_value, int k_dim_value)
        {
            file = file_value;
            selection = selection_value;

            k_dim = k_dim_value;

            InitializeComponent();
            BuildMatrix(file_value, selection, k_dim);
        }
        /// <summary>
        /// Конструктор - Метод Минковского
        /// </summary>
        /// <param name="file_value">Файл</param>
        /// <param name="selection_value">Метод</param>
        /// <param name="k_dim_value">Размерность</param>
        /// <param name="p_value">Сила P</param>
        public Tab_MDS(string file_value, string selection_value, int k_dim_value, double p_value)
        {
            file = file_value;
            selection = selection_value;

            k_dim = k_dim_value;
            p = p_value;

            InitializeComponent();
            BuildMatrix(file_value, selection, k_dim, p);
        }

        /// <summary>
        /// Построение матрицы "Евклидовый"
        /// </summary>
        /// <param name="method"></param>
        /// <param name="k_dim"></param>
        public void BuildMatrix(string f, string method, int k_dim)
        {
            File_Converter converter = new File_Converter();

            DataTable excel_data = converter.Load_Excel_Table_From_File(f);

            Data_Converter d_conv = new Data_Converter();

            MDS_Matrix matrix = new MDS_Matrix(excel_data, f);
            
            //Matrix_2D Matrix_2D = new Matrix_2D();

           // Matrix_2D.Test_Builder(excel_data, matrix, d_conv.list_m_s, d_conv.list_w_s);

            this.tabControl.Items.Add(new TabItem
            {
                Header = selection + k_dim.ToString(),
                Content = matrix,
                IsSelected = true
            });
        }
        
        /// <summary>
        /// Построение матрицы "Минковский"
        /// </summary>
        /// <param name="method"></param>
        /// <param name="k_dim"></param>
        /// <param name="p"></param>
        public void BuildMatrix(string f, string method, int k_dim, double p)
        {
            File_Converter converter = new File_Converter();

            DataTable excel_data = converter.Load_Excel_Table_From_File(f);

            Data_Converter d_conv = new Data_Converter();

            MDS_Matrix matrix = new MDS_Matrix(excel_data, f);
            this.tabControl.Items.Add(new TabItem
            {
                Header = selection + k_dim.ToString() + p.ToString(),
                Content = matrix,
                IsSelected = true
            });
        }

        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu cm = (ContextMenu)sender;
            var target = cm.PlacementTarget;

            if (e.Source == Delete_tab)
            {
                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить эту вкладку?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    this.tabControl.Items.Remove(this.tabControl.SelectedItem);
                }
            }
        }  
    }
}
