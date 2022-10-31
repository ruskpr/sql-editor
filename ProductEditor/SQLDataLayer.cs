using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace ProductEditor
{
    public class SQLDataLayer
    {
        #region Public properties
        public string ServerName { get; set; }
        public string DBName { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string connString { get; }
        #endregion
        #region Constructors (default / custom connection)
        // default constructor
        public SQLDataLayer() =>
            connString = ConfigurationManager.ConnectionStrings["localconnection"].ConnectionString;

        // constructor for custom connection
        public SQLDataLayer(string servername, string dbName, string userid, string password)
        {
            ServerName = servername;
            DBName = dbName;
            UserID = userid;
            Password = password; 
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
        } // bool to check if user can connect to a server
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
        } // get string list of database's table names
        public void DisplayAllFromTable(DataGrid dg, string tablename)
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
                    MessageBox.Show("An issue occured while getting the table.","Error");
                }
            }
        } // fill data grid using SQLDataAdapter
        public bool UpdateRecord(string tablename,string colname, DataGrid dg, string updateText)
        {
            string qry = $"UPDATE {tablename}\nSET {colname} = '{updateText}'\nWHERE ";

            DataRowView rowView = (DataRowView)dg.SelectedItem;

            for (int i = 0; i < dg.Columns.Count; i++) 
            {
                if (rowView != null)
                {
                    if (rowView.Row[i].ToString() != String.Empty)
                    {
                        qry += $"{dg.Columns[i].Header} = '{rowView.Row[i]}'\n";
                        string and = i != dg.Columns.Count - 1 ? "AND " : "";
                        qry += and;
                    }       
                }
            }

            MessageBoxResult result = MessageBox.Show(qry, "Execute Query?", MessageBoxButton.YesNo);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    bool ret = this.ExecuteNonQuery(qry);
                    if (ret)
                    {
                        MessageBox.Show("Record has been updated.", "Success!");
                        return ret;
                    }
                    else
                        MessageBox.Show("Could not update field. Please review the value and ensure it is " +
                                "entered correctly and matches the corresponding datatype.", "Error");
                    break;
                case MessageBoxResult.No:
                    return false;
            }
            return false;
        } // Update record
        public bool DeleteRecord(string tablename, DataGrid dg)
        {
            bool ret = true;
            string qry = $"DELETE FROM {tablename}\nWHERE ";

            DataRowView rowView = (DataRowView)dg.SelectedItem;

            int colcount = dg.Columns.Count;

            for (int i = 0; i < colcount; i++)
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
                    if (ret)
                    {
                        MessageBox.Show($"record has been deleted from {tablename}", "Success!");
                        return ret;
                    }
                    else
                        MessageBox.Show($"Could not delete record from {tablename}", "Error");
                    break;
                case MessageBoxResult.No:
                    return false;
            }
            return false;
        } // Delete record
        public bool InsertIntoTable(string tablename, List<string> colnames, List<TextBox> textboxes)
        {
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
        } // Insert record
        public bool DisplaySearched(DataGrid dg, string tablename, List<TextBox> textboxes)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    // initialize query string
                    string qry = $"SELECT * FROM {tablename}\n";

                    // declare variables
                    int colcount = dg.Columns.Count;
                    int fieldsUsed = 0;

                    //loop through textboxes to check how many are used
                    for (int i = 0; i < colcount; i++)
                        if (textboxes[i].Text != "")
                            fieldsUsed++;

                    // if a field is used, start WHERE statement
                    if (fieldsUsed > 0)
                        qry += "WHERE ";

                    // loop through each textbox that has text inside
                    for (int j = 0; j < textboxes.Count; j++)
                    {
                        if (textboxes[j].Text != "")
                        {
                            //add WHERE clause statements
                            qry += $"{dg.Columns[j].Header} = '{textboxes[j].Text}'\n";
                            qry += "AND ";
                        }                        
                    }

                    // remove the redundant AND that was added at the end of the loop
                    if (fieldsUsed > 0)
                        qry = qry.Remove(qry.Length - 4); 

                    // transfer query to datatable
                    SqlCommand cmd = new SqlCommand(qry, conn);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable(tablename);

                    //if no fields are entered execute 'SELECT * FROM [tablename]' statement without message dialog
                    if (fieldsUsed == 0)
                    {
                        sda.Fill(dt);
                        dg.ItemsSource = dt.DefaultView;
                        return true;
                    }
                    
                    // show message dialog to ask user if they want to execute the query
                    MessageBoxResult result = MessageBox.Show(qry, "Execute Query?", MessageBoxButton.YesNo);

                    switch (result)
                    {
                        case MessageBoxResult.Yes: // set datagrid source as queried result
                            sda.Fill(dt);
                            dg.ItemsSource = dt.DefaultView;
                            return true;
                        case MessageBoxResult.No: // exit dialog without any changes
                            return false;
                    }
                }
                catch
                {
                    MessageBox.Show("An issue occured while getting the table.", "Error");
                }

                return false;
            }
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
        #region Override ToString()
        public override string ToString() =>
            $"server = '{ServerName}', database = '{DBName}'";
        #endregion
    }
}
