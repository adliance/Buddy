using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
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
    public Task Save(byte[] bytes, bool overwrite = true, params string[] path)
    {
        return Save(bytes, overwrite, path, CancellationToken.None);
    }

    /// <summary>
    /// Stores a blob file <paramref name="bytes"/> with a specified <paramref name="path"/>.
    /// </summary>
    /// <param name="bytes">The file content as byte array.</param>
    /// <param name="overwrite">Whether the upload should overwrite any files or throw if the file already exists.</param>
    /// <param name="path">The path of the file to store.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task Save(byte[] bytes, bool overwrite, string[] path, CancellationToken cancellationToken);

    public Task Save(byte[] bytes, bool overwrite, IStorageFile file)
    {
        return Save(bytes, overwrite, file.Path, CancellationToken.None);
    }

    public Task Save(byte[] bytes, bool overwrite, IStorageFile file, CancellationToken cancellationToken)
    {
        return Save(bytes, overwrite, file.Path, cancellationToken);
    }

    /// <summary>
    /// Stores a blob file <paramref name="stream"/> with a specified <paramref name="path"/>.
    /// </summary>
    /// <param name="stream">The file content as stream.</param>
    /// <param name="overwrite">Whether the upload should overwrite any files or throw if the file already exists. The default value is true.</param>
    /// <param name="path">The path of the file to store.</param>
    public Task Save(Stream stream, bool overwrite = true, params string[] path)
    {
        return Save(stream, overwrite, path, CancellationToken.None);
    }

    /// <summary>
    /// Stores a blob file <paramref name="stream"/> with a specified <paramref name="path"/>.
    /// </summary>
    /// <param name="stream">The file content as stream.</param>
    /// <param name="overwrite">Whether the upload should overwrite any files or throw if the file already exists.</param>
    /// <param name="path">The path of the file to store.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task Save(Stream stream, bool overwrite, string[] path, CancellationToken cancellationToken);

    public Task Save(Stream stream, bool overwrite, IStorageFile file)
    {
        return Save(stream, overwrite, file.Path, CancellationToken.None);
    }

    public Task Save(Stream stream, bool overwrite, IStorageFile file, CancellationToken cancellationToken)
    {
        return Save(stream, overwrite, file.Path, cancellationToken);
    }

    /// <summary>
    /// Loads a blob file by a specified <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path of the file to load.</param>
    /// <returns>The loaded blob file.</returns>
    public Task<byte[]?> Load(params string[] path)
    {
        return Load(path, CancellationToken.None);
    }

    /// <summary>
    /// Loads a blob file by a specified <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path of the file to load.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The loaded blob file.</returns>
    Task<byte[]?> Load(string[] path, CancellationToken cancellationToken);

    public Task<byte[]?> Load(IStorageFile file)
    {
        return Load(file.Path, CancellationToken.None);
    }

    public Task<byte[]?> Load(IStorageFile file, CancellationToken cancellationToken)
    {
        return Load(file.Path, cancellationToken);
    }

    public Task<DateTime?> GetUpdatedUtc(params string[] path)
    {
        return GetUpdatedUtc(path, CancellationToken.None);
    }

    Task<DateTime?> GetUpdatedUtc(string[] path, CancellationToken cancellationToken);

    public Task<DateTime?> GetUpdatedUtc(IStorageFile file)
    {
        return GetUpdatedUtc(file.Path, CancellationToken.None);
    }

    public Task<DateTime?> GetUpdatedUtc(IStorageFile file, CancellationToken cancellationToken)
    {
        return GetUpdatedUtc(file.Path, cancellationToken);
    }

    public Task<DateTime?> GetCreatedUtc(params string[] path)
    {
        return GetCreatedUtc(path, CancellationToken.None);
    }

    Task<DateTime?> GetCreatedUtc(string[] path, CancellationToken cancellationToken);

    public Task<DateTime?> GetCreatedUtc(IStorageFile file)
    {
        return GetCreatedUtc(file.Path, CancellationToken.None);
    }

    public Task<DateTime?> GetCreatedUtc(IStorageFile file, CancellationToken cancellationToken)
    {
        return GetCreatedUtc(file.Path, cancellationToken);
    }

    /// <summary>
    /// Loads a blob file by a specified <paramref name="path"/>.
    /// </summary>
    /// <param name="stream">The stream for the file.</param>
    /// <param name="path">The path of the file to load.</param>
    public Task Load(Stream stream, params string[] path)
    {
        return Load(stream, path, CancellationToken.None);
    }

    /// <summary>
    /// Loads a blob file by a specified <paramref name="path"/>.
    /// </summary>
    /// <param name="stream">The stream for the file.</param>
    /// <param name="path">The path of the file to load.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task Load(Stream stream, string[] path, CancellationToken cancellationToken);

    public Task Load(Stream stream, IStorageFile file)
    {
        return Load(stream, file.Path, CancellationToken.None);
    }

    public Task Load(Stream stream, IStorageFile file, CancellationToken cancellationToken)
    {
        return Load(stream, file.Path, cancellationToken);
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
    public Task<Uri?> GetDownloadUrl(string niceName, DateTimeOffset expiresOn, params string[] path)
    {
        return GetDownloadUrl(niceName, expiresOn, path, CancellationToken.None);
    }

    /// <summary>
    /// Gets an URL for a file to access it for downloading.
    /// </summary>
    /// <param name="niceName">A nicer name for the file.</param>
    /// <param name="expiresOn">The time at which the shared access signature becomes invalid.
    /// This field must be omitted if it has been specified in an
    /// associated stored access policy.</param>
    /// <param name="path">The path of the file to load.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The URL of the file to download</returns>
    Task<Uri?> GetDownloadUrl(string niceName, DateTimeOffset expiresOn, string[] path, CancellationToken cancellationToken);

    public Task<Uri?> GetDownloadUrl(string niceName, DateTimeOffset expiresOn, IStorageFile file)
    {
        return GetDownloadUrl(niceName, expiresOn, file.Path, CancellationToken.None);
    }

    public Task<Uri?> GetDownloadUrl(string niceName, DateTimeOffset expiresOn, IStorageFile file, CancellationToken cancellationToken)
    {
        return GetDownloadUrl(niceName, expiresOn, file.Path, cancellationToken);
    }

    /// <summary>
    /// Determines whether the specified file exists.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <returns>The result is true if the file given by the specified path exists;
    /// otherwise, the result is false. Note that if path describes a directory, Exists will return true.</returns>
    public Task<bool> Exists(params string[] path)
    {
        return Exists(path, CancellationToken.None);
    }

    /// <summary>
    /// Determines whether the specified file exists.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The result is true if the file given by the specified path exists;
    /// otherwise, the result is false. Note that if path describes a directory, Exists will return true.</returns>
    Task<bool> Exists(string[] path, CancellationToken cancellationToken);

    public Task<bool> Exists(IStorageFile file)
    {
        return Exists(file.Path, CancellationToken.None);
    }

    public Task<bool> Exists(IStorageFile file, CancellationToken cancellationToken)
    {
        return Exists(file.Path, cancellationToken);
    }

    /// <summary>
    /// Deletes the specified file.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    public Task Delete(params string[] path)
    {
        return Delete(path, CancellationToken.None);
    }

    /// <summary>
    /// Deletes the specified file.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task Delete(string[] path, CancellationToken cancellationToken);

    public Task Delete(IStorageFile file)
    {
        return Delete(file.Path, CancellationToken.None);
    }

    public Task Delete(IStorageFile file, CancellationToken cancellationToken)
    {
        return Delete(file.Path, cancellationToken);
    }

    /// <summary>
    /// Lists all file in the specified container/directory.
    /// </summary>
    /// <param name="path">Either the name of a container, or the name of a directory located directly inside your root directory.</param>
    /// <returns>A list of all files in the directory.</returns>
    public Task<IList<IStorageFile>> List(string path)
    {
        return List(path, CancellationToken.None);
    }

    /// <summary>
    /// Lists all file in the specified container/directory.
    /// </summary>
    /// <param name="path">Either the name of a container, or the name of a directory located directly inside your root directory.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of all files in the directory.</returns>
    Task<IList<IStorageFile>> List(string path, CancellationToken cancellationToken);

    /// <summary>
    /// Lists all containers/directories on the root level of the storage.
    /// </summary>
    /// <returns>The name sof all containers/directories on the root level of the storage.</returns>
    public Task<IList<string>> ListContainers()
    {
        return ListContainers(CancellationToken.None);
    }

    /// <summary>
    /// Lists all containers/directories on the root level of the storage.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The name sof all containers/directories on the root level of the storage.</returns>
    Task<IList<string>> ListContainers(CancellationToken cancellationToken);
}
