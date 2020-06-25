using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MathNet.Numerics.LinearAlgebra;

namespace Wpf_Matrices
{
    public partial class MainWindow
    {
        string path_to_FS = "";//путь в корне программы до папки с матрицами
        readonly ImageSource[] orientations = new ImageSource[4];//массив на 4 пути к картинкам с ориентацией треуг матрицы
        int index_image = 0;//индекс текущей картинки в массиве orientations

        List<string> list_matrices = new List<string>();//хранится имя матрицы
        List<int> list_matrices_orient = new List<int>();
        List<string> list_matrices_typic_elt = new List<string>();//заменил double на string из-за ненужного округления
        int orientation_number = 1;
        int dimension = 50;
        string[,] Datagrid1stpage_result_matrix = new string[0, 0];//массив для работы с первым datagrid
        string[,] array_inp = new string[50, 50];//массив для работы со вторым datagrid
        double typical_elt = 0;//значение однотип элемента
        string[,] array_dense = new string[0, 0];
        bool cancel_handle_norm_form = false;//проверяет отменил ли пользователь переход из dense form в normal form

        //<Нужны для того чтобы отображать и сохранять результат вычислений матриц>
        int last_orientation = -1;
        double last_typical_elt = -1;
        char last_operation = ' ';
        int last_dimension = -1;
        //<Нужны для того чтобы отображать и сохранять результат вычислений матриц>
        public MainWindow()
        {
            InitializeComponent();
            DG_Expressions.ItemsSource = history_exprs;//привязка списка выражений хранящихся в гриде 
            orientations[0] = Imaging.CreateBitmapSourceFromHBitmap(WpfApp1.Properties.Resources.OP2.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            orientations[1] = Imaging.CreateBitmapSourceFromHBitmap(WpfApp1.Properties.Resources.ОР.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            orientations[2] = Imaging.CreateBitmapSourceFromHBitmap(WpfApp1.Properties.Resources.OP4.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            orientations[3] = Imaging.CreateBitmapSourceFromHBitmap(WpfApp1.Properties.Resources.OP3.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            Image_OR.Source = orientations[0];

            fill_array_depend_on_orient(array_inp, 50, 1, typical_elt);
            dataGrid2D_sec_page.ItemsSource2D = array_inp;
            double[,] default_datagrid_result = new double[25, 25];
            dataGrid2D.ItemsSource2D = default_datagrid_result;

            ComboBox_orientation.SelectedIndex = 0;
            comboBox_name.SelectedIndex = 0;
            ComboBox_common_elts.SelectedIndex = 0;
        }
        //Логически важные методы (Лучше вынести в класс потом )
        public void fill_array_depend_on_orient(string[,] array, int dim, int orientation_number, double typical_elt)//заполняет однотипные эл-ты в массиве
        {
            if (dim > 1)
            {
                for (int i = 0; i < dim; i++)
                {
                    for (int j = 0; j < dim; j++)
                    {
                        switch (orientation_number)
                        {
                            case 1:

                                if (i + j > dim - 1)
                                {
                                    array[i, j] = typical_elt.ToString();
                                }
                                else; //пустая строка

                                break;

                            case 2:

                                if (i + j < dim - 1)
                                {
                                    array[i, j] = typical_elt.ToString();
                                }
                                else; //пустая строка
                                break;

                            case 3:

                                if (i > j)
                                {
                                    array[i, j] = typical_elt.ToString();
                                }
                                else; //пустая строка
                                break;

                            case 4:

                                if (i < j)
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
        public void validate_numericup_down()
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
            if (string.IsNullOrEmpty(textBox_typical_elt.Text) || textBox_typical_elt.Text == "0")
            {
                textBox_typical_elt.Text = "0";
            }
            else
            {

                char[] str_numeric = textBox_typical_elt.Text.ToCharArray();
                bool one_point = true;
                bool one_minus = true;
                string handled_str = "";//храним обработанную строку

                for (int i = 0; i < str_numeric.Length; i++)//удаление всего кроме цифр 1 точки и 1 минуса
                {
                    if (char.IsDigit(str_numeric[i]))
                    {
                        handled_str += str_numeric[i];
                    }
                    else
                    {
                        if (one_minus && str_numeric[i] == '-')
                        {
                            handled_str += '-';
                            one_minus = false;
                        }
                        else;

                        if (one_point && str_numeric[i] == '.')
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
                for (int i = 1; i <= dim; i++) // from 1 to figure -1 
                {
                    amount += i;
                }
                return amount;
            }
        }
        public bool validate_2Darray(string[,] array, int dim1, int dim2, string add_message)
        {
            for (int i = 0; i < dim1; i++)
            {
                for (int j = 0; j < dim2; j++)
                {
                    try
                    {
                        double tmp = Convert.ToDouble(array[i, j]);
                    }
                    catch (OverflowException)
                    {
                        MessageBox.Show($"Ошибка {add_message}! Элемент в строке {i} и стобце {j} выходит за границы double!", "Ошибка при вводе элементов");
                        return false;
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
        public void remove_all_elts_combobox(ComboBox comboBox)//для combobox на первой странице
        {
            if (!(comboBox.Items.Count == 0))
            {
                        comboBox.Items.Clear();
            }
            else;//ничего не делаем
        }
        public void read_matrix_from_file(StreamReader file)//читает из файла элементы в упакованной форме в ГЛОБАЛЬНЫЙ массив array_dense ТОЛЬКО ДЛЯ сокращения кода
        {
            array_dense = new string[amount_of_not_null_elts(dimension), 1];
            int i = 0;
            int counter = 0;//по aray_dense
            string line;
            while ((line = file.ReadLine()) != null)
            {
                string[] str_elts = line.Split(' ');
                int j = 0;
                foreach (var item in str_elts)
                {
                    switch (orientation_number)
                    {
                        case 1:

                            if (i + j <= dimension - 1)
                            {

                                array_dense[counter, 0] = item;
                                counter++;
                            }
                            else; //пустая строка

                            break;

                        case 2:

                            if (i + j >= dimension - 1)
                            {
                                array_dense[counter, 0] = item;
                                counter++;
                            }
                            else; //пустая строка
                            break;

                        case 3:

                            if (i <= j)
                            {
                                array_dense[counter, 0] = item;
                                counter++;
                            }
                            else; //пустая строка
                            break;

                        case 4:

                            if (i >= j)
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
        }
        public List<double> get_elts_in_list_from_file(string matrix_name, int orient, int dim)//заполняет в список элементы из файла 
        {
            using (var reader = new StreamReader(path_to_FS + matrix_name + ".txt"))
            {
                //пропуск размерности ориентации и однотип элемента
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();


                List<double> list_from_file = new List<double>();
                if (dim == 1)
                {

                    list_from_file.Add(Convert.ToDouble(reader.ReadLine()));
                    return list_from_file;
                }
                else
                {


                    string line;
                    int i = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] str_elts = line.Split(' ');
                        int j = 0;
                        foreach (var item in str_elts)
                        {
                            switch (orient)
                            {
                                case 1:

                                    if (i + j <= dim - 1)
                                    {
                                        list_from_file.Add(Convert.ToDouble(item));
                                    }
                                    else;
                                    break;

                                case 2:

                                    if (i + j >= dim - 1)
                                    {
                                        list_from_file.Add(Convert.ToDouble(item));
                                    }
                                    else;
                                    break;

                                case 3:

                                    if (i <= j)
                                    {
                                        list_from_file.Add(Convert.ToDouble(item));
                                    }
                                    else;
                                    break;

                                case 4:

                                    if (i >= j)
                                    {
                                        list_from_file.Add(Convert.ToDouble(item));
                                    }
                                    else;
                                    break;

                                default: throw new Exception("Не распознана ориентация для заполнения упакованной формы ");
                            }
                            j++;
                        }
                        i++;
                    }
                    return list_from_file;
                }
            }
        }
        public void show_result_matrix(List<double> list_result, int dim, int orient, double value_typ_elt, bool result_is_full_matrix)//отвечает за отображение матрицы в гриде на первой странице
        {
            Matrix_name_1st_page.Text = "МАТРИЦА - РЕЗУЛЬТАТ";
            Datagrid1stpage_result_matrix = new string[dim, dim];

            if (dim == 1)
            {
                Datagrid1stpage_result_matrix[0, 0] = list_result.ElementAt(0).ToString();
            }
            else
            {
                int counter = 0;//двигаемся по списку с ненулевыми эл-ми
                string str_typ_elt = value_typ_elt.ToString();
                if (result_is_full_matrix)
                {
                    for (int i = 0; i < dim; i++)
                    {
                        for (int j = 0; j < dim; j++)
                        {
                            Datagrid1stpage_result_matrix[i, j] = list_result.ElementAt(counter).ToString();
                            counter++;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < dim; i++)
                    {
                        for (int j = 0; j < dim; j++)
                        {
                            switch (orient)
                            {
                                case 1:

                                    if (i + j <= dim - 1)
                                    {
                                        Datagrid1stpage_result_matrix[i, j] = list_result.ElementAt(counter).ToString();
                                        counter++;
                                    }
                                    else
                                    {
                                        Datagrid1stpage_result_matrix[i, j] = str_typ_elt;
                                    }

                                    break;

                                case 2:

                                    if (i + j >= dim - 1)
                                    {
                                        Datagrid1stpage_result_matrix[i, j] = list_result.ElementAt(counter).ToString();
                                        counter++;
                                    }
                                    else
                                    {
                                        Datagrid1stpage_result_matrix[i, j] = str_typ_elt;
                                    }

                                    break;

                                case 3:

                                    if (i <= j)
                                    {
                                        Datagrid1stpage_result_matrix[i, j] = list_result.ElementAt(counter).ToString();
                                        counter++;
                                    }
                                    else
                                    {
                                        Datagrid1stpage_result_matrix[i, j] = str_typ_elt;
                                    }

                                    break;

                                case 4:

                                    if (i >= j)
                                    {
                                        Datagrid1stpage_result_matrix[i, j] = list_result.ElementAt(counter).ToString();
                                        counter++;
                                    }
                                    else
                                    {
                                        Datagrid1stpage_result_matrix[i, j] = str_typ_elt;
                                    }
                                    break;

                                default: throw new Exception("Не распознана ориентация для заполнения упакованной формы ");
                            }
                        }
                    }
                }


            }
            dataGrid2D.ItemsSource2D = Datagrid1stpage_result_matrix;
        }
        //Math .NET Numerics!!!!!!!
        public void fill_2D_array_from_file_and_show(string matrix_name,int dim)//заполняет массив и находит обратную матрицу
        {
            double[,] array_inv = new double[dim,dim];

            using (var reader = new StreamReader(path_to_FS + matrix_name + ".txt"))
            {
                //пропуск размерности ориентации и однотип элемента
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();

                string line;
                int i = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] str_elts = line.Split(' ');
                    int j = 0;
                    foreach (var item in str_elts)
                    {
                        array_inv[i,j] =Convert.ToDouble(item);
                        j++;
                    }
                    i++;
                }
            }

            var matrix = Matrix<double>.Build.DenseOfArray(array_inv);
            if ((int)matrix.Determinant() == 0 )
            {
                MessageBox.Show("Определитель матрицы равен нулю","НЕВОЗМОЖНО НАЙТИ ОБРАТНУЮ МАТРИЦУ");
            }
            else
            {
                array_inv = matrix.Inverse().ToArray();
                dataGrid2D.ItemsSource2D =array_inv;
                add_exp_to_history();
            }
            
            

        }
        //Math .NET Numerics!!!!!!!
     
        public void choose_operation_of_M1_and_M2(char operation, string matrix_name1, string matrix_name2, int dim, int orient, double val_typ_elt)//производит выбор функции в зависимости от перации
        {
            Sparse_Matrix sparse_m = new Sparse_Matrix();
            switch (operation)
            {
                case '+':
                    double chnged_val_typ_elt = val_typ_elt;
                    if (val_typ_elt != 0)
                    {
                        chnged_val_typ_elt = val_typ_elt * 2;
                    }
                    else;//нам не надо его удваивать
                    show_result_matrix(sparse_m.triangular_matrix_sum(get_elts_in_list_from_file(matrix_name1, orient, dim), get_elts_in_list_from_file(matrix_name2, orient, dim)),
                                       dim, orient, chnged_val_typ_elt, false);
                    break;

                case '-':
                    val_typ_elt = 0;//при вычитании однотипный элемент будет получаться всегда 0 в результате
                    show_result_matrix(sparse_m.triangular_matrix_sub(get_elts_in_list_from_file(matrix_name1, orient, dim), get_elts_in_list_from_file(matrix_name2, orient, dim)),
                                        dim, orient, val_typ_elt, false);
                    break;

                case '*':
                    switch (val_typ_elt)
                    {
                        case 0:
                            switch (orient)
                            {
                                case 1:
                                    show_result_matrix(sparse_m.upper_triangular_matrix_secondary_diagonal_with_nulls(get_elts_in_list_from_file(matrix_name1, orient, dim),
                                                          get_elts_in_list_from_file(matrix_name2, orient, dim), dim),
                                       dim, orient, val_typ_elt, true);
                                    break;

                                case 2:
                                    show_result_matrix(sparse_m.lower_triangular_matrix_secondary_diagonal_with_nulls(get_elts_in_list_from_file(matrix_name1, orient, dim),
                                                          get_elts_in_list_from_file(matrix_name2, orient, dim), dim),
                                       dim, orient, val_typ_elt, true);
                                    break;

                                case 3:
                                    show_result_matrix(sparse_m.upper_triangular_matrix_major_diagonal_with_nulls(get_elts_in_list_from_file(matrix_name1, orient, dim),
                                                       get_elts_in_list_from_file(matrix_name2, orient, dim), dim),
                                        dim, orient, val_typ_elt, false);
                                    break;

                                case 4:
                                    show_result_matrix(sparse_m.lower_triangular_matrix_major_diagonal_with_nulls(get_elts_in_list_from_file(matrix_name1, orient, dim),
                                                      get_elts_in_list_from_file(matrix_name2, orient, dim), dim),
                                     dim, orient, val_typ_elt, false);
                                    break;
                                default: throw new Exception("Ориентацию при попытке умножения не удалось распознать!");
                            }
                            break;


                        default:
                            switch (orient)
                            {
                                case 1:
                                    show_result_matrix(sparse_m.triangular_matrix__any_diagonal_not_nulls_multiplication(get_elts_in_list_from_file(matrix_name1, orient, dim),
                                                       get_elts_in_list_from_file(matrix_name2, orient, dim), dim, val_typ_elt, "up_secondary"),
                                    dim, orient, val_typ_elt, true);
                                    break;
                                case 2:
                                    show_result_matrix(sparse_m.triangular_matrix__any_diagonal_not_nulls_multiplication(get_elts_in_list_from_file(matrix_name1, orient, dim),
                                                       get_elts_in_list_from_file(matrix_name2, orient, dim), dim, val_typ_elt, "low_secondary"),
                                    dim, orient, val_typ_elt, true);
                                    break;
                                case 3:
                                    show_result_matrix(sparse_m.triangular_matrix__any_diagonal_not_nulls_multiplication(get_elts_in_list_from_file(matrix_name1, orient, dim),
                                                       get_elts_in_list_from_file(matrix_name2, orient, dim), dim, val_typ_elt, "up_major"),
                                    dim, orient, val_typ_elt, true);
                                    break;
                                case 4:
                                    show_result_matrix(sparse_m.triangular_matrix__any_diagonal_not_nulls_multiplication(get_elts_in_list_from_file(matrix_name1, orient, dim),
                                                       get_elts_in_list_from_file(matrix_name2, orient, dim), dim, val_typ_elt, "low_secondary"),
                                    dim, orient, val_typ_elt, true);
                                    break;

                                default:
                                    break; throw new Exception("Ориентация при попытке умножения не распознана!");
                            }
                            break;
                    }
                    break;
                case '~':
                    fill_2D_array_from_file_and_show(matrix_name1, dim);
                    break;

                default:
                    throw new Exception("Операция была не распознана");
            }
        }

        List<DG_history> history_exprs = new List<DG_history>();//список выражений
        public void add_exp_to_history()
        {
            if (history_exprs.Count == 10)
            {
                List<DG_history> stack_list = new List<DG_history>
                {
                    new DG_history { Number = 1, Expression = txtBox_Expression.Text }
                };//в списке будет удалено послед выражение и добавлено новое в начало и изменена нумерация
                history_exprs.RemoveAt(9);
                stack_list.AddRange(history_exprs);
                history_exprs.Clear();

                int counter = 1;
                foreach (var item in stack_list)//после того как соединили поправим нумерацию
                {
                    item.Number = counter;
                    counter++;
                }

                history_exprs.AddRange(stack_list);


            }
            else//если не набралось 10 выражений то просто добавим новое 
            {

                history_exprs.Add(new DG_history { Number = history_exprs.Count + 1, Expression = txtBox_Expression.Text });

            }
            DG_Expressions.Items.Refresh();
        }
        //Логически важные методы (Лучше вынести в класс потом )
        private void save_matrix_in_file(string File_path, string message)
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

                                            if (i + j <= dimension - 1)
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

                                            if (i + j >= dimension - 1)
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

                                            if (i <= j)
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

                                            if (i >= j)
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

        private void Save_Matrix_Dialog(object sender, RoutedEventArgs e)
        {

            WpfApp1.Window_save_result_matrix window_Save_Result = new WpfApp1.Window_save_result_matrix();
            window_Save_Result.ShowDialog();

            if (window_Save_Result.isSelectedSaveButton)
            {
                if (last_operation == '+')
                {
                    last_typical_elt *= 2;
                }
                if (last_operation == '-')
                {
                    last_typical_elt = 0;
                }

                using (var file = File.CreateText(window_Save_Result.get_Path))
                {
                    file.WriteLine(last_dimension);
                    file.WriteLine(last_orientation);
                    file.WriteLine(last_typical_elt);

                    for (int i = 0; i < last_dimension; i++)
                    {
                        string buffer = "";
                        for (int j = 0; j < last_dimension; j++)
                        {
                            buffer += Datagrid1stpage_result_matrix[i, j] + " ";
                        }
                        file.WriteLine(buffer.TrimEnd(' '));
                    }
                }

                    //сохранить матрицу по пути пользователя
                    if (window_Save_Result.isSelectedAddToMatrixList && (last_operation == '+' || last_operation == '-'
                        || (last_operation == '*' && (last_orientation == 3 || last_orientation == 4)) ))//если матрица треугольная и выбрана опция добавить в список
                    {
                        if (list_matrices.Find(delegate (string str) { return str == window_Save_Result.get_Matrix_name; }) == null)
                        {
                            try
                            {
                                //скопировать матрицу в корень проги
                                File.Copy(window_Save_Result.get_Path, path_to_FS + window_Save_Result.get_Matrix_name + ".txt");
                                //добавить в список
                                list_matrices.Add(window_Save_Result.get_Matrix_name);
                                list_matrices_orient.Add(last_orientation);
                                list_matrices_typic_elt.Add(last_typical_elt.ToString());
                                List_Matrices.Items.Refresh();
                            }
                            catch ( Exception ex)
                            {
                                MessageBox.Show($"Не удалось добавить матрицу-результат в список {ex.Message}") ;
                            }
                            
                        }
                        else
                        {
                            MessageBox.Show("Не удалось добавить матрицу в список, так как матрица с таким именем уже существует!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не удалось добавить матрицу в список, так как результат является полноразмерной матрицей!", "МАТРИЦА НЕ ЯВЛЯЕТСЯ ТРЕУГОЛЬНОЙ");
                    }
                
            }
            else;//пользователь отменил сохр-ие
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
                            if (local_dimension > 0 && local_dimension <= 100000)
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
                                if (local_orientation_number > 0 && local_orientation_number <= 4)
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
                                    //MessageBox.Show("Матрица в файле валидна");
                                    //cancel_handle_norm_form = true;
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

                                        read_matrix_from_file(file2);
                                        dataGrid2D_sec_page.ItemsSource2D = array_dense;
                                        MessageBox.Show("Файл с матрицей успешно открыт!");
                                        Label_Matrix_Name.Text = "Матрица из ФАЙЛА";
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

            if (!string.IsNullOrEmpty(e.Text))
                if (char.IsDigit(e.Text[0]) || e.Text[0] == '.' || e.Text[0] == '-')
                {

                }
                else
                {
                    e.Handled = true;//Значение true для обхода обработки элемента управления по умолчанию;
                }
        }

        private  void Button_Click_Create_Matrix(object sender, RoutedEventArgs e)
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
                        Label_Matrix_Name.Text = "НЕСОХРАНЕННАЯ МАТРИЦА";
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
                    MessageBox.Show("Вам не хватает памяти для матрицы такой размерности", "НЕВОЗМОЖНО СОЗДАТЬ МАТРИЦУ");
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
                    MessageBoxResult boxResult = MessageBox.Show("Вы уверены что хотите создать новую матрицу в упакованной форме?", "ВОЗМОЖНАЯ ПОТЕРЯ ДАННЫХ", MessageBoxButton.YesNo);
                    if (boxResult == MessageBoxResult.Yes)
                    {
                        try
                        {
                            int tmp_dimension = (int)NumericUpDown.Value;
                            int tmp_number_orientation = index_image + 1;
                            double tmp_typical_elt = Convert.ToDouble(textBox_typical_elt.Text);

                            if (amount_of_not_null_elts(tmp_dimension) > 1000000)
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
                                Label_Matrix_Name.Text = "НЕСОХРАНЕННАЯ МАТРИЦА";
                            }
                            if (messageBoxResult == MessageBoxResult.Yes)
                            {
                                dimension = tmp_dimension;//если пользователь согласился только тогда меняем размерность
                                orientation_number = tmp_number_orientation;//то же самое с ориентацией
                                typical_elt = tmp_typical_elt;//и с элементом однотипным
                                array_dense = new string[amount_of_not_null_elts(dimension), 1];
                                
                                dataGrid2D_sec_page.ItemsSource2D = array_dense;
                                
                                Label_Matrix_Name.Text = "НЕСОХРАНЕННАЯ МАТРИЦА";
                                MessageBox.Show("Матрица в упакованной форме создана успешно!");
                            }
                            else;//NO
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

                                    if (i + j <= dimension - 1)
                                    {

                                        array_dense[counter, 0] = array_inp[i, j];
                                        counter++;
                                    }
                                    else; //пустая строка

                                    break;

                                case 2:

                                    if (i + j >= dimension - 1)
                                    {
                                        array_dense[counter, 0] = array_inp[i, j];
                                        counter++;
                                    }
                                    else; //пустая строка
                                    break;

                                case 3:

                                    if (i <= j)
                                    {
                                        array_dense[counter, 0] = array_inp[i, j];
                                        counter++;
                                    }
                                    else; //пустая строка
                                    break;

                                case 4:

                                    if (i >= j)
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
            if (dimension > 1000)
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

                                    if (i + j <= dimension - 1)
                                    {
                                        array_inp[i, j] = array_dense[counter, 0];
                                        counter++;
                                    }
                                    else; //пустая строка

                                    break;

                                case 2:

                                    if (i + j >= dimension - 1)
                                    {
                                        array_inp[i, j] = array_dense[counter, 0];
                                        counter++;
                                    }
                                    else; //пустая строка
                                    break;

                                case 3:

                                    if (i <= j)
                                    {
                                        array_inp[i, j] = array_dense[counter, 0];
                                        counter++;
                                    }
                                    else; //пустая строка
                                    break;

                                case 4:

                                    if (i >= j)
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
            if (TabItem_Normal_Form.IsSelected)
            {
                for (int i = 0; i < dimension; i++)
                {
                    for (int j = 0; j < dimension; j++)
                    {
                        if (string.IsNullOrEmpty(array_inp[i, j]))
                        {
                            array_inp[i, j] = (random.Next() + random.NextDouble()).ToString();
                        }
                        else;//не заполняем непустые
                    }
                }
                dataGrid2D_sec_page.Items.Refresh();
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

                    dataGrid2D_sec_page.Items.Refresh();
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
                string Matrix_name = Matrix_name_pop_up.get_Matrix_name;//TODO: ДОБАВИТЬ ВАЛИДАЦИЮ
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
                        list_matrices_orient.Add(orientation_number);
                        list_matrices_typic_elt.Add(typical_elt.ToString());
                        List_Matrices.Items.Refresh();
                        List_Matrices.ItemsSource = list_matrices;
                        Label_Matrix_Name.Text = Matrix_name;
                    }
                    else
                    {
                        MessageBox.Show("Матрица с таким названием уже существует!", "ОШИБКА ПРИ ДОБАВЛЕНИИ МАТРИЦЫ");
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
            List_Matrices.ItemsSource = sorted_list.Concat(left_elts);
        }

        private void Button_Click_delete_from_Matrix_List(object sender, RoutedEventArgs e)
        {
            string file_path = path_to_FS + List_Matrices.SelectedItem + ".txt";

            if (List_Matrices.SelectedItem is null)
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
                        int index_of_elt_in_list = list_matrices.IndexOf(List_Matrices.SelectedItem.ToString());
                        list_matrices.Remove(List_Matrices.SelectedItem.ToString());
                        list_matrices_orient.RemoveAt(index_of_elt_in_list);
                        list_matrices_typic_elt.RemoveAt(index_of_elt_in_list);
                        List_Matrices.Items.Refresh();
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
                    int index_of_matrix_in_list = list_matrices.IndexOf(List_Matrices.SelectedItem.ToString());
                    list_matrices.Remove(List_Matrices.SelectedItem.ToString());
                    list_matrices_orient.RemoveAt(index_of_matrix_in_list);
                    list_matrices_typic_elt.RemoveAt(index_of_matrix_in_list);
                    List_Matrices.Items.Refresh();
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

                    if (MessageBoxResult.Yes == MessageBox.Show($"Обновить матрицу {Label_Matrix_Name.Text}", "Повторное нажтие", MessageBoxButton.YesNo))
                    {
                        cancel_handle_norm_form = true;
                        TabItem_Dense_Form.IsSelected = true;
                        using (var file = new StreamReader(path_to_FS + List_Matrices.SelectedItem + ".txt"))
                        {
                            dimension = Convert.ToInt32(file.ReadLine());
                            orientation_number = Convert.ToInt32(file.ReadLine());
                            typical_elt = Convert.ToDouble(file.ReadLine());
                            index_image = orientation_number - 1;
                            NumericUpDown.Value = dimension;
                            Image_OR.Source = orientations[index_image];
                            textBox_typical_elt.Text = typical_elt.ToString();

                            //TODO : ТЕСТЫ НУЖНЫ ВЫНЕСТИ КУСОК КОДА НИЖЕ в отдельную функция так как 2 раза повторяется
                            read_matrix_from_file(file);
                            dataGrid2D_sec_page.ItemsSource2D = array_dense;
                            Label_Matrix_Name.Text = List_Matrices.SelectedItem.ToString();
                            MessageBox.Show($"Матрица \"{List_Matrices.SelectedItem}\" успешно прогружена!");
                        }
                    }
                    else;//значит обновлять не надо
                }
                else
                {
                    cancel_handle_norm_form = true;
                    TabItem_Dense_Form.IsSelected = true;
                    using (var file = new StreamReader(path_to_FS + List_Matrices.SelectedItem + ".txt"))
                    {
                        dimension = Convert.ToInt32(file.ReadLine());
                        orientation_number = Convert.ToInt32(file.ReadLine());
                        typical_elt = Convert.ToDouble(file.ReadLine());
                        index_image = orientation_number - 1;
                        NumericUpDown.Value = dimension;
                        Image_OR.Source = orientations[index_image];
                        textBox_typical_elt.Text = typical_elt.ToString();

                        //TODO : ТЕСТЫ НУЖНЫ ВЫНЕСТИ КУСОК КОДА НИЖЕ в отдельную функция так как 2 раза повторяется
                        read_matrix_from_file(file);
                        dataGrid2D_sec_page.ItemsSource2D = array_dense;
                        Label_Matrix_Name.Text = List_Matrices.SelectedItem.ToString();
                        MessageBox.Show($"Матрица \"{List_Matrices.SelectedItem}\" успешно прогружена!");
                    }

                }
            }

        }

        private void TabItem_PreviewMouseDown_Addresses(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
          //  e.Handled = true;//TODO:реализация адресов

        }
        bool first_call_miss = false;//пропустить первый раз при загрузке
        private void ComboBox_orientation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (first_call_miss)
            {
                //очистить combobox с ориентацией и однотип элементом
                remove_all_elts_combobox(ComboBox_common_elts);
                remove_all_elts_combobox(comboBox_name);

                List<string> unique_typ_elts_value = new List<string>();//хранятся значения однотипных элементов
                int index = 0;//проходимся по всем элементам списка list_matrices_orient
                foreach (var item in list_matrices_orient)
                {

                    if (item == ComboBox_orientation.SelectedIndex)//ЕСЛИ МАТРИЦА ИМЕЕТ ВЫБРАННУЮ ОРИЕНТАЦИЮ
                    {
                        unique_typ_elts_value.Add(list_matrices_typic_elt.ElementAt(index));
                    }
                    else;//ненужная ориентация
                    index++;
                }

                foreach (var item in unique_typ_elts_value.Distinct())//добавимс только различные элементы
                {
                    ComboBox_common_elts.Items.Add(item);
                }
            }
            else
            {
                first_call_miss = true;
            }

        }

        bool first_call_miss2 = false;//пропустить первый раз при загрузке
        private void ComboBox_common_elts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (first_call_miss2)
            {
                if (ComboBox_common_elts.Items.Count != 0)
                {
                    //remove_all_elts_combobox(comboBox_name); //очистка чтобы имена старые удалялись 
                    if (!(comboBox_name.Items.Count == 0))
                    {
                        comboBox_name.Items.Clear();
                    }
                    else;//empty
                        List<string> typ_elts_value = new List<string>();//хранятся  значения однотипных элементов
                    List<int> indexes_of_elts_in_full_list = new List<int>();//индексы для списка 
                    int index = 0;//проходимся по всем элементам списка list_matrices_orient
                    foreach (var item in list_matrices_orient)
                    {

                        if (item == ComboBox_orientation.SelectedIndex)//ЕСЛИ МАТРИЦА ИМЕЕТ ВЫБРАННУЮ ОРИЕНТАЦИЮ
                        {
                            typ_elts_value.Add(list_matrices_typic_elt.ElementAt(index));
                            indexes_of_elts_in_full_list.Add(index);
                        }
                        else;//ненужная ориентация
                        index++;
                    }

                    int count = 0;
                    foreach (var item in typ_elts_value)//пройдемся по всему списку всех однотипных эл-тов
                    {
                        if (item == ComboBox_common_elts.SelectedItem.ToString())
                        {
                            int index_for_name = indexes_of_elts_in_full_list.ElementAt(count);//индекс для элемента из всего списка
                            comboBox_name.Items.Add(list_matrices.ElementAt(index_for_name));//добавим элемент по этому индексу
                        }
                        else;
                        count++;
                    }


                }
                else
                {
                    ComboBox_common_elts.Foreground = Brushes.Gray;
                    comboBox_name.Foreground = Brushes.Gray;

                }//ничего не делаем



            }
            else
            {
                first_call_miss2 = true;
            }
        }

        private void comboBox_name_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(comboBox_name.SelectedItem is null) && comboBox_name.SelectedItem.ToString() != "System.Windows.Controls.ComboBoxItem: Название")//TODO: ЕБУЧИЙ КОСТЫЛЬ ТАК КАК ЕСЛИ ПОЛЬЗОВАТЕЛЬ ВВЕДЕТ НАЗВАНИЕ КАК В ПРОВЕРКЕ, ТО ОН НЕ СМОЖЕТ С НЕЙ РАБОТАТЬ
            {

                using (var file = new StreamReader(path_to_FS + comboBox_name.SelectedItem.ToString() + ".txt"))
                {
                    //Пропуск инф-ции о матрице
                    int dim = Convert.ToInt32(file.ReadLine());
                    file.ReadLine();
                    file.ReadLine();
                    string[,] array_in_normal_form = new string[dim, dim];
                    string line = "";
                    int i = 0;
                    while ((line = file.ReadLine()) != null)
                    {
                        int j = 0;
                        string[] str_elts = line.Split(' ');
                        foreach (var item in str_elts)
                        {
                            array_in_normal_form[i, j] = item;
                            j++;
                        }
                        i++;
                    }
                    dataGrid2D.ItemsSource2D = array_in_normal_form;
                    Matrix_name_1st_page.Text = comboBox_name.SelectedItem.ToString();
                }
            }
            else;//значит нечего выводить
        }

        private void Button_Click_Compute_Expression(object sender, RoutedEventArgs e)
        {
            string expr = txtBox_Expression.Text.Trim();
            if (!string.IsNullOrEmpty(expr))
                if (expr[0] == '~')
                {
                    int i = 1;
                    string matrix_inv = "";
                    while (i < expr.Length && char.IsLetterOrDigit(expr[i]))
                    {
                        matrix_inv += expr[i];
                        i++;
                    }

                    if (expr.Length == i)
                    {
                        if (File.Exists(path_to_FS + matrix_inv + ".txt"))
                        {
                            try
                            {
                                int dim;
                                double typ_elt;
                                int orientation;
                                using (var reader = new StreamReader(path_to_FS + matrix_inv + ".txt"))
                                {
                                    dim = Convert.ToInt32(reader.ReadLine());
                                    orientation = Convert.ToInt32(reader.ReadLine());
                                    typ_elt = Convert.ToDouble(reader.ReadLine());
                                }
                                choose_operation_of_M1_and_M2('~', matrix_inv, "", dim, orientation, typ_elt);
                              
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка:{ex.Message}","НЕ УДАЛОСЬ НАЙТИ ОБРАТНУЮ МАТРИЦУ");
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Матрицы с именем \"{matrix_inv}\" не существует", "Не удалось найти матрицу");
                        }

                           
                    }
                    else
                    {
                        MessageBox.Show("После имени матрицы содержатся лишние символы!", "ОШИБКА ПРИ НАХОЖДЕНИИ ОБРАТНОЙ МАТРИЦЫ");
                    }
                }
                else
                {
                    int i = 0;
                    string matrix_1 = "";//первая матрица
                    while (char.IsLetterOrDigit(expr[i]))
                    {
                        if (i < expr.Length - 1)
                        {

                        }
                        else
                        {
                            MessageBox.Show("НЕ ХВАТАЕТ оператора и второй матрицы");
                            return;
                        }
                        matrix_1 += expr[i];
                        i++;
                    }
                    //Проверка оператора
                    if (i > 0)
                    {
                        if (expr[i] == '+' || expr[i] == '*' || expr[i] == '-')
                        {
                            char operator_btw_matrices = expr[i];
                            i++;
                            if (i < expr.Length)
                            {
                                string matrix_2 = "";
                                while (i < expr.Length && char.IsLetterOrDigit(expr[i]))
                                {
                                    matrix_2 += expr[i];
                                    i++;

                                }
                                if (i == expr.Length)
                                {
                                    //MessageBox.Show("Выражение корректно!");
                                    if (File.Exists(path_to_FS + matrix_1 + ".txt"))
                                    {
                                        if (File.Exists(path_to_FS + matrix_2 + ".txt"))
                                        {
                                            int dim1;
                                            double typ_elt1;
                                            int orientation1;
                                            using (var reader = new StreamReader(path_to_FS + matrix_1 + ".txt"))
                                            {
                                                dim1 = Convert.ToInt32(reader.ReadLine());
                                                orientation1 = Convert.ToInt32(reader.ReadLine());
                                                typ_elt1 = Convert.ToDouble(reader.ReadLine());

                                            }

                                            int dim2;
                                            double typ_elt2;
                                            int orientation2;
                                            using (var reader = new StreamReader(path_to_FS + matrix_2 + ".txt"))
                                            {
                                                dim2 = Convert.ToInt32(reader.ReadLine());
                                                orientation2 = Convert.ToInt32(reader.ReadLine());
                                                typ_elt2 = Convert.ToDouble(reader.ReadLine());
                                            }
                                            if (dim1 == dim2)
                                            {
                                                if (typ_elt1 == typ_elt2)
                                                {
                                                    if (orientation1 == orientation2)
                                                    {
                                                        choose_operation_of_M1_and_M2(operator_btw_matrices, matrix_1, matrix_2, dim1, orientation1, typ_elt1);
                                                        add_exp_to_history();
                                                        //Нужно чтобы проверить можно ли добавить результат в список
                                                        last_dimension = dim1;
                                                        last_operation = operator_btw_matrices;
                                                        last_orientation = orientation1;
                                                        last_typical_elt = typ_elt1;
                                                        //Нужно чтобы проверить можно ли добавить результат в список
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show($"матрица {matrix_1} и матрица {matrix_2} имеют разные ориентации",
                                                                         "ОШИБКА, у матриц разная ориентация");
                                                    }
                                                }
                                                else
                                                {
                                                    MessageBox.Show($"Значения однотипных эл-ов матрицы {matrix_1}({typ_elt1}) и матрицы {matrix_2}({typ_elt2}) не совпадают",
                                                                    "ОШИБКА, у матриц разные значения однотипного элемента");
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show($"Размерности матрицы {matrix_1}({dim1}) и матрицы {matrix_2}({dim2}) не совпадают",
                                                                 "ОШИБКА, у матриц разная размерность");
                                            }

                                        }
                                        else
                                        {
                                            MessageBox.Show($"Матрицы с именем \"{matrix_2}\" не существует", "Не удалось найти матрицу");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show($"Матрицы с именем \"{matrix_1}\" не существует", "Не удалось найти матрицу");
                                    }



                                    //1 проверить обе матрицы сущ-ют у них одинакова размерность и ориентация и однотип эл-нт
                                    //TODO: //все хорошо
                                }
                                else
                                {
                                    MessageBox.Show("В имени второй матрицы обнаружены неизвестные символы!", "ОШИБКА В ИМЕНИ ВТОРОЙ МАТРИЦЫ");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Вторая матрица не указана!", "ОШИБКА ПРИ НАПИСАНИИ ВТОРОЙ МАТРИЦЫ");
                            }
                        }
                        else
                        {
                            MessageBox.Show("НЕОПОЗНАННЫЙ ОПЕРАТОР,ожидался оператор +,-,*", "ОШИБКА ПРИ НАПИСАНИИ ОПЕРАТОРА");
                            return;
                        }
                    }
                    else//букв не встретилось
                    {
                        MessageBox.Show("Использованы запрещены символы, ожидалось имя матрицы!", "ОШИБКА ПРИ НАПИСАНИИ ИМЕНИ МАТРИЦЫ");
                        return;
                    }
                }

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO : ОЧЕНЬ СТРАННОЕ ПОВЕДЕНИЕ КОДА
            //ПРИ ДЕБАГЕ ДИРЕКТОРИЯ УДАЛЯЕТСЯ НО ПРИ ЗАПУСКЕ В РЕЛИЗЕ КОД НЕ УДАЛЯЕТ УЖЕ СУЩЕСТВУЮЩУЮ ДИРЕКТОРИЮ 
            //это хренова мистика 
            //Ошибка возникает только в студии
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
            if (Directory.Exists($@"{Directory.GetCurrentDirectory()}\Матрицы"))
            {
                Directory.Delete($@"{Directory.GetCurrentDirectory()}\Матрицы", true);
            }
            else;//если нет то просто создадим её

            path_to_FS = Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}\Матрицы").FullName + "\\";
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {

            if (Directory.Exists($@"{Directory.GetCurrentDirectory()}\Матрицы"))
            {
                Directory.Delete($@"{Directory.GetCurrentDirectory()}\Матрицы", true);
            }
            else;// и так нету                 
        }

        private void Button_Click_plus(object sender, RoutedEventArgs e)
        {
            txtBox_Expression.Text += "+";
        }

        private void Button_Click_inverse(object sender, RoutedEventArgs e)
        {
            txtBox_Expression.Text += "~";
        }

        private void Button_Click_multiple(object sender, RoutedEventArgs e)
        {
            txtBox_Expression.Text += "*";
        }

        private void Button_Click_minus(object sender, RoutedEventArgs e)
        {
            txtBox_Expression.Text += "-";
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBoxResult.Yes == MessageBox.Show("Вы уверены что хотите выйти, ВСЕ не сохраненные матрицы будут УДАЛЕНЫ", "Подтвердите выход", MessageBoxButton.YesNo))
            {
                //пользователь согласился
            }
            else
            {
                e.Cancel = true;
            }
        }

        string[] keys_alowed = new string[] { "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "E", ".", "OemPlus"};
        private void dataGrid2D_sec_page_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
       
            if (keys_alowed.Contains(e.Key.ToString()))
            {

            }
            else
            {
                e.Handled = true;
            }
        }
    }
}

