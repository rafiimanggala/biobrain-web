namespace BiobrainWebAPI.Values.Options
{
    public class SiteUrlSettings
    {
        public const string Section = "SiteUrl";

        public string Scheme { get; set; }
        public string Host { get; set; }
    }
}