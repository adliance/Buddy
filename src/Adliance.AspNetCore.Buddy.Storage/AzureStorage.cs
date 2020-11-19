using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;
using Azure.Storage.Blobs;

namespace Adliance.AspNetCore.Buddy.Storage
{
    public class AzureStorage : IStorage
    {
        private readonly IStorageConfiguration _configuration;

        public AzureStorage(IStorageConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Save(byte[] bytes, params string[] path)
        {
            await using (var ms = new MemoryStream())
            {
                ms.Write(bytes);
                ms.Seek(0, SeekOrigin.Begin);
                await Save(ms, path);
            }
        }

        public async Task Save(Stream stream, params string[] path)
        {
            await GetBlobClient(path).UploadAsync(stream, true);
        }

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

        public async Task Load(Stream stream, params string[] path)
        {
            if (await Exists(path))
            {
                await GetBlobClient(path).DownloadToAsync(stream);
            }
        }

        public async Task<bool> Exists(params string[] path)
        {
            return await GetBlobClient(path).ExistsAsync();
        }

        public async Task Delete(params string[] path)
        {
            await GetBlobClient(path).DeleteIfExistsAsync();
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
}