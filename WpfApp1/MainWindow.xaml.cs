using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
        const string path_to_FS = @"C:\Users\Sova IS\Desktop\WPF_triangular_matrix_root\Матрицы\";//путь в корне программы до папки с матрицами
        readonly ImageSource[] orientations  = new ImageSource[4];//массив на 4 пути к картинкам с ориентацией треуг матрицы
        int index_image=0;//индекс текущей картинки в массиве orientations
        List<string> list_matrices = new List<string>();
        int orientation_number = 1; 
        int dimension=50;
        string[,] array_inp = new string[50, 50];//массив для работы со вторым datagrid
        double typical_elt = 0;//значение однотип элемента
        string[,] array_dense = new string[0,0];
        bool cancel_handle_norm_form = false;//проверяет отменил ли пользователь переход из dense form в normal form
        public MainWindow()
        {
            InitializeComponent();
            ImageSourceConverter cvrt = new ImageSourceConverter();
            orientations[0] = (ImageSource)cvrt.ConvertFromString(@"C:\Users\Sova IS\source\repos\WpfApp1\WpfApp1\Images\Orientations for button\ОР.png");
            orientations[1] = (ImageSource)cvrt.ConvertFromString(@"C:\Users\Sova IS\source\repos\WpfApp1\WpfApp1\Images\Orientations for button\OP2.png");
            orientations[2] = (ImageSource)cvrt.ConvertFromString(@"C:\Users\Sova IS\source\repos\WpfApp1\WpfApp1\Images\Orientations for button\OP3.png");
            orientations[3] = (ImageSource)cvrt.ConvertFromString(@"C:\Users\Sova IS\source\repos\WpfApp1\WpfApp1\Images\Orientations for button\OP4.png");

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
         public bool validate_2Darray(string[,] array,int dim1,int dim2,string add_message)
         {
            for (int i = 0; i < dim1; i++)
            {
                for (int j = 0; j < dim2; j++)
                {
                    try
                    {
                        double tmp = Convert.ToDouble(array[i, j]);
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show($"Ошибка {add_message}! Элемент в строке {i} и стобце {j} не является действительным числом!", "Ошибка при вводе элементов");
                        return false;
                    }
                }
            }
            return true;
         }

        //Логически важные методы (Лучше вынести в класс потом )
        private void save_matrix_in_file(string File_path,string message)
        {
            

                if ((TabItem_Normal_Form.IsSelected && validate_2Darray(array_inp, dimension, dimension, $"при {message} матрицы в файл  из Матричной формы")) ||
                        (TabItem_Dense_Form.IsSelected && validate_2Darray(array_dense, amount_of_not_null_elts(dimension), 1, $"при {message}  матрицы в файл из Упакованной формы"))) //если в массивах только double числа
                    using (var file = File.CreateText(File_path))
                    {
                        file.WriteLine(dimension);
                        file.WriteLine(orientation_number);
                        file.WriteLine(typical_elt);

                        if (TabItem_Normal_Form.IsSelected)
                        {

                            for (int i = 0; i < dimension; i++)
                            {
                                string row_buffer = "";
                                for (int j = 0; j < dimension; j++)
                                {
                                    row_buffer += array_inp[i, j] + " ";
                                }
                                file.WriteLine(row_buffer.TrimEnd(' '));
                            }
                            MessageBox.Show("Файл с матрицей успешно сохранен!");
                        }
                        else
                        {
                            if (TabItem_Dense_Form.IsSelected)//если матрица в норм форме и все элементы - double
                            {
                                int counter = 0;//обращение к упакованному массиву
                                for (int i = 0; i < dimension; i++)
                                {
                                    string row_buffer = "";
                                    for (int j = 0; j < dimension; j++)
                                    {
                                        switch (orientation_number)
                                        {
                                            case 1:

                                                if (i + j < dimension - 1)
                                                {
                                                    row_buffer += array_dense[counter, 0] + " ";
                                                    counter++;
                                                }
                                                else
                                                {
                                                    row_buffer += typical_elt.ToString() + " ";
                                                }
                                                break;

                                            case 2:

                                                if (!(i + j < dimension))
                                                {
                                                    row_buffer += array_dense[counter, 0] + " ";
                                                    counter++;
                                                }
                                                else
                                                {
                                                    row_buffer += typical_elt.ToString() + " ";
                                                }
                                                break;

                                            case 3:

                                                if (!(i >= j))
                                                {
                                                    row_buffer += array_dense[counter, 0] + " ";
                                                    counter++;
                                                }
                                                else
                                                {
                                                    row_buffer += typical_elt.ToString() + " ";
                                                }
                                                break;

                                            case 4:

                                                if (!(i <= j))
                                                {
                                                    row_buffer += array_dense[counter, 0] + " ";
                                                    counter++;
                                                }
                                                else
                                                {
                                                    row_buffer += typical_elt.ToString() + " ";
                                                }
                                                break;

                                            default: throw new Exception("Не распознана ориентация для заполнения упакованной формы при записи в файл ");
                                        }
                                    }
                                    file.WriteLine(row_buffer.TrimEnd(' '));
                                }
                                MessageBox.Show("Файл с матрицей успешно сохранен!");
                            }
                            else;
                        }

                    }
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
            var dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "TXT Files|*.txt";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

           
            if (result == true)
            {
                using (var file = new StreamReader(dlg.FileName))
                {
                    string line;
                    int number_of_rows = 0;

                    int local_dimension = 0;
                    int local_orientation_number;
                    double local_typical_elt;
                    //1)размерность
                    //2)ориентация
                    //3)однотип эл-нт
                    if ((line = file.ReadLine()) != null)//размерность
                    {
                        try
                        {
                            local_dimension = Convert.ToInt32(line);
                            if (local_dimension > 0 && local_dimension <=100000)
                            {

                            }
                            else
                            {
                                MessageBox.Show($"В файле {dlg.FileName} размерность выходит из диапазона {{0:100,000}}!", "ОШИБКА ПРИ СЧИТЫВАНИИ ФАЙЛА");
                                return;
                            }
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show($"В файле {dlg.FileName} указана неверная размерность!", "ОШИБКА ПРИ СЧИТЫВАНИИ ФАЙЛА");
                            return;
                        }

                        if ((line = file.ReadLine()) != null)//ориентация
                        {
                            try
                            {
                                local_orientation_number = Convert.ToInt32(line);
                                if (local_orientation_number>0 && local_orientation_number<=4)
                                {

                                }
                                else
                                {
                                    MessageBox.Show($"В файле {dlg.FileName} указана неверная ориентация! Виды ориентации:1)1 -ВЫШЕ Побочной 2)2 -НИЖЕ Побочной 3)3 -ВЫШЕ Главной 4)4 -НИЖЕ Главной ",
                                        "ОШИБКА ПРИ СЧИТЫВАНИИ ФАЙЛА");
                                    return;
                                }
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show($"В файле {dlg.FileName} указана неверная ориентация!", "ОШИБКА ПРИ СЧИТЫВАНИИ ФАЙЛА");
                                return;
                            }

                            if ((line = file.ReadLine()) != null)//однотипный элемент
                            {
                                try
                                {
                                    local_typical_elt = Convert.ToDouble(line);
                                }
                                catch (FormatException)
                                {
                                    MessageBox.Show($"В файле {dlg.FileName} указан невереный однотипный элемент!", "ОШИБКА ПРИ СЧИТЫВАНИИ ФАЙЛА");
                                    return;
                                }

                                while ((line = file.ReadLine()) != null)
                                {
                                    string[] str_elts = line.Split(' ');

                                    int i = 0;
                                    for (; i < str_elts.Length; i++)
                                    {
                                        try
                                        {
                                            double tmp = Convert.ToDouble(str_elts[i]);
                                        }
                                        catch (FormatException)
                                        {

                                            MessageBox.Show($"В файле {dlg.FileName} не удалось преобразовать элемент №{i + 1} в строке {number_of_rows + 1 + 3} !", "ОШИБКА ПРИ СЧИТЫВАНИИ ФАЙЛА");
                                            return;
                                        }
                                    }
                                    number_of_rows++;
                                    if (i != local_dimension)//
                                    {
                                        MessageBox.Show($"В файле {dlg.FileName} число столбцов({i}) в строке {number_of_rows} не совпадает с размерностью{local_dimension}", "ОШИБКА ПРИ СЧИТЫВАНИИ ФАЙЛА");
                                        return;
                                    }
                                    else;//строка = размрности
                                }
                                if (number_of_rows != local_dimension)
                                {
                                    MessageBox.Show($"В файле {dlg.FileName} число строк({number_of_rows}) не совпадает с размерностью({local_dimension})", "ОШИБКА ПРИ СЧИТЫВАНИИ ФАЙЛА");
                                    return;
                                }
                                else//если в файле матрица квадратная
                                {
                                    //TODO: ДОБАВИТЬ ПРОВЕРКУ НА ОРИЕНТАЦИя но ОНА будет много жрать ЦП так как проверить нужно все элементы 
                                    //все в порядке то добавим в список и надо ли считывать?
                                    MessageBox.Show("Матрица в файле валидна");
                                    cancel_handle_norm_form = true;
                                    TabItem_Dense_Form.IsSelected = true;

                                    using (var file2 = new StreamReader(dlg.FileName))//второй раз открываю файл чтобы уже считать,так как он был проверен
                                    {
                                     //Пропустим размерность, ориентацию и однотип эл-нт   
                                        file2.ReadLine();
                                        file2.ReadLine();
                                        file2.ReadLine();
                                        dimension = local_dimension;
                                        orientation_number = local_orientation_number;
                                        typical_elt = local_typical_elt;
                                        index_image = orientation_number - 1;
                                        //обновляем графический интерфейс
                                        NumericUpDown.Value = dimension;
                                        textBox_typical_elt.Text = typical_elt.ToString();
                                        Image_OR.Source = orientations[index_image];
                                       //выделим память
                                        array_dense = new string[amount_of_not_null_elts(dimension), 1];
                                        int i = 0;
                                        int counter = 0;//по aray_dense
                                        while ((line = file2.ReadLine()) != null)
                                        {
                                            string[] str_elts = line.Split(' ');
                                            int j = 0;
                                            foreach (var item in str_elts)
                                            {
                                                switch (orientation_number)
                                                {
                                                    case 1:

                                                        if (i + j < dimension - 1)
                                                        {

                                                            array_dense[counter, 0] = item;
                                                            counter++;
                                                        }
                                                        else; //пустая строка

                                                        break;

                                                    case 2:

                                                        if (!(i + j < dimension))
                                                        {
                                                            array_dense[counter, 0] = item;
                                                            counter++;
                                                        }
                                                        else; //пустая строка
                                                        break;

                                                    case 3:

                                                        if (!(i >= j))
                                                        {
                                                            array_dense[counter, 0] = item;
                                                            counter++;
                                                        }
                                                        else; //пустая строка
                                                        break;

                                                    case 4:

                                                        if (!(i <= j))
                                                        {
                                                            array_dense[counter, 0] = item;
                                                            counter++;
                                                        }
                                                        else; //пустая строка
                                                        break;

                                                    default: throw new Exception("Не распознана ориентация для заполнения упакованной формы ");
                                                }
                                                j++;
                                            }
                                            i++;
                                        }
                                        
                                        dataGrid2D_sec_page.ItemsSource2D = array_dense;
                                        MessageBox.Show("Файл с матрицей успешно открыт!");
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show($"В файле {dlg.FileName} НЕ указано значение однотипного элемента!", "ОШИБКА ПРИ СЧИТЫВАНИИ ФАЙЛА");
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show($"В файле {dlg.FileName} НЕ указана ориентация!", "ОШИБКА ПРИ СЧИТЫВАНИИ ФАЙЛА");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Файл пуст!");
                    }
                }
               
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
            bool? result = dlg.ShowDialog();


            if (result == true)
            {
                save_matrix_in_file(dlg.FileName, "сохранении");
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
                    MessageBoxResult    boxResult= MessageBox.Show("Вы уверены что хотите создать новую матрицу в упакованной форме?", "ВОЗМОЖНАЯ ПОТЕРЯ ДАННЫХ", MessageBoxButton.YesNo);
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

                if (dimension < 100)
                {
                    dataGrid2D_sec_page.ItemsSource2D = new string[0,0];
                    dataGrid2D_sec_page.ItemsSource2D = array_inp;
                }
                else;
                
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
                    if (dimension < 100)
                    {
                        dataGrid2D_sec_page.ItemsSource2D = new string[0, 0];
                        dataGrid2D_sec_page.ItemsSource2D = array_dense;
                    }
                    else;
                }
                else;//адреса не реализованы(((((
            }
            
            MessageBox.Show("Матрица успешно заполнена случайными числами");
        }

        private void Button_Click_Add_To_List(object sender, RoutedEventArgs e)
        {
            
            
            if (TabItem_Normal_Form.IsSelected)
            {
                for (int i = 0; i < dimension; i++)
                {
                    for (int j = 0; j < dimension; j++)
                    {
                        if (string.IsNullOrEmpty(array_inp[i, j]))
                        {
                            MessageBox.Show($"Невозможно добавить матрицу в матричной форме т.к. в ней присутсвуют пустые элементы(строка{i} столбец{j})");
                            return;
                        }
                        else;//элемент на месте
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
                            MessageBox.Show($"Невозможно добавить матрицу в упакованной форме т.к. в ней присутсвуют пустые элементы(строка{i})");
                            return;
                        }
                        else;//элемент на месте
                    }
                }
                else;
            }
            WpfApp1.Window1_pop_up_input_matrix_name Matrix_name_pop_up = new WpfApp1.Window1_pop_up_input_matrix_name();
            Matrix_name_pop_up.ShowDialog();
            if (Matrix_name_pop_up.IsCancelled)
            {
               //ничего не делаем
            }
            else
            {
                string Matrix_name = Matrix_name_pop_up.get_Matrix_name;
                if (string.IsNullOrEmpty(Matrix_name))
                {
                    MessageBox.Show("Имя матрицы не может быть пустым!");
                }
                else
                {

                    if (list_matrices.Find(delegate (string str) { return str == Matrix_name; }) == null)
                    {
                        save_matrix_in_file(path_to_FS + Matrix_name + ".txt", "добавлении");
                        list_matrices.Add(Matrix_name);
                        List_Matrices.ItemsSource = new List<string>();//чтобы при обновлении списка обновлялся интерфейс
                        List_Matrices.ItemsSource = list_matrices;
                        Label_Matrix_Name.Text = Matrix_name;
                    }
                    else
                    {
                        MessageBox.Show("Матрица с таким названием уже существует!","ОШИБКА ПРИ ДОБАВЛЕНИИ МАТРИЦЫ");
                    }
                    
                }
            }
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<string> sorted_list = new List<string>();
            List<string> left_elts = new List<string>();
            foreach (var item in List_Matrices.Items)
            {
                if (item.ToString().StartsWith(text_box_search.Text))//
                {
                    sorted_list.Add(item.ToString());
                }
                else
                {
                    left_elts.Add(item.ToString());
                }
            }
            List_Matrices.ItemsSource =  sorted_list.Concat(left_elts);
        }

        private void Button_Click_delete_from_Matrix_List(object sender, RoutedEventArgs e)
        {
            string file_path = path_to_FS + List_Matrices.SelectedItem + ".txt";

            if (List_Matrices.SelectedItem is  null)
            {
                //ничего не делаем
            }
            else
            {
                if (File.Exists(file_path))
                {
                    
                    if (MessageBox.Show($"Вы уверены что хотите удалить матрицу \"{List_Matrices.SelectedItem}\"?", "Подтвердите удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {

                        if (List_Matrices.SelectedItem.ToString() == Label_Matrix_Name.Text)//если пользователь удаляет матрицу которая открыта
                        {
                            Label_Matrix_Name.Text = "СОЗДАЙТЕ или ВЫБЕРИТЕ матрицу";//мы в файле удалили но не в ОП оставили
                        }
                        else;//просто удалим
                        
                        
                        File.Delete(file_path);
                        list_matrices.Remove(List_Matrices.SelectedItem.ToString());
                        List_Matrices.ItemsSource = new List<string>();
                        List_Matrices.ItemsSource = list_matrices;
                        MessageBox.Show("Матрица успешно удалена");
                    }
                    else
                    {
                         //мы ничего не удаляем
                    }
                    
                }
                else//если файла нет то, удалим его из списка раз пользователь влез в файловый каталог проги 
                {
                    MessageBox.Show("Матрица для удаления не найдена!");
                    list_matrices.Remove(List_Matrices.SelectedItem.ToString());
                    List_Matrices.ItemsSource = new List<string>();
                    List_Matrices.ItemsSource = list_matrices;
                }
              
            }
            
        }

        private void List_Matrices_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (List_Matrices.SelectedItem is null)
            {

            }
            else
            {
                if (List_Matrices.SelectedItem.ToString() == Label_Matrix_Name.Text)
                {
                    //матрица и так уже выбрана
                }
                else
                {
                    cancel_handle_norm_form = true;
                    TabItem_Dense_Form.IsSelected = true;
                    string line = "";
                    using (var file = new StreamReader(path_to_FS + List_Matrices.SelectedItem + ".txt"))
                    {
                        dimension = Convert.ToInt32(file.ReadLine());
                        orientation_number = Convert.ToInt32(file.ReadLine());
                        typical_elt = Convert.ToDouble(file.ReadLine());
                        index_image = orientation_number - 1;
                        NumericUpDown.Value = dimension;
                        Image_OR.Source = orientations[index_image];
                        textBox_typical_elt.Text = typical_elt.ToString();
                        
                        //TODO : ВЫНЕСТИ КУСОК КОДА НИЖЕ в отдельную функция так как 2 раза повторяется
                        array_dense = new string[amount_of_not_null_elts(dimension), 1];
                        int i = 0;
                        int counter = 0;//по aray_dense
                        while ((line = file.ReadLine()) != null)
                        {
                            string[] str_elts = line.Split(' ');
                            int j = 0;
                            foreach (var item in str_elts)
                            {
                                switch (orientation_number)
                                {
                                    case 1:

                                        if (i + j < dimension - 1)
                                        {

                                            array_dense[counter, 0] = item;
                                            counter++;
                                        }
                                        else; //пустая строка

                                        break;

                                    case 2:

                                        if (!(i + j < dimension))
                                        {
                                            array_dense[counter, 0] = item;
                                            counter++;
                                        }
                                        else; //пустая строка
                                        break;

                                    case 3:

                                        if (!(i >= j))
                                        {
                                            array_dense[counter, 0] = item;
                                            counter++;
                                        }
                                        else; //пустая строка
                                        break;

                                    case 4:

                                        if (!(i <= j))
                                        {
                                            array_dense[counter, 0] = item;
                                            counter++;
                                        }
                                        else; //пустая строка
                                        break;

                                    default: throw new Exception("Не распознана ориентация для заполнения упакованной формы ");
                                }
                                j++;
                            }
                            i++;
                        }
                        dataGrid2D_sec_page.ItemsSource2D = array_dense;
                        Label_Matrix_Name.Text = List_Matrices.SelectedItem.ToString();
                        MessageBox.Show($"Матрица \"{List_Matrices.SelectedItem}\" успешно прогружена!");
                    }
                
                }
            }

        }

        private void TabItem_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            throw new NotImplementedException();
        }
    }
}
                         
