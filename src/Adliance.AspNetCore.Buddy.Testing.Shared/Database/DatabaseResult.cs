using Testcontainers.MsSql;

namespace Adliance.AspNetCore.Buddy.Testing.Shared.Database;

public class DatabaseResult : IAsyncDisposable
{
    public MsSqlContainer? Container { get; set; }
    public string? DbConnectionStringInternal { get; set; }
    public string? DbConnectionStringExternal { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (Container != null)
        {
            await Container.DisposeAsync().ConfigureAwait(false);
        }
    }
}
