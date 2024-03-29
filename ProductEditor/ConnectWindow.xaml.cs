﻿using Microsoft.Data.SqlClient;
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
    public partial class ConnectWindow : Window
    {
        #region Delegates and Constructor
        public delegate void PassSQLConnectionDel(SQLDataLayer dataLayer);
        public static event PassSQLConnectionDel PassConnection;               
        public ConnectWindow() => InitializeComponent();
        #endregion
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
        /// if it connects, pass datalayer to main window through delegate,
        /// if not, show error message
        /// </summary>
        private void GetConnection(SQLDataLayer datalayer) 
        {
            try
            {
                if (datalayer.IsConnected()) // if the database is connected...
                {
                    PassConnection.Invoke(datalayer); // pass the sql connection through delegate (to be used in main window)
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
