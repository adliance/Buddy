using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace Adliance.AspNetCore.Buddy.Storage;

public class AzureStorage : IStorage
{
    private readonly IStorageConfiguration _configuration;

    public AzureStorage(IStorageConfiguration configuration)
    {
        _configuration = configuration;
    }

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
            await using (var ms = new MemoryStream())
            {
                await Load(ms, path);
                return ms.ToArray();
            }
        }

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

    /// <inheritdoc cref="IStorage.GetDownloadUrl" />
    public async Task<Uri?> GetDownloadUrl(string niceName, DateTimeOffset expiresOn, params string[] path)
    {
        if (await Exists(path))
        {
            if (string.IsNullOrWhiteSpace(niceName))
            {
                niceName = "unknown";
            }

            niceName = WebUtility.UrlEncode(niceName)!;
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

            var accountName = Regex.Match(_configuration.AzureStorageConnectionString, "AccountName=(.*?);", RegexOptions.IgnoreCase).Groups[1].Value;
            var accountKey = Regex.Match(_configuration.AzureStorageConnectionString, "AccountKey=(.*?);", RegexOptions.IgnoreCase).Groups[1].Value;
            var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(accountName, accountKey));
            return new Uri(blobClient.Uri + "?" + sasToken);
        }

        return null;
    }

    private BlobContainerClient GetContainerClient(params string[] path)
    {
        if (path.Length < 2)
        {
            throw new Exception($"Path is not complete, it needs to consist of at least 2 parts, but only has {path.Length}.");
        }

        var blobServiceClient = new BlobServiceClient(_configuration.AzureStorageConnectionString);

        var container = path.First();

        var client = blobServiceClient.GetBlobContainerClient(container);
        if (_configuration.AutomaticallyCreateDirectories && !client.Exists())
        {
            client.Create();
        }

        return client;
    }

    private BlobClient GetBlobClient(params string[] path)
    {
        if (path.Length < 2)
        {
            throw new Exception($"Path is not complete, it needs to consist of at least 2 parts, but only has {path.Length}.");
        }

        var fileName = string.Join('/', path.Skip(1));
        return GetContainerClient(path).GetBlobClient(fileName);
    }
}
