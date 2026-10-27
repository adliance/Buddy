using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace Adliance.AspNetCore.Buddy.Storage.Azure;

public class AzureStorage(IStorageConfiguration configuration) : IStorage
{
    /// <inheritdoc cref="IStorage.Save(byte[],bool,string[])"/>
    public Task Save(byte[] bytes, bool overwrite = true, params string[] path)
    {
        return Save(bytes, overwrite, path, CancellationToken.None);
    }

    /// <inheritdoc cref="IStorage.Save(byte[],bool,string[],CancellationToken)"/>
    public async Task Save(byte[] bytes, bool overwrite, string[] path, CancellationToken cancellationToken)
    {
        await using var ms = new MemoryStream();
        ms.Write(bytes);
        ms.Seek(0, SeekOrigin.Begin);
        await Save(ms, overwrite, path, cancellationToken);
    }

    /// <inheritdoc cref="IStorage.Save(System.IO.Stream,bool,string[])"/>
    public Task Save(Stream stream, bool overwrite = true, params string[] path)
    {
        return Save(stream, overwrite, path, CancellationToken.None);
    }

    /// <inheritdoc cref="IStorage.Save(System.IO.Stream,bool,string[],CancellationToken)"/>
    public async Task Save(Stream stream, bool overwrite, string[] path, CancellationToken cancellationToken)
    {
        await GetBlobClient(path, cancellationToken).UploadAsync(stream, overwrite, cancellationToken);
    }

    /// <inheritdoc cref="IStorage.Load(string[])"/>
    public Task<byte[]?> Load(params string[] path)
    {
        return Load(path, CancellationToken.None);
    }

    /// <inheritdoc cref="IStorage.Load(string[],CancellationToken)"/>
    public async Task<byte[]?> Load(string[] path, CancellationToken cancellationToken)
    {
        if (await Exists(path, cancellationToken))
        {
            // ReSharper disable once ConvertToUsingDeclaration
            await using (var ms = new MemoryStream())
            {
                await Load(ms, path, cancellationToken);
                return ms.ToArray();
            }
        }

        return null;
    }

    public Task<DateTime?> GetUpdatedUtc(params string[] path)
    {
        return GetUpdatedUtc(path, CancellationToken.None);
    }

    public async Task<DateTime?> GetUpdatedUtc(string[] path, CancellationToken cancellationToken)
    {
        if (await Exists(path, cancellationToken))
        {
            return (await GetBlobClient(path, cancellationToken).GetPropertiesAsync(cancellationToken: cancellationToken)).Value.LastModified.UtcDateTime;
        }

        return null;
    }

    public Task<DateTime?> GetCreatedUtc(params string[] path)
    {
        return GetCreatedUtc(path, CancellationToken.None);
    }

    public async Task<DateTime?> GetCreatedUtc(string[] path, CancellationToken cancellationToken)
    {
        if (await Exists(path, cancellationToken))
        {
            return (await GetBlobClient(path, cancellationToken).GetPropertiesAsync(cancellationToken: cancellationToken)).Value.CreatedOn.UtcDateTime;
        }

        return null;
    }

    /// <inheritdoc cref="IStorage.Load(System.IO.Stream,string[])"/>
    public Task Load(Stream stream, params string[] path)
    {
        return Load(stream, path, CancellationToken.None);
    }

    /// <inheritdoc cref="IStorage.Load(System.IO.Stream,string[],CancellationToken)"/>
    public async Task Load(Stream stream, string[] path, CancellationToken cancellationToken)
    {
        if (await Exists(path, cancellationToken))
        {
            await GetBlobClient(path, cancellationToken).DownloadToAsync(stream, cancellationToken);
        }
    }

    public Task<bool> Exists(params string[] path)
    {
        return Exists(path, CancellationToken.None);
    }

    public async Task<bool> Exists(string[] path, CancellationToken cancellationToken)
    {
        return await GetBlobClient(path, cancellationToken).ExistsAsync(cancellationToken);
    }

    public Task Delete(params string[] path)
    {
        return Delete(path, CancellationToken.None);
    }

