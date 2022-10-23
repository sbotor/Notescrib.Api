namespace Notescrib.Api.Core.Entities;

public class User : EntityIdBase
{
    public string Email { get; set; } = null!;
    public bool IsActive { get; set; }
}
