﻿namespace Masa.Auth.Service.Admin.Dto.Subjects;

public class StaffItemDto
{
    public Guid Id { get; set; }

    public string Avatar { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string JobNumber { get; set; } = string.Empty;

    public string Position { get; set; } = string.Empty;

    public bool Enabled { get; set; }
}
