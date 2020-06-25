using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Window1_pop_up_input_matrix_name.xaml
    /// </summary>
    public partial class Window1_pop_up_input_matrix_name : Window
    {
        public string get_Matrix_name {
            get
            {
             return   Matrix_name_in_popup.Text;
            }
        }
        public bool IsCancelled { get; private set; } = false;
        public Window1_pop_up_input_matrix_name()
        {
            InitializeComponent();
        }

        private void Button_Click_ADD(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            IsCancelled = true;
            Close();
        }

        private void Matrix_name_in_popup_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!(e.Text is null))
            {
                if (char.IsLetterOrDigit(e.Text[0]))
                {

                }
                else 
                {
                    e.Handled = true;
                }
            }
            else;
        }

       
    }
}
