using System;
using System.IO;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Adliance.AspNetCore.Buddy.Storage;

public class LocalStorage(IStorageConfiguration configuration) : IStorage
{
    /// <inheritdoc cref="IStorage.Save(byte[],bool,string[])"/>
    public async Task Save(byte[] bytes, bool overwrite, params string[] path)
    {
        var filePath = GetFilePath(path);
        var mode = GetFileMode(overwrite);
        await using var fileStream = File.Open(filePath, mode, FileAccess.ReadWrite);
        await fileStream.WriteAsync(bytes);
    }

    /// <inheritdoc cref="IStorage.Save(System.IO.Stream,bool,string[])"/>
    public async Task Save(Stream stream, bool overwrite, params string[] path)
    {
        var filePath = GetFilePath(path);
        var mode = GetFileMode(overwrite);
        await using var fileStream = File.Open(filePath, mode, FileAccess.ReadWrite);
        await stream.CopyToAsync(fileStream);
    }

    /// <inheritdoc cref="IStorage.Load(string[])"/>
    public async Task<byte[]?> Load(params string[] path)
    {
        if (await Exists(path))
        {
            return await File.ReadAllBytesAsync(GetFilePath(path));
        }

        return null;
    }

    /// <inheritdoc cref="IStorage.Load(string[])"/>
    public async Task Load(Stream stream, params string[] path)
    {
        if (await Exists(path))
        {
            var fileStream = File.OpenRead(GetFilePath(path));
            await fileStream.CopyToAsync(stream);
            fileStream.Close();
        }
    }

    /// <inheritdoc cref="IStorage.Load(System.IO.Stream,string[])"/>
    public async Task<Uri?> GetDownloadUrl(string niceName, DateTimeOffset expiresOn, params string[] path)
    {
        if (await Exists(path))
        {
            return new Uri("file://" + GetFilePath(path));
        }

        return null;
    }

    /// <inheritdoc cref="IStorage.Exists"/>>
    public Task<bool> Exists(params string[] path)
    {
        return Task.FromResult(File.Exists(GetFilePath(path)));
    }

    /// <inheritdoc cref="IStorage.Delete" />
    public async Task Delete(params string[] path)
    {
        if (await Exists(path))
        {
            File.Delete(GetFilePath(path));
        }
    }

    private static FileMode GetFileMode(bool overwrite)
    {
        return overwrite ? FileMode.Create : FileMode.CreateNew;
    }

    private string GetFilePath(params string[] path)
    {
        var filePath = Path.Combine(path);
        filePath = Path.Combine(configuration.LocalStorageBasePath ?? "", filePath);

        if (configuration.AutomaticallyCreateDirectories)
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
