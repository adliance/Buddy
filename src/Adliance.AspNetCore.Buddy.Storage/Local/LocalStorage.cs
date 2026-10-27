using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Storage.Local;

public class LocalStorage(IStorageConfiguration configuration) : IStorage
{
    /// <inheritdoc cref="IStorage.Save(byte[],bool,string[])"/>
    public Task Save(byte[] bytes, bool overwrite, params string[] path)
    {
        return Save(bytes, overwrite, path, CancellationToken.None);
    }

    /// <inheritdoc cref="IStorage.Save(byte[],bool,string[],CancellationToken)"/>
    public async Task Save(byte[] bytes, bool overwrite, string[] path, CancellationToken cancellationToken)
    {
        var filePath = GetFilePath(path);
        var mode = GetFileMode(overwrite);
        await using var fileStream = File.Open(filePath, mode, FileAccess.ReadWrite);
        await fileStream.WriteAsync(bytes, cancellationToken);
        await fileStream.FlushAsync(cancellationToken);
        fileStream.Close();
        File.SetLastWriteTimeUtc(filePath, DateTime.UtcNow);
    }

    /// <inheritdoc cref="IStorage.Save(System.IO.Stream,bool,string[])"/>
    public Task Save(Stream stream, bool overwrite, params string[] path)
    {
        return Save(stream, overwrite, path, CancellationToken.None);
    }

    /// <inheritdoc cref="IStorage.Save(System.IO.Stream,bool,string[],CancellationToken)"/>
    public async Task Save(Stream stream, bool overwrite, string[] path, CancellationToken cancellationToken)
    {
        var filePath = GetFilePath(path);
        var mode = GetFileMode(overwrite);
        await using var fileStream = File.Open(filePath, mode, FileAccess.ReadWrite);
        await stream.CopyToAsync(fileStream, cancellationToken);
        await fileStream.FlushAsync(cancellationToken);
        fileStream.Close();
        File.SetLastWriteTimeUtc(filePath, DateTime.UtcNow);
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
            return await File.ReadAllBytesAsync(GetFilePath(path), cancellationToken);
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
            return File.GetLastWriteTimeUtc(GetFilePath(path));
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
            return File.GetCreationTimeUtc(GetFilePath(path));
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
            var fileStream = File.OpenRead(GetFilePath(path));
            await fileStream.CopyToAsync(stream, cancellationToken);
            fileStream.Close();
        }
    }

    public Task<Uri?> GetDownloadUrl(string niceName, DateTimeOffset expiresOn, params string[] path)
    {
        return GetDownloadUrl(niceName, expiresOn, path, CancellationToken.None);
    }

    public async Task<Uri?> GetDownloadUrl(string niceName, DateTimeOffset expiresOn, string[] path, CancellationToken cancellationToken)
    {
        if (await Exists(path, cancellationToken))
        {
            return new Uri("file://" + GetFilePath(path));
        }

        return null;
    }

    public Task<bool> Exists(params string[] path)
    {
        return Exists(path, CancellationToken.None);
    }

    public Task<bool> Exists(string[] path, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(File.Exists(GetFilePath(path)));
    }

    public Task Delete(params string[] path)
    {
        return Delete(path, CancellationToken.None);
    }

    public async Task Delete(string[] path, CancellationToken cancellationToken)
    {
        if (await Exists(path, cancellationToken))
        {
            File.Delete(GetFilePath(path));
        }
    }

    /// <inheritdoc cref="IStorage.List(string)" />
    public Task<IList<IStorageFile>> List(string path)
    {
        return List(path, CancellationToken.None);
    }

    /// <inheritdoc cref="IStorage.List(string,CancellationToken)" />
    public Task<IList<IStorageFile>> List(string path, CancellationToken cancellationToken)
    {
        var directoryPath = Path.GetDirectoryName(GetFilePath(path, "some_filename_that_is_not_used.txt"));
        var directoryInfo = new DirectoryInfo(directoryPath!);
        var baseDirectoryInfo = new DirectoryInfo(configuration.LocalStorageBasePath ?? "");

        var result = new List<IStorageFile>();
        foreach (var f in directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filePath = f.FullName;
            filePath = filePath.Substring(baseDirectoryInfo.FullName.Length).Trim(Path.PathSeparator);

            result.Add(new LocalStorageFile
            {
                Path = filePath.Split('/', '\\'),
                UpdatedUtc = f.LastWriteTimeUtc,
                CreatedUtc = f.CreationTimeUtc,
                SizeBytes = f.Length
            });
        }

        result = result.OrderBy(x => x.ToString()).ToList();
        return Task.FromResult<IList<IStorageFile>>(result);
    }

    public Task<IList<string>> ListContainers()
    {
        return ListContainers(CancellationToken.None);
    }

    public async Task<IList<string>> ListContainers(CancellationToken cancellationToken)
    {
        var directoryInfo = new DirectoryInfo(GetFilePath());
        IList<string> result = [];
        foreach (var d in directoryInfo.EnumerateDirectories())
        {
            cancellationToken.ThrowIfCancellationRequested();
            result.Add(d.Name);
        }

        return await Task.FromResult(result);
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
