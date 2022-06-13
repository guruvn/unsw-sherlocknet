using System;
using System.Collections.Immutable;
using System.Reflection;
using Sherlock.App;
using Sherlock.App.Exceptions;
using Sherlock.App.Helpers;
// ReSharper disable ConstantConditionalAccessQualifier

Console.Clear();

new WenceyWang.FIGlet.AsciiArt("UNSW - SherlockNET")
    .Result
    .ToImmutableList()
    .ForEach(text => text.YellowDump());

//Load all supported sites from the supplied data.json file into memory
var supportedSites = await "data.json".DeserializeAsync().ConfigureAwait(false);

//Print out help information if no argument provided
if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0])) {
    Constant.SupportedFlags.ForEach(flag => Console.WriteLine($"{flag.Name,-20}{flag.Abbreviation,-10}{flag.Description}"));
    return;
}

try
{
    //Build instructions from arguments that sent by CLI
    var instructions = args[0].Parse(Constant.SupportedFlags);
    var client = HttpClientFacade.Create();
    "Please wait...".DumpWith(ConsoleColor.White);
    
    // Scan the nominated user across all supported sites if no specific site nominated
    var specificSite = instructions.FirstOrDefault(site => site.Key.Name == "--site").Value?.FirstOrDefault();
    if (string.IsNullOrWhiteSpace(specificSite))
    {
        foreach (var site in supportedSites)
            await site.HuntAsync(client,
                instructions.Single(i => i.Key.Name == "--user-names").Value.ToImmutableList(),
                instructions.Any(i => i.Key.Name == "--debug"));
        
        return;
    }
    
    //Only scan the user on nominated site if provided
    var nominatedSite = supportedSites.SingleOrDefault(site
        => string.Compare(site.Name, specificSite, StringComparison.InvariantCultureIgnoreCase) == 0);

    if (nominatedSite == null)
        throw new NotSupportedSiteException($"{specificSite} is not supported yet.");

    await nominatedSite.HuntAsync(client,
        instructions.Single(i => i.Key.Name == "--user-names").Value.ToImmutableList(),
        instructions.Any(i => i.Key.Name == "--debug"));
}
catch (NotSupportedFlagException ex)
{
    ex.Message.RedDump();
    Constant.SupportedFlags.ForEach(flag => Console.WriteLine($"{flag.Name,-20}{flag.Abbreviation,-10}{flag.Description}"));
}
catch (NotSupportedSiteException ex)
{
    ex.Message.RedDump();
    Constant.SupportedFlags.ForEach(flag => Console.WriteLine($"{flag.Name,-20}{flag.Abbreviation,-10}{flag.Description}"));
}
catch (PrintingHelpException)
{
    Constant.SupportedFlags.ForEach(flag => Console.WriteLine($"{flag.Name,-20}{flag.Abbreviation,-10}{flag.Description}"));
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
    $"Failed to process. \n[Reason] {ex.Message}. Check the help info below".RedDump();
    Constant.SupportedFlags.ForEach(flag => Console.WriteLine($"{flag.Name,-20}{flag.Abbreviation,-10}{flag.Description}"));
}