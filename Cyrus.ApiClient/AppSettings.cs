namespace Cyrus.ApiClient
{
    internal class AppSettings
    {
        public static AppSettings Instance = new AppSettings();
        public AuthenticationOptions AuthenticationOptions { get; set; }
        public OpenApiOptions OpenApiOptions { get; set; }
    }

    internal class AuthenticationOptions
    {
        public string Host { get; set; }
        public string TokenUrl { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
    }

    internal class OpenApiOptions
    {
        public string BaseUrl { get; set; }
    }
}
