using Adliance.AspNetCore.Buddy.Testing.Shared;

namespace Adliance.AspNetCore.Buddy.Testing.v3.Test.Test.WithMockContainer;

// ReSharper disable once ClassNeverInstantiated.Global
public class WithMockContainerFixture<TOptions> : BuddyFixture<TOptions, Program> where TOptions : BuddyFixtureOptions<Program>, new();
