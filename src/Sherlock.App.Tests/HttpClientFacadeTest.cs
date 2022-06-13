using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Sherlock.App.Tests;

public sealed class HttpClientFacadeTest
{
    private readonly Site _site;
    public HttpClientFacadeTest()
    {
        _site = new Site {
            Name = "eBay.com",
            ErrorMessage = "The user ID you entered was not found. Please check the User Id and try again.",
            ErrorType = "message",
            Url = "https://www.ebay.com/usr/{}",
            MainUrl = "https://www.ebay.com/",
            UserNameClaimed = "blue",
            UserNameUnclaimed = "noonewouldeverusethis7",
            RegexCheck = null
        };
    }

    [Theory]
    [InlineData("naqekae", HttpStatusCode.NotFound)]
    [InlineData("nguyen.nguyen", HttpStatusCode.OK)]
    public async Task ExecuteAsyncTest(string username, HttpStatusCode code)
    {
        var client = HttpClientFacade.Create();
        var response = await client.ExecuteAsync(username, _site, false).ConfigureAwait(false);
        response.Should()
            .NotBeNull()
            .And.Match<Result>(r 
                => r.Name == _site.Name 
                   && r.Url == _site.Url!.Replace("{}", username)
                   && r.StatusCode == code);
    }
}