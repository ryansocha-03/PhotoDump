using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Infrastructure.Models;

public class Event
{
    public int Id { get; set; }
    
    public string EventName { get; set; }
    public DateTime EventDate { get; set; }
    public int DurationDays { get; set; }
    public string EventPasswordHash { get; set; }
    
    public int EventStateId { get; set; }
    public EventState EventState { get; set; }
    public int EventTypeId { get; set; }
    public EventType EventType { get; set; }
    public IEnumerable<Guest> Guests { get; set; } = new List<Guest>();
    public IEnumerable<Admin> Admins { get; set; } = new List<Admin>();
    
}