using System.Text.RegularExpressions;

namespace MergeDayApi.Tests.Infrastructure;

public static class NamespaceRegexHelpers
{
    /// <summary>
    /// Given a CLR namespace (e.g. "BE.Features.Events"), 
    /// returns a regex that matches exactly that namespace or any child namespace.
    /// </summary>
    public static string ExactOrChildNamespace(string @namespace)
    {
        // Escape all regex‐special characters (dots, etc.) in the namespace,
        // then append "(\.|$)" so that it matches either a literal "." (child)
        // or the end of the string.
        var escaped = Regex.Escape(@namespace);
        return $"^{escaped}(\\.|$)";
    }

    /// <summary>
    /// Given a type, build the same regex using its Namespace.
    /// E.g. if T lives in "KCT.BE.Features.Events", returns "^KCT\.BE\.Features\.Events(\.|$)".
    /// </summary>
    public static string ExactOrChildNamespace(this Type type)
    {
        var ns = type.Namespace ?? string.Empty;
        return ExactOrChildNamespace(ns);
    }
}