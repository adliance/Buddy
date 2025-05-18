namespace Adliance.AspNetCore.Buddy.Testing.Test.Test.WithoutDatabase;

public class WithoutDatabaseFixture<TOptions> : BuddyFixture<TOptions, Program> where TOptions : BuddyFixtureOptions<Program>, new();
