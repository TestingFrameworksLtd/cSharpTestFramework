using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UITests.Utilities
{
    public class Database
    {
         // Local instance
        private static Database instance = null;
        //private string _connectionString = ConfigurationManager.AppSettings.Get("DbConnectionString");
        
        /*******************************
         * Get singleton instance method
         *******************************/
        public static Database Instance
        {
            get
            {
                // If null instance, return new
                if (instance == null)
                {
                    instance = new Database();
                }

                // Return new or ecisting instance
                return instance;
            }
        }

        private string GetConnectionString()
        {
            if (ConfigurationManager.AppSettings.Get("Env").Equals("beta"))
            {
              return  ConfigurationManager.AppSettings.Get("BetaDbConnectionString");
            } else if (ConfigurationManager.AppSettings.Get("Env").Equals("live"))
            {
              return  ConfigurationManager.AppSettings.Get("LiveDbConnectionString");
            }
            return null;

        }
        
        /**
         * Run query 
         * */
     
        public long GetIdFromCustomer(int ssoGlobalId)
        {
          
           // return (from ot in cscwebEntities.Customers where ot.SsoGlobalId == ssoGlobalId select ot.Id).FirstOrDefault<long>();
            return 5;
        }

        public void ExcecuteNonQuery(string query)
        {
            string connectionString = GetConnectionString();
            // Create connection and open
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            // Create command
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();            
            conn.Close();
        }

        public void GetTableRow(string strQuery)
        {
            // Get connection string
            string connectionString = GetConnectionString();
            // Create connection and open
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            // Create command
            SqlCommand cmd = new SqlCommand(strQuery, conn);
            SqlDataReader r = cmd.ExecuteReader();
            r.Read();
            conn.Close();
        }

        /**
         * Get data set based on connection string and query
         * */
        public DataSet GetDataSet(string connectionString, string strQuery)
        {
            // Create connection and open
            var conn = new SqlConnection(connectionString);
            conn.Open();

            // Create command
            var cmd = new SqlCommand(strQuery, conn);

            // Create adapter
            var sda = new SqlDataAdapter(cmd);

            // Create new data set and fill it
            DataSet ds = new DataSet();
            sda.Fill(ds);

            // Close connection
            conn.Close();

            // Return data set
            return ds;
        }
    }
}
