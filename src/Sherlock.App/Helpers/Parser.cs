using System.Collections.Immutable;
using Sherlock.App.Exceptions;

namespace Sherlock.App.Helpers;

/// <summary>
/// Parser is a parsing helper that will take the message instruction from CLI and transform it to a dictionary
/// of instructions. Each individual key-pair is corresponding with flag and its value.
/// </summary>
public static class Parser
{
    public static IDictionary<Flag, IList<string>> Parse(this string message, IImmutableList<Flag> supportedFlags)
    {
        //Terminate processing if message instruction is invalid
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException($"{nameof(message)} cannot be NULL or EMPTY");

        var basket = new Dictionary<Flag, IList<string>>();
        var instructions = message.Trim().Split(" ").ToImmutableList();
        var idx = 0;

        // message-instruction comprises of multiple sub-instruction(s)
        // each sub-instruction consists of a flag and its corresponding value (i.e., --flag flag_value)
        // flag and its value are separated by space
        while (idx < instructions.Count) {
            if (!instructions[idx].StartsWith("-")) // message-instruction must always start with flag
                throw new NotSupportedFlagException($"{instructions[idx]} is not a supported flag");
            
            var flag = ExtractFlag(supportedFlags, instructions[idx]);
            EmitNotifyIfRequired(flag, instructions[idx]);
            
            // We don't want duplicated flags. So any violation will end with exception
            // which then handled at global level
            if (basket.Keys.Any(k => k.Equals(flag)))
                throw new DuplicatedFlagException($"{instructions[idx]} already provided");

            // Obtain flag's values
            ++idx;
            var values = new List<string>();
            while (idx < instructions.Count && !instructions[idx].StartsWith("-")) {
                values.AddRange(instructions[idx].Split(","));
                ++idx;
            }
            
            // Store both the flag and its value to a memory basket
            basket.Add(flag, values.Where(v => !string.IsNullOrWhiteSpace(v)).ToImmutableList());
        }

        return basket;
    }

    /// <summary>
    /// Extract the flag from nominated instruction
    /// </summary>
    /// <param name="supportedFlags">A collection of supported flags</param>
    /// <param name="instruction">An instruction that indicates the flag to extract</param>
    /// <returns>a support flag</returns>
    /// <exception cref="NotSupportedFlagException">Represent the not supported flag situation</exception>
    private static Flag ExtractFlag(IImmutableList<Flag> supportedFlags, string instruction)
    {
        // Extract a supported flag by nominated instruction[idx]
        var flag = supportedFlags.SingleOrDefault(flag => flag.Match(instruction));
        
        if (flag == null)
            throw new NotSupportedFlagException($"{instruction} is not a supported flag");
        
        return flag;
    }

    /// <summary>
    /// The method is responsible orchestrating communication between Parser and its consumer.
    /// Bear in mind, not all exceptions are the actual concern. The idea of throwing exception in this method is
    /// a technique of orchestrating communication and encapsulating the concern.
    /// For example: Parser is responsible for parsing instruction, but not printing out any help information.
    /// The concern of printing out help information should be handled globally.
    /// </summary>
    /// <param name="flag">The flag that requires validation</param>
    /// <param name="instruction">The instruction that requires validation</param>
    /// <exception cref="NotSupportedFlagException"></exception>
    /// <exception cref="PrintingHelpException"></exception>
    /// <exception cref="DisplayVersionException"></exception>
    /// <exception cref="ListAllSupportedSitesException"></exception>
    private static void EmitNotifyIfRequired(Flag flag, string instruction)
    {
        if (string.Compare(instruction, "-h", StringComparison.InvariantCultureIgnoreCase) == 0
            || string.Compare(instruction, "--help", StringComparison.InvariantCultureIgnoreCase) == 0)
            throw new PrintingHelpException();

        if (string.Compare(instruction, "-v", StringComparison.InvariantCultureIgnoreCase) == 0
            || string.Compare(instruction, "--version", StringComparison.InvariantCultureIgnoreCase) == 0)
            throw new DisplayVersionException();

        if (string.Compare(instruction, "-ls", StringComparison.InvariantCultureIgnoreCase) == 0
            || string.Compare(instruction, "--list-sites", StringComparison.InvariantCultureIgnoreCase) == 0)
            throw new ListAllSupportedSitesException();
    }
}