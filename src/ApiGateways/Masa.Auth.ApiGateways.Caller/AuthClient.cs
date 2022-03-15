﻿namespace Masa.Auth.ApiGateways.Caller;

public class AuthClient
{
    public AuthClient()
    {

    }

    #region ThirdPartyIdp

    List<ThirdPartyIdpItemResponse> PlatformItems = new List<ThirdPartyIdpItemResponse>()
    {
        new ThirdPartyIdpItemResponse(Guid.NewGuid(),"微信","weixin",Guid.NewGuid().ToString(),Guid.NewGuid().ToString(),"/weixin","",VerifyType.OAuth,DateTime.Now,null),
        new ThirdPartyIdpItemResponse(Guid.NewGuid(),"QQ","qq",Guid.NewGuid().ToString(),Guid.NewGuid().ToString(),"/qq","",VerifyType.OAuth,DateTime.Now,null),
    };

    public async Task<ApiResultResponse<List<ThirdPartyIdpItemResponse>>> GetThirdPartyPlatformItemsAsync(GetThirdPartyIdpItemsRequest request)
    {
        return await Task.FromResult(ApiResultResponse<List<ThirdPartyIdpItemResponse>>.ResponseSuccess(PlatformItems, "查询成功"));
    }

    public async Task<ApiResultResponse<ThirdPartyIdpItemResponse>> GetThirdPartyPlatformDetailAsync(Guid id)
    {
        return await Task.FromResult(ApiResultResponse<ThirdPartyIdpItemResponse>.ResponseSuccess(PlatformItems.First(p => p.ThirdPartyIdpId == id), "查询成功"));
    }

    public async Task<ApiResultResponse<List<ThirdPartyIdpItemResponse>>> SelectThirdPartyPlatformAsync()
    {
        return await Task.FromResult(ApiResultResponse<List<ThirdPartyIdpItemResponse>>.ResponseSuccess(PlatformItems, "查询成功"));
    }

