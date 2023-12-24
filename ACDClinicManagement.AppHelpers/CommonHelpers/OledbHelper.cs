using System;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;

namespace ACDClinicManagement.AppHelpers.CommonHelpers
{
    public static class OledbHelper
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

        #region public static OleDbConnection ConnectToOleDb(string database, string userId, string password)

        /// <summary>
        /// Create new connection to OleDb
        /// </summary>
        /// <param name="database">String, Specifies the database path and file name</param>
        /// <param name="userId">String, Specifies the user name. If not exists, "admin" is used by default</param>
        /// <param name="password">String, Specifies the user password. If not exists, ("") is used by default</param>
        /// <returns>OleDbConnection</returns>
        public static OleDbConnection ConnectToOleDb(string database, string userId, string password)
        {
            var strCon = "PROVIDER=Microsoft.Jet.OLEDB.4.0" +
                         ";DATA SOURCE=" + database +
                         ";USER ID='" + userId + "'" +
                         ";PASSWORD='" + password + "'";

            var connection = new OleDbConnection(strCon);
            _errorMessage = _errorMessage + string.Empty;
            return connection;

        }

        #endregion

        #region public static bool IsOleDbRecord(OleDbConnection connection, string tableName, string primaryKeyValue)

        /// <summary>
        /// Return True if requested Primary key is exist
        /// </summary>
        /// <param name="connection">String, SqlServer connection name</param>
        /// <param name="tableName">String, Name of table for search</param>
        /// <param name="primaryKeyValue">String, Value of primary key for search</param>
        /// <returns>bool</returns>
        public static bool IsOleDbRecord(OleDbConnection connection, string tableName, string primaryKeyValue)
        {
            var ds = new DataSet();

            try
            {
                var primaryKey = GetOleDbPrimaryKeys(connection, tableName);
                if (primaryKey == null)
                {
                    return false;
                }
                var strOleSelect = "SELECT " + primaryKey + " FROM " + tableName + " WHERE " + primaryKey + "=" +
                                   primaryKeyValue;
                var da = new OleDbDataAdapter(strOleSelect, connection);
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

        #region public static string MaxOleDbRecord (OleDbConnection Connection,string TableName)

        /// <summary>
        /// Find maximum of Primary Key, For emplty tables return string '0'
        /// </summary>
        /// <param name="connection">OleDbConnection, Connection's Name</param>
        /// <param name="tableName">String, Table's Name</param>
        /// <returns>string</returns>
        public static string MaxOleDbRecord(OleDbConnection connection, string tableName)
        {
            try
            {
                var primaryKey = GetOleDbPrimaryKeys(connection, tableName);

                var cmd = new OleDbCommand("SELECT MAX(" + primaryKey + ") FROM " + tableName, connection);
                return cmd.ExecuteScalar().ToString();
            }
            catch (Exception exception)
            {
                _errorMessage = exception.Message;
            }

            return null;

        }

        #endregion

        #region public static bool InsertOleDbData (OleDbConnection Connection, string TableName, string[] ArrayofFieldsValue)

        /// <summary>
        /// Insert new record to table
        /// </summary>
        /// <param name="connection">OleDbConnection, Connection's Name</param>
        /// <param name="tableName">String, Table's Name</param>
        /// <param name="arrayofFieldsValue">String[], Array of fields</param>
        /// <returns>bool</returns>
        public static bool InsertOleDbData(OleDbConnection connection, string tableName, string[] arrayofFieldsValue)
        {
            const int nVarCharMax = int.MaxValue;

            try
            {
                var strOleInsert = "INSERT INTO " + tableName + " (";
                var strParams = "";
                byte bytePos = 0;

                var dt = GetOleDbDataTable(connection, tableName);
                if (dt == null)
                {
                    return false;
                }

                foreach (DataColumn dc in dt.Columns)
                {
                    strOleInsert += dc.ColumnName + ",";
                    strParams += "@" + dc.ColumnName + ",";
                }
                strOleInsert = strOleInsert.Substring(0, strOleInsert.Length - 1);
                strOleInsert += ") VALUES (" + strParams.Substring(0, strParams.Length - 1) + ")";

                var cmd = new OleDbCommand(strOleInsert, connection);

                foreach (DataColumn dc in dt.Columns)
                {
                    switch (dc.DataType.ToString())
                    {
                        case "System.Int64":
                            cmd.Parameters.Add("@" + dc.ColumnName, OleDbType.BigInt, 8, dc.ColumnName);
                            break;
                        case "System.Int32":
                            cmd.Parameters.Add("@" + dc.ColumnName, OleDbType.Integer, 4, dc.ColumnName);
                            break;
                        case "System.Int16":
                            cmd.Parameters.Add("@" + dc.ColumnName, OleDbType.SmallInt, 2, dc.ColumnName);
                            break;
                        case "System.Byte":
                            cmd.Parameters.Add("@" + dc.ColumnName, OleDbType.TinyInt, 1, dc.ColumnName);
                            break;
                        case "System.Decimal":
                            cmd.Parameters.Add("@" + dc.ColumnName, OleDbType.Decimal, 17, dc.ColumnName);
                            break;
                        case "System.Double":
                            cmd.Parameters.Add("@" + dc.ColumnName, OleDbType.Double, 8, dc.ColumnName);
                            break;
                        case "System.Single":
                            cmd.Parameters.Add("@" + dc.ColumnName, OleDbType.Single, 4, dc.ColumnName);
                            break;
                        case "System.Boolean":
                            cmd.Parameters.Add("@" + dc.ColumnName, OleDbType.Boolean, 1, dc.ColumnName);
                            break;
                        case "System.String":
                            cmd.Parameters.Add("@" + dc.ColumnName, OleDbType.VarWChar, nVarCharMax, dc.ColumnName);
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

        #region public static bool UpdateOleDbData (OleDbConnection Connection, string TableName, string[] ArrayofFieldsValue)

        /// <summary>
        /// Update current record
        /// </summary>
        /// <param name="connection">OleDbConnection, Connection's Name</param>
        /// <param name="tableName">String, Table's Name</param>
        /// <param name="arrayofFieldsValue">String[], Array of Fields</param>
        /// <returns>bool</returns>
        public static bool UpdateOleDbData(OleDbConnection connection, string tableName, string[] arrayofFieldsValue)
        {
            const int nVarCharMax = int.MaxValue;

            try
            {
                var primaryKey = GetOleDbPrimaryKeys(connection, tableName);
                var strOleUpdate = "UPDATE " + tableName + " SET ";
                byte bytePos = 0;

                var dt = GetOleDbDataTable(connection, tableName);
                if (dt == null)
                {
                    return false;
                }

                strOleUpdate = dt.Columns.Cast<DataColumn>()
                    .Aggregate(strOleUpdate,
                        (current, dc) => current + dc.ColumnName + "=" + "@" + dc.ColumnName + ",");
                strOleUpdate = strOleUpdate.Substring(0, strOleUpdate.Length - 1);
                strOleUpdate += " WHERE " + primaryKey + "=" + arrayofFieldsValue[0];

                var cmd = new OleDbCommand(strOleUpdate, connection);

                foreach (DataColumn dc in dt.Columns)
                {
                    switch (dc.DataType.ToString())
                    {
                        case "System.Int64":
                            cmd.Parameters.Add("@" + dc.ColumnName, OleDbType.BigInt, 8, dc.ColumnName);
                            break;
                        case "System.Int32":
                            cmd.Parameters.Add("@" + dc.ColumnName, OleDbType.Integer, 4, dc.ColumnName);
                            break;
                        case "System.Int16":
                            cmd.Parameters.Add("@" + dc.ColumnName, OleDbType.SmallInt, 2, dc.ColumnName);
                            break;
                        case "System.Byte":
                            cmd.Parameters.Add("@" + dc.ColumnName, OleDbType.TinyInt, 1, dc.ColumnName);
                            break;
                        case "System.Decimal":
                            cmd.Parameters.Add("@" + dc.ColumnName, OleDbType.Decimal, 17, dc.ColumnName);
                            break;
                        case "System.Double":
                            cmd.Parameters.Add("@" + dc.ColumnName, OleDbType.Double, 8, dc.ColumnName);
                            break;
                        case "System.Single":
                            cmd.Parameters.Add("@" + dc.ColumnName, OleDbType.Single, 4, dc.ColumnName);
                            break;
                        case "System.Boolean":
                            cmd.Parameters.Add("@" + dc.ColumnName, OleDbType.Boolean, 1, dc.ColumnName);
                            break;
                        case "System.String":
                            cmd.Parameters.Add("@" + dc.ColumnName, OleDbType.VarWChar, nVarCharMax, dc.ColumnName);
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

        #region public static bool UpdateOleDbData (OleDbConnection Connection, string TableName, string SetClause, string WhereCondition)

        /// <summary>
        /// Update current record
        /// </summary>
        /// <param name="connection">OleDbConnection, Connection's Name</param>
        /// <param name="tableName">String, Table's Name</param>
        /// <param name="setClause">String, Setting new values, can't be null, without "SET" keyword</param>
        /// <param name="whereCondition">String, Filterring records, "null" for including all record, without "WHERE" keyword</param>
        /// <returns>bool</returns>
        public static bool UpdateOleDbData(OleDbConnection connection, string tableName, string setClause,
            string whereCondition)
        {
            try
            {
                if (setClause == null)
                {
                    return false;
                }
                var strOleUpdate = "UPDATE " + tableName + " SET " + setClause;
                if (whereCondition != null)
                {
                    strOleUpdate += " WHERE " + whereCondition;
                }

                var cmd = new OleDbCommand(strOleUpdate, connection);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region public static bool DeleteOleDbData (OleDbConnection Connection, string TableName, string[] PrimaryKeyValue)

        /// <summary>
        /// Delete array of record(s)
        /// </summary>
        /// <param name="connection">OleDbConnection, Connection's Name</param>
        /// <param name="tableName">String, Table's Name</param>
        /// <param name="primaryKeyValue">String[], Array of primary keys for removing from table</param>
        /// <returns>bool</returns>
        public static bool DeleteOleDbData(OleDbConnection connection, string tableName, string[] primaryKeyValue)
        {
            string strParams = "";

            try
            {
                var primaryKey = GetOleDbPrimaryKeys(connection, tableName);
                var strOleDelete = "DELETE FROM " + tableName + " WHERE " + primaryKey + " IN (";

                if (primaryKeyValue.Length == 0)
                {
                    return false;
                }
                strParams = primaryKeyValue.Aggregate(strParams,
                    (current, t) => current + "'" + t.ToString(CultureInfo.InvariantCulture) + "',");
                strParams = strParams.Substring(0, strParams.Length - 1);

                strOleDelete += strParams + ")";

                var cmd = new OleDbCommand(strOleDelete, connection);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region public static bool DeleteOleDbData (OleDbConnection Connection,string TableName,string PrimaryKey, string Condition)

        /// <summary>
        /// Delete requested record(s)
        /// </summary>
        /// <param name="connection">OleDbConnection, Connection's Name</param>
        /// <param name="tableName">String, Table's Name</param>
        /// <param name="condition">String, Delete Condition, can be null, without "WHERE" keyword</param>
        /// <returns>bool</returns>
        public static bool DeleteOleDbData(OleDbConnection connection, string tableName, string condition)
        {
            try
            {
                string strOleDelete = "DELETE FROM " + tableName + " WHERE " + condition;

                var cmd = new OleDbCommand(strOleDelete, connection);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region private static DataTable GetOleDbDataTable(OleDbConnection Connection, string TableName)

        /// <summary>
        /// Get requsted table's scheme
        /// </summary>
        /// <param name="connection">OleDbConnection, Connection's Name</param>
        /// <param name="tableName">String, Table's Name</param>
        /// <returns>DataTable</returns>
        private static DataTable GetOleDbDataTable(OleDbConnection connection, string tableName)
        {
            var ds = new DataSet();

            try
            {
                var strOleSelect = "SELECT TOP(1) FROM " + tableName;

                var da = new OleDbDataAdapter(strOleSelect, connection);
                da.Fill(ds, tableName);

                if (ds.Tables.Count == 0)
                {
                    return null;
                }
                var dt = ds.Tables[0];
                return dt;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region private static string GetOleDbPrimaryKeys(OleDbConnection Connection, string TableName)

        /// <summary>
        /// Get primary key column's name of requested table
        /// </summary>
        /// <param name="connection">OleDbConnection, Connection's Name</param>
        /// <param name="tableName">String, Table's Nam</param>
        /// <returns>String</returns>
        private static string GetOleDbPrimaryKeys(OleDbConnection connection, string tableName)
        {
            try
            {
                var strOleQuery = "SELECT COLUMN_NAME" + //Query with database name
                                  " FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS cu" +
                                  " WHERE EXISTS" +
                                  " (SELECT CONSTRAINT_CATALOG, CONSTRAINT_SCHEMA, CONSTRAINT_NAME, TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, " +
                                  " CONSTRAINT_TYPE, IS_DEFERRABLE, INITIALLY_DEFERRED" +
                                  " FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc" +
                                  " WHERE (CONSTRAINT_CATALOG = '" + connection.Database + "')" +
                                  " AND (TABLE_NAME = '" + tableName + "')" +
                                  " AND (CONSTRAINT_TYPE = 'PRIMARY KEY')" +
                                  " AND (CONSTRAINT_NAME = cu.CONSTRAINT_NAME))";

                var cmd = new OleDbCommand(strOleQuery, connection);
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
    }
}
