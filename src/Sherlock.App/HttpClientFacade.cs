using System.Net;
using System.Text.RegularExpressions;
using Sherlock.App.Helpers;

namespace Sherlock.App;

/// <summary>
/// HttpClientFacade is responsible to facilitate the interaction between site abstraction and its actual site
/// through http protocol. It basicaly plays the role of detection engine based on request/response pattern.
/// There're some limitation in the current implementation of the Detection Engine that definately be able to improve in the future
/// Below is the list of limitation that can be improved in the future
/// 1. Only support HEAD & GET query
/// 2. Using static text matching to detect failures due to the lack of support HTTP Status Code of some particular sites
/// 3. Lack of supporting timeout. The detect engine will scan through al 358 sites
/// 4. Cannot tell if Bob on twitter and Bob on instagram is the same person.
/// </summary>
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

    /// <summary>
    /// Asynchronously submit a scan request to the nominated site for querying the existence of the nominated user
    /// </summary>
    /// <param name="username">A specific user whose existence need to be checked</param>
    /// <param name="site">A specific site where user's existence is concerned</param>
    /// <param name="debug">A true/false value to indicate debug-printing</param>
    /// <returns></returns>
    public async Task<Result> ExecuteAsync(string username, Site site, bool debug)
    {
        if (!string.IsNullOrWhiteSpace(site.RegexCheck) && !Regex.IsMatch(username, site.RegexCheck))
            return await Task.FromResult(Result.Empty);

        if (!string.IsNullOrWhiteSpace(site.Method) && site.Method != HttpMethod.Get.Method)
            return await Task.FromResult(Result.Empty);
        
        var endpoint = site.Url.IndexOf("@{}", StringComparison.Ordinal) != -1 
            ? site.Url.Replace("@{}", username) 
            : site.Url.Replace("{}", username);
        
        // If site uses HttpStatusCode to communicate then HEAD verb is good enough for scanning
        // However if side doesn't use HttpStatusCode we then need to use GET verb to get the entire of payload
        // So we can confirm the scanning result accordingly
        var response = site.ErrorType == Constant.ErrorType.ResponseUrl
            ? await _autoRedirectHttpClient.GetAsync(endpoint).ConfigureAwait(false)
            : await _httpClient.SendAsync(
                new HttpRequestMessage(
                    site.ErrorType == Constant.ErrorType.StatusCode ? HttpMethod.Head : HttpMethod.Get
                    , endpoint)).ConfigureAwait(false);

        // Not all site supports REST or uses HttpStatusCode to communicate the result but streaming out
        // the error message as part of response payload. Therefore, we always need to confirm the response behavior
        // of the site and in this specific context if ErrorType is Message it means, the given site doesn't use
        // HttpStatusCode for its communication
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