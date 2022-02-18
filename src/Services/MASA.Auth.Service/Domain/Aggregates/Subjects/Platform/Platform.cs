namespace MASA.Auth.Service.Domain.Aggregate
{
    public class Platform : AuditAggregateRoot<Guid, Guid>
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string? Url { get; set; }

        public string? Icon { get; set; }

        public PlatformType PlatformType { get; set; }

        public VerifyType VerifyType { get; set; }

        public IdentificationType IdentificationType { get; set; }

        private Platform()
        {
            Name = "";
            Code = "";
            ClientId = "";
            ClientSecret = "";
        }

        public Platform(string name, string code, string clientId, string clientSecret)
        {
            Name = name;
            Code = code;
            ClientId = clientId;
            ClientSecret = clientSecret;
        }
    }

    public enum PlatformType
    {
        ThirdParty,
        Private
    }

    public enum IdentificationType
    {
        MobilePhone,
        Email
    }

    public enum VerifyType
    {
        OAuth
    }
}
