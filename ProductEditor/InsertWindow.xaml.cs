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
using static System.Net.Mime.MediaTypeNames;

namespace SQLEditor
{
    /// <summary>
    /// Generate fields based on number of columns the corrensponding table has.
    /// Allow user to fill out fields and insert record to table through SQL Datalayer
    /// </summary>
    public partial class InsertWindow : Window
    {
        #region Delegate definition
        public delegate void ItemInserted();
        public static event ItemInserted OnItemInserted;
        #endregion
        #region Fields
        private List<TextBox> textboxes = new List<TextBox>();
        private List<string> colnames = new List<string>();
        private string tableName;
        #endregion
        #region Constructor
        public InsertWindow(string tablename, List<DataGridColumn> columns)
        {
            InitializeComponent();
            // assign table name to local variable and set window title
            this.tableName = tablename;
            this.Title = $"Insert into {this.tableName}";

            // add column names to list
            foreach (var col in columns)
                colnames.Add(col.Header.ToString());

            lbHeader.Content = $"Fill in the following fields:";
            btnInsert.TabIndex = colnames.Count;
            btnInsert.Content = $"Insert into {tableName}";

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
                lb.Margin = new Thickness(80,5,0,0);
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
        #region Button click events
        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            var success = MainWindow.DataLayer.InsertIntoTable(tableName, colnames, textboxes);

            if (success)
                OnItemInserted.Invoke();
        }
        #endregion
    }
}
