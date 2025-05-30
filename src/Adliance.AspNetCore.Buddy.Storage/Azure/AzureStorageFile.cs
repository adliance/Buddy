using System;

namespace Adliance.AspNetCore.Buddy.Storage.Azure;

public class AzureStorageFile : IStorageFile
{
    public required string[] Path { get; set; }
    public required DateTime UpdatedUtc { get; set; }
    public required DateTime CreatedUtc { get; set; }
    public required long SizeBytes { get; set; }
    public override string ToString() => string.Join("/", Path);
}
