using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;



namespace BoatService
{
    public interface ISqlDBHelper
    {
        T Get<T>(string storedProcedureName, DynamicParameters parameters);
        Task<T> GetScalarAsync<T>(string storedProcedureName, DynamicParameters parameters);
        Task<T> GetAsync<T>(string storedProcedureName, DynamicParameters parameters);
        List<T> GetList<T>(string storedProcedureName, DynamicParameters parameters);
        List<T> GetQueryList<T>(string queryString);
        Task<List<T>> GetListAsync<T>(string storedProcedureName, DynamicParameters parameters);
        int Execute(string storedProcedureName, DynamicParameters parameters);
        List<object> QueryMultipleResult(string storedProcedureName, DynamicParameters parameters, params Func<GridReader, object>[] readerFuncs);
        List<object> QueryMultipleResultTVP(string storedProcedureName, object parameter, params Func<GridReader, object>[] readerFuncs);
      
    }

    public class SqlDBHelper : ISqlDBHelper
    {
        //SqlConnectionStringBuilder str_brle = new SqlConnectionStringBuilder
        //{
        //    ConnectionString = oneTimeCon,
        //    ColumnEncryptionSetting = SqlConnectionColumnEncryptionSetting.Enabled
        //};
        private string oneTimeCon = ConfigurationManager.ConnectionStrings["strConn"].ToString();
        public T Get<T>(string storedProcedureName, DynamicParameters parameters) //where T:class
        {

            var connection = new SqlConnection(oneTimeCon);
            connection.Open();
            T item = default(T);
            item = BuildQueryFirstOrDefault<T>(connection, storedProcedureName, parameters);
            connection.Close();
            return item;
        }

        public List<T> GetList<T>(string storedProcedureName, DynamicParameters parameters)
        {
            var connection = new SqlConnection(oneTimeCon);
            connection.Open();
            IEnumerable<T> query = null;
            query = BuildQuery<T>(connection, storedProcedureName, parameters);
            connection.Close();
            return query?.ToList();
        }

        public List<T> GetQueryList<T>(string queryString)
        {
            IDbConnection connection = new SqlConnection(oneTimeCon);
            connection.Open();
            var query = BuildSqlQuery<T>(connection, queryString);
            connection.Close();
            return query?.ToList();
        }

        async Task<List<T>> ISqlDBHelper.GetListAsync<T>(string storedProcedureName, DynamicParameters parameters)
        {
            using (IDbConnection connection = ReliableSqlConnection(oneTimeCon))
            {
                var query = await BuildQueryAsync<T>(connection, storedProcedureName, parameters);
                return query?.ToList();
            }
        }

        async Task<T> ISqlDBHelper.GetScalarAsync<T>(string storedProcedureName, DynamicParameters parameters)
        {
            using (IDbConnection connection = ReliableSqlConnection(oneTimeCon))
            {
                return await BuildExecuteScalarAsync<T>(connection, storedProcedureName, parameters);
            }
        }

        async Task<T> ISqlDBHelper.GetAsync<T>(string storedProcedureName, DynamicParameters parameters)
        {
            using (IDbConnection connection = ReliableSqlConnection(oneTimeCon))
            {
                return await BuildQueryFirstOrDefaultAsync<T>(connection, storedProcedureName, parameters);
            }
        }

        int ISqlDBHelper.Execute(string storedProcedureName, DynamicParameters parameters)
        {
            var connection = new SqlConnection(oneTimeCon);
            connection.Open();
            int response = ExecuteQuery(connection, storedProcedureName, parameters);
            connection.Close();
            return response;
        }

        async Task<T> BuildExecuteScalarAsync<T>(IDbConnection connection, string storedProcedureName, DynamicParameters parameters)
        {
            return await SqlMapper.ExecuteScalarAsync<T>(connection, storedProcedureName.ToString(), parameters, commandType: CommandType.StoredProcedure);
        }


