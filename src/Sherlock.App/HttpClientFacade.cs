using System.Net;
using System.Text.RegularExpressions;
using Sherlock.App.Helpers;

namespace Sherlock.App;

public sealed class HttpClientFacade
{
    private readonly HttpClient _httpClient;
    private readonly HttpClient _autoRedirectHttpClient;

    HttpClientFacade()
    {   
        _httpClient = new HttpClient(new HttpClientHandler {
            AllowAutoRedirect = false
        });
        
        _httpClient.DefaultRequestHeaders.Add(
            "User-Agent",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.12; rv:55.0) Gecko/20100101 Firefox/55.0");
        
        _autoRedirectHttpClient = new HttpClient(new HttpClientHandler {
            AllowAutoRedirect = true
        });
        
        _autoRedirectHttpClient.DefaultRequestHeaders.Add(
            "User-Agent",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.12; rv:55.0) Gecko/20100101 Firefox/55.0");
    }

    public async Task<Result> ExecuteAsync(string username, Site site, bool debug)
    {
        if (!string.IsNullOrWhiteSpace(site.RegexCheck) && !Regex.IsMatch(username, site.RegexCheck))
            return await Task.FromResult(Result.Empty);

        if (!string.IsNullOrWhiteSpace(site.Method) && site.Method != HttpMethod.Get.Method)
            return await Task.FromResult(Result.Empty);
        
        var endpoint = site.Url.IndexOf("@{}", StringComparison.Ordinal) != -1 
            ? site.Url.Replace("@{}", username) 
            : site.Url.Replace("{}", username);
        
        var response = site.ErrorType == Constant.ErrorType.ResponseUrl
            ? await _autoRedirectHttpClient.GetAsync(endpoint).ConfigureAwait(false)
            : await _httpClient.SendAsync(
                new HttpRequestMessage(
                    site.ErrorType == Constant.ErrorType.StatusCode ? HttpMethod.Head : HttpMethod.Get
                    , endpoint)).ConfigureAwait(false);

        if (site.ErrorType == Constant.ErrorType.Message)
        {
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            content.DumpFor(debug);
            
            var notFound = site.ErrorMessage is string
                ? content.IndexOf(site.ErrorMessage, StringComparison.InvariantCultureIgnoreCase) != -1
                : (site.ErrorMessage as string[])!.All(s =>
                    content.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) != -1);
            
            return Result.Create(site.Name, endpoint, notFound ? HttpStatusCode.NotFound : HttpStatusCode.OK);
        }
            
        return Result.Create(site.Name, endpoint, (int)response.StatusCode is >= 200 and < 300 ? HttpStatusCode.OK : HttpStatusCode.NotFound);
    }

    public static HttpClientFacade Create() => new();
}