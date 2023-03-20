// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects
{
    public class TeamCacheCommandHandler
    {
        readonly IMultilevelCacheClient _multilevelCacheClient;
        readonly ITeamRepository _teamRepository;

        public TeamCacheCommandHandler(AuthClientMultilevelCacheProvider authClientMultilevelCacheProvider, ITeamRepository teamRepository)
        {
            _multilevelCacheClient = authClientMultilevelCacheProvider.GetMultilevelCacheClient();
            _teamRepository = teamRepository;
        }

        [EventHandler(99)]
        public async Task AddTeamAsync(AddTeamCommand command)
        {
            await SetTeamCacheAsync(command.Result);
        }

        [EventHandler(99)]
        public async Task UpdateTeamAsync(UpdateTeamCommand command)
        {
            await SetTeamCacheAsync(command.UpdateTeamDto.Id);
        }

        [EventHandler(99)]
        public async Task RemoveTeamAsync(RemoveTeamCommand command)
        {
            var teamCache = await _multilevelCacheClient.GetAsync<Team>(CacheKey.TeamKey(command.TeamId));
            await _multilevelCacheClient.RemoveAsync<Team>(CacheKey.TeamKey(command.TeamId));

            if (teamCache != null)
            {
                var userIdKeys = teamCache!.TeamStaffs.Select(e => CacheKey.UserTeamKey(e.UserId));
                var userTeamCacheDic = await _multilevelCacheClient.GetListAsync<KeyValuePair<string, List<Team>>>(userIdKeys);
                foreach (var item in userTeamCacheDic)
                {
                    var userTeamCache = item.Value.FirstOrDefault(e => e.Id == teamCache.Id);
                    if (userTeamCache!=null)
                    {
                        item.Value.Remove(userTeamCache);
                    }
                }
                await _multilevelCacheClient.SetListAsync(userTeamCacheDic.ToDictionary(e => e.Key, e => e.Value)!);
            }
        }

        private async Task SetTeamCacheAsync(Guid teamId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);

            await _multilevelCacheClient.SetAsync(CacheKey.TeamKey(teamId), team);

            var userIdKeys = team.TeamStaffs.Select(e => CacheKey.UserTeamKey(e.UserId));
            var userTeamCacheDic = await _multilevelCacheClient.GetListAsync<KeyValuePair<string, List<Team>>>(userIdKeys);
            userTeamCacheDic = userTeamCacheDic ?? new Dictionary<string, List<Team>>();
            foreach (var item in team.TeamStaffs)
            {
                var userTeamCache = userTeamCacheDic.FirstOrDefault(e => e.Key == CacheKey.UserTeamKey(item.UserId));
                userTeamCache = userTeamCache.Value == null ? new KeyValuePair<string, List<Team>>(userTeamCache.Key, new List<Team>()) : userTeamCache;
                if (!userTeamCache.Value.Any(e => e.Id == team.Id))
                {
                    userTeamCache.Value.Add(team);
                }
            }
            await _multilevelCacheClient.SetListAsync(userTeamCacheDic.ToDictionary(e => e.Key, e => e.Value)!);
        }
    }
}
