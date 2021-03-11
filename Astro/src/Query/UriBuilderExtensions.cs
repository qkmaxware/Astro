using System; // For UriBuilder
using System.Web; // ForHttpUtility

namespace Qkmaxware.Astro.Query {

/// <summary>
/// Extensions for making urls easier
/// </summary>
public static class UriBuilderExtensions {
    /// <summary>
    /// Add a query url parametre
    /// </summary>
    /// <param name="uri">uri being edited</param>
    /// <param name="name">parametre name</param>
    /// <param name="value">parametre value</param>
    /// <returns>uri builder</returns>
    public static UriBuilder AddParametre(this UriBuilder uri, string name, object value) {
        var encName = HttpUtility.UrlEncode(name);
        var encValue = HttpUtility.UrlEncode(value?.ToString() ?? string.Empty);
        uri.Query += (string.IsNullOrEmpty(uri.Query) ? string.Empty : "&") + encName + "=" + encValue;
        return uri;
    }
    /// <summary>
    /// Add a subpath to a url
    /// </summary>
    /// <param name="uri">uri being edited</param>
    /// <param name="path">sub-path name</param>
    /// <returns>uri builder</returns>
    public static UriBuilder AddSubpath(this UriBuilder uri, object path) {
        uri.Path += (uri.Path.EndsWith("/") ? string.Empty : "/") + (path?.ToString() ?? string.Empty);
        return uri;
    }
}

}