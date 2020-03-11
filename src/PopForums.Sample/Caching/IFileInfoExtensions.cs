using System.Security.Cryptography;
using System.Linq;
using System.Text;
using Microsoft.Extensions.FileProviders;

namespace PopForums.Sample.Caching
{
    public static class IFileInfoExtensions
    {

        public static string ComputeHash<THashAlgorithm>(this IFileInfo file, string format = "x2")
            where THashAlgorithm : HashAlgorithm, new()
        {
            // nothing to compute
            if (file?.Exists != true || file.IsDirectory || file.Length == 0)
            {
                return null;
            }

            using (var hasher = new THashAlgorithm())
            using (var stream = file.CreateReadStream())
            {

                byte[] bytes = hasher.ComputeHash(stream);
                stream.Close();

                return bytes.Aggregate(
                    new StringBuilder(bytes.Length * 2),
                    (hash, value) => hash.Append(value.ToString(format))
                ).ToString();
            }
        }

    }
}
