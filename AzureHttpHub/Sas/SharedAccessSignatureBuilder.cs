using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

// see https://github.com/jierong/SharedAccessSignatureGenerator

namespace AzureHttpHub.Sas
{
    public class SharedAccessSignatureBuilder
    {
        public string Key { get; set; }

        public string KeyName
        {
            get;
            set;
        }

        public string Target
        {
            get;
            set;
        }

        public TimeSpan TimeToLive
        {
            get;
            set;
        }

        public string TargetService
        {
            get;
            set;
        }

        public SharedAccessSignatureBuilder()
        {
            TimeToLive = TimeSpan.FromMinutes(20);
            TargetService = "iothub";
        }

        private static string BuildExpiresOn(TimeSpan timeToLive)
        {
            var dateTime = DateTime.UtcNow.Add(timeToLive);
            var timeSpan = dateTime.Subtract(SharedAccessSignatureConstants.EpochTime);
            return Convert.ToString(Convert.ToInt64(timeSpan.TotalSeconds, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        }

        private static string BuildSignature(string keyName, string key, string target, TimeSpan timeToLive, string targetService = "iothub")
        {
            var str = BuildExpiresOn(timeToLive);
            var str1 = WebUtility.UrlEncode(target);
            var strs = new List<string>
            {
                str1,
                str
            };
            var str2 = Sign(string.Join("\n", strs), key, targetService);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0} {1}={2}&{3}={4}&{5}={6}", "SharedAccessSignature", "sr", str1, "sig", WebUtility.UrlEncode(str2), "se", WebUtility.UrlEncode(str));
            if (!string.IsNullOrEmpty(keyName) && targetService != "iothub") // jpd: no skn/policy name needed for iothub
            {
                stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "&{0}={1}", new object[] { "skn", WebUtility.UrlEncode(keyName) });
            }
            return stringBuilder.ToString();
        }

        private static string Sign(string requestString, string key, string targetService)
        {
            string base64String;

            if (!string.IsNullOrEmpty(targetService) && targetService.ToLower() == "servicebus")
            {
                using (var hMACSHA256 = new HMACSHA256(Encoding.UTF8.GetBytes(key))) // key is not decoded
                {
                    base64String = Convert.ToBase64String(hMACSHA256.ComputeHash(Encoding.UTF8.GetBytes(requestString)));
                }
            }
            else
            {
                using (var hMACSHA256 = new HMACSHA256(Convert.FromBase64String(key))) // key is decoded
                {
                    base64String = Convert.ToBase64String(hMACSHA256.ComputeHash(Encoding.UTF8.GetBytes(requestString)));
                }
            }

            return base64String;
        }

        public string ToSignature()
        {
            return BuildSignature(KeyName, Key, Target, TimeToLive, TargetService);
        }
    }

    public class SharedAccessSignatureConstants
    {
        public const int MaxKeyNameLength = 256;

        public const int MaxKeyLength = 256;

        public const string SharedAccessSignature = "SharedAccessSignature";

        public const string AudienceFieldName = "sr";

        public const string SignatureFieldName = "sig";

        public const string KeyNameFieldName = "skn";

        public const string ExpiryFieldName = "se";

        public const string SignedResourceFullFieldName = "SharedAccessSignature sr";

        public const string KeyValueSeparator = "=";

        public const string PairSeparator = "&";

        public static readonly DateTime EpochTime;

        public static readonly TimeSpan MaxClockSkew;

        static SharedAccessSignatureConstants()
        {
            EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            MaxClockSkew = TimeSpan.FromMinutes(5);
        }
    }


}
