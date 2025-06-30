// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

/// <summary>
/// Data masking extension methods
/// </summary>
public static class DataMaskingExtensions
{
    /// <summary>
    /// Apply data masking to UserDto
    /// </summary>
    /// <param name="userDto">User DTO</param>
    /// <returns>Masked user DTO</returns>
    public static UserDto ApplyDataMasking(this UserDto userDto)
    {
        userDto.Account = DataMaskingHelper.MaskAccount(userDto.Account) ?? userDto.Account;
        userDto.PhoneNumber = DataMaskingHelper.MaskPhoneNumber(userDto.PhoneNumber);
        userDto.IdCard = DataMaskingHelper.MaskIdCard(userDto.IdCard);
        userDto.Email = DataMaskingHelper.MaskEmail(userDto.Email);
        userDto.Landline = DataMaskingHelper.MaskPhoneNumber(userDto.Landline);

        return userDto;
    }

    /// <summary>
    /// Apply data masking to UserDto list
    /// </summary>
    /// <param name="userDtos">User DTO list</param>
    /// <returns>Masked user DTO list</returns>
    public static List<UserDto> ApplyDataMasking(this List<UserDto> userDtos)
    {
        foreach (var userDto in userDtos)
        {
            userDto.ApplyDataMasking();
        }

        return userDtos;
    }

    /// <summary>
    /// Apply data masking to UserSelectModel
    /// </summary>
    /// <param name="userSelectModel">User select model</param>
    /// <returns>Masked user select model</returns>
    public static UserSelectModel ApplyDataMasking(this UserSelectModel userSelectModel)
    {
        userSelectModel.Account = DataMaskingHelper.MaskAccount(userSelectModel.Account) ?? userSelectModel.Account;
        userSelectModel.PhoneNumber = DataMaskingHelper.MaskPhoneNumber(userSelectModel.PhoneNumber);
        userSelectModel.Email = DataMaskingHelper.MaskEmail(userSelectModel.Email);

        return userSelectModel;
    }

    /// <summary>
    /// Apply data masking to UserSelectModel list
    /// </summary>
    /// <param name="userSelectModels">User select model list</param>
    /// <returns>Masked user select model list</returns>
    public static List<UserSelectModel> ApplyDataMasking(this List<UserSelectModel> userSelectModels)
    {
        foreach (var userSelectModel in userSelectModels)
        {
            userSelectModel.ApplyDataMasking();
        }

        return userSelectModels;
    }

    /// <summary>
    /// Apply data masking to paginated UserDto
    /// </summary>
    /// <param name="paginationDto">Pagination DTO</param>
    /// <returns>Masked pagination DTO</returns>
    public static PaginationDto<UserDto> ApplyDataMasking(this PaginationDto<UserDto> paginationDto)
    {
        if (paginationDto.Items != null)
        {
            paginationDto.Items.ApplyDataMasking();
        }

        return paginationDto;
    }

    /// <summary>
    /// Apply data masking to paginated UserSelectModel
    /// </summary>
    /// <param name="paginationDto">Pagination DTO</param>
    /// <returns>Masked pagination DTO</returns>
    public static PaginationDto<UserSelectModel> ApplyDataMasking(this PaginationDto<UserSelectModel> paginationDto)
    {
        if (paginationDto.Items != null)
        {
            paginationDto.Items.ApplyDataMasking();
        }
        return paginationDto;
    }

    /// <summary>
    /// Apply data masking to StaffDto
    /// </summary>
    /// <param name="staffDto">Staff DTO</param>
    /// <returns>Masked staff DTO</returns>
    public static StaffDto ApplyDataMasking(this StaffDto staffDto)
    {
        staffDto.Account = DataMaskingHelper.MaskAccount(staffDto.Account) ?? "";
        staffDto.PhoneNumber = DataMaskingHelper.MaskPhoneNumber(staffDto.PhoneNumber) ?? "";
        staffDto.IdCard = DataMaskingHelper.MaskIdCard(staffDto.IdCard) ?? "";
        staffDto.Email = DataMaskingHelper.MaskEmail(staffDto.Email) ?? "";

        return staffDto;
    }

    /// <summary>
    /// Apply data masking to StaffDto list
    /// </summary>
    /// <param name="staffDtos">Staff DTO list</param>
    /// <returns>Masked staff DTO list</returns>
    public static List<StaffDto> ApplyDataMasking(this List<StaffDto> staffDtos)
    {
        foreach (var staffDto in staffDtos)
        {
            staffDto.ApplyDataMasking();
        }

        return staffDtos;
    }

    /// <summary>
    /// Apply data masking to paginated StaffDto
    /// </summary>
    /// <param name="paginationDto">Pagination DTO</param>
    /// <returns>Masked pagination DTO</returns>
    public static PaginationDto<StaffDto> ApplyDataMasking(this PaginationDto<StaffDto> paginationDto)
    {
        if (paginationDto.Items != null)
        {
            paginationDto.Items.ApplyDataMasking();
        }

        return paginationDto;
    }
}