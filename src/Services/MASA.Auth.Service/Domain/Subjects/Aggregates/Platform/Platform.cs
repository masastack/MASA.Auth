namespace MASA.Auth.Service.Domain.Subjects.Aggregates
{
    public class Platform : AuditAggregateRoot<Guid, Guid>
    {
        public string Name { get; private set; }

        public string Code { get; private set; }

        public string ClientId { get; private set; }

        public string ClientSecret { get; private set; }

        public string? Url { get; private set; }

        public string? Icon { get; private set; }

        public PlatformType PlatformType { get; private set; }

        public VerifyType VerifyType { get; private set; }

        public IdentificationType IdentificationType { get; private set; }

        private Platform()
        {
            Name = "";
            Code = "";
            ClientId = "";
            ClientSecret = "";
        }

        public Platform(string name, string code, string clientId, string clientSecret, string? url, string? icon, PlatformType platformType, VerifyType verifyType, IdentificationType identificationType)
        {
            Name = name;
            Code = code;
            ClientId = clientId;
            ClientSecret = clientSecret;
            Url = url;
            Icon = icon;
            PlatformType = platformType;
            VerifyType = verifyType;
            IdentificationType = identificationType;
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
