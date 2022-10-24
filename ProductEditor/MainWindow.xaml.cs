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
        public MainWindow()
        {
            InitializeComponent();

            ConnectWindow.PassConnection += ConnectWindow_PassConnection;
            
            lblCurrentConnection.Content = "Not connected, click the button to connect to a database";

            dgRecords.AutoGenerateColumns = true;
            dgRecords.CanUserAddRows = false;

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
        private void DisplayRecords()
        {
            if (this.dataLayer != null)
            {
                dgRecords.Columns.Clear();

                // add field names to datagrid
                //foreach (var item in dataLayer.GetFieldNames(cbTables.Text))
                //dgRecords.Columns.Add(item);

                // add data records
                //List<string[]> recordList = new List<string[]>();
                //recordList.AddRange(dataLayer.GetRecords(cbTables.Text));

                dataLayer.FillDataGrid(dgRecords, cbTables.Text);




            }
        }

        #region button click events
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            // open connection window to get connection
            ConnectWindow cw = new ConnectWindow();
            cw.Owner = this;
            cw.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            cw.ShowDialog();
        }

        private void btnLoadRecords_Click(object sender, RoutedEventArgs e) =>
            DisplayRecords();

        #endregion
    }
}
