// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Stores;

public class ConsentResponseStore : IMessageStore<ConsentResponse>
{
    public Task<Message<ConsentResponse>> ReadAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<string> WriteAsync(Message<ConsentResponse> message)
    {
        throw new NotImplementedException();
    }
}
