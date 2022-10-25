using System.Collections.Generic;
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
            
            lblCurrentConnection.Content = "Not connected, click the button to connect to a database";

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
                foreach (var item in tableSelectionControls)
                    item.Visibility = Visibility.Visible;

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
            if (dgRecords.SelectedCells.Count != 1)
                return;

            selectedColumn = (string)dgRecords.SelectedCells[0].Column.Header;
            MessageBox.Show(selectedColumn);

        }
        #endregion


        #region button click events
        private void dgRecords_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => GetColumn();
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

            //dataLayer.UpdateProduct(cbTables.Text, selectedColumn, lbSelectedItem.Content.ToString(), tbChangeTo.Text);
        }
        #endregion



    }
}
