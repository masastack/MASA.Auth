// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates
{
    public class CustomLogin : AuditAggregateRoot<int, Guid>, ISoftDelete
    {
        private Client? _client;
        private List<CustomLoginThirdPartyIdp> _thirdPartyIdps = new();
        private List<RegisterField> _registerFields = new();
        private User? _createUser;
        private User? _modifyUser;

        public string Name { get; private set; }

        public string Title { get; private set; }

        public int ClientId { get; private set; }

        public bool Enabled { get; private set; }

        public Client Client => _client ?? throw new UserFriendlyException("Failed to get client data");

        public IReadOnlyCollection<CustomLoginThirdPartyIdp> ThirdPartyIdps => _thirdPartyIdps;

        public IReadOnlyCollection<RegisterField> RegisterFields => _registerFields;

        public User? CreateUser => _createUser;

        public User? ModifyUser => _modifyUser;

        public bool IsDeleted { get; private set; }

        public CustomLogin(string name, string title, int clientId, bool enabled)
        {
            Name = name;
            Title = title;
            ClientId = clientId;
            Enabled = enabled;
        }

        public static implicit operator CustomLoginDetailDto(CustomLogin customLogin)
        {
            var client = new ClientDto
            {
                Id = customLogin.Client.Id,
                ClientId = customLogin.Client.ClientId,
                Enabled = customLogin.Enabled,
                ClientName = customLogin.Client.ClientName,
                ClientType = customLogin.Client.ClientType,
                Description = customLogin.Client.Description,
                LogoUri = customLogin.Client.LogoUri,
            };
            var thirdPartyIdps = customLogin.ThirdPartyIdps.Select(tp => new CustomLoginThirdPartyIdpDto(tp.ThirdPartyIdpId, tp.Sort)).ToList();
            var registerFields = customLogin.RegisterFields.Select(rf => new RegisterFieldDto(rf.RegisterFieldType, rf.Sort, rf.Required)).ToList();
            return new CustomLoginDetailDto(customLogin.Id, customLogin.Name, customLogin.Title, client, customLogin.Enabled, customLogin.CreationTime, customLogin.ModificationTime, customLogin.CreateUser?.Name ?? "", customLogin.ModifyUser?.Name ?? "", thirdPartyIdps, registerFields); ;
        }

        public void Update(string name,string title, bool enabled)
        {
            Name = name;
            Title = title;
            Enabled = enabled;
        }

        public void BindThirdPartyIdps(IEnumerable<CustomLoginThirdPartyIdpDto> thirdPartyIdps)
        {
            _thirdPartyIdps.Clear();
            _thirdPartyIdps.AddRange(thirdPartyIdps.Select(tp => new CustomLoginThirdPartyIdp(tp.Id, tp.Sort)));
        }

        public void BindRegisterFields(IEnumerable<RegisterFieldDto> registerFields)
        {
            _registerFields.Clear();
            _registerFields.AddRange(registerFields.Select(registerField => new RegisterField(registerField.RegisterFieldType, registerField.Sort, registerField.Required)));
        }
    }
}
