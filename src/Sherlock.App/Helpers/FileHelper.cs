using System.Collections.Immutable;
using Newtonsoft.Json;

namespace Sherlock.App.Helpers;

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