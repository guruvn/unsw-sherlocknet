using System.Net;

namespace Sherlock.App;

public class Result
{
    public string Name { get; }
    public string Url { get; }
    public HttpStatusCode StatusCode { get; }

    Result(string name, string url, HttpStatusCode code)
    {
        Name = name;
        Url = url;
        StatusCode = code;
    }

    public static Result Create(string name, string url, HttpStatusCode code) => new(name, url, code);

    public static Result Empty => Create("NULL", "NULL", HttpStatusCode.Unused);
}