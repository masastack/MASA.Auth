// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masa.Auth.Contracts.Admin.Subjects
{
    public class RemoveThirdPartyUserByThridPartyIdentityDto
    {
        public string ThridPartyIdentity { get; set; }

        public RemoveThirdPartyUserByThridPartyIdentityDto(string thridPartyIdentity)
        {
            ThridPartyIdentity = thridPartyIdentity;
        }
    }
}
