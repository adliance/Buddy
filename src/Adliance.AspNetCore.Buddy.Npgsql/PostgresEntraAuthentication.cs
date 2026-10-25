using System;
using System.Buffers.Text;
using System.Text.Json;
using System.Threading;
using Azure.Core;
using Azure.Identity;
using Npgsql;

namespace Adliance.AspNetCore.Buddy.Npgsql;

public static class PostgresEntraAuthentication
{
    private const string Scope = "https://ossrdbms-aad.database.windows.net/.default";

    // Managed identity tokens live for about a day and user tokens for about an hour. The credential
    // caches the token itself, so asking it this often costs nothing until one is actually about to expire.
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan FailedRefreshInterval = TimeSpan.FromSeconds(10);

    public static bool IsRequired(string connectionString)
        => string.IsNullOrWhiteSpace(new NpgsqlConnectionStringBuilder(connectionString).Password);

    public static void Configure(NpgsqlDataSourceBuilder builder)
    {
        Configure(builder, new DefaultAzureCredential());
    }

    public static void Configure(NpgsqlDataSourceBuilder builder, TokenCredential credential)
    {
        var tokenRequest = new TokenRequestContext([Scope]);

        // PostgreSQL still needs a role to log in as. Without one in the connection string, it is the
        // name the identity was added to the server under, which by default is its display name - and
        // for a managed identity that is the last segment of its resource id. This guess is wrong when
        // access is granted through a group.
        if (string.IsNullOrWhiteSpace(builder.ConnectionStringBuilder.Username))
        {
            builder.ConnectionStringBuilder.Username = UsernameFromToken(credential.GetToken(tokenRequest, CancellationToken.None).Token);
        }

        builder.UsePeriodicPasswordProvider(
            async (_, cancellationToken) => (await credential.GetTokenAsync(tokenRequest, cancellationToken)).Token,
            RefreshInterval,
            FailedRefreshInterval);
    }

    private static string UsernameFromToken(string token)
    {
        var parts = token.Split('.');
        if (parts.Length < 2)
        {
            throw new InvalidOperationException("The Entra access token for PostgreSQL is not a JWT.");
        }

        using var claims = JsonDocument.Parse(Base64Url.DecodeFromChars(parts[1]));

        // A managed identity: /subscriptions/.../userAssignedIdentities/{name}, or for a system-assigned
        // one the resource it belongs to, e.g. /subscriptions/.../sites/{app name}.
        if (claims.RootElement.TryGetProperty("xms_mirid", out var resourceId) && resourceId.GetString() is { Length: > 0 } id)
        {
            return id.TrimEnd('/').Split('/')[^1];
        }

        // A user, which is what a developer running the site locally signs in as.
        foreach (var claim in new[] { "upn", "preferred_username", "unique_name" })
        {
            if (claims.RootElement.TryGetProperty(claim, out var value) && value.GetString() is { Length: > 0 } name)
            {
                return name;
            }
        }

        throw new InvalidOperationException("Unable to derive a PostgreSQL username from the Entra access token. Add \"Username=...\" to the database connection string.");
    }
}
