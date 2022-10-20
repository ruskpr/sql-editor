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

namespace SQLEditor
{
    /// <summary>
    /// Interaction logic for MyMbox.xaml
    /// </summary>
    public partial class MyMbox : Window
    {
        public MyMbox(string message, string title)
        {
            InitializeComponent();
            lblContent.Content = message;
            Title = title;
        }

        public MyMbox(string message)
        {
            InitializeComponent();
            lblContent.Content = message;
        }
    }
}
