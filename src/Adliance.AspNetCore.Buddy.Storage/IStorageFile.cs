using System;

namespace Adliance.AspNetCore.Buddy.Storage;

public interface IStorageFile
{
    public string[] Path { get; set; }
    public DateTime UpdatedUtc { get; set; }
    public DateTime CreatedUtc { get; set; }
    public long SizeBytes { get; set; }
}
