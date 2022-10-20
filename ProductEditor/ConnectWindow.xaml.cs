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

namespace ProductEditor
{
    /// <summary>
    /// Interaction logic for ConnectWindow.xaml
    /// </summary>

    public delegate void PassSQLConnectionDel(SQLDataLayer dataLayer);
    public partial class ConnectWindow : Window
    {
        public static event PassSQLConnectionDel PassConnection;
        public ConnectWindow()
        {
            InitializeComponent();
            lblChecking.Visibility = Visibility.Hidden;

        }

        private void btnDefaultValues_Click(object sender, RoutedEventArgs e)
        {
            tbServerName.Text = "localhost,1434";
            tbDBName.Text = "Northwind";
            tbUserID.Text = "sa";
            tbPassword.Text = "P@ssword!";
            lblChecking.Visibility = Visibility.Visible;

        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
             GetConnection();
        }

        /// <summary>
        /// make new instance of SQLDataLayer,
        /// if it connects, pass datalayer to mainform with delegate method,
        /// if not, show error message box
        /// </summary>
        private void GetConnection() 
        {
            SQLDataLayer dl = new SQLDataLayer(tbServerName.Text, tbDBName.Text, tbUserID.Text, tbPassword.Text);

            try
            {
                if (dl.CheckConnection()) // if the database is connected...
                {
                    PassConnection.Invoke(dl); // pass the sql connection through delegate (to be used in main window)
                    MessageBox.Show("Connected to: " + dl.ToString());
                    this.Close(); // close window if connected
                }
                else // if you cant connect...
                {
                    MessageBox.Show("Failed to connect.", "Error");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString()); // throw exception as message box
            }
        }
    }
}
