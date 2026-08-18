using Testcontainers.MsSql;
using Testcontainers.PostgreSql;

namespace Adliance.AspNetCore.Buddy.Testing.Shared.Database;

public class DatabaseResult : IAsyncDisposable
{
    public MsSqlContainer? MsSqlContainer { get; set; }
    public PostgreSqlContainer? PostgresContainer { get; set; }
    public string? DbConnectionStringInternal { get; set; }
    public string? DbConnectionStringExternal { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (MsSqlContainer != null)
        {
            await MsSqlContainer.DisposeAsync().ConfigureAwait(false);
        }

        if (PostgresContainer != null)
        {
            await PostgresContainer.DisposeAsync().ConfigureAwait(false);
        }
    }
}
