using System;

namespace UrlShortener.Application.Utils.Extensions
{
    public static class UrlStringExtensions
    {
        public static bool IsUrl(this string value)
        {
            return !string.IsNullOrWhiteSpace(value)
                && Uri.TryCreate(value, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
