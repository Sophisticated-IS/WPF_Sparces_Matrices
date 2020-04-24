using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_Matrices
{
    public class Sparse_Matrix
    {
        //ВСЕ ФУНКЦИИ ДОЛЖНЫ БЫТЬ РЕАЛИЗОВАНЫ В ВИДЕ ПОДПРОГРАММ
        public static double triangular_matrix_not_null_upper_major_diagonal_get_elt(ref List<double> list, int i, int j, double common_elt,int dim)//возвращает элемент верхней главной диагонали матрицы в упакованной форме
        {
            if (i <= j)
            {
                i++;j++;
                int index = 1;
                for (int d =0; d <i-1 ; d++)
                {
                    index += dim;
                    dim--;
                }
                index += j - i;
                return list.ElementAt(index-1);
            }
            else
            {
                return common_elt;
            }

        }
        public static double triangular_matrix_not_null_lower_major_diagonal_get_elt(ref List<double> list, int i, int j, double common_elt)//возвращает элемент нижней главной диагонали матрицы в упакованной форме
        {
            
            if (i >= j)
            {
                i++;j++;
                return list.ElementAt((i * (i - 1) / 2 + j)-1);
            }
            else
            {
                return common_elt;
            }

        }
        public static double triangular_matrix_not_null_upper_secondary_diagonal_get_elt(ref List<double> list, int i, int j, double common_elt, int dim)//возвращает элемент верхней побочной диагонали матрицы в упакованной форме
        {
            if (i + j <= dim - 1)//TODO:обратить внимание на индексы при тесте
            {
                i++; j++;
                int index = 1;
                for (int d = 0; d < i-1; d++)
                {
                    index += dim;
                    dim--;
                }
                index += j-1;
                return list.ElementAt(index - 1);
            }
            else
            {
                return common_elt;
            }

        }

        public static double triangular_matrix_not_null_lower_secondary_diagonal_get_elt(ref List<double> list, int i, int j, double common_elt, int dim)//возвращает элемент нижней побочной диагонали матрицы в упакованной форме
        {
            
            if (i + j >= dim - 1)
            {
                i++;j++;
                int index = 1;
                for (int d = 1; d < i; d++)
                {
                    index += d;
                }
                index +=i+j - (dim + 1);
                return list.ElementAt(index-1);
            }
            else
            {
                return common_elt;
            }

        }

        public  List<double> triangular_matrix_sum(List<double> A, List<double> B)//cумма двух любых матриц
        {
            List<double> list_result = new List<double>();
            for (int i = 0; i < A.Count; i++)
            {
                list_result.Add(A.ElementAt(i) + B.ElementAt(i));
            }

            return list_result;
        }
        public  List<double> triangular_matrix_sub(List<double> A, List<double> B)//разность двух любых матриц
        {
            List<double> list_result = new List<double>();
            for (int i = 0; i < A.Count; i++)
            {
                list_result.Add(A.ElementAt(i) - B.ElementAt(i));
            }

            return list_result;
        }
        public  List<double> triangular_matrix__any_diagonal_not_nulls_multiplication(List<double> A, List<double> B, int dimension,
                                                                                                  double common_element, string orientation)//умножение матриц с ненулевым однотипным элементом
        {
            //Виды ориентаций up_major,low_major,up_secondary,low_secondary
            List<double> list_result = new List<double>();
            switch (orientation)
            {
                case "up_major":
                    for (int i = 0; i < dimension; i++)
                    {
                        for (int j = 0; j < dimension; j++)
                        {
                            double sum = 0;
                            for (int k = 0; k < dimension; k++)
                            {
                                sum += triangular_matrix_not_null_upper_major_diagonal_get_elt(ref A, i, k, common_element,dimension) *
                                       triangular_matrix_not_null_upper_major_diagonal_get_elt(ref B, k, j, common_element,dimension);
                            }
                            list_result.Add(sum);
                        }
                    }
                    return list_result;
                case "low_major":
                    for (int i = 0; i < dimension; i++)
                    {
                        for (int j = 0; j < dimension; j++)
                        {
                            double sum = 0;
                            for (int k = 0; k < dimension; k++)
                            {
                                sum += triangular_matrix_not_null_lower_major_diagonal_get_elt(ref A, i, k, common_element) *
                                       triangular_matrix_not_null_lower_major_diagonal_get_elt(ref B, k, j, common_element);
                            }
                            list_result.Add(sum);
                        }
                    }
                    return list_result;
                case "up_secondary":
                    for (int i = 0; i < dimension; i++)
                    {
                        for (int j = 0; j < dimension; j++)
                        {
                            double sum = 0;
                            for (int k = 0; k < dimension; k++)
                            {                                
                                sum += triangular_matrix_not_null_upper_secondary_diagonal_get_elt(ref A, i, k, common_element, dimension) *
                                       triangular_matrix_not_null_upper_secondary_diagonal_get_elt(ref B, k, j, common_element, dimension);
                            }
                            list_result.Add(sum);
                        }
                    }
                    return list_result;
                case "low_secondary":
                    for (int i = 0; i < dimension; i++)
                    {
                        for (int j = 0; j < dimension;j++)
                        {
                            double sum = 0;
                            for (int k = 0; k < dimension; k++)
                            {
                                sum += triangular_matrix_not_null_lower_secondary_diagonal_get_elt(ref A, i, k, common_element, dimension) *
                                      triangular_matrix_not_null_lower_secondary_diagonal_get_elt(ref B, k, j, common_element, dimension);
                            }
                            list_result.Add(sum);
                        }
                    }
                    return list_result;

                default: throw new Exception("Ориентация треугольной матрицы с ненулевыми однотипными элементами не была распознана");

            }

        }

        public  List<double> lower_triangular_matrix_major_diagonal_with_nulls(List<double> A, List<double> B, int dimension)
        {
            List<double> list_result = new List<double>();//TODO: Нужно ли сохрнять результат в отдельный список????? ПАМЯТЬ --
            int st_ind_A = 0;
            for (int i = 0; i < dimension; i++)//
            {
                st_ind_A += i;

                int offset_B = 2;
                int st_ind_B = 0;
                int count_elts = 0;
                for (int st_elt = 0; st_elt < i + 1; st_elt++)
                {
                    double sum = 0;
                    int row = st_elt;

                    for (int a = st_ind_A + st_elt, b = st_ind_B; a <= st_ind_A + i; a++)
                    {
                        sum += A.ElementAt(a) * B.ElementAt(b);
                        row++;
                        b += row;
                    }

                    list_result.Add(sum);
                    st_ind_B += offset_B;
                    offset_B++;
                    count_elts++;
                }

            }
            return list_result;
        }
        public  List<double> upper_triangular_matrix_major_diagonal_with_nulls(List<double> A, List<double> B, int dimension)
        {
            List<double> list_result = new List<double>();//TODO: Нужно ли сохрнять результат в отдельный список????? ПАМЯТЬ --
                                                          //TODO: Можно оптимизировать доступ к эл-ту в списке через сохранение указателя
            int st_A_and_B = 0;
            int st_B_shift = dimension - 1;
            for (int i = dimension - 1; i >= 0; i--)
            {
                int b = st_A_and_B;

                for (int j = 0; j < i + 1; j++)
                {
                    double sum = 0;
                    int b_shift = b;
                    int counter = 0;
                    int next_shift = st_B_shift;
                    for (int a = st_A_and_B; a <= st_A_and_B + j; a++, counter++, b_shift += next_shift, next_shift--)
                    {
                        sum += A.ElementAt(a) * B.ElementAt(b_shift);
                    }
                    list_result.Add(sum);
                    b++;
                }
                st_A_and_B += i + 1;
                st_B_shift--;
            }

            return list_result;
        }
        public static int sum_of_all_numbers_from_1_in_figure(int figure)//сумма от 1 до числа-1
        {
            int sum = 0;
            for (int i = 1; i < figure; i++) // from 1 to figure -1 
            {
                sum += i;
            }
            return sum;
        }
        public  List<double> lower_triangular_matrix_secondary_diagonal_with_nulls(List<double> A, List<double> B, int dimension)
        {
            List<double> list_result = new List<double>();//TODO: Нужно ли сохрнять результат в отдельный список????? ПАМЯТЬ --

            int st_A = 0;
            int st_A_ch = 2;
            int st_B = sum_of_all_numbers_from_1_in_figure(dimension);  //нахожу индекс элемента в листе, который на диагонали расположен самым последним

            for (int i = 0; i < dimension; i++)
            {
                int offset = dimension - 1;
                int counter = 0;
                int ctr = 0;
                int b = st_B;
                int b_shift;//????
                for (int j = 0; j < dimension; j++)
                {
                    double sum = 0;
                    b_shift = dimension;
                    if (counter >= 2)
                    {
                        b_shift -= counter - 1;
                    }
                    int b_tmp = b;
                    for (int k = st_A - counter; k <= st_A; k++)
                    {
                        sum += A.ElementAt(k) * B.ElementAt(b_tmp);
                        b_tmp += b_shift;
                        b_shift++;
                    }
                    list_result.Add(sum);

                    if (ctr < i)
                    {
                        b = b - offset;
                        offset--;
                        ctr++;
                        counter++;
                    }
                    else
                    {
                        b++;
                    }


                }
                st_A += st_A_ch;
                st_A_ch++;
            }
            return list_result;
        }
        public  List<double> upper_triangular_matrix_secondary_diagonal_with_nulls(List<double> A, List<double> B, int dimension)
        {
            List<double> list_result = new List<double>();//TODO: Нужно ли сохранять результат в отдельный список????? ПАМЯТЬ --

            int st_A_offset = dimension;
            int st_A = 0;
            int counter = 0;
            for (int i = 0; i < dimension; i++)
            {
                int shift = 0;
                int st_B = 0;
                for (int j = 0; j < dimension; j++)
                {
                    int b = st_B;
                    int b_offset = dimension;
                    double sum = 0;
                    for (int a = st_A; a < st_A + dimension - i - shift; a++)
                    {
                        sum += A.ElementAt(a) * B.ElementAt(b);
                        b += b_offset;
                        b_offset--;
                    }
                    list_result.Add(sum);
                    st_B++;
                    if (j >= counter)
                    {
                        shift++;
                    }
                }
                st_A += st_A_offset;
                st_A_offset--;
                counter++;
            }
            return list_result;
        }

    }
}
