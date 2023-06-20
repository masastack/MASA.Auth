// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization;

public partial class CopyOrgSheet
{
    [Parameter]
    public CopyDepartmentDto Dto { get; set; } = new();

    [Parameter]
    public List<DepartmentDto> Departments { get; set; } = new();

    [Parameter]
    public EventCallback<CopyDepartmentDto> OnSubmit { get; set; }

    StringNumber _step = 1;
    bool _visible, _staffLoading;
    string _search = "";
    List<StaffSelectDto> _staffs = new();
    List<StaffDto> _sourceStaffs = new();

    protected StaffService StaffService => AuthCaller.StaffService;

    protected override void OnInitialized()
    {
        PageName = "OrganizationBlock";
        base.OnInitialized();
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.TryGetValue(nameof(Dto), out CopyDepartmentDto? dto) && dto != Dto)
        {
            _sourceStaffs.AddRange(dto!.Staffs);
        }
        return base.SetParametersAsync(parameters);
    }

    private void NextStep()
    {
        _step = 2;
        if (!Dto.MigrateStaff)
        {
            Dto.Staffs.Clear();
        }
    }

    private async Task OnSubmitHandler()
    {
        if (OnSubmit.HasDelegate)
        {
            await OnSubmit.InvokeAsync(Dto);
        }
        _visible = false;
    }

    private void Return()
    {
        _step = 1;
        Dto.Staffs.Clear();
        Dto.Staffs.AddRange(_sourceStaffs);
    }

    private void RemoveStaff(StaffDto staffDto)
    {
        Dto.Staffs.Remove(staffDto);
        _staffs.Add(new StaffSelectDto(staffDto.Id, staffDto.JobNumber, staffDto.Name, staffDto.DisplayName, staffDto.Avatar, staffDto.Email, staffDto.PhoneNumber));
    }

    private void AddStaff(StaffSelectDto staffDto)
    {
        _staffs.Remove(staffDto);
        Dto.Staffs.Add(new StaffDto
        {
            Id = staffDto.Id,
            Name = staffDto.Name,
            Avatar = staffDto.Avatar,
            JobNumber = staffDto.JobNumber,
            DisplayName = staffDto.DisplayName,
            Email = staffDto.Email,
            PhoneNumber = staffDto.PhoneNumber
        });
    }

    public void Show(CopyDepartmentDto model)
    {
        Dto = model;
        _sourceStaffs.AddRange(Dto.Staffs);
        Dto.Name = model.Name + " " + T("Duplicate");
        _step = 1;
        _staffs = new();
        _visible = true;
    }

    private async Task QueryStaff(string search)
    {
        search = search.Trim(' ');
        _search = search;
        await Task.Delay(300);
        if (search != _search)
        {
            return;
        }

        _staffLoading = true;
        _staffs = await StaffService.GetSelectAsync(search);
        _staffs.RemoveAll(s => Dto.Staffs.Any(ss => ss.Id == s.Id));
        _staffLoading = false;
    }
}