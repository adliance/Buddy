using Adliance.AspNetCore.Buddy.Testing.InProcess;
using DotNet.Testcontainers.Builders;

namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithoutDatabase;

public class InProcessOptions : BuddyFixtureOptions<Program>
{
    public InProcessOptions()
    {
        InProcess = new InProcessOptions<Program>
        {
            ContentRoot = CommonDirectoryPath.GetProjectDirectory().DirectoryPath
        };
    }
}