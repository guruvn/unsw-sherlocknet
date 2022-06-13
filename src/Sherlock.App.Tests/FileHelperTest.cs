using System.Threading.Tasks;
using FluentAssertions;
using Sherlock.App.Helpers;
using Xunit;

namespace Sherlock.App.Tests;

public sealed class FileHelperTest
{
    [Theory]
    [InlineData("data.json")]
    public async Task DeserializeAsyncTest(string filePath)
    {
        var sites = await filePath.DeserializeAsync().ConfigureAwait(false);
        sites.Should().NotBeNull().And.HaveCount(358);
    }
}