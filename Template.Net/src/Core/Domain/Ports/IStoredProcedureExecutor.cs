using System.Data;
using Microsoft.Data.SqlClient;

namespace Template.Core.Domain.Ports;

public interface IStoredProcedureExecutor : IDisposable
{
    public Task<SqlDataReader> ExecuteSp(string storeProcedureName, ICollection<SqlParameter>? parameters = null);

    public SqlParameter CreateInputParameter(string parameterName, object value, SqlDbType dbType);
}