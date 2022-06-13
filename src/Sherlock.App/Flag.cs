namespace Sherlock.App;

/// <summary>
/// Flag is a value-object. Which means its equality will be its actual value not reference or both
/// https://martinfowler.com/bliki/ValueObject.html
/// </summary>
public sealed record Flag
{
    public string Name { get; }
    public string Abbreviation { get; }
    
    public string Description { get; }

    private Flag(string abbreviation, string name, string description)
    {
        Name = name;
        Abbreviation = abbreviation;
        Description = description;
    }
    
    public bool Match(string instruction)
    {
        if (string.IsNullOrWhiteSpace(instruction))
            return false;
        return string.Compare(Name, instruction, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(Abbreviation, instruction, StringComparison.OrdinalIgnoreCase) == 0;
    }
    
    /// <summary>
    /// Compare the actual value between two flags
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Flag? other)
        => other != null && 
           string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase) == 0 
           && string.Compare(Abbreviation, other.Abbreviation, StringComparison.OrdinalIgnoreCase) == 0;
    
    public override int GetHashCode()
        => HashCode.Combine(Name, Abbreviation, Description);
    
    /// <summary>
    /// Create an instance of Flag
    /// </summary>
    /// <param name="abbreviation">The short-hand version of the flag</param>
    /// <param name="name">The fullname of the flag</param>
    /// <param name="description">A description that explains what the flag is</param>
    /// <returns>An instance of flag</returns>
    /// <exception cref="ArgumentException"></exception>
    public static Flag Create(string abbreviation, string name, string description)
    {
        if (abbreviation.Length is < 2 or > 3 || !abbreviation.StartsWith("-"))
            throw new ArgumentException($"invalid format for {nameof(abbreviation)}");

        if (string.IsNullOrWhiteSpace(name) || !name.StartsWith("--"))
            throw new ArgumentException($"invalid format for {nameof(name)}");
        
        return new Flag(abbreviation, name, description);
    }
}