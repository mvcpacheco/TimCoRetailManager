using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace TRMDataManager.Library.Internal.DataAccess
{
    internal class SqlDataAccess
    {
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

        public dynamic SaveData<T>(string storedProcedure, T parameters, string ConnectionStringName, string outputPareter, DbType dbType)
        {
            var connectionString = GetConnectionString(ConnectionStringName);

            var p = new DynamicParameters(parameters);
            p.Add(outputPareter, dbType: dbType, direction: ParameterDirection.Output);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute(storedProcedure, p, commandType: CommandType.StoredProcedure);
            }

            return p.Get<dynamic>(outputPareter);
        }
    }
}