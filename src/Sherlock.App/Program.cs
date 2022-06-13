using System.Collections.Immutable;
using System.Reflection;
using Sherlock.App;
using Sherlock.App.Exceptions;
using Sherlock.App.Helpers;
// ReSharper disable ConstantConditionalAccessQualifier

new WenceyWang.FIGlet.AsciiArt("UNSW - SherlockNET")
    .Result
    .ToImmutableList()
    .ForEach(text => text.YellowDump());

var supportedFlags = new List<Flag> {
    Flag.Create("-h", "--help", "Display help message"),
    Flag.Create("-v", "--version", "Dcleisplay the application version"),
    Flag.Create("-u", "--user-names", "One or more usernames (seperated by commas) to check with social networks."),
    Flag.Create("-d", "--debug", "Printing out all debug log information."),
    Flag.Create("-s", "--site", "Hunt in a specific social site"),
    Flag.Create("-ls", "--list-sites", "List all supported sites")
}.ToImmutableList();

var supportedSites = await "data.json".DeserializeAsync().ConfigureAwait(false);

if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0])) {
    supportedFlags.ForEach(flag => Console.WriteLine($"{flag.Name,-20}{flag.Abbreviation,-10}{flag.Description}"));
    return;
}

try
{
    var instructions = args[0].Parse(supportedFlags);
    var client = HttpClientFacade.Create();
    var specificSite = instructions.FirstOrDefault(site => site.Key.Name == "--site").Value?.FirstOrDefault();

    if (string.IsNullOrWhiteSpace(specificSite))
        foreach (var site in supportedSites)
            await site.HuntAsync(client,
                instructions.Single(i => i.Key.Name == "--user-names").Value.ToImmutableList(),
                instructions.Any(i => i.Key.Name == "--debug"));
    else
        await supportedSites
            .Single(site => string.Compare(site.Name, specificSite, StringComparison.InvariantCultureIgnoreCase) == 0)
            .HuntAsync(client,
                instructions.Single(i => i.Key.Name == "--user-names").Value.ToImmutableList(),
                instructions.Any(i => i.Key.Name == "--debug"));
}
catch (NotSupportedFlagException ex)
{
    ex.Message.RedDump();
    supportedFlags.ForEach(flag => Console.WriteLine($"{flag.Name,-20}{flag.Abbreviation,-10}{flag.Description}"));
}
catch (PrintingHelpException)
{
    supportedFlags.ForEach(flag => Console.WriteLine($"{flag.Name,-20}{flag.Abbreviation,-10}{flag.Description}"));
}
catch (DisplayVersionException)
{
    var assembly = Assembly.GetExecutingAssembly();
    var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
    fileVersionInfo.FileVersion?.YellowDump();
}
catch (ListAllSupportedSitesException)
{
    foreach (var site in supportedSites)
        site.Name.YellowDump();
}
catch (DuplicatedFlagException ex)
{
    $"Check your instruction {args[0]}. It looks like {ex.Message}".RedDump();
}
catch (Exception ex)
{
    $"Failed to process. \n[Reason] {ex.Message}\n [StackTrace]: {ex.StackTrace}".RedDump();
}