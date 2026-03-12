using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Storage;

/// <summary>
/// Specifies the contract for a storage provider.
/// </summary>
public interface IStorage
{
    /// <summary>
    /// Stores a blob file <paramref name="bytes"/> with a specified <paramref name="path"/>.
    /// </summary>
    /// <param name="bytes">The file content as byte array.</param>
    /// <param name="overwrite">Whether the upload should overwrite any files or throw if the file already exists. The default value is true.</param>
    /// <param name="path">The path of the file to store.</param>
    Task Save(byte[] bytes, bool overwrite = true, params string[] path);

    public async Task Save(byte[] bytes, bool overwrite, IStorageFile file)
    {
        await Save(bytes, overwrite, file.Path);
    }

    /// <summary>
    /// Stores a blob file <paramref name="stream"/> with a specified <paramref name="path"/>.
    /// </summary>
    /// <param name="stream">The file content as stream.</param>
    /// <param name="overwrite">Whether the upload should overwrite any files or throw if the file already exists. The default value is true.</param>
    /// <param name="path">The path of the file to store.</param>
    Task Save(Stream stream, bool overwrite = true, params string[] path);

    public async Task Save(Stream stream, bool overwrite, IStorageFile file)
    {
        await Save(stream, overwrite, file.Path);
    }

    /// <summary>
    /// Loads a blob file by a specified <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path of the file to load.</param>
    /// <returns>The loaded blob file.</returns>
    Task<byte[]?> Load(params string[] path);

    public async Task<byte[]?> Load(IStorageFile file)
    {
        return await Load(file.Path);
    }

    Task<DateTime?> GetUpdatedUtc(params string[] path);

    public async Task<DateTime?> GetUpdatedUtc(IStorageFile file)
    {
        return await GetUpdatedUtc(file.Path);
    }

    Task<DateTime?> GetCreatedUtc(params string[] path);

    public async Task<DateTime?> GetCreatedUtc(IStorageFile file)
    {
        return await GetCreatedUtc(file.Path);
    }

    /// <summary>
    /// Loads a blob file by a specified <paramref name="path"/>.
    /// </summary>
    /// <param name="stream">The stream for the file.</param>
    /// <param name="path">The path of the file to load.</param>
    Task Load(Stream stream, params string[] path);

    public async Task Load(Stream stream, IStorageFile file)
    {
        await Load(stream, file.Path);
    }

    /// <summary>
    /// Gets an URL for a file to access it for downloading.
    /// </summary>
    /// <param name="niceName">A nicer name for the file.</param>
    /// <param name="expiresOn">The time at which the shared access signature becomes invalid.
    /// This field must be omitted if it has been specified in an
    /// associated stored access policy.</param>
    /// <param name="path">The path of the file to load.</param>
    /// <returns>The URL of the file to download</returns>
    Task<Uri?> GetDownloadUrl(string niceName, DateTimeOffset expiresOn, params string[] path);

    public async Task<Uri?> GetDownloadUrl(string niceName, DateTimeOffset expiresOn, IStorageFile file file)
    {
        return await GetDownloadUrl(niceName, expiresOn, file.Path);
    }

    /// <summary>
    /// Determines whether the specified file exists.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <returns>The result is true if the file given by the specified path exists;
    /// otherwise, the result is false. Note that if path describes a directory, Exists will return true.</returns>
    Task<bool> Exists(params string[] path);

    public async Task<bool> Exists(IStorageFile file)
    {
        return await Exists(file.Path);
    }

    /// <summary>
    /// Deletes the specified file.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    Task Delete(params string[] path);

    public async Task Delete(IStorageFile file)
    {
        await Delete(file.Path);
    }

    /// <summary>
    /// Lists all file in the specified container/directory.
    /// </summary>
    /// <param name="path">Either the name of a container, or the name of a directory located directly inside your root directory.</param>
    /// <returns>A list of all files in the directory.</returns>
    Task<IList<IStorageFile>> List(string path);

    /// <summary>
    /// Lists all containers/directories on the root level of the storage.
    /// </summary>
    /// <returns>The name sof all containers/directories on the root level of the storage.</returns>
    Task<IList<string>> ListContainers();
}
