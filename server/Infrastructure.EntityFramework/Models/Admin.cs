namespace Infrastructure.EntityFramework.Models;

public class Admin
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    
    public int EventId { get; set; }
    public Event Event { get; set; }
}