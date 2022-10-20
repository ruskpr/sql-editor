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
using SQLEditor;

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
        public string connectionString { get; set; }

        public SQLDataLayer(string servername, string dbName, string userid, string password)
        {
            ServerName = servername;
            DBName = dbName;
            UserID = userid;
            Password = password;

            //connectionString = ConfigurationManager.ConnectionStrings["localconnection"].ConnectionString;
            connectionString = $"server={ServerName};database={DBName};user id={UserID};password={Password};encrypt=false;";
        }

        //Checks if it is able to connect
        public bool IsConnected()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
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
        }
        public override string ToString()
        {
            return $"server = '{ServerName}', database = '{DBName}'";
        }

        //ExecuteNonQuery
        public string ExecuteNonQuery(string qry)
        {
;
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(qry, conn);
                return cmd.ExecuteNonQuery().ToString();
            }
            catch
            {
                return "failed";
            }
            finally
            {
                conn.Close();
            }
        }

        private object ExecuteScalar(string qry)
        {
            object ret = null;
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(qry, conn);
                ret = cmd.ExecuteScalar();
            }
            catch
            {
                ret = null;
            }
            finally
            {
                conn.Close();
            }

            return ret;
        }

        public List<string> GetTables()
        {

            using (SqlConnection conn = new SqlConnection(connectionString))
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



            //DataTable dt = conn.GetSchema("Tables");


            //string lineOfTableNames = string.Join(Environment.NewLine, dt.Rows.OfType<DataRow>().Select(x => string.Join(",", x.ItemArray)));

            //string[] arrTableNames = lineOfTableNames.Split(",");

            //string s = conn.GetSchema("Tables").TableName;

            //string[] arrTableNames = new string[];

        }

        public List<DataGridTextColumn> GetFieldNames(string tableName)
        {
            //list to be returned
            List<DataGridTextColumn> columnNames = new List<DataGridTextColumn>();

            //open connection and add field names
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand($"SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = {tableName}", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                }
            }

            DataGridTextColumn textColumn = new DataGridTextColumn();

            

            textColumn.Header = "First Name";
            textColumn.Binding = new Binding("FirstName");

            return columnNames;
        }
        public List<string> LoadRecords(string tablename)
        {
            List<string> records = new List<string>();
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand($"select * from {tablename}", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string line = "";

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        MessageBox.Show(reader.FieldCount.ToString());
                        //line += reader[i];
                    }
                    string currentMsg = $"{reader[0]} {reader[2]}";
                    records.Add(line);
                }
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }

            return records;
        }
    }
}