        async Task<T> BuildQueryFirstOrDefaultAsync<T>(IDbConnection connection, string storedProcedureName, DynamicParameters parameters)
        {
            return await SqlMapper.QueryFirstOrDefaultAsync<T>(connection, storedProcedureName.ToString(), parameters, commandType: CommandType.StoredProcedure);
        }

        async Task<IEnumerable<T>> BuildQueryAsync<T>(IDbConnection connection, string storedProcedureName, DynamicParameters parameters)
        {
            return await SqlMapper.QueryAsync<T>(connection, storedProcedureName.ToString(), parameters, commandType: CommandType.StoredProcedure);
        }

        T BuildQueryFirstOrDefault<T>(IDbConnection connection, string storedProcedureName, DynamicParameters parameters)
        {

            return SqlMapper.QueryFirstOrDefault<T>(connection, storedProcedureName.ToString(), parameters, commandType: CommandType.StoredProcedure);
        }

        IEnumerable<T> BuildQuery<T>(IDbConnection connection, string storedProcedureName, DynamicParameters parameters)
        {
            return SqlMapper.Query<T>(connection, storedProcedureName.ToString(), parameters, commandType: CommandType.StoredProcedure);
        }

        int ExecuteQuery(IDbConnection connection, string storedProcedureName, DynamicParameters parameters)
        {
            return SqlMapper.Execute(cnn: connection, sql: storedProcedureName.ToString(), param: parameters, commandType: CommandType.StoredProcedure);
        }

        List<object> ISqlDBHelper.QueryMultipleResult(string storedProcedureName, DynamicParameters parameters, params Func<GridReader, object>[] readerFuncs)
        {
            var returnResults = new List<object>();

            using (IDbConnection connection = ReliableSqlConnection(oneTimeCon))
            {
                var query = connection.QueryMultiple(storedProcedureName.ToString(), param: parameters, commandTimeout: 5000, commandType: CommandType.StoredProcedure);

                foreach (var readerFunc in readerFuncs)
                {
                    var obj = readerFunc(query);
                    returnResults.Add(obj);
                }
                return returnResults;
            }
        }


        IEnumerable<T> BuildSqlQuery<T>(IDbConnection connection, string queryString)
        {
            return SqlMapper.Query<T>(connection, queryString, commandType: CommandType.Text);
        }

        IDbConnection ReliableSqlConnection(string connectionString)
        {
            //[TODO] Need add connection resilency logic
            //SqlConnectionStringBuilder strbldr = new SqlConnectionStringBuilder();
            //strbldr.ConnectionString = oneTimeCon;
            //strbldr.ColumnEncryptionSetting = SqlConnectionColumnEncryptionSetting.Enabled;
            var db = new SqlConnection(connectionString);
            db.Open();
            return db;
        }

        protected void Log(string storedProcedureName, DynamicParameters parameters, Action method)
        {
            var sw = new Stopwatch();
            var startTime = DateTimeOffset.Now;
            bool success = true;
            string message = "";
            try
            {
                sw.Start();
                method?.Invoke();
                sw.Stop();
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }
            finally
            {
                //LogSPAndParameters(storedProcedureName, parameters, startTime, sw.ElapsedMilliseconds, success, message);
            }
        }

        List<object> ISqlDBHelper.QueryMultipleResultTVP(string storedProcedureName, object parameter, params Func<GridReader, object>[] readerFuncs)
        {
            var returnResults = new List<object>();



            using (IDbConnection connection = ReliableSqlConnection(oneTimeCon))
            {
                var query = connection.QueryMultiple(storedProcedureName.ToString(), parameter, commandTimeout: 5000, commandType: CommandType.StoredProcedure);

                foreach (var readerFunc in readerFuncs)
                {
                    var obj = readerFunc(query);
                    returnResults.Add(obj);
                }
            }

            return returnResults;
        }


      
    }
}
