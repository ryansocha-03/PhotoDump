using Infrastructure.EntityFramework.Models;

namespace Infrastructure.EntityFramework.Repositories;

public interface IEventRepository : IRepository<Event>
{
   Task<Event?> GetByPublicIdAsync(Guid publicId); 
}