using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Storage.Azure;
using Adliance.AspNetCore.Buddy.Storage.Local;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Storage.Test;

public abstract class StorageTestBase
{
    private readonly IStorage _storage;

    protected StorageTestBase(StorageType type)
    {
        _storage = type switch
        {
            StorageType.Local => _storage = new LocalStorage(new MockedStorageConfiguration()),
            StorageType.Azure => _storage = new AzureStorage(new MockedStorageConfiguration()),
            _ => throw new Exception("Unsupported type.")
        };
    }

    [Fact]
    public async Task Can_Read_Write_Delete_Bytes()
    {
        var filePath = new[]
        {
            "directory",
            "another_directory",
            $"file-bytes-{Guid.NewGuid()}"
        };
        Assert.False(await _storage.Exists(filePath));
        Assert.Null(await _storage.Load(filePath));

        await _storage.Save([1, 2, 3], true, filePath);
        Assert.True(await _storage.Exists(filePath));
        var bytes = await _storage.Load(filePath);
        Assert.NotNull(bytes);
        Assert.Equal(3, bytes.Length);

        await Assert.ThrowsAnyAsync<Exception>(() => _storage.Save([4, 5, 6], false, filePath));

        await _storage.Save([4, 5], true, filePath);

        Assert.True(await _storage.Exists(filePath));
        var bytesOverwritten = await _storage.Load(filePath);
        Assert.NotNull(bytesOverwritten);
        Assert.Equal(2, bytesOverwritten.Length);

        var uri = await _storage.GetDownloadUrl("nice_name", DateTimeOffset.UtcNow.AddDays(1), filePath);
        Assert.NotNull(uri);
        Assert.True(uri.ToString().Length > 30);

        await _storage.Delete(filePath);
        Assert.False(await _storage.Exists(filePath));
        Assert.Null(await _storage.Load(filePath));
    }

    [Fact]
    public async Task Can_Read_Write_Delete_Stream()
    {
        var filePath = new[]
        {
            "directory",
            "another_directory",
            $"file-stream-{Guid.NewGuid()}"
        };
        Assert.False(await _storage.Exists(filePath));

        await using (var ms = new MemoryStream())
        {
            await _storage.Load(ms, filePath);
            Assert.Empty(ms.ToArray());
        }

        await using (var ms = new MemoryStream())
        {
            ms.Write([1, 2, 3]);
            ms.Seek(0, SeekOrigin.Begin);
            await _storage.Save(ms, true, filePath);
        }

        Assert.True(await _storage.Exists(filePath));

        await using (var ms = new MemoryStream())
        {
            await _storage.Load(ms, filePath);
            Assert.NotEmpty(ms.ToArray());
            Assert.Equal(3, ms.ToArray().Length);
        }

        await using (var ms = new MemoryStream())
        {
            ms.Write([4, 5]);
            ms.Seek(0, SeekOrigin.Begin);
            await Assert.ThrowsAnyAsync<Exception>(() => _storage.Save(ms, false, filePath));
        }

        await using (var ms = new MemoryStream())
        {
            ms.Write([4, 5]);
            ms.Seek(0, SeekOrigin.Begin);
            await _storage.Save(ms, true, filePath);
        }

        Assert.True(await _storage.Exists(filePath));

        await using (var ms = new MemoryStream())
        {
            await _storage.Load(ms, filePath);
            Assert.NotEmpty(ms.ToArray());
            Assert.Equal(2, ms.ToArray().Length);
        }

        var uri = await _storage.GetDownloadUrl("nice_name", DateTimeOffset.UtcNow.AddDays(1), filePath);
        Assert.NotNull(uri);
        Assert.True(uri.ToString().Length > 30);

        await _storage.Delete(filePath);
        Assert.False(await _storage.Exists(filePath));

        await using (var ms = new MemoryStream())
        {
            await _storage.Load(ms, filePath);
            Assert.Empty(ms.ToArray());
        }
    }

    [Fact]
    public async Task Can_Read_Write_Update_Delete_List()
    {
        var container = "container-" + Guid.NewGuid();
        Assert.Empty(await _storage.List(container));

        await _storage.Save([1, 2, 3], false, container, "file1.bin");
        await _storage.Save([1, 2, 3, 4], false, container, "file2.bin");
        await _storage.Save([1, 2, 3, 4, 5], false, container, "file3.bin");
        Assert.Equal(3, (await _storage.List(container)).Count);
        Thread.Sleep(1_000);
        await _storage.Save([1, 2, 3, 4, 5, 6, 7], true, container, "file3.bin");

        var files = await _storage.List(container);
        Assert.Equal(3, files.Count);
        var file1 = files.Single(x => x.Path[1] == "file1.bin");
        Assert.InRange(file1.CreatedUtc, file1.UpdatedUtc.AddMilliseconds(-100), file1.UpdatedUtc);
        Assert.Equal(3, file1.SizeBytes);
        var file3 = files.Single(x => x.Path[1] == "file3.bin");
        Assert.True(file3.UpdatedUtc > file3.CreatedUtc, $"{file3.UpdatedUtc} is not greater than {file3.CreatedUtc}, but should be.");
        Assert.Equal(container, file3.Path[0]);
        Assert.Equal("file3.bin", file3.Path[1]);
        Assert.Equal(7, file3.SizeBytes);

        await _storage.Delete(container, "file2.bin");
        Assert.Equal(2, (await _storage.List(container)).Count);

        await _storage.Delete(container, "file1.bin");
        await _storage.Delete(container, "file3.bin");
        Assert.Empty(await _storage.List(container));
    }
}
