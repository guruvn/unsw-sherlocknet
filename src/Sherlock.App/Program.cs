using System.Collections.Immutable;
using Sherlock.App;

new WenceyWang.FIGlet.AsciiArt("SherlockNET")
    .Result
    .ToImmutableList()
    .ForEach(Console.WriteLine);

var supportedFlags = new List<Flag> {
    Flag.Create("-h", "--help", "Display help message"),
    Flag.Create("-v", "--version", "Display the application version"),
    Flag.Create("-u", "--user-names", "One or more usernames (seperated by commas) to check with social networks."),
    Flag.Create("-t", "--timeout", "Time (in seconds) to wait for response. Default timeout is infinity."),
    Flag.Create("-p", "--print-all", "Print all sites including sites where the user not found. Default is only printing found sites")
}.ToImmutableList();

if (args!.Length == 0) {
    supportedFlags.ForEach(flag => Console.WriteLine($"{flag.Name,-20}{flag.Abbreviation,-10}{flag.Description}"));
    return;
}

args.ToImmutableList().ForEach(Console.WriteLine);
    