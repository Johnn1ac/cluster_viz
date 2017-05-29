using System.Windows;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using RDotNet;
//-----------------------------------------------------------------------КОНВЕНЦИЯ СТИЛЯ--------------------------------------------------------------------------------------
//---------------------------------------------------------------ВСЕ ПЕРЕМЕННЫЕ - С МАЛЕНЬКОЙ БУКВЫ!--------------------------------------------------------------------------
//-------------------------------------------------------------------МЕТОДЫ И КЛАССЫ - С БОЛЬШОЙ!-----------------------------------------------------------------------------
namespace Cluster_Analisys
{
    /// <summary>
    /// Класс для работы с языком R
    /// </summary>
    class R_framework
    {
        private REngine r_engine;

        private static string a32_path;
        private static string a64_path;

        private static string a32_dll;
        private static string a64_dll;

        private static string r_home;

        private static string r_tools_zip;

        private static string input_path;
        private static string output_path;

        protected internal R_framework()
        {
            Initialize_R();
        }

        /// <summary>
        /// Инициализация и настройка R.NET
        /// </summary>
        private void Initialize_R()
        {
            Set_Paths();

            REngine.SetEnvironmentVariables(a64_path, r_home);
            r_engine = REngine.GetInstance(a64_dll);
            r_engine.Initialize();

            // Установка рабочей директории + загрузка библиотеки + пути к RTools Zip 
            r_engine.Evaluate("setwd('" + input_path + "')");

            // Загрузка библиотеки + пути к RTools Zip 
            r_engine.Evaluate(@"library(openxlsx)
            Sys.setenv(R_ZIPCMD = '" + r_tools_zip + "')");
        }

        /// <summary>
        /// Установка путей для R
        /// </summary>
        private void Set_Paths()
        {
            a32_path = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\R\\R-3.3.2\\bin\\i386").Replace("\\", "//");
            a64_path = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\R\\R-3.3.2\\bin\\x64").Replace("\\", "//");

            a32_dll = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\R\\R-3.3.2\\bin\\i386\\R.dll").Replace("\\", "//");
            a64_dll = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\R\\R-3.3.2\\bin\\x64\\R.dll").Replace("\\", "//");

            r_home = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\R\\R-3.3.2").Replace("\\", "//");

            r_tools_zip = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\R\\Rtools\\bin\\zip").Replace("\\", "//");

            input_path = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\input_files").Replace("\\", "//");
            output_path = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\output_files").Replace("\\", "//");
        }

        /// <summary>
        /// Многомерное шкалировние с помощью языка R. Euclidean method
        /// </summary>
        /// <param name="k">Размерность (dimension)</param>
        /// <param name="file_name">Входной файл</param>
        /// <param name="header">Заголовок</param>
        protected internal string Perform_MDS(int k, string file_name, string[] header)
        {
            // Начало подсчета время выполнения метода
            Stopwatch stopwatch = Stopwatch.StartNew();

            string output_file = "MDS_euclidean_d_" + k + "_" + file_name;

            var mydata = r_engine.Evaluate("mydata <- read.xlsx(xlsxFile = '" + file_name + "', colNames = TRUE)");
            r_engine.SetSymbol("mydata", mydata);

            //var d = r_engine.Evaluate("d <- dist(mydata) # euclidean distances between the rows");
            var d = r_engine.Evaluate("d <- dist(mydata, method = 'euclidean')");
            r_engine.SetSymbol("d", d);

            var fit = r_engine.Evaluate("fit <- cmdscale(d, eig = TRUE, k = " + k + ") # k is the number of dim");
            r_engine.SetSymbol("fit", fit);

            //Здесь нужно добавить в HEADER строку с названиями из оригинального файла (List?)

            string header_r = null;
            for(int i = 0; i < k; i++)
            {
                if(i != k-1)
                {
                    header_r = header_r + "'" + header[i] + "',";
                }
                else
                {
                    header_r = header_r + "'" + header[i] + "'";
                }
            }

            r_engine.Evaluate("colnames(fit$points) <- c(" + header_r + ")");

            r_engine.Evaluate("setwd('" + output_path + "')");
            r_engine.Evaluate("write.xlsx(fit$points, '" + output_file + "') # создание Excel файла");
            r_engine.Evaluate("setwd('" + input_path + "')");

            // Конец подсчета время выполнения метода
            stopwatch.Stop();
            return output_file;
        }
        /// <summary>
        /// Многомерное шкалировние с помощью языка R. Minkowski method
        /// </summary>
        /// <param name="k">Размерность (dimension)</param>
        /// <param name="p">Power of method</param>
        /// <param name="file_name">Входной файл</param>
        /// <param name="header">Заголовок</param>
        /// <returns></returns>
        protected internal string Perform_MDS(int k, double p, string file_name, string[] header)
        {
            // Начало подсчета время выполнения метода
            Stopwatch stopwatch = Stopwatch.StartNew();

            string output_file = "MDS_Minkowski_d" + k + "p" + p + "_" + file_name;

            var mydata = r_engine.Evaluate("mydata <- read.xlsx(xlsxFile = '" + file_name + "', colNames = TRUE)");
            r_engine.SetSymbol("mydata", mydata);

            //var d = r_engine.Evaluate("d <- dist(mydata) # euclidean distances between the rows");
            var d = r_engine.Evaluate("d <- dist(mydata, method = 'minkowski', p = " + p + ")");
            r_engine.SetSymbol("d", d);

            var fit = r_engine.Evaluate("fit <- cmdscale(d, eig = TRUE, k = " + k + ") # k is the number of dim");
            r_engine.SetSymbol("fit", fit);

            //Здесь нужно добавить в HEADER строку с названиями из оригинального файла (List?)

            string header_r = null;
            for (int i = 0; i < k; i++)
            {
                if (i != k - 1)
                {
                    header_r = header_r + "'" + header[i] + "',";
                }
                else
                {
                    header_r = header_r + "'" + header[i] + "'";
                }
            }

            r_engine.Evaluate("colnames(fit$points) <- c(" + header_r + ")");

            r_engine.Evaluate("setwd('" + output_path + "')");
            r_engine.Evaluate("write.xlsx(fit$points, '" + output_file + "') # создание Excel файла");
            r_engine.Evaluate("setwd('" + input_path + "')");

            // Конец подсчета время выполнения метода
            stopwatch.Stop();
            return output_file;
        }

        /// <summary>
        /// Неметрическое многомерное шкалировние - метод Крускала
        /// </summary>
        /// <param name="k"></param>
        /// <param name="file_name"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        protected internal string Perform_NM_MDS(int k, string file_name, string[] header)
        {
            // Начало подсчета время выполнения метода
            Stopwatch stopwatch = Stopwatch.StartNew();

            string output_file = "MDS_NM_sammon_d_" + k + "_" + file_name;
            r_engine.Evaluate("library(MASS)");
            var mydata = r_engine.Evaluate("mydata <- read.xlsx(xlsxFile = '" + file_name + "', colNames = TRUE)");
            r_engine.SetSymbol("mydata", mydata);

            //var d = r_engine.Evaluate("d <- dist(mydata) # euclidean distances between the rows");
            var d = r_engine.Evaluate("d <- dist(mydata, method = 'euclidean')");
            r_engine.SetSymbol("d", d);

            var fit = r_engine.Evaluate("fit <- sammon(d, k = " + k + ") # k is the number of dim");
            r_engine.SetSymbol("fit", fit);

            //Здесь нужно добавить в HEADER строку с названиями из оригинального файла (List?)

            string header_r = null;
            for (int i = 0; i < k; i++)
            {
                if (i != k - 1)
                {
                    header_r = header_r + "'" + header[i] + "',";
                }
                else
                {
                    header_r = header_r + "'" + header[i] + "'";
                }
            }

            r_engine.Evaluate("colnames(fit$points) <- c(" + header_r + ")");

            r_engine.Evaluate("setwd('" + output_path + "')");
            r_engine.Evaluate("write.xlsx(fit$points, '" + output_file + "') # создание Excel файла");
            r_engine.Evaluate("setwd('" + input_path + "')");

            // Конец подсчета время выполнения метода
            stopwatch.Stop();
            return output_file;
        }

        /// <summary>
        /// Неметрическое многомерное шкалировние - метод Саммона
        /// </summary>
        /// <param name="k"></param>
        /// <param name="p"></param>
        /// <param name="file_name"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        protected internal string Perform_NM_MDS(int k, double p, string file_name, string[] header)
        {
            // Начало подсчета время выполнения метода
            Stopwatch stopwatch = Stopwatch.StartNew();

            string output_file = "MDS_NM_kruskal_d_" + k + "_p_" + p + "_" + file_name;
            r_engine.Evaluate("library(MASS)");
            var mydata = r_engine.Evaluate("mydata <- read.xlsx(xlsxFile = '" + file_name + "', colNames = TRUE)");
            r_engine.SetSymbol("mydata", mydata);

            //var d = r_engine.Evaluate("d <- dist(mydata) # euclidean distances between the rows");
            var d = r_engine.Evaluate("d <- dist(mydata, method = 'euclidean')");
            r_engine.SetSymbol("d", d);

            var fit = r_engine.Evaluate("fit <- isoMDS(d, k = " + k + ", p = " + p + ") # k is the number of dim");
            r_engine.SetSymbol("fit", fit);

            //Здесь нужно добавить в HEADER строку с названиями из оригинального файла (List?)

            string header_r = null;
            for (int i = 0; i < k; i++)
            {
                if (i != k - 1)
                {
                    header_r = header_r + "'" + header[i] + "',";
                }
                else
                {
                    header_r = header_r + "'" + header[i] + "'";
                }
            }

            r_engine.Evaluate("colnames(fit$points) <- c(" + header_r + ")");

            r_engine.Evaluate("setwd('" + output_path + "')");
            r_engine.Evaluate("write.xlsx(fit$points, '" + output_file + "') # создание Excel файла");
            r_engine.Evaluate("setwd('" + input_path + "')");

            // Конец подсчета время выполнения метода
            stopwatch.Stop();
            return output_file;
        }
    }
}

