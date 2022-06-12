namespace Sherlock.App;

public record Flag
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

    public static Flag Create(string abbreviation, string name, string description)
    {
        if (abbreviation.Length != 2 || !abbreviation.StartsWith("-"))
            throw new ArgumentException($"invalid format for {nameof(abbreviation)}");

        if (string.IsNullOrWhiteSpace(name) || !name.StartsWith("--"))
            throw new ArgumentException($"invalid format for {nameof(name)}");
        
        return new Flag(abbreviation, name, description);
    }
}