    public async Task<ApiResultResponse> AddThirdPartyPlatformAsync(AddThirdPartyIdpRequest request)
    {
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("新增成功"));
    }

    public async Task<ApiResultResponse> EditThirdPartyPlatformAsync(EditThirdPartyIdpRequest request)
    {
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("编辑成功"));
    }

    public async Task<ApiResultResponse> DeleteThirdPartyPlatformAsync(Guid id)
    {
        PlatformItems.Remove(PlatformItems.First(p => p.ThirdPartyIdpId == id));
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("删除成功"));
    }

    #endregion

    #region User

    List<UserItemResponse> UserItems = new List<UserItemResponse>()
    {
        new UserItemResponse(Guid.NewGuid(),"wuweilai","wwl","https://masa-blazor-pro.lonsid.cn/img/avatar/2.svg","362330199508146736","15168440402","数闪科技",true,"15168440402","824255785@qq.com","","","","","","","","",DateTime.Now.AddDays(-1)),
        new UserItemResponse(Guid.NewGuid(),"wujiang","wj","https://masa-blazor-pro.lonsid.cn/img/avatar/2.svg","362330199508146735","15168440403","数闪科技",false,"15168440403","824255783@qq.com","","","","","","","","",DateTime.Now.AddDays(-2)),
    };

    public async Task<ApiResultResponse<List<UserItemResponse>>> GetUserItemsAsync(GetUserItemsRequest request)
    {
        var users = UserItems.Where(u => u.Enabled == request.Enabled && (u.Name.Contains(request.Search) || u.DisplayName.Contains(request.Search) || u.PhoneNumber.Contains(request.Search))).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
        return await Task.FromResult(ApiResultResponse<List<UserItemResponse>>.ResponseSuccess(users, "查询成功"));
    }

    public async Task<ApiResultResponse<UserItemResponse>> GetUserDetailAsync(Guid id)
    {
        return await Task.FromResult(ApiResultResponse<UserItemResponse>.ResponseSuccess(UserItems.First(u => u.UserId == id), "查询成功"));
    }

    public async Task<ApiResultResponse> AddUserAsync(AddUserRequest request)
    {
        UserItems.Add(new UserItemResponse(Guid.NewGuid(), request.Name, request.DisplayName, request.Avatar, request.IDCard, request.PhoneNumber, request.CompanyName, request.Enabled, request.PhoneNumber, request.Email, request.HouseholdRegisterAddress, request.HouseholdRegisterProvinceCode, request.HouseholdRegisterCityCode, request.HouseholdRegisterDistrictCode, request.ResidentialAddress, request.ResidentialProvinceCode, request.ResidentialCityCode, request.ResidentialDistrictCode, DateTime.Now));
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("新增成功"));
    }

    public async Task<ApiResultResponse> EditUserAsync(EditUserRequest request)
    {
        var oldData = UserItems.First(u => request.UserId == request.UserId);
        UserItems.Remove(oldData);
        UserItems.Add(new UserItemResponse(request.UserId, request.Name, request.DisplayName, request.Avatar, oldData.IDCard, oldData.PhoneNumber, request.CompanyName, request.Enabled, oldData.PhoneNumber, request.Email, request.HouseholdRegisterAddress, request.HouseholdRegisterProvinceCode, request.HouseholdRegisterCityCode, request.HouseholdRegisterDistrictCode, request.ResidentialAddress, request.ResidentialProvinceCode, request.ResidentialCityCode, request.ResidentialDistrictCode, oldData.CreationTime));
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("新增成功"));
    }

    public async Task<ApiResultResponse> DeleteUserAsync(Guid userId)
    {
        UserItems.Remove(UserItems.First(u => u.UserId == userId));
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("删除成功"));
    }
    #endregion

    #region ThirdPartyUser

    List<ThirdPartyUserItemResponse> ThirdPartyUserItems => new List<ThirdPartyUserItemResponse>()
    {
        //new ThirdPartyUserItemResponse(Guid.Parse("A446CD5D-B35F-7029-4A30-8232744A3A8E"),PlatformItems[0].ThirdPartyPlatformId,true,UserItems[0],DateTime.Now,DateTime.Now,Guid.Empty),
        //new ThirdPartyUserItemResponse(Guid.Parse("8056549B-7D96-E377-2D03-A27C77837EFB"),PlatformItems[1].ThirdPartyPlatformId,false,UserItems[1],DateTime.Now,DateTime.Now,Guid.Empty),
    };

    public async Task<ApiResultResponse<List<ThirdPartyUserItemResponse>>> GetThirdPartyUserItemsAsync(GetThirdPartyUserItemsRequest request)
    {
        var thirdPartyUsers = ThirdPartyUserItems.Where(tpu => tpu.Enabled == request.Enabled && tpu.ThirdPartyIdpId == request.ThirdPartyPlatformId && (tpu.User.Name.Contains(request.Search) || tpu.User.PhoneNumber.Contains(request.Search) || tpu.User.DisplayName.Contains(request.Search))).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
        return await Task.FromResult(ApiResultResponse<List<ThirdPartyUserItemResponse>>.ResponseSuccess(thirdPartyUsers, "查询成功"));
    }

    public async Task<ApiResultResponse<ThirdPartyUserItemResponse>> GetThirdPartyUserDetailAsync(Guid id)
    {
        return await Task.FromResult(ApiResultResponse<ThirdPartyUserItemResponse>.ResponseSuccess(ThirdPartyUserItems.First(u => u.ThirdPartyIdpId == id), "查询成功"));
    }

    public async Task<ApiResultResponse> AddThirdPartyUserAsync(AddThirdPartyUserRequest request)
    {
        await AddUserAsync(request.User);
        ThirdPartyUserItems.Add(new ThirdPartyUserItemResponse(Guid.NewGuid(), request.ThirdPartyIdpId, request.Enabled, UserItems.First(u => u.PhoneNumber == request.User.PhoneNumber), DateTime.Now, DateTime.Now, Guid.Empty));
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("新增成功"));
    }

    #endregion

    #region Staff

    List<StaffItemResponse> StaffItems => new List<StaffItemResponse>()
    {
        new StaffItemResponse(Guid.Parse("A446CD5D-B35F-7029-4A30-8232744A3A8E"),"","开发工程师","0123",true,StaffTypes.InternalStaff,UserItems[0]),
        new StaffItemResponse(Guid.Parse("8056549B-7D96-E377-2D03-A27C77837EFB"),"","开发工程师","9527",false,StaffTypes.ExternalStaff,UserItems[1]),
    };

    public async Task<ApiResultResponse<List<StaffItemResponse>>> GetStaffItemsAsync(GetStaffItemsRequest request)
    {
        var thirdPartyUsers = StaffItems.Where(s => s.Enabled == request.Enabled && (s.User.Name.Contains(request.Search) || s.User.PhoneNumber.Contains(request.Search) || s.User.DisplayName.Contains(request.Search))).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
        return await Task.FromResult(ApiResultResponse<List<StaffItemResponse>>.ResponseSuccess(thirdPartyUsers, "查询成功"));
    }

    public async Task<ApiResultResponse<StaffDetailResponse>> GetStaffDetailAsync(Guid id)
    {
        return await Task.FromResult(ApiResultResponse<StaffDetailResponse>.ResponseSuccess(StaffDetailResponse.Default, "查询成功"));
    }

    public async Task<ApiResultResponse> AddStaffAsync(AddStaffRequest request)
    {
        await AddUserAsync(request.User);
        StaffItems.Add(new StaffItemResponse(Guid.NewGuid(), "", request.Position, request.JobNumber, request.Enabled, request.StaffType, UserItems.First(u => u.PhoneNumber == request.User.PhoneNumber)));
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("新增成功"));
    }

    public async Task<ApiResultResponse> EditStaffAsync(EditStaffRequest request)
    {
        await EditUserAsync(request.User);
        var oldData = StaffItems.First(s => s.StaffId == request.StaffId);
        StaffItems.Remove(oldData);
        StaffItems.Add(new StaffItemResponse(request.StaffId, "", request.Position, request.JobNumber, request.Enabled, request.StaffType, UserItems.First(u => u.UserId == request.User.UserId)));
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("编辑成功"));
    }

    public async Task<ApiResultResponse> DeleteStaffAsync(Guid staffId)
    {
        StaffItems.Remove(StaffItems.First(s => s.StaffId == staffId));
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("删除成功"));
    }
    #endregion

    #region Role

    List<RoleItemResponse> Roles = new List<RoleItemResponse>
    {
        new RoleItemResponse(Guid.NewGuid(),"admin","00001",1,"admin Number One",true,DateTime.Now,DateTime.Now,"wwl","wwl"),
        new RoleItemResponse(Guid.NewGuid(),"student","10001",1,"student Number One",true,DateTime.Now,DateTime.Now,"wwl","wwl"),
    };

    public async Task<ApiResultResponse<List<RoleItemResponse>>> GetRoleItemsAsync(GetRoleItemsRequest request)
    {
        return await Task.FromResult(ApiResultResponse<List<RoleItemResponse>>.ResponseSuccess(Roles, "查询成功"));
    }

    public async Task<ApiResultResponse<RoleDetailResponse>> GetRoleDetailAsync(Guid id)
    {
        return await Task.FromResult(ApiResultResponse<RoleDetailResponse>.ResponseSuccess(default!, "查询成功"));
    }

    public async Task<ApiResultResponse<List<RoleItemResponse>>> SelectRolesAsync()
    {
        return await Task.FromResult(ApiResultResponse<List<RoleItemResponse>>.ResponseSuccess(Roles, "查询成功"));
    }

    public async Task<ApiResultResponse> AddRoleAsync(AddRoleRequest request)
    {
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("新增成功"));
    }

    public async Task<ApiResultResponse> EditRoleAsync(EditRoleRequest request)
    {
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("编辑成功"));
    }

    #endregion

    #region Team

    List<TeamItemResponse> Teams = new List<TeamItemResponse>
    {
        new TeamItemResponse(Guid.NewGuid(),"Masa Stack","","Masa Stack Number One","cyy","","cyy",DateTime.Now.AddYears(-1)),
        new TeamItemResponse(Guid.NewGuid(),"Lonsid","","Lonsid Number One","zjc","","zjc",DateTime.Now.AddYears(-10)),
    };

    public async Task<ApiResultResponse<List<TeamItemResponse>>> GetTeamItemsAsync()
    {
        return await Task.FromResult(ApiResultResponse<List<TeamItemResponse>>.ResponseSuccess(Teams, "查询成功"));
    }

    public async Task<ApiResultResponse<TeamDetailResponse>> GetTeamDetailAsync(Guid id)
    {
        return await Task.FromResult(ApiResultResponse<TeamDetailResponse>.ResponseSuccess(default!, "查询成功"));
    }

    public async Task<ApiResultResponse<List<TeamItemResponse>>> SelectTeamAsync()
    {
        return await Task.FromResult(ApiResultResponse<List<TeamItemResponse>>.ResponseSuccess(Teams, "查询成功"));
    }

    public async Task<ApiResultResponse> AddTeamAsync(AddTeamRequest request)
    {
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("新增成功"));
    }

    public async Task<ApiResultResponse> EditTeamAsync(EditTeamRequest request)
    {
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("编辑成功"));
    }

    #endregion
}

