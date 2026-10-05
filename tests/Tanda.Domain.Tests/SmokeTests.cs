using FluentAssertions;
namespace Tanda.Domain.Tests;

public class SmokeTests
{
    [Fact]
    public void TestInfrastructure_IsWorking()
    {
        var result = 2 + 2;

        result.Should().Be(4);
    }
}