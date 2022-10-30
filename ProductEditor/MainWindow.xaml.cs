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
        private SQLDataLayer? dataLayer;

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
            this.dataLayer = dataLayer;

            if (this.dataLayer != null)
            {
                lblCurrentConnection.Content = this.dataLayer.ToString();

                cbTables.Items.Clear();
                foreach (string table in this.dataLayer.GetTableNames())
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
            if (this.dataLayer != null)
            {
                dgRecords.Columns.Clear();

                dataLayer.FillDataGrid(dgRecords, cbTables.Text);

                lbCurrentTable.Content = $"Current Table: {cbTables.Text}";
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
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (this.dataLayer != null)
            {
                dataLayer.UpdateProduct(cbTables.Text, selectedColumn, lbSelectedItem.Content.ToString(), tbChangeTo.Text);
            }
        }


        #endregion

        private void dgRecords_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void dgRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dgRecords_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            string str = "";
            List<DataRowView> rowdetails = new List<DataRowView>();
            object[] rowItems = new object[dgRecords.Columns.Count];
            if (dgRecords.SelectedItems.Count > 0)
            {
                System.Data.DataRowView selectedRow = (System.Data.DataRowView)dgRecords.SelectedItems[0];
                for (int j = 0; j < dgRecords.Columns.Count; j++)
                {
                    rowItems[j] = (selectedRow.Row.ItemArray[j]);
                }
            }
            foreach (var item in rowItems)
            {
                MessageBox.Show(item.ToString());
            }
        }
    }
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int SupplierID { get; set; }
        public int CategoryID { get; set; }
        public string QuantityPerUnit { get; set; }
        public decimal UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        public int UnitsOnOrder { get; set; }
        public int ReorderLevel { get; set; }
        public int Discontinued { get; set; }

        public List<object> Columns = new List<object>();

        public Product()
        {
            Columns.Add(ProductID);
            Columns.Add(ProductName);
            Columns.Add(CategoryID);
            Columns.Add(QuantityPerUnit);
            Columns.Add(UnitPrice);
            Columns.Add(UnitsInStock);
            Columns.Add(UnitsOnOrder);
            Columns.Add(ReorderLevel);
            Columns.Add(Discontinued);
        }
    }
}
