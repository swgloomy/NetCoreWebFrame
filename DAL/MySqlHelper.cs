using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Common;
using MySql.Data.MySqlClient;
namespace DAL
{
    public class MySqlHelper
    {
        // //Database connection strings
        // public static readonly string ConnectionStringLocalTransaction = ConfigurationManager.ConnectionStrings["SQLConnString1"].ConnectionString;
        // public static readonly string ConnectionStringInventoryDistributedTransaction = ConfigurationManager.ConnectionStrings["SQLConnString2"].ConnectionString;
        // public static readonly string ConnectionStringOrderDistributedTransaction = ConfigurationManager.ConnectionStrings["SQLConnString3"].ConnectionString;
		// public static readonly string ConnectionStringProfile = ConfigurationManager.ConnectionStrings["SQLProfileConnString"].ConnectionString;
        // Hashtable to store cached parameters
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// Execute a MySqlCommand (that returns no resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters) {

            MySqlCommand cmd = new MySqlCommand();            
            using (MySqlConnection conn = new MySqlConnection(connectionString)) {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                cmdText = cmdText.Trim();
                if (val>0&&cmdText.ToLower().IndexOf("select")==0)
                {
                    var selectCmd=new MySqlCommand("SELECT LAST_INSERT_ID()",conn);
                    val = Convert.ToInt32(selectCmd.ExecuteScalar());
                }
                return val;
            }
        }
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, Dictionary<string,object> commandParameters) {
            var dicArray = PrepareCommand(commandParameters, cmdText).SingleOrDefault();
            return ExecuteNonQuery(connectionString, cmdType, dicArray.Key, dicArray.Value);
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns no resultset) against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">an existing database connection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(MySqlConnection connection, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters) {

            MySqlCommand cmd = new MySqlCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns no resultset) using an existing SQL Transaction 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">an existing sql transaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(MySqlTransaction trans, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters) {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// Execute a MySqlCommand that returns a resultset against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  SqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a MySqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>A SqlDataReader containing the results</returns>
        public static List<T> ExecuteReader<T>(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters) {
            MySqlCommand cmd = new MySqlCommand();
            MySqlConnection conn = new MySqlConnection(connectionString);
            List<T> tlst = new List<T>();
            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    cmd.Parameters.Clear();
                    tlst = rdr.ConvertToList<T>();
                }              
                return tlst;
            }
            catch {
                conn.Close();
                throw;
            }
        }

        public static List<T> ExecuteReader<T>(string connectionString, CommandType cmdType, string cmdText, Dictionary<string,object> commandParameters)
        {
            var dicArray = PrepareCommand(commandParameters, cmdText).SingleOrDefault();
            return ExecuteReader<T>(connectionString, cmdType, dicArray.Key, dicArray.Value);
        }

        /// <summary>
        /// Execute a MySqlCommand that returns the first column of the first record against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a MySqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters) {
            MySqlCommand cmd = new MySqlCommand();

            using (MySqlConnection connection = new MySqlConnection(connectionString)) {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, Dictionary<string,object> commandParameters ) {
            var dicArray = PrepareCommand(commandParameters, cmdText).SingleOrDefault();
            return ExecuteScalar(connectionString, cmdType, dicArray.Key, dicArray.Value);
        }

        /// <summary>
        /// Execute a MySqlCommand that returns the first column of the first record against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">an existing database connection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(MySqlConnection connection, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters) {

            MySqlCommand cmd = new MySqlCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// add parameter array to the cache
        /// </summary>
        /// <param name="cacheKey">Key to the parameter cache</param>
        /// <param name="cmdParms">an array of SqlParamters to be cached</param>
        public static void CacheParameters(string cacheKey, params MySqlParameter[] commandParameters) {
            parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// Prepare a command for execution
        /// </summary>
        /// <param name="cmd">MySqlCommand object</param>
        /// <param name="conn">MySqlConnection object</param>
        /// <param name="trans">SqlTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">SqlParameters to use in the command</param>
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, CommandType cmdType, string cmdText, MySqlParameter[] cmdParms) {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null) {
                foreach (MySqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        private static Dictionary<string,MySqlParameter[]> PrepareCommand(Dictionary<string,object> cmdParms,string cmdText)
        {
            if (cmdParms==null)
            {
                return  new Dictionary<string, MySqlParameter[]>
                {
                    {cmdText,null}
                };
            }
            var parameterKey = string.Empty;
            var list=new List<MySqlParameter>();
            foreach (var item in cmdParms)
            {
                var paramsArray = new List<string>();
                switch (item.Value.GetType().Name)
                {
                        case "String[]":
                            var itemArray = (string[]) item.Value;
                            for (var index = 0; index < itemArray.Length; index++)
                            {
                                parameterKey = string.Format("{0}{1}", item.Key, index);
                                paramsArray.Add(parameterKey);
                                list.Add(new MySqlParameter(parameterKey,itemArray[index]));
                            }
                            cmdText = cmdText.Replace(item.Key, string.Join(",", paramsArray.ToArray()));
                            break;
                        case "Int32[]":
                            var intArray = (int[]) item.Value;
                            for (var index = 0; index < intArray.Length; index++)
                            {
                                parameterKey = string.Format("{0}{1}", item.Key, index);
                                paramsArray.Add(parameterKey);
                                list.Add(new MySqlParameter(parameterKey,intArray[index]));
                            }
                            cmdText = cmdText.Replace(item.Key, string.Join(",", paramsArray.ToArray()));
                            break;
                        case "Int64[]":
                            var longArray = (int[]) item.Value;
                            for (var index = 0; index < longArray.Length; index++)
                            {
                                parameterKey = string.Format("{0}{1}", item.Key, index);
                                paramsArray.Add(parameterKey);
                                list.Add(new MySqlParameter(parameterKey,longArray[index]));
                            }
                            cmdText = cmdText.Replace(item.Key, string.Join(",", paramsArray.ToArray()));
                            break;
                            default:
                                list.Add(new MySqlParameter(item.Key,item.Value));
                                break;
                }
            }

            return new Dictionary<string, MySqlParameter[]>
            {
                {cmdText,list.ToArray()}
            };
        }
    }

    public static class MySqlHelperExtends
    {
        public static List<T> ConvertToList<T>(this IDataReader datareader)
        {
            return DataReaderMapToList<T>(datareader);
        }
        public static List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties()) {
                    for (int index = 0; index < dr.FieldCount; index++)
                    {
                        if (dr.GetName(index)==prop.Name&&!object.Equals(dr[prop.Name], DBNull.Value)) {
                            prop.SetValue(obj, Convert.ChangeType(dr[prop.Name],prop.PropertyType) , null);
                        }
                    }
                }
                list.Add(obj);
            }
            return list;
        }

    }
}