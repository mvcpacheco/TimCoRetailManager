using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace TRMDataManager.Library.Internal.DataAccess
{
    internal class SqlDataAccess : IDisposable
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        public string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            var connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection cnn = new SqlConnection(connectionString))
            {
                return cnn.Query<T>(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            var connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Execute(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public dynamic SaveData<T>(string storedProcedure, T parameters, string connectionStringName, string outputPareter, DbType dbType)
        {
            var connectionString = GetConnectionString(connectionStringName);

            var p = new DynamicParameters(parameters);
            p.Add(outputPareter, dbType: dbType, direction: ParameterDirection.Output);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute(storedProcedure, p, commandType: CommandType.StoredProcedure);
            }

            return p.Get<dynamic>(outputPareter);
        }

        public void BeginTransaction(string connectionStringName)
        {
            var connectionString = GetConnectionString(connectionStringName);
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public void SaveDataInTransaction<T>(string storedProcedure, T parameters)
        {
            _connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction);
        }

        public dynamic SaveDataInTransaction<T>(string storedProcedure, T parameters, string outputPareter, DbType dbType)
        {
            var p = new DynamicParameters(parameters);
            p.Add(outputPareter, dbType: dbType, direction: ParameterDirection.Output);

            _connection.Execute(storedProcedure, p, commandType: CommandType.StoredProcedure, transaction: _transaction);

            return p.Get<dynamic>(outputPareter);
        }

        public void CommitTransaction()
        {
            _transaction?.Commit();
            _connection.Close();
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _connection.Close();
        }

        public void Dispose()
        {
            CommitTransaction();
        }
    }
}