using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace WpfApp1
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
     
            double[,] m_intArray = new double[21, 20];
            string[,] str_arr = new string[50, 50];

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

        }

    }                     
}                       
                         
