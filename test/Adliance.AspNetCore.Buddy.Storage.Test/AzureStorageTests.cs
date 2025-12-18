using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Storage.Azure;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Storage.Test;

public class AzureStorageTest() : StorageTestBase(StorageType.Azure)
{
    [Fact]
    public async Task Can_Read_Containers_With_Certificate_Errors()
    {
        var options = new MockedStorageConfiguration
        {
            IgnoreCertificateErrors = true
        };
        var storage = new AzureStorage(options);
        var containers = await storage.ListContainers();
        Assert.NotEmpty(containers);
    }
}
