using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Wpf_Matrices
{
    public partial class MainWindow
    {
        ImageSource[] orientations = new ImageSource[4];//массив на 4 пути к картинкам с ориентацией треуг матрицы
        int index_image=0;//индекс текущей картинки в массиве orientations
        public MainWindow()
        {
            InitializeComponent();
            ImageSourceConverter cvrt = new ImageSourceConverter();
            orientations[0] = (ImageSource)cvrt.ConvertFromString("C:\\Users\\Sova IS\\source\\repos\\WpfApp1\\WpfApp1\\bin\\Debug\\ОР.png");
            orientations[1] = (ImageSource)cvrt.ConvertFromString("C:\\Users\\Sova IS\\source\\repos\\WpfApp1\\WpfApp1\\bin\\Debug\\OP2.png");
            orientations[2] = (ImageSource)cvrt.ConvertFromString("C:\\Users\\Sova IS\\source\\repos\\WpfApp1\\WpfApp1\\bin\\Debug\\OP3.png");
            orientations[3] = (ImageSource)cvrt.ConvertFromString("C:\\Users\\Sova IS\\source\\repos\\WpfApp1\\WpfApp1\\bin\\Debug\\OP4.png");
            


            double[,] m_intArray = new double[21, 20];
            string[,] str_arr = new string[50, 50];
            string[,] str_arr2 = new string[50, 50];
            //for (int i = 0; i < 15; i++)
            //{
            //    for (int j = 0; j < 19; j++)
            //    {
            //        m_intArray[i, j] = (i * 10 + j);

            //    }
            //}
            //m_intArray[20, 19] = 56767454.98798976587;
            //dataGrid2D.ItemsSource2D = m_intArray;
            dataGrid2D.ItemsSource2D = str_arr;
            dataGrid2D_sec_page.ItemsSource2D = str_arr2;
            ComboBox_orientation.SelectedIndex = 0;
            comboBox_name.SelectedIndex = 0;
            ComboBox_common_elts.SelectedIndex = 0;
        }

        private void Save_Matrix_Dialog(object sender, System.Windows.RoutedEventArgs e)
        {
       
            WpfApp1.Window_save_result_matrix window_Save_Result = new WpfApp1.Window_save_result_matrix();
            window_Save_Result.ShowDialog();


        }


        private void Button_Orientation_Click(object sender, RoutedEventArgs e)
        {
            index_image++;
            if (index_image == 4)
            {
                index_image = 0;
            }
            else;
            Image_OR.Source = orientations[index_image];
        }

        private void Button_Open_existing_Matrix_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "TXT Files|*.txt";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

           
            if (result == true)
            {
                MessageBox.Show("Файл с матрицей успешно открыт!");
            }
        }

        private void Button_save_as_left_expander_Click(object sender, RoutedEventArgs e)
        {
            // Create SaveFileDialog 
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();



            // Set filter for Savefile extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "TXT Files|*.txt";


            // Display SaveFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            if (result == true)
            {
                MessageBox.Show("Файл с матрицей успешно сохранен!");
            }
        }

     }                     
}                       
                         
