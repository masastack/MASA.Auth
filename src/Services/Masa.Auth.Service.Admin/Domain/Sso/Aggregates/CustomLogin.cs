// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates
{
    public class CustomLogin : FullAggregateRoot<int, Guid>
    {
        private List<CustomLoginThirdPartyIdp> _thirdPartyIdps = new();
        private List<RegisterField> _registerFields = new();
        private User? _createUser;
        private User? _modifyUser;

        public string Name { get; private set; }

        public string Title { get; private set; }

        public string ClientId { get; private set; }

        public bool Enabled { get; private set; }

        public Client? Client { get; set; }

        public IReadOnlyCollection<CustomLoginThirdPartyIdp> ThirdPartyIdps => _thirdPartyIdps;

        public IReadOnlyCollection<RegisterField> RegisterFields => _registerFields;

        public User? CreateUser => _createUser;

        public User? ModifyUser => _modifyUser;

        public CustomLogin(string name, string title, string clientId, bool enabled)
        {
            Name = name;
            Title = title;
            ClientId = clientId;
            Enabled = enabled;
        }

        public void Update(string name, string title, bool enabled)
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
