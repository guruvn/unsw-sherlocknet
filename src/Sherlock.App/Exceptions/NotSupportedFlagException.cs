namespace Sherlock.App.Exceptions;

public sealed class NotSupportedFlagException: Exception
{
    public NotSupportedFlagException(string message): base(message)
    {
    }
}