using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
        public string ConnectionString { get; set; }

        #region Constructors (default / custom connection)
        // default constructor
        public SQLDataLayer() =>
            ConnectionString = ConfigurationManager.ConnectionStrings["localconnection"].ConnectionString;

        // constructor for custom connection
        public SQLDataLayer(string servername, string dbName, string userid, string password)
        {

            ServerName = servername;
            DBName = dbName;
            UserID = userid;
            Password = password;

            ConnectionString = $"server={ServerName};database={DBName};user id={UserID};password={Password};encrypt=false;";
        }
        #endregion
        #region Public methods
        public bool IsConnected()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
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
            using (SqlConnection conn = new SqlConnection(ConnectionString))
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
        
        public void FillDataGrid(DataGrid dg, string tablename)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string qry = "";
                qry = $"SELECT * FROM {tablename}";
                SqlCommand cmd = new SqlCommand(qry, conn);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Employee");
                sda.Fill(dt);
                dg.ItemsSource = dt.DefaultView;
            }
        }
        public List<string[]> GetRecords(string tablename)
        {
            List<string[]> records = new List<string[]>();
            SqlConnection conn = new SqlConnection(ConnectionString);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand($"select * from {tablename}", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    
                    string line = "";
                    string splitter = "\u007E";

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        //MessageBox.Show(reader.FieldCount.ToString());
                        line += reader[i];

                        if (i != reader.FieldCount - 1) ///
                            line += splitter;
                    }

                    string[] record = line.Split(splitter);
                    records.Add(record);
                }
            }
            catch { }
            finally { conn.Close(); }

            return records;
        }
        #endregion
        #region Private methods
        private object? ExecuteScalar(string qry)
        {
            object? ret = null;
            SqlConnection conn = new SqlConnection(ConnectionString);

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
        private string ExecuteNonQuery(string qry)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(qry, conn);
                return cmd.ExecuteNonQuery().ToString();
            }
            catch { return "failed."; }
            finally { conn.Close(); }
        }
        #endregion

        public override string ToString() =>
            $"server = '{ServerName}', database = '{DBName}'";
    }
}
