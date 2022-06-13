using System.Collections.Immutable;

namespace Sherlock.App.Helpers;

public static class CollectionHelper
{
    public static void ForEach<T>(this IImmutableList<T>? collection, Action<T> action)
    {
        if (collection == null)
            return;

        foreach (var item in collection)
            action(item);
    }
}