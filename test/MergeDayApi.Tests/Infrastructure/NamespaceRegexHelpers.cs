using System.Text.RegularExpressions;

namespace MergeDayApi.Tests.Infrastructure;

public static class NamespaceRegexHelpers
{
    /// <summary>
    /// Given a CLR namespace (e.g. "MergeDayApi.Features.Absences"), 
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
}