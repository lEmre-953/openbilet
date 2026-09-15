using System;
using System.Configuration;

namespace openbilet
{
    internal static class AppSettings
    {
        public static string MySqlConnectionString
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["OpenBilet"]?.ConnectionString;
                if (string.IsNullOrWhiteSpace(cs))
                    throw new ConfigurationErrorsException("App.config içinde OpenBilet connection string tanımlı değil.");
                return cs;
            }
        }

        public static string SmtpHost => Get("SmtpHost", "smtp.gmail.com");

        public static int SmtpPort => int.TryParse(Get("SmtpPort", "587"), out var port) ? port : 587;

        public static string SmtpEmail => GetRequired("SmtpEmail");

        public static string SmtpPassword => GetRequired("SmtpPassword");

        public static string SmtpSenderName => Get("SmtpSenderName", "OpenTicket");

        public static string IyzicoApiKey => GetRequired("IyzicoApiKey");

        public static string IyzicoSecretKey => GetRequired("IyzicoSecretKey");

        public static string IyzicoBaseUrl => Get("IyzicoBaseUrl", "https://sandbox-api.iyzipay.com");

        private static string Get(string key, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        private static string GetRequired(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value))
                throw new ConfigurationErrorsException($"App.config içinde {key} ayarı eksik.");
            return value;
        }
    }
}
