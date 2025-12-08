namespace Adliance.AspNetCore.Buddy.Testing.Shared;

public static class BuddyEnvironment
{
    public static bool IsRunningInAgent => System.Environment.GetEnvironmentVariable("TF_BUILD") != null;
}
