namespace Sherlock.App.Exceptions;

public sealed class DuplicatedFlagException: Exception
{
    public DuplicatedFlagException(string message): base(message)
    {}
}