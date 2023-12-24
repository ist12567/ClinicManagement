using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace ACDClinicManagement.AppHelpers.CommonHelpers
{
    public static class SqlHelper
    {
        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Variables

        private static string _errorMessage = "";

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region public static SqlConnection ConnectToSql (string Server, string Database, string UserId, string Password)

        /// <summary>
        /// Create new connection to SqlServer
        /// </summary>
        /// <param name="server">String, The name or network address of the instance of SQL Server to which to connect.\n The port number can be specified after the server name:</param>
        /// <param name="database">String, The name of the primary database file</param>
        /// <param name="userId">String, String, Specifies the user name. If not exists, "admin" is used by default</param>
        /// <param name="password">String, Specifies the user password. If not exists, ("") is used by default</param>
        /// <returns>SqlConnection</returns>
        public static SqlConnection ConnectToSql(string server, string database, string userId, string password)
        {
            var strCon = "SERVER=" + server +
                         ";DATABASE=" + database +
                         ";USER ID=" + userId +
                         ";PASSWORD=" + password;

            var connection = new SqlConnection(strCon);
            _errorMessage = _errorMessage + string.Empty;
            return connection;


        }

        #endregion

        #region public static SqlConnection LoadConnection(this SqlConnection connection)
        public static SqlConnection LoadConnection(this SqlConnection connection)
        {
            Thread.Sleep(100);
            if (connection.State != ConnectionState.Open) connection.Open();
            return connection;
        }

        #endregion

        #region public static bool IsSqlRecord (SqlConnection Connection, string TableName, string PrimaryKeyValue)

        /// <summary>
        /// Return True if requested primary key is exist
        /// </summary>
        /// <param name="connection">String, SqlServer connection name</param>
        /// <param name="tableName">String, Name of table for search</param>
        /// <param name="primaryKeyValue">String, Value of primary key for search</param>
        /// <returns>bool</returns>
        public static bool IsSqlRecord(SqlConnection connection, string tableName, string primaryKeyValue)
        {
            var ds = new DataSet();

            try
            {
                var primaryKey = GetSqlPrimaryKeys(connection, tableName);
                if (primaryKey == "")
                {
                    return false;
                }
                var strSqlSelect = "SELECT " + primaryKey + " FROM " + tableName + " WHERE " + primaryKey + "=" +
                                   primaryKeyValue;
                var da = new SqlDataAdapter(strSqlSelect, connection);
                da.Fill(ds, tableName);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    return true;
                }
            }
            catch (Exception exception)
            {
                _errorMessage = exception.Message;
            }

            return false;

        }

        #endregion

        #region public static string MaxSqlRecord (SqlConnection Connection,string TableName)

        /// <summary>
        /// Find maximum of primary key, For emplty tables return string '0'
        /// </summary>
        /// <param name="connection">SqlConnection, Connection's Name</param>
        /// <param name="tableName">String, Table's Name</param>
        /// <returns>string</returns>
        public static string MaxSqlRecord(SqlConnection connection, string tableName)
        {
            try
            {
                var primaryKey = GetSqlPrimaryKeys(connection, tableName);

                var cmd = new SqlCommand("SELECT MAX(" + primaryKey + ") FROM " + tableName, connection);
                var result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value) return 0.ToString();
                return result.ToString();
            }
            catch (Exception exception)
            {
                _errorMessage = exception.Message;
            }
            return null;
        }

        #endregion

        #region public static bool InsertSqlUnique (this SqlConnection Connection, string TableName, object[] ArrayofFieldsValue)

        /// <summary>
        /// Insert new record to table with UniqueIdentifier Primary Key
        /// </summary>
        /// <param name="connection">SqlConnection, Connection's Name</param>
        /// <param name="tableName">String, Table's Name</param>
        /// <param name="arrayofFieldsValue">String[], Array of Fields</param>
        /// <returns>bool</returns>
        public static bool InsertSqlUnique(this SqlConnection connection, string tableName, object[] arrayofFieldsValue)
        {
            const int nVarCharMax = int.MaxValue;
            var ds = new DataSet();

            try
            {
                var strSqlSelect = "SELECT TOP(1) * FROM " + tableName;
                var strSqlInsert = "INSERT INTO " + tableName + " (";
                var strParams = "";
                byte bytePos = 0;

                var da = new SqlDataAdapter(strSqlSelect, connection);
                da.Fill(ds, tableName);
                var dt = ds.Tables[0];

                DataColumn dc;
                for (var i = 1; i < dt.Columns.Count; i++)
                {
                    dc = dt.Columns[i];
                    strSqlInsert += dc.ColumnName + ",";
                    strParams += "@" + dt.Columns[i].ColumnName + ",";
                }
                strSqlInsert = strSqlInsert.Substring(0, strSqlInsert.Length - 1);
                strSqlInsert += ") VALUES (" + strParams.Substring(0, strParams.Length - 1) + ")";

                var cmd = new SqlCommand(strSqlInsert, connection);

                for (var i = 1; i < dt.Columns.Count; i++)
                {
                    dc = dt.Columns[i];
                    switch (dc.DataType.ToString())
                    {
                        //case "System.Guid":
                        //    cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.UniqueIdentifier, 16, dc.ColumnName);
                        //    break;
                        case "System.Guid":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.NVarChar, 255, dc.ColumnName);
                            break;
                        case "System.Int64":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.BigInt, 8, dc.ColumnName);
                            break;
                        case "System.Int32":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.Int, 4, dc.ColumnName);
                            break;
                        case "System.Int16":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.SmallInt, 2, dc.ColumnName);
                            break;
                        case "System.Byte":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.TinyInt, 1, dc.ColumnName);
                            break;
                        case "System.Byte[]":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.VarBinary, nVarCharMax, dc.ColumnName);
                            break;
                        case "System.Decimal":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.Decimal, 17, dc.ColumnName);
                            break;
                        case "System.Double":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.Float, 8, dc.ColumnName);
                            break;
                        case "System.Single":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.Real, 4, dc.ColumnName);
                            break;
                        case "System.Boolean":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.Bit, 1, dc.ColumnName);
                            break;
                        case "System.String":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.NVarChar, nVarCharMax, dc.ColumnName);
                            break;
                        case "System.TimeSpan":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.Time, 10, dc.ColumnName);
                            break;
                        case "System.DateTime":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.DateTime, 40, dc.ColumnName);
                            break;
                    }
                    cmd.Parameters[bytePos].Value = arrayofFieldsValue[bytePos];
                    bytePos++;
                }

                cmd.ExecuteNonQuery();
                return true;
            }
            catch(Exception exception)
            {
                _errorMessage = exception.Message;
                return false;
            }
        }

        #endregion

        #region public static bool InsertSqlData (this SqlConnection Connection, string TableName, object[] ArrayofFieldsValue)

        /// <summary>
        /// Insert new record to table
        /// </summary>
        /// <param name="connection">SqlConnection, Connection's Name</param>
        /// <param name="tableName">String, Table's Name</param>
        /// <param name="arrayofFieldsValue">String[], Array of Fields</param>
        /// <returns>bool</returns>
        public static bool InsertSqlData(this SqlConnection connection, string tableName, object[] arrayofFieldsValue)
        {
            const int nVarCharMax = int.MaxValue;

            try
            {
                var strSqlInsert = "INSERT INTO " + tableName + " (";
                var strParams = "";
                byte bytePos = 0;

                var dt = GetSqlDataTable(connection, tableName);
                if (dt == null)
                {
                    return false;
                }

                foreach (DataColumn dc in dt.Columns)
                {
                    strSqlInsert += dc.ColumnName + ",";
                    strParams += "@" + dc.ColumnName + ",";
                }
                strSqlInsert = strSqlInsert.Substring(0, strSqlInsert.Length - 1);
                strSqlInsert += ") VALUES (" + strParams.Substring(0, strParams.Length - 1) + ")";

                var cmd = new SqlCommand(strSqlInsert, connection);

                foreach (DataColumn dc in dt.Columns)
                {
                    switch (dc.DataType.ToString())
                    {
                        case "System.Int64":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.BigInt, 8, dc.ColumnName);
                            break;
                        case "System.Int32":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.Int, 4, dc.ColumnName);
                            break;
                        case "System.Int16":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.SmallInt, 2, dc.ColumnName);
                            break;
                        case "System.Byte":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.TinyInt, 1, dc.ColumnName);
                            break;
                        case "System.Byte[]":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.VarBinary, nVarCharMax, dc.ColumnName);
                            break;
                        case "System.Decimal":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.Decimal, 17, dc.ColumnName);
                            break;
                        case "System.Double":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.Float, 8, dc.ColumnName);
                            break;
                        case "System.Single":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.Real, 4, dc.ColumnName);
                            break;
                        case "System.Boolean":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.Bit, 1, dc.ColumnName);
                            break;
                        case "System.String":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.NVarChar, nVarCharMax, dc.ColumnName);
                            break;
                        case "System.TimeSpan":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.Time, 10, dc.ColumnName);
                            break;
                        case "System.DateTime":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.DateTime, 40, dc.ColumnName);
                            break;
                    }
                    cmd.Parameters[bytePos].Value = arrayofFieldsValue[bytePos];
                    bytePos++;
                }

                cmd.ExecuteNonQuery();
                return true;
            }
            catch(Exception exception)
            {
                _errorMessage = exception.Message;
                return false;
            }
        }

        #endregion

        #region public static bool UpdateSqlData (SqlConnection Connection, string TableName, string[] ArrayofFieldsValue)

        /// <summary>
        /// Update current record
        /// </summary>
        /// <param name="connection">SqlConnection, Connection's Name</param>
        /// <param name="tableName">String, Table's Name</param>
        /// <param name="arrayofFieldsValue">String[], Array of Fields</param>
        /// <returns>bool</returns>
        public static bool UpdateSqlData(SqlConnection connection, string tableName, string[] arrayofFieldsValue)
        {
            const int nVarCharMax = int.MaxValue;

            try
            {
                string primaryKey = GetSqlPrimaryKeys(connection, tableName);
                string strSqlUpdate = "UPDATE " + tableName + " SET ";
                byte bytePos = 0;

                DataTable dt = GetSqlDataTable(connection, tableName);
                if (dt == null)
                {
                    return false;
                }

                strSqlUpdate = dt.Columns.Cast<DataColumn>().Aggregate(strSqlUpdate, (current, dc) => current + dc.ColumnName + "=" + "@" + dc.ColumnName + ",");
                strSqlUpdate = strSqlUpdate.Substring(0, strSqlUpdate.Length - 1);
                strSqlUpdate += " WHERE " + primaryKey + "=" + arrayofFieldsValue[0];

                var cmd = new SqlCommand(strSqlUpdate, connection);

                foreach (DataColumn dc in dt.Columns)
                {
                    switch (dc.DataType.ToString())
                    {
                        case "System.Guid":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.NVarChar, 255, dc.ColumnName);
                            break;
                        case "System.Int64":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.BigInt, 8, dc.ColumnName);
                            break;
                        case "System.Int32":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.Int, 4, dc.ColumnName);
                            break;
                        case "System.Int16":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.SmallInt, 2, dc.ColumnName);
                            break;
                        case "System.Byte":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.TinyInt, 1, dc.ColumnName);
                            break;
                        case "System.Decimal":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.Decimal, 17, dc.ColumnName);
                            break;
                        case "System.Double":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.Float, 8, dc.ColumnName);
                            break;
                        case "System.Single":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.Real, 4, dc.ColumnName);
                            break;
                        case "System.Boolean":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.Bit, 1, dc.ColumnName);
                            break;
                        case "System.String":
                            cmd.Parameters.Add("@" + dc.ColumnName, SqlDbType.NVarChar, nVarCharMax, dc.ColumnName);
                            break;
                    }
                    cmd.Parameters[bytePos].Value = arrayofFieldsValue[bytePos];
                    bytePos++;
                }

                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region public static bool UpdateSqlData (this SqlConnection Connection, string TableName, string SetClause, string WhereCondition)

        /// <summary>
        /// Update current record
        /// </summary>
        /// <param name="connection">SqlConnection, Connection's Name</param>
        /// <param name="tableName">String, Table's Name</param>
        /// <param name="setClause">String, Setting new values, can't be null, without "SET" keyword</param>
        /// <param name="whereCondition">String, Filterring records, "null" for including all record, without "WHERE" keyword</param>
        /// <returns>bool</returns>
        public static bool UpdateSqlData(this SqlConnection connection, string tableName, string setClause,
                                  string whereCondition)
        {
            try
            {
                if (setClause == null)
                {
                    return false;
                }
                var strSqlUpdate = "UPDATE " + tableName + " SET " + setClause;
                if (whereCondition != null)
                {
                    strSqlUpdate += " WHERE " + whereCondition;
                }

                var cmd = new SqlCommand(strSqlUpdate, connection);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch(Exception exception)
            {
                _errorMessage = exception.Message;
                return false;
            }
        }

        #endregion

        #region public static bool DeleteSqlData (SqlConnection Connection, string TableName, string[] PrimaryKeyValue)

        /// <summary>
        /// Delete array of record(s)
        /// </summary>
        /// <param name="connection">SqlConnection, Connection's Name</param>
        /// <param name="tableName">String, Table's Name</param>
        /// <param name="primaryKeyValue">String[], Array of primary keys for removing from table</param>
        /// <returns>bool</returns>
        public static bool DeleteSqlData(SqlConnection connection, string tableName, string[] primaryKeyValue)
        {
            var strParams = "";

            try
            {
                var primaryKey = GetSqlPrimaryKeys(connection, tableName);
                var strSqlDelete = "DELETE FROM " + tableName + " WHERE " + primaryKey + " IN (";

                if (primaryKeyValue.Length == 0)
                {
                    return false;
                }
                strParams = primaryKeyValue.Aggregate(strParams, (current, t) => current + "'" + t.ToString(CultureInfo.InvariantCulture) + "',");
                strParams = strParams.Substring(0, strParams.Length - 1);

                strSqlDelete += strParams + ")";

                var cmd = new SqlCommand(strSqlDelete, connection);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region public static bool DeleteSqlData(this SqlConnection connection, string tableName, string condition)

        /// <summary>
        /// Delete requested record(s)
        /// </summary>
        /// <param name="connection">SqlConnection, Connection's Name</param>
        /// <param name="tableName">String, Table's Name</param>
        /// <param name="condition">String, Delete Condition, can be null, without "WHERE" keyword</param>
        /// <returns>bool</returns>
        public static bool DeleteSqlData(this SqlConnection connection, string tableName, string condition)
        {
            try
            {
                var strSqlDelete = "DELETE FROM " + tableName + " WHERE " + condition;

                var cmd = new SqlCommand(strSqlDelete, connection);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region private static DataTable GetSqlDataTable(SqlConnection Connection, string TableName)

        /// <summary>
        /// Get requsted table's scheme
        /// </summary>
        /// <param name="connection">SqlConnection, Connection's Name</param>
        /// <param name="tableName">String, Table's Name</param>
        /// <returns>DataTable</returns>
        private static DataTable GetSqlDataTable(SqlConnection connection, string tableName)
        {
            var ds = new DataSet();

            try
            {
                string strSqlSelect = "SELECT TOP(1) * FROM " + tableName;

                var da = new SqlDataAdapter(strSqlSelect, connection);
                da.Fill(ds, tableName);

                if (ds.Tables.Count != 0)
                {
                    var dt = ds.Tables[0];
                    return dt;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region private static string GetSqlPrimaryKeys(SqlConnection Connection, string TableName)

        /// <summary>
        /// Get primary key column's name of requested table
        /// </summary>
        /// <param name="connection">SqlConnection, Connection's Name</param>
        /// <param name="tableName">String, Table's Nam</param>
        /// <returns>String</returns>
        private static string GetSqlPrimaryKeys(SqlConnection connection, string tableName)
        {
            try
            {
                var strSqlQuery = "SELECT COLUMN_NAME" + //Query with database name
                                  " FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS cu" +
                                  " WHERE EXISTS" +
                                  " (SELECT CONSTRAINT_CATALOG, CONSTRAINT_SCHEMA, CONSTRAINT_NAME, TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, " +
                                  " CONSTRAINT_TYPE, IS_DEFERRABLE, INITIALLY_DEFERRED" +
                                  " FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc" +
                                  " WHERE (CONSTRAINT_CATALOG = '" + connection.Database + "')" +
                                  " AND (TABLE_NAME = '" + tableName + "')" +
                                  " AND (CONSTRAINT_TYPE = 'PRIMARY KEY')" +
                                  " AND (CONSTRAINT_NAME = cu.CONSTRAINT_NAME))";

                var cmd = new SqlCommand(strSqlQuery, connection);
                var pkColumn = cmd.ExecuteScalar().ToString();

                return pkColumn;

            }
            catch (Exception exception)
            {
                _errorMessage = exception.Message;
            }

            return null;

        }

        #endregion

        #region public static bool AnyActionInDatabaseByUser(this SqlConnection connection, int userId)
        public static bool AnyActionInDatabaseByUser(this SqlConnection connection, int userId)
        {
            var status = false;
            var dataAdapterTables = new SqlDataAdapter("SELECT TABLE_NAME " +
                                                       "FROM information_schema.tables " +
                                                       "WHERE TABLE_NAME <> 'Users'", connection);
            var dataTableTables = new DataTable();
            dataAdapterTables.Fill(dataTableTables);
            foreach (DataRow table in dataTableTables.Rows)
            {
                var commandCheck = new SqlCommand("SELECT COUNT(*) " +
                                                  "FROM " + table[0] + " " +
                                                  "WHERE CreatedBy = " + userId + " OR " +
                                                  "ModifiedBy = " + userId,
                    connection);
                if (Convert.ToInt32(commandCheck.ExecuteScalar()) > 0)
                    status = true;
            }
            return status;
        }

        #endregion

        #region public static DataTable GetDataTable(this SqlConnection connection, string commandString)
        public static DataTable GetDataTable(this SqlConnection connection, string commandString)
        {
            var dataAdapter = new SqlDataAdapter(commandString, connection);
            var dataTableResult = new DataTable();
            dataAdapter.Fill(dataTableResult);
            return dataTableResult;
        }

        #endregion

        #region public static DataTable ReplaceInDataTabe(this DataTable dataTable, string str1, string str2)
        public static DataTable ReplaceInDataTabe(this DataTable dataTable, string str1, string str2)
        {
            foreach (DataRow dataRow in dataTable.Rows)
            {
                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    if (dataRow[dataColumn].ToString() == str1)
                        dataRow[dataColumn] = str2;
                }
            }
            return dataTable;
        }

        #endregion

        #region public static DataTable SortDataTable(this DataTable dataTable, string colName, string direction)
        public static DataTable SortDataTable(this DataTable dataTable, string colName, string direction)
        {
            dataTable.DefaultView.Sort = colName + " " + direction;
            dataTable = dataTable.DefaultView.ToTable();
            return dataTable;
        }

        #endregion
    }
}
