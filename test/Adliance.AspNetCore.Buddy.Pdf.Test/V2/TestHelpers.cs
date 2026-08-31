using System;
using System.IO;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Pdf.Test.V2;

internal static class TestHelpers
{
    public static async Task StoreForInspection(byte[] bytes, string? name = null)
    {
        var downloadsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        if (Directory.Exists(downloadsDirectory))
        {
            var directory = Path.Combine(downloadsDirectory, "buddy-pdf-tests");
            Directory.CreateDirectory(directory);
            await File.WriteAllBytesAsync(Path.Combine(directory, (name ?? Guid.NewGuid().ToString()) + ".pdf"), bytes);
        }
    }
}
