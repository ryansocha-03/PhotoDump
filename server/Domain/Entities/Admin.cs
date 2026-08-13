namespace Domain.Entities;

/// <summary>
/// Entity model defining an Admin.
/// </summary>
public class Admin
{
    public long Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    
    #region Foreign Keys
    
    public long EventId { get; set; }
    
    #endregion
    
    public Event? Event { get; private set; }
}