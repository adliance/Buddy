using System;
using System.IO;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Storage
{
    public class LocalStorage : IStorage
    {
        private readonly IStorageConfiguration _configuration;

        public LocalStorage(IStorageConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Save(byte[] bytes, params string[] path)
        {
            await File.WriteAllBytesAsync(GetFilePath(path), bytes);
        }

        public async Task Save(Stream stream, params string[] path)
        {
            await using (var fileStream = File.OpenWrite(GetFilePath(path)))
            {
                await stream.CopyToAsync(fileStream);
            }
        }

        public async Task<byte[]?> Load(params string[] path)
        {
            if (await Exists(path))
            {
                return await File.ReadAllBytesAsync(GetFilePath(path));
            }

            return null;
        }

        public async Task Load(Stream stream, params string[] path)
        {
            if (await Exists(path))
            {
                var fileStream = File.OpenRead(GetFilePath(path));
                await fileStream.CopyToAsync(stream);
                fileStream.Close();
            }
        }

        public Task<bool> Exists(params string[] path)
        {
            return Task.FromResult(File.Exists(GetFilePath(path)));
        }

        public async Task Delete(params string[] path)
        {
            if (await Exists(path))
            {
                File.Delete(GetFilePath(path));
            }
        }

        private string GetFilePath(params string[] path)
        {
            var filePath = Path.Combine(path);
            filePath = Path.Combine(_configuration.LocalStorageBasePath, filePath);

            if (_configuration.AutomaticallyCreateDirectories)
            {
                var directoryPath = Path.GetDirectoryName(filePath)!;
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }

            return filePath;
        }
    }
}