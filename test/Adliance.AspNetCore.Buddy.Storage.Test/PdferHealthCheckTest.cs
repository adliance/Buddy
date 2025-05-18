using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Storage.Test;

public class StorageHealthCheckTest
{
    [Fact]
    public async Task Health_Check_Succeeds()
    {
        var check = new StorageHealthCheck(new LocalStorage(new MockedStorageConfiguration()));
        Assert.Equal(HealthStatus.Healthy, (await check.CheckHealthAsync(new HealthCheckContext())).Status);
    }
}
