﻿using SQLEditor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

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
            InsertWindow.OnItemInserted += InsertWindow_OnItemInserted;
            lblCurrentConnection.Content = "Not connected, click the button to connect to a database.";

            dgRecords.AutoGenerateColumns = true;
            dgRecords.CanUserAddRows = false;
            dgRecords.IsReadOnly = true;

            tableSelectionControls.Add(lblSelectTable);
            tableSelectionControls.Add(cbTables);
            tableSelectionControls.Add(btnInsert);

            foreach (var item in tableSelectionControls)
                item.Visibility = Visibility.Hidden;

        }
        #region Delegate events
        private void InsertWindow_OnItemInserted() => DisplayRecords();
        private void ConnectWindow_PassConnection(SQLDataLayer dataLayer)
        {
            //set main window instance of 'SQLDataLayer' as data layer from 'ConnectWindow'
            DataLayer = dataLayer;

            if (DataLayer != null)
            {
                // show current connection in label
                lblCurrentConnection.Content = DataLayer.ToString();

                // show table names in dropdown box
                cbTables.Items.Clear();
                foreach (string table in DataLayer.GetTableNames())
                    cbTables.Items.Add(table);

                // table selection if connected
                foreach (var control in tableSelectionControls)
                    control.Visibility = Visibility.Visible;
            }
        }
        #endregion
        #region Private methods
        private void DisplayRecords()
        {
            if (DataLayer != null && cbTables.SelectedItem != null)
            {
                dgRecords.Columns.Clear();

                DataLayer.FillDataGrid(dgRecords, cbTables.SelectedItem.ToString());
                btnInsert.Content = $"Insert new record into {cbTables.SelectedItem.ToString()}";

            }
        }
        private void cbTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayRecords();
            
        }
        #endregion
        #region button click events
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            // open connection window to get connection
            ConnectWindow cw = new ConnectWindow();
            cw.Owner = this;
            cw.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            cw.ShowDialog();
        }
        private void btnLoadRecords_Click(object sender, RoutedEventArgs e)
        {

        }



        #endregion
        private void dgRecords_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            RecordViewer rv = new RecordViewer(cbTables.Text, dgRecords, GetSelectedRow());
            rv.Owner = this;
            rv.ShowDialog();
            DisplayRecords();
        }
        private object[] GetSelectedRow()
        {
            object[] rowItems = new object[dgRecords.Columns.Count];

            if (dgRecords.SelectedItems.Count > 0)
            {
                DataRowView selectedRow = (DataRowView)dgRecords.SelectedItems[0];
                for (int j = 0; j < dgRecords.Columns.Count; j++)
                {
                    if (selectedRow.Row.ItemArray[j].GetType() == typeof(Boolean))
                        rowItems[j] = Convert.ToString(selectedRow.Row.ItemArray[j]);
                    else
                        rowItems[j] = selectedRow.Row.ItemArray[j];
                }
            }
            return rowItems;
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            if (cbTables.Text != String.Empty)
            {
                InsertWindow insertWindow = new InsertWindow(cbTables.Text, dgRecords.Columns.ToList());
                insertWindow.Owner = this;
                insertWindow.Show();
            }
        }
    }
}
