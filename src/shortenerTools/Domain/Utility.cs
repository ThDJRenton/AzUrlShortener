using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Cloud5mins.domain
{
    public static class Utility
    {
        //reshuffled for randomisation, same unique characters just jumbled up, use your own for security
        private const string ConversionCode = "aoq6lewdfit0nbvp3ukz8mc941gsj57r2hyx";
        private static readonly int Base = ConversionCode.Length;
        private const int MinVanityLength = 5;

        public static async Task<string> GetValidEndUrl(string vanity, StorageTableHelper stgHelper)
        {
            if (string.IsNullOrEmpty(vanity))
            {
                var newKey = await stgHelper.GetNextTableId();
                string getCode() => Encode(newKey, MinVanityLength);
                return string.Join(string.Empty, getCode());
            }
            else
            {
                return string.Join(string.Empty, vanity);
            }
        }

        public static string Encode(int i, int minVanityLength)
        {
            if (i == 0)
                return ConversionCode[0].ToString();

            return GenerateUniqueRandomToken(i);

            var s = string.Empty;
            while (i > 0)
            {
                s += ConversionCode[i % Base];
                i = i / Base;
                //if we setting a minimum length just extend the code accordingly
                if (minVanityLength > 0)
                {
                    while (s.Length < minVanityLength)
                    {
                        s += ConversionCode[s.Length % Base];
                    }
                }
            }

            return string.Join(string.Empty, s.Reverse());
        }

        public static string GetShortUrl(string host, string vanity)
        {
            return host + "/" + vanity;
        }

        public static string GenerateUniqueRandomToken(int uniqueId)
        // generates a unique, random, and alphanumeric token for the use as a url (not entirely secure but not sequential so generally not guessable)
        {
            const string availableChars =
                "FjTG0s5dgWkbLf_8etOZqMzNhmp7u6lUJoXIDiQB9-wRxCKyrPcv4En3Y21aASHV";
            using (var generator = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[5]; //minimum size, longer the better but we want short URLs!
                generator.GetBytes(bytes);
                var chars = bytes
                    .Select(b => availableChars[b % availableChars.Length]);
                var token = new string(chars.ToArray());
                //reshuffle the id into the token
                var reversedToken = string.Join(string.Empty, token.Reverse());
                return uniqueId + reversedToken;
            }
        }
    }
}