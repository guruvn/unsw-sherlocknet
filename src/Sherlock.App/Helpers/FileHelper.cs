using System.Collections.Immutable;
using Newtonsoft.Json;

namespace Sherlock.App.Helpers;

/// <summary>
/// FileHelper is a class which takes responsibility of loading a nominated json file path and transform the
/// file content into an immutable collection of Site
/// </summary>
public static class FileHelper
{
    public static async Task<IImmutableList<Site>> DeserializeAsync(this string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"{nameof(filePath)} does not exist");
        
        var content = await File.ReadAllTextAsync(filePath).ConfigureAwait(false);
        
        return JsonConvert.DeserializeObject<IImmutableList<Site>>(content);
    }
}