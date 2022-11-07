using ProductEditor;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using static SQLEditor.InsertWindow;

namespace SQLEditor
{
    public partial class SearchWindow : Window
    {
        #region Delegate definition
        public delegate void ItemSearched();
        public event ItemSearched OnItemSearched;
        #endregion
        #region Fields
        private List<TextBox> textboxes = new List<TextBox>();
        private List<string> colnames = new List<string>();
        private string tableName;
        private DataGrid datagrid;
        #endregion
        #region Constructor
        public SearchWindow(DataGrid dg, string tablename, List<DataGridColumn> columns)
        {
            InitializeComponent();
            // assign table name to local variable and set window title
            this.tableName = tablename;
            this.datagrid = dg;
            this.Title = $"Search records in {this.tableName}";

            // add column names to list
            foreach (var col in columns)
                colnames.Add(col.Header.ToString());

            lbHeader.Content = $"Search {tableName}:";
            lbInstructions2.Text = $"*Leave all fields empty to show all records in {tableName}.";
            btnSearch.TabIndex = colnames.Count;
            btnSearch.Content = $"Search {tableName}";

            AddFields();

            if (textboxes.Count > 0) // focus on first textbox
                textboxes[0].Focus();
        }
        #endregion
        #region Add fields method
        private void AddFields()
        {
            for (int i = 0; i < colnames.Count; i++)
            {
                Label lb = new Label();
                lb.Content = $"{colnames[i]}:";
                lb.HorizontalAlignment = HorizontalAlignment.Left;
                lb.Margin = new Thickness(80, 5, 0, 0);
                lb.FontSize = 10;

                TextBox tb = new TextBox();
                tb.TextWrapping = TextWrapping.NoWrap;
                tb.Text = "";
                tb.Width = 150;
                tb.TabIndex = i; // set chronological tab order

                spFields.Children.Add(lb);
                spFields.Children.Add(tb);
                textboxes.Add(tb);
            }
        }
        #endregion
        #region Click events
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //var success = MainWindow.DataLayer.InsertIntoTable(tableName, colnames, textboxes);
            bool success = MainWindow.DataLayer.DisplaySearched(datagrid, tableName, textboxes);

            if (success)
                this.Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => Owner.Focus();
        #endregion
    }
}
