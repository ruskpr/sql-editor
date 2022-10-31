using ProductEditor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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
        #region Delegate definition
        public delegate void ItemUpdated();
        public event ItemUpdated OnItemUpdated;
        #endregion
        #region Fields
        private object[] items;
        private string tableName;
        private string selColumn;
        private string selItem;
        #endregion
        #region Constructor
        public RecordViewer(string tablename, DataGrid datagrid, object[] selectedItems)
        {
            InitializeComponent();
            dgRecordViewer.AutoGenerateColumns = true;
            dgRecordViewer.CanUserAddRows = false;
            dgRecordViewer.IsReadOnly = true;

            //get current table name 
            this.tableName = tablename;

            // get selected row items
            items = selectedItems;

            InitializeDataGrid(tableName, datagrid, items);

            UpdateText();

            lbSelectedItem.Content = "Selected Item: none";
            btnDeleteRecord.Content = $"Delete this from {this.tableName}";
        }
        #endregion
        #region Private methods
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
            lbSelectedItem.Content = $"Selected Item: {selColumn}: '{selItem}' ";
        } // update labels 
        private void dgRecordViewer_CurrentCellChanged(object sender, EventArgs e) // get selected cell value
        {
            try
            {
                // set selected column
                if (dgRecordViewer.CurrentCell.Column != null)
                {
                    selColumn = Convert.ToString(dgRecordViewer.CurrentCell.Column.Header);

                    // set selected item 
                    var cellInfo = dgRecordViewer.CurrentCell;
                    if (cellInfo != null)
                    {
                        var column = cellInfo.Column as DataGridBoundColumn;
                        if (column != null)
                        {
                            var element = new FrameworkElement() { DataContext = cellInfo.Item };
                            BindingOperations.SetBinding(element, FrameworkElement.TagProperty, column.Binding);
                            var cellValue = element.Tag;

                            if (cellValue != null)  
                                selItem = cellValue.ToString();
                        }
                    }
                }
            }
            catch { }
            
            UpdateText();
        }
        #endregion
        #region Button click events
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.DataLayer != null && tbNewValue.Text.Trim() != "")
            {
                bool success = MainWindow.DataLayer.UpdateRecord(tableName, selColumn, dgRecordViewer, tbNewValue.Text);
                
                if (success)
                {
                    this.Close();
                    OnItemUpdated.Invoke();
                } 
            }
            else if (tbNewValue.Text.Trim() == "")
            {
                MessageBox.Show("Enter a value.");
                tbNewValue.Focus();
            }

        }
        private void btnDeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            if (dgRecordViewer.SelectedItems.Count > 0)
            {
                if (MainWindow.DataLayer != null)
                {
                    bool success = MainWindow.DataLayer.DeleteRecord(tableName, dgRecordViewer);

                    if (success)
                    {
                        OnItemUpdated.Invoke();
                        this.Close();
                    }
                }
            }
            else if (dgRecordViewer.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select record above to confirm.");
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => Owner.Focus();
        #endregion

    }
}
