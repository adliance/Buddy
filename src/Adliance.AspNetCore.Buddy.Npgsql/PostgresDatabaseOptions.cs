namespace Adliance.AspNetCore.Buddy.Npgsql;

public class PostgresDatabaseOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public bool IsConfigured => !string.IsNullOrWhiteSpace(ConnectionString);
    public bool EnableMigrations { get; set; } = true;

    /// <summary>
    /// The timeout for regular database commands. Kept short so that a database that has stopped answering
    /// does not hold every request open for the provider's default half-minute.
    /// </summary>
    public int CommandTimeoutSeconds { get; set; } = 10;

    /// <summary>
    /// The timeout for each command of a migration, which may rebuild indexes or move data around.
    /// </summary>
    public int MigrationCommandTimeoutSeconds { get; set; } = 600;
}
