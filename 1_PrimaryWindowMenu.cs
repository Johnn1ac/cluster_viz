using System;
using System.Windows;
using System.Data;
using System.Windows.Controls;
using ConverterLibrary;
//-----------------------------------------------------------------------КОНВЕНЦИЯ СТИЛЯ--------------------------------------------------------------------------------------
//---------------------------------------------------------------ВСЕ ПЕРЕМЕННЫЕ - С МАЛЕНЬКОЙ БУКВЫ!--------------------------------------------------------------------------
//-------------------------------------------------------------------МЕТОДЫ И КЛАССЫ - С БОЛЬШОЙ!-----------------------------------------------------------------------------
namespace Cluster_Analisys
{
    // В отдельный файл вынесены конфигурации и настройки меню
    partial class MainWindow : Window
    {
        #region Menu Items Click
        #region File
        private void Main_File_Open_Click(object sender, RoutedEventArgs e) // Меню -> Файл -> Отрыть
        {
            File_Converter converter = new File_Converter();
            DataTable excel_table_data = converter.Load_Excel_Table_From_File();
            if (excel_table_data != null)
            {
                Create_Workspace(excel_table_data, converter.File_name);
            }
            else
            {
                MessageBox.Show("Загрузка не удалась", "Не удалось загрузить файл Excel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void Main_File_Exit_Click(object sender, RoutedEventArgs e) // Меню -> Файл -> Выход
        {
            Exit_App(sender, e);
        }
        #endregion
        #region Folders
        private void Main_Folder_Input_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new File_Converter().Input_path);
        }

        private void Main_Folder_Output_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new File_Converter().Output_path);
        }
        #endregion
        #endregion

        #region Menu Events
        private void Create_Workspace(DataTable excel_temp_data, string file)
        {
            Tab_Workspace tab_content = new Tab_Workspace (excel_temp_data, file); // Доделать { } конструктор
            this.File_Tab_Control.Items.Add(new TabItem
            {
                Header = new TextBlock { Text = excel_temp_data.TableName },
                TabIndex = file_counter,
                Content = tab_content
            });
            this.File_Tab_Control.SelectedIndex = file_counter;
            file_counter = file_counter + 1;
        }

        // Выходы - перегрузка методов
        /// <summary>
        /// Выход 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_App(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите выйти из приложения?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }
        /// <summary>
        /// Выход 2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_App(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите выйти из приложения?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
                Environment.Exit(0);
            }
            else
            {
                e.Handled = false;
            }
        }
        #endregion
    }
}
