using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace PopForums.Sample.Caching
{
    [HtmlTargetElement("img", Attributes = CacheContentTagHelper.CacheSrcAttributeName)]
    [HtmlTargetElement("link", Attributes = CacheContentTagHelper.CacheHrefAttributeName)]
    [HtmlTargetElement("script", Attributes = CacheContentTagHelper.CacheSrcAttributeName)]
    public sealed class CacheContentTagHelper : TagHelper
    {
        #region Fields
        internal const string CacheHrefAttributeName = "cache-href";
        internal const string CacheSrcAttributeName = "cache-src";
        private static string FileHashKey(string path) => $"{path}.hash";

        private readonly IDistributedCache cache;
        private readonly IFileProvider fileProvider;
        private readonly IUrlHelper urlHelper;
        #endregion

        public CacheContentTagHelper(IDistributedCache cache, IOptions<StaticFileOptions> staticFileOptions, IUrlHelper urlHelper)
        {
            this.cache = cache;

            // use the IFileProvider configured for the Static Files middleware
            this.fileProvider = staticFileOptions.Value.FileProvider;
            this.urlHelper = urlHelper;
        }

        private TagHelperAttribute GetCacheAttribute(TagHelperContext context)
        {
            TagHelperAttribute attribute = null;
            switch (context.TagName)
            {
                case "img":
                case "script":
                    context.AllAttributes.TryGetAttribute(CacheSrcAttributeName, out attribute);
                    break;

                case "link":
                    context.AllAttributes.TryGetAttribute(CacheHrefAttributeName, out attribute);
                    break;

                default:
                    throw new NotSupportedException($"Unsupported tag '{context.TagName}'.");
            }

            return attribute;
        }

        private async Task<string> GetFileHashAsync(IFileInfo file)
        {

            // generate a cache key
            string key = FileHashKey(file.PhysicalPath);

            // has this file been hashed?
            string hash = await cache.GetStringAsync(key);
            if (hash != null)
            {
                return hash;
            }

            // hash + cache!
            hash = file.ComputeHash<SHA256Managed>();
            await cache.SetStringAsync(key, hash);

            return hash;
        }

        private bool IsValidPath(string path)
            => !string.IsNullOrWhiteSpace(path)
                && !path.StartsWith("//")
                && !path.StartsWith("http:// ")
                && !path.StartsWith("https:// ");

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var cacheAttribute = GetCacheAttribute(context);

            string cacheSrcValue = cacheAttribute?.Value?.ToString();
            string srcValue = cacheSrcValue;

            if (IsValidPath(cacheSrcValue))
            {
                // get the web-url path to the file using UrlHelper (this resolve relative paths)
                string path = urlHelper.Content(cacheSrcValue);
                var file = fileProvider.GetFileInfo(path);

                // does the file exist (on the filesystem)?
                if (
                    file?.Exists == true &&
                    !file.IsDirectory &&
                    !string.IsNullOrWhiteSpace(file.PhysicalPath)
                )
                {
                    string hash = await GetFileHashAsync(file);
                    srcValue = !string.IsNullOrWhiteSpace(hash)
                        ? $"{path}?v={hash}"
                        : path;
                }
            }

            // set the target attribute, and remove the `cache-*` attribute
            output.Attributes.SetAttribute(cacheAttribute.Name.Substring("cache-".Length), srcValue);
            output.Attributes.RemoveAll(cacheAttribute.Name);
        }
    }
}
