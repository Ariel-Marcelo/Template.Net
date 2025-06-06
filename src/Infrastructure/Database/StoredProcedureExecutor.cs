using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Template.Shared.Infrastructure.Settings;
using Template.Core.Domain.Ports;

namespace Template.Infrastructure.Database;

public class StoredProcedureExecutor : IStoredProcedureExecutor, IAsyncDisposable
{
    private readonly string _connectionString;
    private readonly ILogger<StoredProcedureExecutor> _logger;
    private SqlConnection? _connection;
    private bool _disposed;

    public StoredProcedureExecutor(IOptions<DatabaseSettings> settings, ILogger<StoredProcedureExecutor> logger)
    {
        _logger = logger;
        _connectionString = settings.Value.SqlConnection;
        _logger.LogInformation("Connection string initialized: {ConnectionStringEmpty}", !string.IsNullOrEmpty(_connectionString));
    }

    public async Task<SqlDataReader> ExecuteSp(string storeProcedureName, ICollection<SqlParameter>? parameters = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrEmpty(_connectionString))
        {
            _logger.LogError("Connection string is empty or null");
            throw new InvalidOperationException("Database connection string is not configured");
        }

        _connection = new SqlConnection(_connectionString);
        await _connection.OpenAsync();

        var command = new SqlCommand(storeProcedureName, _connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        if (parameters?.Any() == true)
        {
            command.Parameters.AddRange(parameters.ToArray());
        }

        return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
    }

    // Helper methods for common parameter types
    public SqlParameter CreateInputParameter(string parameterName, object value, SqlDbType dbType)
    {
        return new SqlParameter
        {
            ParameterName = parameterName.StartsWith("@") ? parameterName : $"@{parameterName}",
            Value = value ?? DBNull.Value,
            Direction = ParameterDirection.Input,
            SqlDbType = dbType
        };
    }

    public static SqlParameter CreateOutputParameter(string parameterName, SqlDbType dbType)
    {
        return new SqlParameter
        {
            ParameterName = parameterName.StartsWith("@") ? parameterName : $"@{parameterName}",
            Direction = ParameterDirection.Output,
            SqlDbType = dbType
        };
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(StoredProcedureExecutor));
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            if (_connection != null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _connection?.Dispose();
            _connection = null;
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}