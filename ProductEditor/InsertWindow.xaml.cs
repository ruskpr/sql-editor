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
    /// Interaction logic for InsertWindow.xaml
    /// </summary>
    public partial class InsertWindow : Window
    {
        public delegate void ItemInserted();
        public static event ItemInserted OnItemInserted;

        List<TextBox> textboxes = new List<TextBox>();
        private List<string> colnames = new List<string>();
        private string tableName;
        public InsertWindow(string tablename, List<DataGridColumn> columns)
        {
            InitializeComponent();
            this.tableName = tablename;
            // add column names to list
            foreach (var col in columns)
                colnames.Add(col.Header.ToString());

            lbHeader.Content = $"Fill in the following fields:";

            btnInsert.TabIndex = colnames.Count;
            btnInsert.Content = $"Insert into {tableName}";

            AddFields();
            textboxes[0].Focus();
        }

        private void AddFields()
        {
            //< Label Content = "ProductID" HorizontalAlignment = "Left" Margin = "80,5,0,0" FontSize = "10" />
            //< TextBox TextWrapping = "NoWrap" Text = "" Width = "150" />

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
                tb.TabIndex = i;

                spFields.Children.Add(lb);
                spFields.Children.Add(tb);
                textboxes.Add(tb);
            }
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            var success = MainWindow.DataLayer.InsertIntoTable(tableName, colnames, textboxes);

            if (success)
                OnItemInserted.Invoke();
        }
    }
}
