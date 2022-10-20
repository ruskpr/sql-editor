using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
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
            
            lblCurrentConnection.Content = "Not connected, click the button to connect to a database";


            dgRecords.AutoGenerateColumns = true;
            dgRecords.CanUserAddRows = false;

            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("colStatus", typeof(string)));
            table.Columns.Add("column1");
            table.Rows.Add("row");

            DataGridTextColumn textColumn = new DataGridTextColumn();
            textColumn.Header = "First Name";
            textColumn.Binding = new Binding("FirstName");
            dgRecords.Columns.Add(textColumn);


            dgRecords.DataContext = table;

            foreach (DataRow dataRow in table.Rows)
            {
                foreach (var item in dataRow.ItemArray)
                {
                    lstTest.Items.Add(item);
                    
                }
            }
            foreach (DataColumn dataCol in table.Columns)
            {
                foreach (var item in dataCol.ColumnName)
                {
                    lstTest.Items.Add(item);
                    
                }
            }

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
                foreach (string table in this.dataLayer.GetTables())
                    cbTables.Items.Add(table);
                    

                //DisplayRecords();


            }
        }
        #endregion
        private void DisplayRecords()
        {

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

        private void btnLoadRecords_Click(object sender, RoutedEventArgs e)
        {
            if(this.dataLayer != null)
            {
                
            }
        }
        #endregion
    }
}
