using System;
using System.IO;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Abstractions;

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

    /// <summary>
    /// Stores a blob file <paramref name="stream"/> with a specified <paramref name="path"/>.
    /// </summary>
    /// <param name="stream">The file content as stream.</param>
    /// <param name="overwrite">Whether the upload should overwrite any files or throw if the file already exists. The default value is true.</param>
    /// <param name="path">The path of the file to store.</param>
    Task Save(Stream stream, bool overwrite = true, params string[] path);

    /// <summary>
    /// Loads a blob file by a specified <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path of the file to load.</param>
    /// <returns>The loaded blob file.</returns>
    Task<byte[]?> Load(params string[] path);

    /// <summary>
    /// Loads a blob file by a specified <paramref name="path"/>.
    /// </summary>
    /// <param name="stream">The stream for the file.</param>
    /// <param name="path">The path of the file to load.</param>
    Task Load(Stream stream, params string[] path);

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

    /// <summary>
    /// Determines whether the specified file exists. 
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <returns>The result is true if the file given by the specified path exists;
    /// otherwise, the result is false. Note that if path describes a directory, Exists will return true.</returns>
    Task<bool> Exists(params string[] path);

    /// <summary>
    /// Deletes the specified file.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    Task Delete(params string[] path);
}
