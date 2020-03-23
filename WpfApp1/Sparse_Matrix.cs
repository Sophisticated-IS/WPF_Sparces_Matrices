using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_Matrices
{
    class Sparse_Matrix
    {
       //ВСЕ ФУНКЦИИ ДОЛЖНЫ БЫТЬ РЕАЛИЗОВАНЫ В ВИДЕ ПОДПРОГРАММ

        private int dimension_calculate(int matrix_dense_length)//вычисление размерности матрицы по длине масссива в упакованной форме
        {
            //вычитаем -1 -2 -3 -4 -5 пока не дойдем до нуля , если результат < 0 то размерность неверная
            int i = 1;
            while (matrix_dense_length > 0)
            {
                matrix_dense_length -= i;
                i++;
            }

            if (matrix_dense_length == 0)
            {
                return i;
            }
            else
            {
                throw new System.Exception("Не получается корректно вычислить размерность матрицы в упакованной форме");
            }
        }

        //public List<double> matrix_dense_form(double[,] matrix_common_form, double typical_element)//метод упаковки матрицы в нормальной форме двумерного массива

        //{

        //    List<double> dense_matrix = new List<double>();
        //    for (int i = 0; i < matrix_common_form.Length; i++)
        //    {
        //        for (int j = 0; j < matrix_common_form.Length; j++)
        //        {
        //            if (matrix_common_form[i, j] != typical_element)
        //            {
        //                dense_matrix.Add(matrix_common_form[i, j]);
        //            }
        //            else;//продвигаемся дальше
        //        }
        //    }
        //    return dense_matrix;
        //}

        

    }
}
