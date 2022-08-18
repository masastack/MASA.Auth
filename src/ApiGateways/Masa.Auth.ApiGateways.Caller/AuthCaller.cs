// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller;

public class AuthCaller : HttpClientCallerBase
{
    #region Field
    ThirdPartyIdpService? _thirdPartyIdpService;
    UserService? _userService;
    ThirdPartyUserService? _thirdPartyUserService;
    StaffService? _staffService;
    TeamService? _teamService;
    RoleService? _roleService;
    DepartmentService? _departmentService;
    PermissionService? _permissionService;
    ProjectService? _projectService;
    ClientService? _clientService;
    IdentityResourceService? _identityResourceService;
    ApiScopeService? _apiScopeService;
    ApiResourceService? _apiResourceService;
    UserClaimService? _userClaimService;
    CustomLoginService? _customLoginService;
    PositionService? _positionService;
    OssService? _ossService;
    OperationLogService? _operationLogService;
    ITokenProvider _tokenProvider;
    ILogger<AuthCaller> _logger;
    #endregion

    public ThirdPartyIdpService ThirdPartyIdpService => _thirdPartyIdpService ?? (_thirdPartyIdpService = new(Caller));

    public UserService UserService => _userService ?? (_userService = new(Caller));

    public ThirdPartyUserService ThirdPartyUserService => _thirdPartyUserService ?? (_thirdPartyUserService = new(Caller));

    public StaffService StaffService => _staffService ?? (_staffService = new(Caller));

    public TeamService TeamService => _teamService ?? (_teamService = new(Caller));

    public RoleService RoleService => _roleService ?? (_roleService = new(Caller));

    public DepartmentService DepartmentService => _departmentService ?? (_departmentService = new(Caller));

    public PermissionService PermissionService => _permissionService ?? (_permissionService = new(Caller));

    public ProjectService ProjectService => _projectService ?? (_projectService = new(Caller));

    public ClientService ClientService => _clientService ?? (_clientService = new(Caller));

    public IdentityResourceService IdentityResourceService => _identityResourceService ?? (_identityResourceService = new(Caller));

    public ApiScopeService ApiScopeService => _apiScopeService ?? (_apiScopeService = new(Caller));

    public ApiResourceService ApiResourceService => _apiResourceService ?? (_apiResourceService = new(Caller));

    public UserClaimService UserClaimService => _userClaimService ?? (_userClaimService = new(Caller));

    public CustomLoginService CustomLoginService => _customLoginService ?? (_customLoginService = new(Caller));

    public PositionService PositionService => _positionService ?? (_positionService = new(Caller));

    public OssService OssService => _ossService ?? (_ossService = new OssService(Caller));

    public OperationLogService OperationLogService => _operationLogService ?? (_operationLogService = new OperationLogService(Caller));

    protected override string BaseAddress { get; set; }

    public override string Name { get; set; }

    public AuthCaller(
        IServiceProvider serviceProvider,
        ITokenProvider tokenProvider,
        ILogger<AuthCaller> logger,
        AuthApiOptions options) : base(serviceProvider)
    {
        Name = "AuthCaller";
        _tokenProvider = tokenProvider;
        _logger = logger;
        BaseAddress = options.AuthServiceBaseAddress;
    }

    protected override void ConfigHttpRequestMessage(HttpRequestMessage requestMessage)
    {
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
        base.ConfigHttpRequestMessage(requestMessage);
    }
}

