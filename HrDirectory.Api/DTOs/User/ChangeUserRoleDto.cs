using HrDirectory.Api.Enums;

namespace HrDirectory.Api.DTOs;

public class ChangeUserRoleDTO
{
    public required Role NewRole {get; set;}
}