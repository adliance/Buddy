using Adliance.AspNetCore.Buddy.Testing.Shared;

namespace Adliance.AspNetCore.Buddy.Testing.v3.Test.Test.WithoutDatabase;

public class WithoutDatabaseFixture<TOptions> : BuddyFixture<TOptions, Program> where TOptions : BuddyFixtureOptions<Program>, new();
