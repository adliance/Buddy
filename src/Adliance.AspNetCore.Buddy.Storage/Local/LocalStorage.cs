using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Storage.Local;

public class LocalStorage(IStorageConfiguration configuration) : IStorage
{
    /// <inheritdoc cref="IStorage.Save(byte[],bool,string[])"/>
    public async Task Save(byte[] bytes, bool overwrite, params string[] path)
    {
        var filePath = GetFilePath(path);
        var mode = GetFileMode(overwrite);
        await using var fileStream = File.Open(filePath, mode, FileAccess.ReadWrite);
        await fileStream.WriteAsync(bytes);
        await fileStream.FlushAsync();
        fileStream.Close();
    }

    /// <inheritdoc cref="IStorage.Save(System.IO.Stream,bool,string[])"/>
    public async Task Save(Stream stream, bool overwrite, params string[] path)
    {
        var filePath = GetFilePath(path);
        var mode = GetFileMode(overwrite);
        await using var fileStream = File.Open(filePath, mode, FileAccess.ReadWrite);
        await stream.CopyToAsync(fileStream);
        await fileStream.FlushAsync();
        fileStream.Close();
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
        if (await Exists(path)) File.Delete(GetFilePath(path));
    }

    /// <inheritdoc cref="IStorage.List" />
    public Task<IList<IStorageFile>> List(string path)
    {
        var directoryPath = Path.GetDirectoryName(GetFilePath(path, "some_filename_that_is_not_used.txt"));
        var directoryInfo = new DirectoryInfo(directoryPath!);
        var baseDirectoryInfo = new DirectoryInfo(configuration.LocalStorageBasePath ?? "");

        var result = new List<IStorageFile>();
        foreach (var f in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
        {
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
