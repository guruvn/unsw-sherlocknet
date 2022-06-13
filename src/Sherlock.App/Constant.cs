using System.Collections.Immutable;

namespace Sherlock.App;

public static class Constant
{
    public static class ErrorType
    {
        public const string Message = "message";
        public const string StatusCode = "status_code";
        public const string ResponseUrl = "response_url";
    }
    
    /// <summary>
    /// A collection of predefined supported flags
    /// </summary>
    public static readonly IImmutableList<Flag> SupportedFlags = new List<Flag> {
        Flag.Create("-h", "--help", "Displays help information"),
        Flag.Create("-v", "--version", "Displays the application version"),
        Flag.Create("-u", "--user-names", "One or more usernames (seperated by commas or space) to check with social networks."),
        Flag.Create("-d", "--debug", "Prints out all debug log information."),
        Flag.Create("-s", "--site", "Hunts in a specific social site"),
        Flag.Create("-ls", "--list-sites", "Lists all supported sites")
    }.ToImmutableList();
}