using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Window_save_result_matrix.xaml
    /// </summary>
    public partial class Window_save_result_matrix : Window
    {
        public Window_save_result_matrix()
        {
            InitializeComponent();
        }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Create SaveFileDialog 
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();



            // Set filter for Savefile extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "TXT Files|*.txt";
            dlg.Title = "Сохранить результат как";
            dlg.FileName += Text_Box_Matrix_name.Text;
            // Display SaveFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            if (result == true)
            {
                Text_Box_Path.Foreground = Brushes.Black; 
                Text_Box_Path.Text = dlg.FileName;
                
            }

        }
    }
}
