using System;

namespace SimplePdfViewer
{
    public static class UriExtension
    {
        public static bool IsWebUri(this Uri uri)
        {
            if (uri != null)
            {
                var str = uri.ToString().ToLower();
                return str.StartsWith("http://") || str.StartsWith("https://");
            }
            return false;
        }

        public static bool IsApplicationUri(this Uri uri)
        {
            if (uri != null)
            {
                var str = uri.ToString().ToLower();
                return str.StartsWith("ms-appx://") || str.StartsWith("ms-appx://");
            }
            return false;
        }
    }
}
