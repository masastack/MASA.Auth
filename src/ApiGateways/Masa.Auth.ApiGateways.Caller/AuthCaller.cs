﻿namespace Masa.Auth.ApiGateways.Caller;

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
    #endregion

    public ThirdPartyIdpService ThirdPartyIdpService => _thirdPartyIdpService ?? (_thirdPartyIdpService = new(CallerProvider));

    public UserService UserService => _userService ?? (_userService = new(CallerProvider));

    public ThirdPartyUserService ThirdPartyUserService => _thirdPartyUserService ?? (_thirdPartyUserService = new(CallerProvider));

    public StaffService StaffService => _staffService ?? (_staffService = new(CallerProvider));

    public TeamService TeamService => _teamService ?? (_teamService = new(CallerProvider));

    public RoleService RoleService => _roleService ?? (_roleService = new(CallerProvider));

    public DepartmentService DepartmentService => _departmentService ?? (_departmentService = new(CallerProvider));

    public PermissionService PermissionService => _permissionService ?? (_permissionService = new(CallerProvider));

    public ProjectService ProjectService => _projectService ?? (_projectService = new(CallerProvider));

    public ClientService ClientService => _clientService ?? (_clientService = new(CallerProvider));

    public IdentityResourceService IdentityResourceService => _identityResourceService ?? (_identityResourceService = new(CallerProvider));

    protected override string BaseAddress { get; set; }

    public override string Name { get; set; }

    public AuthCaller(IServiceProvider serviceProvider, AuthApiOptions options) : base(serviceProvider)
    {
        Name = nameof(AuthCaller);
        BaseAddress = options.AuthServiceBaseAddress;
    }

    protected override IHttpClientBuilder UseHttpClient()
    {
        return base.UseHttpClient().AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();
    }
}

