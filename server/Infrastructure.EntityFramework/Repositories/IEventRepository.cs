using Infrastructure.EntityFramework.Models;

namespace Infrastructure.EntityFramework.Repositories;

public interface IEventRepository : IRepository<Event>
{
   Task<Event?> GetByPublicIdAsync(Guid publicId); 
   
   Task<IEnumerable<Guest>> GetGuestListForEventAsync(int  eventId);
   
   Task<IEnumerable<Guest>> GuestListSearchAsync(int eventId, string search);
}