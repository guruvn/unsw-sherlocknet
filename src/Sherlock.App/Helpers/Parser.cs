using System.Collections.Immutable;
using Sherlock.App.Exceptions;

namespace Sherlock.App.Helpers;

public static class Parser
{
    public static IDictionary<Flag, IList<string>> Parse(this string message, IReadOnlyCollection<Flag> supportedFlags)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException($"{nameof(message)} cannot be NULL or EMPTY");

        var basket = new Dictionary<Flag, IList<string>>();
        var instructions = message.Trim().Split(" ").ToImmutableList();
        var idx = 0;

        while (idx < instructions.Count) {
            if (!instructions[idx].StartsWith("-"))
                throw new NotSupportedFlagException($"{instructions[idx]} is not a supported flag");

            var flag = supportedFlags.FirstOrDefault(flag => flag.Match(instructions[idx]));
            if (flag == null)
                throw new NotSupportedFlagException($"{instructions[idx]} is not a supported flag");

            if (string.Compare(instructions[idx], "-h", StringComparison.InvariantCultureIgnoreCase) == 0
                || string.Compare(instructions[idx], "--help", StringComparison.InvariantCultureIgnoreCase) == 0)
                throw new PrintingHelpException();
            
            if (string.Compare(instructions[idx], "-v", StringComparison.InvariantCultureIgnoreCase) == 0
                || string.Compare(instructions[idx], "--version", StringComparison.InvariantCultureIgnoreCase) == 0)
                throw new DisplayVersionException();
            
            if (string.Compare(instructions[idx], "-ls", StringComparison.InvariantCultureIgnoreCase) == 0
                || string.Compare(instructions[idx], "--list-sites", StringComparison.InvariantCultureIgnoreCase) == 0)
                throw new ListAllSupportedSitesException();
            
            if (basket.Keys.Any(k => k.Equals(flag)))
                throw new DuplicatedFlagException($"{instructions[idx]} already provided");

            ++idx;
            var values = new List<string>();
            while (idx < instructions.Count && !instructions[idx].StartsWith("-")) {
                values.AddRange(instructions[idx].Split(","));
                ++idx;
            }
            
            basket.Add(flag, values.ToImmutableList());
        }

        return basket;
    }
}