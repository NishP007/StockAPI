namespace StockAPI.Helpers
{
    public class AppSettings
    {
        public string JwtSecret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int TokenValidityInMinutes { get; set; }
    }
}
