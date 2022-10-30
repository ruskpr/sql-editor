using ProductEditor;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SQLEditor
{
    /// <summary>
    /// Interaction logic for RecordViewer.xaml
    /// </summary>
    public partial class RecordViewer : Window
    {

        private object[] items;

        private string tableName;
        private string selColumn;

        
        public RecordViewer(string tablename, DataGrid datagrid, object[] selectedItems)
        {
            InitializeComponent();

            //get current table name 
            this.tableName = tablename;

            // get selected row items
            items = selectedItems;

            InitializeDataGrid(tableName, datagrid, items);

            UpdateText();
        }

        private void InitializeDataGrid(string tablename, DataGrid datagrid, object[] selectedItems)
        {
            ///
            /// 1.Create data table instance that will be used on datagrid as item source
            /// 2.loop through main window datagrid and add columns to datagrid on recordviewer window
            /// 3.add single row of items that were selected from main window to recordviewer window
            /// 4.bind datatable to recordviewer datagrid
            ///

            //1
            DataTable tmpDataTable = new DataTable();

            //2
            foreach (var col in datagrid.Columns)
                tmpDataTable.Columns.Add(new DataColumn(col.Header.ToString(), typeof(String)));

            //3
            tmpDataTable.Rows.Add(items);

            //4
            dgRecordViewer.ItemsSource = tmpDataTable.DefaultView;
            ///
        }
        private void UpdateText()
        {
            lbCurrentTable.Content = $"Current Table: {tableName}";
            lbSelectedColumn.Content = $"Selected Column: {selColumn}";
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.DataLayer != null && tbChangeTo.Text.Trim() != "")
            {
                MainWindow.DataLayer.UpdateProduct(tableName, selColumn, dgRecordViewer, tbChangeTo.Text);
            }
            else if (tbChangeTo.Text.Trim() == "")
            {
                MessageBox.Show("Enter a value.");
                tbChangeTo.Focus();
            }
        }

        private void dgRecordViewer_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //if (dgRecordViewer.SelectedItems.Count > 0)
            //{
            //    //DataRowView selectedRow = (DataRowView)dgRecordViewer.SelectedItems[0];

            //    //MessageBox.Show(selectedRow.Row[2].ToString());
            //    MessageBox.Show(dgRecordViewer.SelectedValue.ToString());
                
            //}
        }

        private void dgRecordViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var s = dgRecordViewer.SelectedCells[0].Item.ToString();
            MessageBox.Show(s);
        }
    }
}
