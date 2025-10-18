using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExpenseTracker.Service.Services.Auth
{
    public record SocialVerificationResult(string Email, string ProviderId, string? Name = null);

    public static class SocialVerifier
    {
        private static readonly HttpClient _http = new HttpClient();

        public static async Task<SocialVerificationResult?> VerifyAsync(string provider, string token, AuthOptions options)
        {
            if (options.SocialMock && token.StartsWith("mock-"))
            {
                var parts = token.Split(':', 2);
                var email = parts.Length > 1 ? parts[1] : $"{Guid.NewGuid()}@example.com";
                return new SocialVerificationResult(email, Guid.NewGuid().ToString(), email);
            }

            if (provider == "google")
            {
                // Validate id_token using Google's tokeninfo endpoint
                var url = $"https://oauth2.googleapis.com/tokeninfo?id_token={token}";
                var resp = await _http.GetAsync(url);
                if (!resp.IsSuccessStatusCode) return null;
                var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
                var aud = json.GetProperty("aud").GetString();
                if (!string.IsNullOrEmpty(options.GoogleClientId) && aud != options.GoogleClientId) return null;
                var email = json.GetProperty("email").GetString();
                var sub = json.GetProperty("sub").GetString();
                var name = json.TryGetProperty("name", out var n) ? n.GetString() : null;
                return new SocialVerificationResult(email ?? string.Empty, sub ?? string.Empty, name);
            }

            if (provider == "facebook")
            {
                // Validate using Facebook Graph API: /me?fields=id,name,email&access_token={token}
                var url = $"https://graph.facebook.com/me?fields=id,name,email&access_token={token}";
                var resp = await _http.GetAsync(url);
                if (!resp.IsSuccessStatusCode) return null;
                var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
                var id = json.GetProperty("id").GetString();
                var name = json.TryGetProperty("name", out var n) ? n.GetString() : null;
                var email = json.TryGetProperty("email", out var e) ? e.GetString() : null;
                return new SocialVerificationResult(email ?? string.Empty, id ?? string.Empty, name);
            }

            return null;
        }
    }
}
