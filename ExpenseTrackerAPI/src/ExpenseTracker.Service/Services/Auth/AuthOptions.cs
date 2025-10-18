namespace ExpenseTracker.Service.Services.Auth
{
    public class AuthOptions
    {
        public string DefaultCurrency { get; set; } = "USD";
        public string DefaultLocale { get; set; } = "en-US";
        public string DefaultTimezone { get; set; } = "UTC";
        public int AccessMinutes { get; set; } = 60;
        public int RefreshDays { get; set; } = 30;
        public bool SocialMock { get; set; } = true;
        public bool AllowSocialAutoLink { get; set; } = true;
        public int ResetHours { get; set; } = 4;

        // Social provider configuration
        public string? GoogleClientId { get; set; }
        public string? FacebookAppId { get; set; }
    }
}
