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
        public string get_Path
        {
            get
            {
                return Text_Box_Path.Text;
            }
        }
        public string get_Matrix_name
        {
            get
            {
                return Text_Box_Matrix_name.Text;
            }
        }
        public bool isSelectedAddToMatrixList { get; set; }
        public bool isSelectedSaveButton { get; set; }
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
                Text_Box_Matrix_name.Text = dlg.SafeFileName.Replace(".txt","");
                
            }

        }

        private void Text_Box_Matrix_name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(string.IsNullOrEmpty(e.Text[0].ToString()) || string.IsNullOrWhiteSpace(e.Text[0].ToString()) || Text_Box_Path.Text == "!двойной клик выбор пути!"))
            {
                if (char.IsLetterOrDigit(e.Text[0]) || e.Text[0] == '+' || e.Text[0] == '-' || e.Text[0] == '~' || e.Text[0] == '=')
                {
                    if (!(string.IsNullOrWhiteSpace(Text_Box_Path.Text) || string.IsNullOrEmpty(Text_Box_Path.Text)))
                    {
                        string path_without_name = "";
                        path_without_name =Text_Box_Path.Text.Remove(Text_Box_Path.Text.LastIndexOf('\\') +1);
                        Text_Box_Path.Text = path_without_name + Text_Box_Matrix_name.Text +e.Text[0] + ".txt";
                    }
                    else
                    {
                        //просто добавим символ
                    }
                }
                else
                {
                    MessageBox.Show($"Символ {e.Text[0]} запрещен!");
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
            
        }

        private void Text_Box_Matrix_name_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString() == "Back" )
            {
                if (Text_Box_Path.Text != "!двойной клик выбор пути!")
                {
                    string path_without_name = Text_Box_Path.Text.Remove(Text_Box_Path.Text.LastIndexOf('\\') + 1);
                    Text_Box_Path.Text =path_without_name + Text_Box_Matrix_name.Text + ".txt";
                }
                else;
            }
            else;//остальные не обрабатыв-ся
        }

        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {
            if (!(string.IsNullOrEmpty(Text_Box_Matrix_name.Text) || string.IsNullOrWhiteSpace(Text_Box_Matrix_name.Text)) )
            {
                if (Text_Box_Path.Text!= "!двойной клик выбор пути!")
                {
                    //тогда сохраним
                    isSelectedAddToMatrixList = checkBox_AddToList.IsChecked.Value;
                    isSelectedSaveButton = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Пожалуйста выберите путь", "Путь НЕ указан");
                }
            }
            else
            {
                MessageBox.Show("Название матрицы не может быть пустым", "НЕВОЗМОЖНО СОХРАНИТЬ ФАЙЛ");

            }
        }
    }
}
