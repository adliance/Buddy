# Adliance.AspNetCore.Buddy.Npgsql

Adds PostgreSQL support to ASP.NET Core projects that use Entity Framework Core with
[Npgsql](https://www.npgsql.org/efcore/).

## Features

- [Passwordless authentication via Microsoft Entra ID](#entra-authentication) for Azure Database for PostgreSQL
- [Registration of a `DbContext`](#setup-library-in-an-aspnet-project) with sensible defaults in a single line
- [Applying pending migrations](#migrations) on startup
- [A health check](#health-check) for connectivity and pending migrations

## Setup library in an ASP.NET project

The package offers the `AddDatabase<T>` extensions to register a `DbContext` that uses PostgreSQL.

```c#
using Adliance.AspNetCore.Buddy.Npgsql.Extensions;

// ...

// reads the options from the "Database" section
builder.Services.AddDatabase<MyDbContext>(builder.Configuration);

// or from a specific section
builder.Services.AddDatabase<MyDbContext>(builder.Configuration.GetSection("MyDatabase"));

// or with a connection string or a PostgresDatabaseOptions instance
builder.Services.AddDatabase<MyDbContext>("Host=localhost;Database=mydb;Username=postgres;Password=secret");
```

### Configuration (appsettings.json)

```json
{
  "Database": {
    "ConnectionString": "Host=myserver.postgres.database.azure.com;Database=mydb",
    "EnableMigrations": true,
    "CommandTimeoutSeconds": 10,
    "MigrationCommandTimeoutSeconds": 600
  }
}
```

| Setting                          | Default | Description                                                                                                       |
|----------------------------------|---------|-------------------------------------------------------------------------------------------------------------------|
| `ConnectionString`               |         | The Npgsql connection string. Without a password, [Entra authentication](#entra-authentication) is used.          |
| `EnableMigrations`               | `true`  | Whether `PostgresMigration.Run` applies pending migrations.                                                       |
| `CommandTimeoutSeconds`          | `10`    | Timeout for regular commands. Kept short so that an unresponsive database does not hold requests open for long.   |
| `MigrationCommandTimeoutSeconds` | `600`   | Timeout for each command while migrations are applied.                                                            |

## Entra authentication

If the connection string contains **no password**, the library authenticates against PostgreSQL with a Microsoft Entra ID
access token instead. Tokens are obtained via `DefaultAzureCredential`, so this works with

- a managed identity (system- or user-assigned) when running in Azure, and
- the developer's own account (Visual Studio, Azure CLI, `azd`, ...) when running locally.

Tokens are refreshed periodically in the background, so connections keep working after a token has expired.

PostgreSQL still needs a role to log in as. If the connection string contains no `Username`, it is derived from the token:

- for a managed identity, it is the name of the identity (user-assigned) or of the resource it belongs to, e.g. the App
  Service (system-assigned),
- for a user, it is the user principal name, e.g. `jane.doe@example.com`.

This matches the default name an identity gets when it is added as an Entra administrator or role on the server. Set
`Username=...` explicitly when access is granted via a group or when the role was created with a different name.

To use Entra authentication with a data source you build yourself, or with a credential other than
`DefaultAzureCredential`, call `PostgresEntraAuthentication.Configure` directly:

```c#
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
if (PostgresEntraAuthentication.IsRequired(connectionString))
{
    PostgresEntraAuthentication.Configure(dataSourceBuilder, new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned));
}
await using var dataSource = dataSourceBuilder.Build();
```

## Migrations

`PostgresMigration.Run` applies all pending Entity Framework migrations, for example during startup. Failures are logged
as critical instead of thrown, so the application still starts and can report its state through the
[health check](#health-check).

```c#
builder.Services.AddDatabase<MyDbContext>(builder.Configuration);
builder.Services.Configure<PostgresDatabaseOptions>(builder.Configuration.GetSection("Database"));

var app = builder.Build();

await PostgresMigration.Run<MyDbContext, PostgresDatabaseOptions>(app.Services);
```

Migrations are skipped when `EnableMigrations` is `false` or no connection string is configured. If no logger is
passed, one is resolved from the service provider.

## Health check

`AddDatabaseCheck<T>` adds a health check for a `DbContext`. It fails when

- no connection string is configured,
- the database cannot be reached (including failed Entra authentication), or
- there are pending migrations, e.g. because applying them on startup failed. The names of the pending migrations are
  included in the result's data. Pass `checkPendingMigrations: false` to skip this.

```c#
using Adliance.AspNetCore.Buddy.Npgsql.Extensions;

// ...

builder.Services.AddHealthChecks().AddDatabaseCheck<MyDbContext>();

// optionally with a name, a failure status, tags and a timeout
builder.Services.AddHealthChecks().AddDatabaseCheck<MyDbContext>("database", HealthStatus.Degraded, ["ready"], TimeSpan.FromSeconds(5));

// ...

app.MapHealthChecks("/health");
```

The check is named after the `DbContext` type unless a name is given, and it reports `Unhealthy` unless a different
failure status is given.
