﻿using SQLEditor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace ProductEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Control> tableSelectionControls = new List<Control>();
        public static SQLDataLayer? DataLayer;

        private string selectedColumn;
        public MainWindow()
        {
            InitializeComponent();

            ConnectWindow.PassConnection += ConnectWindow_PassConnection;

            lblCurrentConnection.Content = "Not connected, click the button to connect to a database.";

            dgRecords.AutoGenerateColumns = true;
            dgRecords.CanUserAddRows = false;
            dgRecords.IsReadOnly = true;

            tableSelectionControls.Add(lblSelectTable);
            tableSelectionControls.Add(cbTables);
            tableSelectionControls.Add(btnLoadRecords);

            foreach (var item in tableSelectionControls)
                item.Visibility = Visibility.Hidden;

        }

        #region Delegate from connect window
        //recieve connection when connected through 'ConnectWindow' 
        private void ConnectWindow_PassConnection(SQLDataLayer dataLayer)
        {
            //set main window instance of 'SQLDataLayer' as data layer from 'ConnectWindow'
            DataLayer = dataLayer;

            if (DataLayer != null)
            {
                lblCurrentConnection.Content = DataLayer.ToString();

                cbTables.Items.Clear();
                foreach (string table in DataLayer.GetTableNames())
                    cbTables.Items.Add(table);

                //DisplayRecords();
                foreach (var control in tableSelectionControls)
                    control.Visibility = Visibility.Visible;
            }
        }
        #endregion
        #region Private methods
        private void DisplayRecords()
        {
            if (DataLayer != null)
            {
                dgRecords.Columns.Clear();

                DataLayer.FillDataGrid(dgRecords, cbTables.Text);

            }
        }
        private void GetColumn()
        {
            //    return;



        }
        #endregion
        #region button click events
        private void dgRecords_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            // open connection window to get connection
            ConnectWindow cw = new ConnectWindow();
            cw.Owner = this;
            cw.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            cw.ShowDialog();
        }
        private void btnLoadRecords_Click(object sender, RoutedEventArgs e) => DisplayRecords();



        #endregion
        private void dgRecords_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            RecordViewer rv = new RecordViewer(cbTables.Text, dgRecords, GetSelectedRow());
            rv.Owner = this;
            rv.Show();
        }
        private object[] GetSelectedRow()
        {
            object[] rowItems = new object[dgRecords.Columns.Count];

            if (dgRecords.SelectedItems.Count > 0)
            {
                DataRowView selectedRow = (DataRowView)dgRecords.SelectedItems[0];
                for (int j = 0; j < dgRecords.Columns.Count; j++)
                {
                    rowItems[j] = selectedRow.Row.ItemArray[j];
                }
            }
            return rowItems;
        }
    }
}
