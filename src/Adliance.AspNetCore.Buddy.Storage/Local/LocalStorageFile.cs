using System;

namespace Adliance.AspNetCore.Buddy.Storage.Local;

public class LocalStorageFile : IStorageFile
{
    public required string[] Path { get; set; }
    public required DateTime UpdatedUtc { get; set; }
    public required DateTime CreatedUtc { get; set; }
    public required long SizeBytes { get; set; }

    public override string ToString() => string.Join(System.IO.Path.PathSeparator, Path);
}
