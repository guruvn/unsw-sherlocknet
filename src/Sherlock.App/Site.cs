using System.Collections.Immutable;
using System.Net;
using Newtonsoft.Json;
using Sherlock.App.Helpers;
#pragma warning disable CS8618

namespace Sherlock.App;

/// <summary>
/// Site encapsulates all important information about the actual social site that we need to
/// establish connection and to check for the user's existence
/// </summary>
public sealed record Site
{
    public string Name { get; init; }
    public string? ErrorType { get; init; }
    [JsonProperty(PropertyName = "errorMsg")]
    public dynamic ErrorMessage { get; init; }
    public string Url { get; init; }
    [JsonProperty(PropertyName = "urlMain")]
    public string? MainUrl { get; init; }
    [JsonProperty(PropertyName = "urlProbe")]
    public string? ProbeUrl { get; init; }
    [JsonProperty(PropertyName = "username_claimed")]
    public string? UserNameClaimed { get; init; }
    [JsonProperty(PropertyName = "username_unclaimed")]
    public string? UserNameUnclaimed { get; init; }
    [JsonProperty(PropertyName = "request_method")]
    public string? Method { get; init; }
    [JsonProperty(PropertyName = "request_payload")]
    public dynamic? Payload { get; init; }
    public string? RegexCheck { get; init; }

    /// <summary>
    /// Check the existence of the nominated users
    /// </summary>
    /// <param name="client">A http client that uses for sending http request</param>
    /// <param name="usernames">The nominated list of username whose existence need to be checked</param>
    /// <param name="debug">A true/false value to indicate debug-printing</param>
    public async Task HuntAsync(HttpClientFacade client, IImmutableList<string> usernames, bool debug)
    {
        $"{nameof(Site)}::HuntAsync(...) => \n[User]: {JsonConvert.SerializeObject(usernames)} \n[Site]: {JsonConvert.SerializeObject(this)}".DumpFor(debug);
        
        try
        {
            foreach (var username in usernames)
            {
                var response = await client.ExecuteAsync(username, this, debug).ConfigureAwait(false);

                if (response.Name == "NULL")
                    continue;

                if (response.StatusCode == HttpStatusCode.NotFound)
                    $"{response.Name,-20}{response.Url,-100}{response.StatusCode,-10}".ErrorDumpFor(debug);
            
                else
                    $"{response.Name,-20}{response.Url,-100}{response.StatusCode,-10}".GreenDump();
            }
        }
        catch (Exception ex)
        {
            $"Failed to process. \n[Reason]: {ex.Message}\n [Stacktrace]: {ex.StackTrace}".ErrorDumpFor(debug);
        }
    }
}