using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebHookApp.AuthenticationTelegram
{
    public static class TelegramHashValidator
    {
        public static bool IsValid(Dictionary<string, string> telegramUserInfo, string botToken)
        {
            var dataString = CombineString(telegramUserInfo);
            var isValid = IsValid(dataString, botToken, telegramUserInfo["hash"]);
            return isValid;
        }

        public static bool IsValidWebAppData(Dictionary<string, string> webAppData, string botToken)
        {
            var dataString = CombineWebAppDataString(webAppData);
            var isValid = IsValidWebAppData(dataString, botToken, webAppData["hash"]);
            return isValid;
        }

        private static bool IsValid(string message, string botToken, string hash)
        {
            /*
             data_check_string = ...
            secret_key = SHA256(<bot_token>)
            if (hex(HMAC_SHA256(data_check_string, secret_key)) == hash) {
              // data is from Telegram
            }
            */
            using var hasher = SHA256.Create();
            var secretKey = hasher.ComputeHash(Encoding.UTF8.GetBytes(botToken));
            using var hmac256 = new HMACSHA256(secretKey);
            var hmac256MessageAndSecretKey = hmac256.ComputeHash(Encoding.UTF8.GetBytes(message));
            var hexHmac256MessageAndSecretKey = Convert.ToHexString(hmac256MessageAndSecretKey).ToLower();

            if (hash == hexHmac256MessageAndSecretKey)
                return true;
            else return false;
        }

        private static bool IsValidWebAppData(string message, string botToken, string hash)
        {
            /*
             data_check_string = ...
                secret_key = HMAC_SHA256(<bot_token>, "WebAppData")
                if (hex(HMAC_SHA256(data_check_string, secret_key)) == hash) {
                  // data is from Telegram
                }
             */
            using var hmac256 = new HMACSHA256(WebAppDataStringBytes);
            var secretKey = hmac256.ComputeHash(Encoding.UTF8.GetBytes(botToken));
            hmac256.Key = secretKey;

            var hmac256MessageAndSecretKey = hmac256.ComputeHash(Encoding.UTF8.GetBytes(message));
            var hexHmac256MessageAndSecretKey = Convert.ToHexString(hmac256MessageAndSecretKey).ToLower();

            if (hash == hexHmac256MessageAndSecretKey)
                return true;
            else return false;
        }

        static byte[] _webAppDataStringBytes;

        static byte[] WebAppDataStringBytes 
        {  
            get 
            {
                if (_webAppDataStringBytes is null)
                    _webAppDataStringBytes = System.Text.Encoding.UTF8.GetBytes("WebAppData");
                return _webAppDataStringBytes; 
            } 
        }

        private static string CombineString(IReadOnlyDictionary<string, string> meta)
        {
            var builder = new StringBuilder();

            TryAppend("auth_date");
            TryAppend("first_name");
            TryAppend("id");
            TryAppend("last_name");
            TryAppend("photo_url");
            TryAppend("username", true);

            return builder.ToString();

            void TryAppend(string key, bool isLast = false)
            {
                if (meta.ContainsKey(key))
                    builder.Append($"{key}={meta[key]}{(isLast ? "" : "\n")}");
            }
        }

        private static string CombineWebAppDataString(IReadOnlyDictionary<string, string> meta)
        {
            var builder = new StringBuilder();

            TryAppend("auth_date");
            TryAppend("chat_instance");
            TryAppend("chat_type");
            //TryAppend("hash");
            TryAppend("start_param");
            TryAppend("user", true);

            return builder.ToString();

            void TryAppend(string key, bool isLast = false)
            {
                if (meta.ContainsKey(key))
                    builder.Append($"{key}={meta[key]}{(isLast ? "" : "\n")}");
            }
        }
    }
}
