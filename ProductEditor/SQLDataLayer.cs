using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Data.SqlClient;

namespace ProductEditor
{
    public class SQLDataLayer
    {
        //public string connectionString;
        string connectionString;
        public SQLDataLayer(string connString = "")
        {
            // if param is blank, use connection string from config
            if (connString == "")
                connectionString = ConfigurationManager.ConnectionStrings["localconnection"].ConnectionString;
            else // otherwise use your own string
                connectionString = connString;
        }


        //ExecuteNonQuery
        private bool ExecuteNonQuery(string qry)
        {
            bool ret = true;
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(qry, conn);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                ret = false;
            }
            finally
            {
                conn.Close();
            }

            return ret;
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

        public bool RegisterUser(string username, string password)
        {
            string cmd = $"insert into users (username, password, register_date) values ({username}, {password}, getdate())";

            return this.ExecuteNonQuery(cmd);
        }

        public bool LoginUser(string username, string password)
        {
            int count = (int)this.ExecuteScalar($"select count(*) from users where username = '{username}' and password = '{password}'");
            return count == 1;
        }

        public bool SendMessage(string msg)
        {
            bool ret = true;
            string qry = $"insert into chat values ('{msg}', getdate(), '{msg}'";
            ret = ExecuteNonQuery(msg);
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

                    for (int i = 0; i < length; i++)
                    {
                        line += reader[i]
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
