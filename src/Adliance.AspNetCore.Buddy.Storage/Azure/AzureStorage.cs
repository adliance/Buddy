using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace Adliance.AspNetCore.Buddy.Storage.Azure;

public class AzureStorage(IStorageConfiguration configuration) : IStorage
{
    /// <inheritdoc cref="IStorage.Save(byte[],bool,string[])"/>
    public async Task Save(byte[] bytes, bool overwrite = true, params string[] path)
    {
        await using var ms = new MemoryStream();
        ms.Write(bytes);
        ms.Seek(0, SeekOrigin.Begin);
        await Save(ms, overwrite, path);
    }

    /// <inheritdoc cref="IStorage.Save(System.IO.Stream,bool,string[])"/>
    public async Task Save(Stream stream, bool overwrite = true, params string[] path)
    {
        await GetBlobClient(path).UploadAsync(stream, overwrite);
    }

    /// <inheritdoc cref="IStorage.Load(string[])"/>
    public async Task<byte[]?> Load(params string[] path)
    {
        if (await Exists(path))
        {
            // ReSharper disable once ConvertToUsingDeclaration
            await using (var ms = new MemoryStream())
            {
                await Load(ms, path);
                return ms.ToArray();
            }
        }

        return null;
    }

    public async Task<DateTime?> GetModifiedDate(params string[] path)
    {
        if (await Exists(path)) return (await GetBlobClient(path).GetPropertiesAsync()).Value.LastModified.UtcDateTime;
        return null;
    }

    public async Task<DateTime?> GetCreatedDate(params string[] path)
    {
        if (await Exists(path)) return (await GetBlobClient(path).GetPropertiesAsync()).Value.CreatedOn.UtcDateTime;
        return null;
    }

    /// <inheritdoc cref="IStorage.Load(System.IO.Stream,string[])"/>
    public async Task Load(Stream stream, params string[] path)
    {
        if (await Exists(path))
        {
            await GetBlobClient(path).DownloadToAsync(stream);
        }
    }

    /// <inheritdoc cref="IStorage.Exists" />
    public async Task<bool> Exists(params string[] path)
    {
        return await GetBlobClient(path).ExistsAsync();
    }

    /// <inheritdoc cref="IStorage.Delete" />
    public async Task Delete(params string[] path)
    {
        await GetBlobClient(path).DeleteIfExistsAsync();
    }

    /// <inheritdoc cref="IStorage.List" />
    public async Task<IList<IStorageFile>> List(string path)
    {
        var containerClient = GetContainerClient(path);

        var result = new List<IStorageFile>();
        await foreach (var b in containerClient.GetBlobsAsync())
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

    /// <inheritdoc cref="IStorage.GetDownloadUrl" />
    public async Task<Uri?> GetDownloadUrl(string niceName, DateTimeOffset expiresOn, params string[] path)
    {
        if (string.IsNullOrWhiteSpace(configuration.AzureStorageConnectionString))
        {
            throw new Exception("A configured Azure Storage Connection String is required to build an SAS download URL.");
        }

        if (await Exists(path))
        {
            if (string.IsNullOrWhiteSpace(niceName))
            {
                niceName = "unknown";
            }

            niceName = WebUtility.UrlEncode(niceName);
            var blobClient = GetBlobClient(path);
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

    private BlobContainerClient GetContainerClient(params string[] path)
    {
        var container = path.First();

        BlobContainerClient? client;

        if (!string.IsNullOrWhiteSpace(configuration.AzureStorageUrl))
        {
            var url = configuration.AzureStorageUrl?.Trim('/');
            if (string.IsNullOrWhiteSpace(url)) throw new Exception("Azure Storage URL is not configured.");
            var credential = string.IsNullOrWhiteSpace(configuration.AzureStorageManagedIdentityClientId)
                ? new ManagedIdentityCredential()
                : new ManagedIdentityCredential(configuration.AzureStorageManagedIdentityClientId);
            client = new BlobContainerClient(new Uri($"{url}/{container}"), credential);
        }
        else if (!string.IsNullOrWhiteSpace(configuration.AzureStorageConnectionString))
        {
            client = new BlobContainerClient(configuration.AzureStorageConnectionString, container);
        }
        else
        {
            throw new Exception("Azure Storage connection information (either AzureStorageUrl for access via Entra-ID, or AzureStorageConnectionString) is not configured.");
        }

        if (configuration.AutomaticallyCreateDirectories && !client.Exists()) client.Create();
        return client;
    }

    private BlobClient GetBlobClient(params string[] path)
    {
        if (path.Length < 2) throw new Exception($"Path is not complete, it needs to consist of at least 2 parts, but only has {path.Length}.");
        var fileName = string.Join('/', path.Skip(1));
        return GetContainerClient(path).GetBlobClient(fileName);
    }
}
