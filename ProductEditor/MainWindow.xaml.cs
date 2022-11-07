using SQLEditor;
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
        #region Static DataLayer component
        public static SQLDataLayer? DataLayer;
        #endregion
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            ConnectWindow.PassConnection += ConnectWindow_PassConnection;
            InsertWindow.OnItemInserted += InsertWindow_OnItemInserted;
            lblCurrentConnection.Content = "Not connected, click the button to connect to a database.";

            dgRecords.AutoGenerateColumns = true;
            dgRecords.CanUserAddRows = false;
            dgRecords.IsReadOnly = true;

            lblSelectTable.Visibility = Visibility.Hidden;
            cbTables.Visibility = Visibility.Hidden;
            btnInsert.Visibility = Visibility.Hidden;
            btnSearch.Visibility = Visibility.Hidden;
        }
        #endregion
        #region Delegate events
        private void Recordviewer_OnItemUpdated() => DisplayAllRecords();
        // refresh records when an item is updated
        private void InsertWindow_OnItemInserted() => DisplayAllRecords();
        // refresh records when new item is inserted
        // refresh records with searched results
        private void ConnectWindow_PassConnection(SQLDataLayer dataLayer)
        // recieve connection from connection window
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

                // show table selection if connected
                lblSelectTable.Visibility = Visibility.Visible;
                cbTables.Visibility = Visibility.Visible;
                btnInsert.Visibility = Visibility.Visible;
                btnSearch.Visibility = Visibility.Visible;
            }
        }
        #endregion
        #region Private methods
        private void DisplayAllRecords() // display records in datagrid using sql connection
        {
            if (DataLayer != null && cbTables.SelectedItem != null)
            {
                dgRecords.Columns.Clear();

                DataLayer.DisplayAllFromTable(dgRecords, cbTables.SelectedItem.ToString());
                btnInsert.Content = $"Insert into {cbTables.SelectedItem.ToString()}";

            }
        } 
        private void cbTables_SelectionChanged(object sender, SelectionChangedEventArgs e) => DisplayAllRecords();
        private object[] GetSelectedRow() // get items in selected row for passing to record viewer interface
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
        #endregion
        #region Click events
        private void btnConnect_Click(object sender, RoutedEventArgs e) // open connection window
        {
            // open connection window to get connection
            ConnectWindow cw = new ConnectWindow();
            cw.Owner = this;
            cw.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            cw.ShowDialog();
        }
        private void btnInsert_Click(object sender, RoutedEventArgs e) // open insert record window
        {
            if (cbTables.Text != String.Empty)
            {
                InsertWindow insertWindow = new InsertWindow(cbTables.Text, dgRecords.Columns.ToList());
                insertWindow.Owner = this;
                insertWindow.Show();
            }
            else
            {
                MessageBox.Show("Select a table from the dropdown menu above.", "SQL Data Editor");
            }
        }
        private void dgRecords_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        //display record viewer when cell is selected
        {
            RecordViewer recordviewer = new RecordViewer(cbTables.Text, dgRecords, GetSelectedRow());
            recordviewer.Owner = this;
            recordviewer.Show();
            recordviewer.OnItemUpdated += Recordviewer_OnItemUpdated;
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (cbTables.Text != String.Empty)
            {
                SearchWindow searchwindow = new SearchWindow(dgRecords, cbTables.Text, dgRecords.Columns.ToList());
                searchwindow.Owner = this;
                searchwindow.Show();
            }
            else
            {
                MessageBox.Show("Select a table from the dropdown menu above.", "SQL Data Editor");
            }
        }
        #endregion
    }
}
