// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using StackExchange.Redis;

namespace Masa.Auth.Service.Admin.Services;

public class OnlineUserService : ServiceBase
{
    const string ONLINE_USERS_KEY = "online:users";
    const string SSO_SESSION_KEY_PREFIX = "online:sso-session:";

    static string SsoSessionKey(string subjectId) => $"{SSO_SESSION_KEY_PREFIX}{subjectId}";

    public OnlineUserService() : base("api/online-user")
    {
        MapGet(GetListAsync);
        MapPost(KickUserAsync, "kick");
    }

    public async Task<PaginationDto<OnlineUserDto>> GetListAsync(
        [FromServices] IEventBus eventBus,
        [FromServices] IConnectionMultiplexer mux,
        [FromServices] AuthDbContext dbContext,
        GetOnlineUsersDto input)
    {
        var db = mux.GetDatabase();

        var webMembers = await db.SetMembersAsync(ONLINE_USERS_KEY);
        var webSubjectIds = webMembers.Select(v => v.ToString()).ToHashSet();

        var appSubjectIds = await dbContext.Set<PersistedGrant>()
                .Where(g => g.Type == "refresh_token" && g.Expiration > DateTime.UtcNow)
                .Select(g => g.SubjectId)
                .Distinct()
                .ToListAsync();

        var staleWebIds = new List<string>();
        var webSessions = new Dictionary<string, WebSessionDto>();
        foreach (var subjectId in webSubjectIds)
        {
            var sessionJson = await db.StringGetAsync(SsoSessionKey(subjectId));
            if (sessionJson.HasValue)
            {
                var session = JsonSerializer.Deserialize<WebOnlineSessionDto>(sessionJson!);
                if (session != null)
                    webSessions[subjectId] = new WebSessionDto(session.LoginTime, session.ClientId);
            }
            else
            {
                staleWebIds.Add(subjectId);
            }
        }

        if (staleWebIds.Count > 0)
        {
            var batch = db.CreateBatch();
            foreach (var staleId in staleWebIds)
                _ = batch.SetRemoveAsync(ONLINE_USERS_KEY, staleId);
            batch.Execute();
        }

        var allSubjectIds = webSessions.Keys.Union(appSubjectIds)
            .Where(id => Guid.TryParse(id, out _))
            .ToList();

        var userIds = allSubjectIds.Select(Guid.Parse).ToList();

        var query = new UserPortraitsQuery(userIds);
        await eventBus.PublishAsync(query);
        var portraits = query.Result.ToDictionary(u => u.Id);

        var allUsers = allSubjectIds.Select(subjectId =>
        {
            var userId = Guid.Parse(subjectId);
            portraits.TryGetValue(userId, out var portrait);
            webSessions.TryGetValue(subjectId, out var webSession);
            return new OnlineUserDto(
                userId,
                portrait?.Account ?? subjectId,
                portrait?.DisplayName ?? string.Empty,
                portrait?.Avatar ?? string.Empty,
                webSession);
        }).ToList();

        if (!string.IsNullOrWhiteSpace(input.Search))
        {
            var search = input.Search.Trim();
            allUsers = allUsers.Where(u =>
                u.Account.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                u.DisplayName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var total = allUsers.Count;
        var items = allUsers
            .Skip((input.Page - 1) * input.PageSize)
            .Take(input.PageSize)
            .ToList();

        return new PaginationDto<OnlineUserDto>(total, items);
    }

    private record WebOnlineSessionDto(DateTime LoginTime, string? ClientId);

    public async Task KickUserAsync(
        [FromBody] KickUserModel model,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromServices] IConnectionMultiplexer mux,
        [FromServices] AuthDbContext dbContext,
        [FromServices] ILogger<OnlineUserService> logger)
    {
        try
        {
            var client = httpClientFactory.CreateClient("SsoAddr");
            var response = await client.GetAsync($"/LogoutUserAsync?subjectId={model.SubjectId}");
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "SSO LogoutUserAsync returned {StatusCode} for subject {SubjectId}",
                    response.StatusCode, model.SubjectId);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to call SSO LogoutUserAsync for subject {SubjectId}, proceeding with local cleanup",
                model.SubjectId);
        }

        try
        {
            var grants = await dbContext.Set<PersistedGrant>()
                .Where(g => g.SubjectId == model.SubjectId)
                .ToListAsync();
            if (grants.Count > 0)
            {
                dbContext.RemoveRange(grants);
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to remove persisted grants for subject {SubjectId}", model.SubjectId);
        }

        var db = mux.GetDatabase();
        await db.SetRemoveAsync(ONLINE_USERS_KEY, model.SubjectId);
        await db.KeyDeleteAsync(SsoSessionKey(model.SubjectId));
    }
}
