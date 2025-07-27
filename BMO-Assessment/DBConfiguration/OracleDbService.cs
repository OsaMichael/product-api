using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace BMO_Assessment.DBConfiguration
{
    public class OracleDbService
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public OracleDbService(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("OracleConnection");
        }

        public IDbConnection CreateConnection()
        {
            return new OracleConnection(_connectionString);
        }
    }
}
