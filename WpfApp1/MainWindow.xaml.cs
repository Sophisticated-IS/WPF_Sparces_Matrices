using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Wpf_Matrices
{

    public partial class MainWindow
    {
      
        ImageSource[] orientations = new ImageSource[4];//массив на 4 пути к картинкам с ориентацией треуг матрицы
        int index_image=0;//индекс текущей картинки в массиве orientations

        int orientation_number = 1; 
        int dimension=50;
        string[,] array_inp = new string[50, 50];//массив для работы со вторым datagrid
        double typical_elt = 0;//значение однотип элемента
        string[,] array_dense = new string[0,0];
        bool cancel_handle_norm_form = false;
        public MainWindow()
        {
            InitializeComponent();
            ImageSourceConverter cvrt = new ImageSourceConverter();
            orientations[0] = (ImageSource)cvrt.ConvertFromString("C:\\Users\\Sova IS\\source\\repos\\WpfApp1\\WpfApp1\\bin\\Debug\\ОР.png");
            orientations[1] = (ImageSource)cvrt.ConvertFromString("C:\\Users\\Sova IS\\source\\repos\\WpfApp1\\WpfApp1\\bin\\Debug\\OP2.png");
            orientations[2] = (ImageSource)cvrt.ConvertFromString("C:\\Users\\Sova IS\\source\\repos\\WpfApp1\\WpfApp1\\bin\\Debug\\OP3.png");
            orientations[3] = (ImageSource)cvrt.ConvertFromString("C:\\Users\\Sova IS\\source\\repos\\WpfApp1\\WpfApp1\\bin\\Debug\\OP4.png");

            fill_array_depend_on_orient(array_inp, 50, 1,typical_elt);
            dataGrid2D_sec_page.ItemsSource2D = array_inp;
            double[,] default_datagrid_result = new double[20,100];
            dataGrid2D.ItemsSource2D = default_datagrid_result;
            
            ComboBox_orientation.SelectedIndex = 0;
            comboBox_name.SelectedIndex = 0;
            ComboBox_common_elts.SelectedIndex = 0;

        }
        //Логически важные методы (Лучше вынести в класс потом )
         public void fill_array_depend_on_orient (string[,] array,int dim, int orientation_number,double typical_elt)//заполняет однотипные эл-ты в массиве
         {
            if (dim>1)
            {
                for (int i = 0; i < dim; i++)
                {
                    for (int j = 0; j < dim; j++)
                    {
                        switch (orientation_number)
                        {
                            case 1:

                                if (i + j >= dim - 1)
                                {
                                    array[i, j] = typical_elt.ToString();
                                }
                                else; //пустая строка

                                break;

                            case 2:

                                if (i + j < dim)
                                {
                                    array[i, j] = typical_elt.ToString();
                                }
                                else; //пустая строка
                                break;

                            case 3:

                                if (i >= j)
                                {
                                    array[i, j] = typical_elt.ToString();
                                }
                                else; //пустая строка
                                break;

                            case 4:

                                if (i <= j)
                                {
                                    array[i, j] = typical_elt.ToString();
                                }
                                else; //пустая строка
                                break;

                            default: throw new Exception("Не распознана ориентация");
                        }

                    }
                }
            }
            else
            {
                //оставим пустым элемент
            }
         }
         public void validate_numericup_down ()
        {
            if (string.IsNullOrEmpty(NumericUpDown.Value.ToString()))
            {
                MessageBox.Show("Размерность не может быть пустой");
                NumericUpDown.Value = 1;
            }
            else;
         }
         public void validate_txt_box_only_double() 
         {
            if (string.IsNullOrEmpty(textBox_typical_elt.Text) || textBox_typical_elt.Text=="0")
            {
                textBox_typical_elt.Text = "0";
            }
            else
            {

                char[] str_numeric = textBox_typical_elt.Text.ToCharArray();
                bool one_point = true;
                bool one_minus = true;
                string handled_str = "";//храним обработанную строку
          
                for (int i = 0; i < str_numeric.Length;i++)//удаление всего кроме цифр 1 точки и 1 минуса
                {
                    if (char.IsDigit(str_numeric[i]))
                    {
                        handled_str += str_numeric[i];
                    }
                    else
                    {
                        if (one_minus && str_numeric[i] == '-' )
                        {
                            handled_str += '-';
                            one_minus = false;
                        }
                        else;

                        if (one_point && str_numeric[i] =='.')
                        {
                            handled_str += '.';
                            one_point = false;
                        }

                    }
                }
                textBox_typical_elt.Text = handled_str;
            }
        }
         public static int amount_of_not_null_elts(int dim)//сумма от 1 до числа-1
         {
            int amount = 0;

            if (dim == 1)
            {
                return 1;
            }
            else
            {
                for (int i = 1; i < dim; i++) // from 1 to figure -1 
                {
                    amount += i;
                }
                return amount;
            }
         }
        //Логически важные методы (Лучше вынести в класс потом )


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

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
           
            if(!string.IsNullOrEmpty( e.Text) )
            if ( char.IsDigit(e.Text[0]) || e.Text[0]=='.' || e.Text[0]=='-')
            {

            }
            else
            {
                e.Handled = true;//Значение true для обхода обработки элемента управления по умолчанию;
            }
        }    

        private void Button_Click_Create_Matrix(object sender, RoutedEventArgs e)
        {
            validate_numericup_down();
            validate_txt_box_only_double();//если кто-то кнопку нажмет через TAB
            MessageBoxResult messageBoxResult = new MessageBoxResult();

            if (TabItem_Normal_Form.IsSelected)
            {

                try
                {
                    int tmp_heavy_dim = (int)NumericUpDown.Value;
                    int tmp_orientation_number = index_image + 1;
                    double tmp_typical_elt = Convert.ToDouble(textBox_typical_elt.Text);

                    if (tmp_heavy_dim > 1000)
                    {
                        messageBoxResult = MessageBox.Show("Размер матрицы превышает 1000,поэтому отображение займет ОЧЕНЬ много времени, ВЫ УВЕРЕНЫ ЧТО ХОТИТЕ ПРОДОЛЖИТЬ?", "МАТРИЦА БОЛЬШОГО РАЗМЕРА", MessageBoxButton.YesNoCancel);
                    }
                    else
                    {
                        dimension = tmp_heavy_dim;
                        orientation_number = tmp_orientation_number;
                        typical_elt = tmp_typical_elt;
                        array_inp = new string[dimension, dimension];
                        fill_array_depend_on_orient(array_inp, dimension, orientation_number, typical_elt);
                        dataGrid2D_sec_page.ItemsSource2D = array_inp;
                        MessageBox.Show("Матрица создана успешно!");
                    }
                    //TODO:
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        dimension = tmp_heavy_dim;//если пользователь согласился только тогда меняем размерность
                        orientation_number = tmp_orientation_number;//то же самое с ориентацией
                        typical_elt = tmp_typical_elt;//и с элементом однотипным
                        array_inp = new string[dimension, dimension];
                        dataGrid2D_sec_page.ItemsSource2D = array_inp;
                        MessageBox.Show("Матрица создана успешно!");
                    }
                    else;//NO,CANCEL
                }
                catch (OutOfMemoryException)
                {
                    throw new OutOfMemoryException();
                }
                catch (FormatException)
                {
                    MessageBox.Show("Неверное значение однотипного элемента!!!", "Проверьте значение однотипного элемента");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Непредвиденная ошибка");
                }
            }
            else
            {
                if (TabItem_Dense_Form.IsSelected)
                {
                    MessageBoxResult boxResult = new MessageBoxResult();
                    boxResult= MessageBox.Show("Вы уверены что хотите создать новую матрицу в упакованной форме?", "ВОЗМОЖНАЯ ПОТЕРЯ ДАННЫХ", MessageBoxButton.YesNo);
                    if (boxResult == MessageBoxResult.Yes)
                    {
                        try
                        {
                            int tmp_dimension = (int)NumericUpDown.Value;
                            int tmp_number_orientation = index_image + 1;
                            double tmp_typical_elt = Convert.ToDouble(textBox_typical_elt.Text);

                            if (amount_of_not_null_elts(tmp_dimension)>1000000)
                            {
                                messageBoxResult = MessageBox.Show("Количество эл-ов превышает 1000000,поэтому след эл-ты будут отображаться отдельно ", "ОЧЕНЬ БОЛЬШОЕ КОЛИЧЕСТВО ЭЛЕМЕНТОВ", MessageBoxButton.YesNoCancel);
                            }
                            else
                            {
                                dimension = tmp_dimension;
                                orientation_number = tmp_number_orientation;
                                typical_elt = tmp_typical_elt;
                                array_dense = new string[amount_of_not_null_elts(dimension), 1];             
                                dataGrid2D_sec_page.ItemsSource2D = array_dense;
                            }
                            if (messageBoxResult == MessageBoxResult.Yes)
                            {
                                dimension = tmp_dimension;//если пользователь согласился только тогда меняем размерность
                                orientation_number = tmp_number_orientation;//то же самое с ориентацией
                                typical_elt = tmp_typical_elt;//и с элементом однотипным
                                array_dense = new string[amount_of_not_null_elts(dimension), 1];
                             //   array_inp = new string[dimension, dimension];//TODO:потеря в памяти если пользователь не заахочет переключаться на полную форму
                              //  fill_array_depend_on_orient(array_inp, dimension, orientation_number, typical_elt);
                                dataGrid2D_sec_page.ItemsSource2D = array_dense;
                                MessageBox.Show("Матрица в упакованной форме создана успешно!");
                            }
                            else;//NO
                        }
                        catch ( OutOfMemoryException)
                        {
                            throw new OutOfMemoryException();
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("Неверное значение однотипного элемента!!!", "Проверьте значение однотипного элемента");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Непредвиденная ошибка");
                        }

                    }
                    else;//ничего не делаем
                }
            }
        }

        private void textBox_typical_elt_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            validate_txt_box_only_double();
        }

        private void TabItem_GotFocus_Dense_Form(object sender, RoutedEventArgs e)
        {
            if (cancel_handle_norm_form)
            {
                cancel_handle_norm_form = false;
            }
            else
            {
                if (dimension > 1)
                {
                    int counter = 0;
                    array_dense = new string[amount_of_not_null_elts(dimension), 1];
                    for (int i = 0; i < dimension; i++)
                    {
                        for (int j = 0; j < dimension; j++)
                        {

                            switch (orientation_number)
                            {
                                case 1:

                                    if (i + j < dimension - 1)
                                    {

                                        array_dense[counter, 0] = array_inp[i, j];
                                        counter++;
                                    }
                                    else; //пустая строка

                                    break;

                                case 2:

                                    if (!(i + j < dimension))
                                    {
                                        array_dense[counter, 0] = array_inp[i, j];
                                        counter++;
                                    }
                                    else; //пустая строка
                                    break;

                                case 3:

                                    if (!(i >= j))
                                    {
                                        array_dense[counter, 0] = array_inp[i, j];
                                        counter++;
                                    }
                                    else; //пустая строка
                                    break;

                                case 4:

                                    if (!(i <= j))
                                    {
                                        array_dense[counter, 0] = array_inp[i, j];
                                        counter++;
                                    }
                                    else; //пустая строка
                                    break;

                                default: throw new Exception("Не распознана ориентация для заполнения упакованной формы ");
                            }
                        }
                    }
                }
                else
                {
                    array_dense = new string[1, 1];
                    array_dense[0, 0] = array_inp[0, 0];
                }
                dataGrid2D_sec_page.ItemsSource2D = array_dense;
            }
           
        }

        private void TabItem_GotFocus_Matrix_Form(object sender, RoutedEventArgs e)
        {
            if (dimension>1000)
            {
                if (MessageBox.Show("В матричной форме элементы займут ОЧЕНЬ много времени на вывод и памяти, ВЫ УВЕРЕНЫ ЧТО ХОТИТЕ ПРОДОЛЖИТЬ?",
                                  "ОЧЕНЬ БОЛЬШАЯ РАЗМЕРНОСТЬ", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
                {
                    array_inp = new string[dimension, dimension];
                    fill_array_depend_on_orient(array_inp, dimension, orientation_number, typical_elt);
                    dataGrid2D_sec_page.ItemsSource2D = array_inp;
                }
                else
                { 
                    cancel_handle_norm_form = true;
                    TabItem_Dense_Form.IsSelected = true;
                   
                }
            }
            else//по памяти и времени все ок
            {
                array_inp = new string[dimension, dimension];
                fill_array_depend_on_orient(array_inp, dimension, orientation_number, typical_elt);

                if (dimension > 1)//применим изменения эл-ов в упакованной форме для полной матричной
                {
                    int counter = 0;
                    for (int i = 0; i < dimension; i++)
                    {
                        for (int j = 0; j < dimension; j++)
                        {

                            switch (orientation_number)
                            {
                                case 1:

                                    if (i + j < dimension - 1)
                                    {
                                        array_inp[i, j] = array_dense[counter, 0];
                                        counter++;
                                    }
                                    else; //пустая строка

                                    break;

                                case 2:

                                    if (!(i + j < dimension))
                                    {
                                        array_inp[i, j] = array_dense[counter, 0];
                                        counter++;
                                    }
                                    else; //пустая строка
                                    break;

                                case 3:

                                    if (!(i >= j))
                                    {
                                        array_inp[i, j] = array_dense[counter, 0];
                                        counter++;
                                    }
                                    else; //пустая строка
                                    break;

                                case 4:

                                    if (!(i <= j))
                                    {
                                        array_inp[i, j] = array_dense[counter, 0];
                                        counter++;
                                    }
                                    else; //пустая строка
                                    break;

                                default: throw new Exception("Не распознана ориентация для заполнения упакованной формы ");
                            }
                        }
                    }
                }
                else
                {
                    array_dense = new string[1, 1];
                    array_dense[0, 0] = array_inp[0, 0];
                }

                dataGrid2D_sec_page.ItemsSource2D = array_inp;
            }
            
            

        }

        private void Button_Click_Auto_Fill(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            if (TabItem_Normal_Form .IsSelected)
            {
                for (int i = 0; i < dimension; i++)
                {
                    for (int j = 0; j < dimension; j++)
                    {
                        if (string.IsNullOrEmpty(array_inp[i, j]))
                        {
                            array_inp[i, j] = ( random.Next() + random.NextDouble() ).ToString();
                        }
                        else;//не заполняем непустые
                    }
                }
            }
            else
            {
                if (TabItem_Dense_Form.IsSelected)
                {
                    for (int i = 0; i < amount_of_not_null_elts(dimension); i++)
                    {
                        if (string.IsNullOrEmpty(array_dense[i, 0]))
                        {
                            array_dense[i, 0] = (random.Next() + random.NextDouble()).ToString();
                        }
                        else;//не заполняем непустые
                }
                }
                else;//адреса не реализованы(((((
            }
        }
    }                     
}                       
                         
