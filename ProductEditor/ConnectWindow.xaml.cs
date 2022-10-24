using Microsoft.Data.SqlClient;
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
        }

        #region Button click events
        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            // default connection string
            SQLDataLayer dl = new SQLDataLayer();
            dl.ServerName = "localhost,1434";
            dl.DBName = "Northwind";
            GetConnection(dl);
        }
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            // custom connection
            SQLDataLayer dl = new SQLDataLayer(tbServerName.Text, tbDBName.Text, tbUserID.Text, tbPassword.Text);
            GetConnection(dl);
        }
        #endregion
        #region GetConnection method
        /// <summary>
        /// make new instance of SQLDataLayer,
        /// if it connects, pass datalayer to mainform with delegate method,
        /// if not, show error message
        /// </summary>
        private void GetConnection(SQLDataLayer datalayer) 
        {
            try
            {
                if (datalayer.IsConnected()) // if the database is connected...
                {
                    PassConnection.Invoke(datalayer); // pass the sql connection through delegate (to be used in main window)
                    //MessageBox.Show("Connected to: " + datalayer.ToString());
                    this.Close(); // close window if connected
                }
                else // if you cant connect...
                    MessageBox.Show("Failed to connect, please try again.", "Error");
            }
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.ToString()); // throw exception as message box
            }
        }
        #endregion
    }
}
