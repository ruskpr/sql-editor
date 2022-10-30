using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using Azure.Identity;
using Microsoft.Data.SqlClient;

namespace ProductEditor
{
    public class SQLDataLayer
    {
        #region Public properties
        public string ServerName { get; set; }
        public string DBName { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        #endregion
        public string connString { get; }
        public SqlDataAdapter DataAdapter { get; set; }

        #region Constructors (default / custom connection)
        // default constructor
        public SQLDataLayer()
        {
            connString = ConfigurationManager.ConnectionStrings["localconnection"].ConnectionString;
            DataAdapter = new SqlDataAdapter();
        }

        // constructor for custom connection
        public SQLDataLayer(string servername, string dbName, string userid, string password)
        {

            ServerName = servername;
            DBName = dbName;
            UserID = userid;
            Password = password; 
            DataAdapter = new SqlDataAdapter();
            connString = $"server={ServerName};database={DBName};user id={UserID};password={Password};encrypt=false;";
        }
        #endregion
        #region Public methods
        public bool IsConnected()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    return true;
                }
                catch (SqlException) 
                {
                    return false;
                }
            }
        } // bool to check if use can connect to specified server
        public List<string> GetTableNames()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand($"select * from sys.tables", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                List<string> tables = new List<string>();

                int index = 0;
                while (reader.Read())
                {
                    tables.Add((string)reader[0]);
                    index++;
                }

                return tables;
            }
        } // get string list of database table names
        
        public void FillDataGrid(DataGrid dg, string tablename) //
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    string qry = "";
                    qry = $"SELECT * FROM {tablename}";
                    SqlCommand cmd = new SqlCommand(qry, conn);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable(tablename);
                    sda.Fill(dt);
                    dg.ItemsSource = dt.DefaultView;
                }
                catch 
                {
                    MessageBox.Show("Error.");
                }
            }
        }
        public bool UpdateProduct(string tablename,string colname, DataGrid dg, string updateText)
        {
            bool ret = true;
            string qry = $"UPDATE {tablename}\nSET {colname} = '{updateText}'\nWHERE ";

            DataRowView rowView = (DataRowView)dg.SelectedItem;

            for (int i = 0; i < dg.Columns.Count; i++) 
            {
                if (rowView != null)
                {
                    qry += $"{dg.Columns[i].Header} = '{rowView.Row[i]}'\n";
                    string and = i != dg.Columns.Count - 1 ? "AND " : "";
                    qry += and;
                }
            }

            MessageBoxResult result = MessageBox.Show(qry, "Execute Query?", MessageBoxButton.YesNo);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    ret = this.ExecuteNonQuery(qry);
                    if (ret == false)
                    {
                        MessageBox.Show("error");
                    }
                    break;
                case MessageBoxResult.No:
                    break;
            }
            return ret;
        }
        public bool InsertIntoTable(string tablename, List<string> colnames, List<TextBox> textboxes)
        {
            //INSERT INTO table_name(column1, column2, column3, ...)
            //VALUES(value1, value2, value3, ...);
            bool ret = true;
            string qry = $"INSERT INTO {tablename} (\n";


            //add column names to query
            for (int i = 0; i < colnames.Count; i++)
            {
                if (textboxes[i].Text != String.Empty)
                {
                    qry += $"{colnames[i]}";
                    qry += i != colnames.Count - 1 ? ",\n" : "\n)\n";
                }
                    
            }

            //add textbox values
            qry += "VALUES (\n";

            for (int j = 0; j < textboxes.Count; j++)
            {
                if (textboxes[j].Text != String.Empty)
                {
                    qry += $"'{textboxes[j].Text}'";
                    qry += j != textboxes.Count - 1 ? ",\n" : "\n)";
                }

            }

            MessageBoxResult result = MessageBox.Show(qry, "Execute Query?", MessageBoxButton.YesNo);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    ret = this.ExecuteNonQuery(qry);
                    if (ret)
                        MessageBox.Show($"record has been added to {tablename}", "Success!");
                    else
                        MessageBox.Show("Please make sure all fields have been filled out with the correct data type.\n" +
                            "Leave AUTO INCREMENT fields and fields that you want to be NULL empty.", "Error");

                    break;
                case MessageBoxResult.No:
                    break;
            }
            return ret;
        }
        #endregion
        #region Private methods
        private object? ExecuteScalar(string qry)
        {
            object? ret = null;
            SqlConnection conn = new SqlConnection(connString);

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(qry, conn);
                ret = cmd.ExecuteScalar();
            }
            catch { ret = null; }
            finally { conn.Close(); }

            return ret;
        }
        private bool ExecuteNonQuery(string qry)
        {
            bool ret = true;
            SqlConnection conn = new SqlConnection(connString);

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(qry, conn);
                cmd.ExecuteNonQuery();
            }
            catch { ret = false; }
            finally { conn.Close(); }

            return ret;
        }
        #endregion

        public override string ToString() =>
            $"server = '{ServerName}', database = '{DBName}'";

        
    }
}
