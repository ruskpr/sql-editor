using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        private string connectionString;
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
        public bool CheckConnection()
        {
            bool ret = false;
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();
                ret = true;
            }
            catch (Exception)
            {

                ret = false;
            }
            finally
            {
                conn.Close();
            }

            return ret;
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
