using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace RedCat.Serializer.Utils
{
    public static class Md5
    {
        public static string GETMD5HASH(string input)
        {
            var hashed = HashAlgorithmProvider.OpenAlgorithm("MD5").HashData(CryptographicBuffer.ConvertStringToBinary(input, BinaryStringEncoding.Utf8));
            return CryptographicBuffer.EncodeToHexString(hashed);
        }

        public static string CREATEUNIQUEHASH()
        {
            return GETMD5HASH(UnixTime.Now);
        }
    }
}