    public async Task Delete(string[] path, CancellationToken cancellationToken)
    {
        await GetBlobClient(path, cancellationToken).DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    /// <inheritdoc cref="IStorage.List(string)" />
    public Task<IList<IStorageFile>> List(string path)
    {
        return List(path, CancellationToken.None);
    }

    /// <inheritdoc cref="IStorage.List(string,CancellationToken)" />
    public async Task<IList<IStorageFile>> List(string path, CancellationToken cancellationToken)
    {
        var containerClient = GetContainerClient(configuration, path, cancellationToken);

        var result = new List<IStorageFile>();
        await foreach (var b in containerClient.GetBlobsAsync(cancellationToken: cancellationToken))
        {
            var blobPath = path + "/" + b.Name;
            result.Add(new AzureStorageFile
            {
                Path = blobPath.Split('/'),
                UpdatedUtc = b.Properties.LastModified?.UtcDateTime ?? default,
                CreatedUtc = b.Properties.CreatedOn?.UtcDateTime ?? default,
                SizeBytes = b.Properties.ContentLength ?? 0
            });
        }

        return result;
    }

    public Task<IList<string>> ListContainers()
    {
        return ListContainers(CancellationToken.None);
    }

    public async Task<IList<string>> ListContainers(CancellationToken cancellationToken)
    {
        var serviceClient = GetServiceClient(configuration);
        IList<string> result = [];
        await foreach (var container in serviceClient.GetBlobContainersAsync(cancellationToken: cancellationToken))
        {
            result.Add(container.Name);
        }

        return result;
    }

    public Task<Uri?> GetDownloadUrl(string niceName, DateTimeOffset expiresOn, params string[] path)
    {
        return GetDownloadUrl(niceName, expiresOn, path, CancellationToken.None);
    }

    public async Task<Uri?> GetDownloadUrl(string niceName, DateTimeOffset expiresOn, string[] path, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(configuration.AzureStorageConnectionString))
        {
            throw new Exception("A configured Azure Storage Connection String is required to build an SAS download URL.");
        }

        if (await Exists(path, cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(niceName))
            {
                niceName = "unknown";
            }

            niceName = WebUtility.UrlEncode(niceName);
            var blobClient = GetBlobClient(path, cancellationToken);
            var sasBuilder = new BlobSasBuilder
            {
                StartsOn = DateTime.UtcNow.AddMinutes(-1),
                ExpiresOn = expiresOn,
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                ContentDisposition = "attachment;filename=\"" + niceName + "\""
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var accountName = Regex.Match(configuration.AzureStorageConnectionString, "AccountName=(.*?);", RegexOptions.IgnoreCase).Groups[1].Value;
            var accountKey = Regex.Match(configuration.AzureStorageConnectionString, "AccountKey=(.*?);", RegexOptions.IgnoreCase).Groups[1].Value;
            var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(accountName, accountKey));
            return new Uri(blobClient.Uri + "?" + sasToken);
        }

        return null;
    }

    private BlobClient GetBlobClient(string[] path, CancellationToken cancellationToken)
    {
        return GetBlobClient(configuration, path, cancellationToken);
    }

    public static BlobContainerClient GetContainerClient(IStorageConfiguration configuration, string container)
    {
        return GetContainerClient(configuration, container, CancellationToken.None);
    }

    public static BlobContainerClient GetContainerClient(IStorageConfiguration configuration, string container, CancellationToken cancellationToken)
    {
        var serviceClient = GetServiceClient(configuration);
        var containerClient = serviceClient.GetBlobContainerClient(container);
        if (configuration.AutomaticallyCreateDirectories && !containerClient.Exists(cancellationToken))
        {
            containerClient.Create(cancellationToken: cancellationToken);
        }

        return containerClient;
    }

    public static BlobServiceClient GetServiceClient(IStorageConfiguration configuration)
    {
        BlobServiceClient? client;

        var blobClientOptions = new BlobClientOptions();

        if (configuration.IgnoreCertificateErrors)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var transport = new HttpClientTransport(new HttpClient(handler));

            blobClientOptions = new BlobClientOptions
            {
                Transport = transport
            };
        }

        if (!string.IsNullOrWhiteSpace(configuration.AzureStorageUrl))
        {
            var url = configuration.AzureStorageUrl?.Trim('/');
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new Exception("Azure Storage URL is not configured.");
            }

            TokenCredential credential;

            if (configuration.UseDefaultAzureCredential == true)
            {
                credential = new DefaultAzureCredential();
            }
            else if (!string.IsNullOrWhiteSpace(configuration.UseManagedIdentityClientId))
            {
                credential = new ManagedIdentityCredential(ManagedIdentityId.FromUserAssignedClientId(configuration.UseManagedIdentityClientId));
            }
            else
            {
                credential = new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned);
            }

            client = new BlobServiceClient(new Uri($"{url}"), credential, blobClientOptions);
        }
        else if (!string.IsNullOrWhiteSpace(configuration.AzureStorageConnectionString))
        {
            client = new BlobServiceClient(configuration.AzureStorageConnectionString, blobClientOptions);
        }
        else
        {
            throw new Exception("Azure Storage connection information (either AzureStorageUrl for access via Entra-ID, or AzureStorageConnectionString) is not configured.");
        }

        return client;
    }

    public static BlobClient GetBlobClient(IStorageConfiguration configuration, params string[] path)
    {
        return GetBlobClient(configuration, path, CancellationToken.None);
    }

    public static BlobClient GetBlobClient(IStorageConfiguration configuration, string[] path, CancellationToken cancellationToken)
    {
        if (path.Length < 2)
        {
            throw new Exception($"Path is not complete, it needs to consist of at least 2 parts, but only has {path.Length}.");
        }

        var fileName = string.Join('/', path.Skip(1));
        return GetContainerClient(configuration, path.First(), cancellationToken).GetBlobClient(fileName);
    }
}
