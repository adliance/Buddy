﻿using System;
using System.IO;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Storage.Test
{
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
            var filePath = new[] {"directory", "another directory", $"file-bytes-{Guid.NewGuid()}"};
            Assert.False(await _storage.Exists(filePath));
            Assert.Null(await _storage.Load(filePath));

            await _storage.Save(new byte[] {1, 2, 3}, filePath);
            Assert.True(await _storage.Exists(filePath));
            var bytes = await _storage.Load(filePath);
            Assert.NotNull(bytes);
            Assert.Equal(3, bytes!.Length);

            await _storage.Delete(filePath);
            Assert.False(await _storage.Exists(filePath));
            Assert.Null(await _storage.Load(filePath));
        }

        [Fact]
        public async Task Can_Read_Write_Delete_Stream()
        {
            var filePath = new[] {"directory", "another directory", $"file-stream-{Guid.NewGuid()}"};
            Assert.False(await _storage.Exists(filePath));

            await using (var ms = new MemoryStream())
            {
                await _storage.Load(ms, filePath);
                Assert.Empty(ms.ToArray());
            }

            await using (var ms = new MemoryStream())
            {
                ms.Write(new byte[] {4, 5, 6, 7});
                ms.Seek(0, SeekOrigin.Begin);
                await _storage.Save(ms, filePath);
            }

            Assert.True(await _storage.Exists(filePath));

            await using (var ms = new MemoryStream())
            {
                await _storage.Load(ms, filePath);
                Assert.NotEmpty(ms.ToArray());
                Assert.Equal(4, ms.ToArray().Length);
            }

            await _storage.Delete(filePath);
            Assert.False(await _storage.Exists(filePath));

            await using (var ms = new MemoryStream())
            {
                await _storage.Load(ms, filePath);
                Assert.Empty(ms.ToArray());
            }
        }
    }
}