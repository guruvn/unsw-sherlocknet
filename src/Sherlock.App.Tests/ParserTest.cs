using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using FluentAssertions;
using Sherlock.App.Exceptions;
using Sherlock.App.Helpers;
using Xunit;

namespace Sherlock.App.Tests;

public class ParserTest
{
    private readonly IImmutableList<Flag> _supportedFlags;
    public ParserTest()
        => _supportedFlags = new List<Flag> {
            Flag.Create("-h", "--help", "Display help message"),
            Flag.Create("-v", "--version", "Display the application version"),
            Flag.Create("-u", "--user-names", "One or more usernames (seperated by commas) to check with social networks."),
            Flag.Create("-t", "--timeout", "Time (in seconds) to wait for response. Default timeout is infinity."),
            Flag.Create("-p", "--print-all", "Print all sites including sites where the user not found. Default is only printing found sites")
        }.ToImmutableList();
    
    [Theory]
    [InlineData("-n nick-name")]
    public void ParseTest_NotSupportedFlagException(string message)
    {
        Action callback = () => message.Parse(_supportedFlags);
        callback.Should()
            .Throw<NotSupportedFlagException>()
            .WithMessage("-n is not a supported flag");
    }
    
    [Theory]
    [InlineData("-u username --user-names username")]
    public void ParseTest_DuplicatedFlagException(string message)
    {
        Action callback = () => message.Parse(_supportedFlags);
        callback.Should()
            .Throw<DuplicatedFlagException>()
            .WithMessage("--user-names already provided");
    }
    
    [Theory]
    [InlineData("-u username -d --site Facebook")]
    public void ParseTest_3ActionsToExecute(string message)
        => message.Parse(_supportedFlags)
            .Should()
            .HaveCount(3);
}