namespace Notescrib.Api.Core.Entities;

public class User : EntityIdBase
{
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
