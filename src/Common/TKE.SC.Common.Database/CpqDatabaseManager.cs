using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
//using Microsoft.Data.SqlClient;
//using Microsoft.Data;
using System.Data.SqlClient;
using TKE.SC.Common.Database.Helper;

namespace TKE.SC.Common.Database
{
    public class CpqDatabaseManager
    {
        private static string _connectionString;
        private static SqlConnection _connection;
        /// <summary>
        /// Constructor for Get Connection String
        /// </summary>
        private static string GetConnectionString()
        {
            if (_connectionString is null)
            {
                _connectionString = new ConfigurationBuilder().
                SetBasePath(System.IO.Directory.GetCurrentDirectory()).AddJsonFile(Constant.APPSETTINGS).Build().GetSection(Constant.PARAMSETTINGS)[Constant.TKEDBConnection];
            }

            return _connectionString;
        }

        private static SqlConnection OpenConnection()
        {
            var connection = new SqlConnection(GetConnectionString());
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Excecute the query using SQL command.ExcecuteQueries
        /// </summary>
        /// <param Name="procName"></param>
        /// <param Name="procParameters"></param>
        /// <returns></returns>
        public static int ExecuteQueries(string procName, SqlParameterCollection procParameters)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(procName, conn))
                {
                    return command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// excecute the query using sqlCommand.ExecuteScalar
        /// </summary>
        /// <param Name="procName"></param>
        /// <param Name="procParameters"></param>
        /// <returns></returns>
        public static string ExecuteScalarForReturnString(string procName, IList<SqlParameter> procParameters)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(procName, conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (var spd in procParameters)
                    {
                        command.Parameters.Add(spd);
                    }
                    return (string)command.ExecuteScalar();
                }
            }
        }

        public static int ExecuteScalar(string procName, IList<SqlParameter> procParameters)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(procName, conn))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    foreach (var spd in procParameters)
                    {
                        command.Parameters.Add(spd);
                    }
                    return (int)command.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// excecute the query using sqlCommand.Execute NonQuery
        /// </summary>
        /// <param Name="procName"></param>
        /// <param Name="procParameters"></param>
        /// <returns></returns>
        public static int ExecuteNonquery(string procName, IList<SqlParameter> procParameters, string outParam = "@Result")
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(procName, conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    return ExecuteNonquery(command, procParameters, outParam);
                }
            }
        }

        /// <summary>
        /// ExecuteNonquery
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="procParameters"></param>
        /// <param name="outParam"></param>
        /// <returns></returns>
        public static int ExecuteNonquery(SqlCommand command, IList<SqlParameter> procParameters, string outParam = "@Result")
        {
            foreach (var spd in procParameters)
            {
                command.Parameters.Add(spd);
            }
            var queryResult = command.ExecuteNonQuery();
            if (string.IsNullOrEmpty(outParam))
                return queryResult;
            else
                return Convert.ToInt32(command.Parameters[outParam].Value);
        }

        /// <summary>
        /// excecute the query using sqlCommand.ExecuteReader
        /// </summary>
        /// <param Name="procName"></param>
        /// <param Name="procParameters"></param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string procName, List<SqlParameter> procParameters)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(procName, conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    if (procParameters != null)
                    {
                        command.Parameters.AddRange(procParameters.ToArray());
                    }
                    return command.ExecuteReader();
                }
            }
        }

        /// <summary>
        /// excecute the query using sqlCommand.ExecuteDataSet
        /// </summary>
        /// <param Name="procName"></param>
        /// <param Name="procParameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string procName, IList<SqlParameter> procParameters)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(procName, conn))
                {
                   return  ExcecuteSP(command, procParameters);
                }
            }
        }

        public static DataSet ExcecuteSP(SqlCommand command, IList<SqlParameter> procParameters)
        {
            DataSet ds = new DataSet();
            command.CommandTimeout = 180;
            command.CommandType = CommandType.StoredProcedure;
            foreach (var spd in procParameters)
            {
                command.Parameters.Add(spd);
            }
            using (var adapter = new SqlDataAdapter(command))
            {
                adapter.Fill(ds);
                return ds;
            }
        }


        public static SqlConnection CreateSqlConnection()
        {
            return new SqlConnection(GetConnectionString());
        }

       


        /// <summary>
        /// excecute the query using sqlCommand.ExecuteDataSet
        /// </summary>
        /// <param Name="procName"></param>
        /// <param Name="procParameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string procName)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(procName, conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        return ds;
                    }
                }
            }
        }

        /// <summary>
        /// excecute the query using sqlCommand.Convert Datatable to convert DataTable to List
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="dt"></param>
        /// <returns></returns>
        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        /// <summary>
        /// excecute the query using sqlCommand.GetItem to get every Row of the table
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="dr"></param>
        /// <returns></returns>
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (System.Reflection.PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        if (dr[column.ColumnName] != DBNull.Value)
                        {
                            pro.SetValue(obj, dr[column.ColumnName], null);
                        }
                        else
                        {
                            pro.SetValue(obj, null, null);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return obj;
        }


        /// <summary>
        /// excecute the query using sqlCommand.ExecuteDataSet
        /// </summary>
        /// <param Name="procName"></param>
        /// <param Name="procParameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSetSample(string procName, IList<SqlParameter> procParameters, string actionType)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                SqlTransaction sqlTransaction;
                conn.Open();
                sqlTransaction = conn.BeginTransaction();
                DataSet ds = new DataSet();
                using (SqlCommand command = new SqlCommand(procName, conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (var spd in procParameters)
                    {
                        command.Parameters.Add(spd);
                    }
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(ds);
                        return ds;
                    }
                }
                //!string.IsNullOrEmpty(actionType)? actionType.ToUpper().Equals("COMMIT") ? sqlTransaction.Commit() : sqlTransaction.Rollback() : actionType ;
            }
        }

    }
}

