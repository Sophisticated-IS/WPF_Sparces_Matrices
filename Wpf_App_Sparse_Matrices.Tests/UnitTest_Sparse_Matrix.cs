using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Wpf_Matrices;

namespace Wpf_Matrices
{
    [TestClass]
    public class UnitTest_Sparse_Matrix
    {
        
        [TestMethod]
        public void low_major_not_nulls_multiple()
        {
            //arrange
            List<double> list1 = new List<double>();
            list1.Add(2);
            List<double> list2 = new List<double>();
            list2.Add(4);
            int dim = 1;
            string orientation = "low_major";
            double common_elt = 9.9;
            double expected = 8;
            //act
            Sparse_Matrix sparse = new Sparse_Matrix();
            double actual = (sparse.triangular_matrix__any_diagonal_not_nulls_multiplication(list1, list2, dim, common_elt, orientation)).First();

            //asssert
            Assert.AreEqual(expected,actual);
        }

        [TestMethod]
        public void up_major_not_nulls_multiple()
        {
            //arrange
            List<double> list1 = new List<double>();
            list1.Add(9);
            List<double> list2 = new List<double>();
            list2.Add(2);
            int dim = 1;
            string orientation = "up_major";
            double common_elt = 9.9;
            double expected = 18;
            //act
            Sparse_Matrix sparse = new Sparse_Matrix();
            double actual = (sparse.triangular_matrix__any_diagonal_not_nulls_multiplication(list1, list2, dim, common_elt, orientation)).First();

            //asssert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void up_secondary_not_nulls_multiple()
        {
            //arrange
            List<double> list1 = new List<double>();
            list1.Add(3);
            List<double> list2 = new List<double>();
            list2.Add(3);
            int dim = 1;
            string orientation = "up_secondary";
            double common_elt = 9.9;
            double expected = 9;
            //act
            Sparse_Matrix sparse = new Sparse_Matrix();
            double actual = (sparse.triangular_matrix__any_diagonal_not_nulls_multiplication(list1, list2, dim, common_elt, orientation)).First();

            //asssert
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void low_secondary_not_nulls_multiple()
        {
            //arrange
            List<double> list1 = new List<double>();
            list1.Add(7);
            List<double> list2 = new List<double>();
            list2.Add(7);
            int dim = 1;
            string orientation = "low_secondary";
            double common_elt = 9.9;
            double expected = 49;
            //act
            Sparse_Matrix sparse = new Sparse_Matrix();
            double actual = (sparse.triangular_matrix__any_diagonal_not_nulls_multiplication(list1, list2, dim, common_elt, orientation)).First();

            //asssert
            Assert.AreEqual(expected, actual);
        }
        //ТЕСТЫ ДЛЯ РАЗМЕРНОСТЕЙ БОЛЬШЕ 1      
        [TestMethod]
        public void up_secondary_not_nulls_multiple_3dim()
        {
            //arrange
            List<double> list1 = new List<double>() { 1, 2, 3, 4, 5, 6 };
            List<double> list2 = new List<double>() { 234, 234, 453, 426, 453, 4 };
            int dim = 3;
            string orientation = "up_secondary";
            double common_elt = 7;
            List<double> expected = new List<double>() { 1098, 1161, 488, 3094, 3250, 1896, 4414, 4624, 2816 };
            //act
            Sparse_Matrix sparse = new Sparse_Matrix();
            List<double> actual = sparse.triangular_matrix__any_diagonal_not_nulls_multiplication(list1, list2, dim, common_elt, orientation);
            //assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void low_secondary_not_nulls_multiple_3dim()//получится полноразмерная
        {
            //arrange
            List<double> list1 = new List<double>() { 1,2,3,4,5,6};
            List<double> list2 = new List<double>() { 56,89,90,100,654,9};
            int dim =3 ;
            string orientation = "low_secondary";
            double common_elt = 7;
            List<double> expected = new List<double>() {198,1326,1031,363,2189,599,663,4397,728 };
            //act
            Sparse_Matrix sparse = new Sparse_Matrix();
            List<double> actual = sparse.triangular_matrix__any_diagonal_not_nulls_multiplication(list1, list2, dim, common_elt, orientation);
            //assert
            CollectionAssert.AreEqual(expected,actual);
        }

        [TestMethod]
        public void lower_major_not_nulls_multiple_3dim()//получится полноразмерная
        {
            //arrange
            List<double> list1 = new List<double>() { 1, 2, 3, 4, 5, 6 };
            List<double> list2 = new List<double>() { 65, 34, 87, 12, 90, 54 };
            int dim = 3;
            string orientation = "low_major";
            double common_elt = 7;
            List<double> expected = new List<double>() { 387,1246,434,316,905,413,502,1003,387 };
            //act
            Sparse_Matrix sparse = new Sparse_Matrix();
            List<double> actual = sparse.triangular_matrix__any_diagonal_not_nulls_multiplication(list1, list2, dim, common_elt, orientation);
            //assert
            CollectionAssert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void upper_major_not_nulls_multiple_3dim()//получится полноразмерная
        {
            //arrange
            List<double> list1 = new List<double>() { 1, 2, 3, 4, 5, 6 };
            List<double> list2 = new List<double>() { 65, 34, 87, 12, 90, 54 };
            int dim = 3;
            string orientation = "up_major";
            double common_elt = 7;
            List<double> expected = new List<double>() { 100, 79, 429, 518, 321, 1239, 546, 364, 1563 };
            //act
            Sparse_Matrix sparse = new Sparse_Matrix();
            List<double> actual = sparse.triangular_matrix__any_diagonal_not_nulls_multiplication(list1, list2, dim, common_elt, orientation);
            //assert
            CollectionAssert.AreEqual(expected, actual);
        }
        //УМНОЖЕНИЕ МАТРИЦ с НУЛЕВЫМИ ЭЛЕМЕНТАМИ
        [TestMethod]
        public void upper_major_with_nulls_multiple_1dim()
        {
            //arrrange 
            const int dim = 1;
            List<double> list1 = new List<double>() { 2};
            List<double> list2 = new List<double>() { 4 };
            List<double> expected = new List<double>() { 8 };

            //act
            List<double> actual = new Sparse_Matrix().upper_triangular_matrix_major_diagonal_with_nulls(list1, list2, dim);

            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void lower_major_with_nulls_multiple_1dim()
        {
            //arrrange 
            const int dim = 1;
            List<double> list1 = new List<double>() { 2 };
            List<double> list2 = new List<double>() { 4 };
            List<double> expected = new List<double>() { 8 };

            //act
            List<double> actual = new Sparse_Matrix().lower_triangular_matrix_major_diagonal_with_nulls(list1, list2, dim);

            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void upper_secondary_with_nulls_multiple_1dim()
        {
            //arrrange 
            const int dim = 1;
            List<double> list1 = new List<double>() { 2 };
            List<double> list2 = new List<double>() { 4 };
            List<double> expected = new List<double>() { 8 };

            //act
            List<double> actual = new Sparse_Matrix().upper_triangular_matrix_secondary_diagonal_with_nulls(list1, list2, dim);

            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void lower_secondary_with_nulls_multiple_1dim()
        {
            //arrrange 
            const int dim = 1;
            List<double> list1 = new List<double>() { 2 };
            List<double> list2 = new List<double>() { 4 };
            List<double> expected = new List<double>() { 8 };

            //act
            List<double> actual = new Sparse_Matrix().lower_triangular_matrix_secondary_diagonal_with_nulls(list1, list2, dim);

            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        //Размерности больше 1
        [TestMethod]
         public void upper_major_with_nulls_multiple_5dim()
         {
            //arrrange 
            const int dim = 5;
            List<double> list1 = new List<double>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            List<double> list2 = new List<double>() { 65, 34, 87, 12, 90, 234, 254, 453, 345, 54, 43, 234, 453, 67, 9 };
            List<double> expected = new List<double>() { 65, 502, 757, 2859, 1795, 1404, 1902, 6643, 4325, 540, 5413, 3185, 5889, 997, 135 };

            //act
            List<double> actual = new Sparse_Matrix().upper_triangular_matrix_major_diagonal_with_nulls(list1, list2, dim);

            //Assert
            CollectionAssert.AreEqual(expected, actual);
         }

        [TestMethod]
        public void lower_major_with_nulls_multiple_5dim()
        {
            //arrrange 
            const int dim = 5;
            List<double> list1 = new List<double>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            List<double> list2 = new List<double>() { 4, 5, 6, 3, 4, 7, 78, 3, 678, 8, 34, 4, 5, 3, 34 };
            List<double> expected = new List<double>() { 4, 23, 18, 59, 54, 42, 875, 114, 6843, 80, 1745, 226, 9658, 157, 510 };

            //act
            List<double> actual = new Sparse_Matrix().lower_triangular_matrix_major_diagonal_with_nulls(list1, list2, dim);

            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void upper_secondary_with_nulls_multiple_5dim()//получится полноразмерная матрица
        {
            //arrrange 
            const int dim = 5;
            List<double> list1 = new List<double>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            List<double> list2 = new List<double>() { 58, 39, 48, 30, 24, 39, 38, 90, 45, 35, 564, 56, 57, 10, 456 };
            List<double> expected = new List<double>() { 2749, 1847, 396, 120, 24,
                                                         1414, 5102, 1366, 495, 144,
                                                         1429, 7576, 2142, 795, 240,
                                                         1300, 1039, 1884, 1020, 312,
                                                          870, 585,  720,  450,  360};

            //act
            List<double> actual = new Sparse_Matrix().upper_triangular_matrix_secondary_diagonal_with_nulls(list1, list2, dim);

            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void lower_secondary_with_nulls_multiple_5dim()//получится полноразмерная матрица
        {
            //arrrange 
            const int dim = 5;
            List<double> list1 = new List<double>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            List<double> list2 = new List<double>() { 80, 30, 38, 20, 40, 456, 34, 50, 35, 70, 34, 60, 56, 10, 60 };
            List<double> expected = new List<double>() { 34, 60, 56, 10, 60,
                                                         102, 248, 268, 100, 320,
                                                         204, 530, 666, 395, 2534,
                                                         340, 906, 1170, 945, 5144,
                                                          510, 1376,  1800,  1520,  9144};

            //act
            List<double> actual = new Sparse_Matrix().lower_triangular_matrix_secondary_diagonal_with_nulls(list1, list2, dim);

            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

    }
}
