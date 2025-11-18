// File: Model/JwtSettings.cs
namespace Model
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = "AuthService";           // Giá tr? m?c ??nh theo ?nh
        public string Audience { get; set; } = "AuthServiceClient";   // Giá tr? m?c ??nh theo ?nh
        public int ExpiresInMinutes { get; set; } = 10080;            // 7 ngày = 10080 phút

        // Helper ?? l?y giá tr? sau khi ?ã override t? Environment Variables
        public string GetSecretKey() =>
            Environment.GetEnvironmentVariable("JWT_KEY") ?? SecretKey ?? throw new InvalidOperationException("JWT SecretKey không ???c c?u hình!");

        public string GetIssuer() =>
            Environment.GetEnvironmentVariable("JWT_ISSUER") ?? Issuer;

        public string GetAudience() =>
            Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? Audience;

        public int GetExpiresInMinutes()
        {
            var env = Environment.GetEnvironmentVariable("JWT_EXPIRES_IN_MINUTES");
            return int.TryParse(env, out int val) ? val : ExpiresInMinutes;
        }
    }
}