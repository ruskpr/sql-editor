﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProductEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SQLDataLayer dataLayer;
        public MainWindow()
        {
            InitializeComponent();

            ConnectWindow.PassConnection += ConnectWindow_PassConnection;
            
            lblConnectedTo.Content = "Not connected, click the button to connect to a database";
        }

        //recieve connection when connected through 'ConnectWindow' 
        private void ConnectWindow_PassConnection(SQLDataLayer dataLayer)
        {
            //set main window instance of 'SQLDataLayer' as data layer from 'ConnectWindow'
            this.dataLayer = dataLayer;

            if (this.dataLayer != null)
                lblConnectedTo.Content = this.dataLayer.ToString();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            //SQLDataLayer dl = new SQLDataLayer();
            ConnectWindow cw = new ConnectWindow();
            cw.Owner = this;
            cw.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            cw.ShowDialog();
            
        }
    }
}