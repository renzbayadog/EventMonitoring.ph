using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EventMonitoring.ph.ViewModels; 
using EventMonitoring.ph.Data.Entities; 


namespace EventMonitoring.ph.Data.Repositories
{
    public interface IEventLineRepository : IRepositoryBase<EventLine>
    {
        Task<List<EventLine>> GetAllEventLineQry(EventLineSearch searchInfo);
        Task<EventLine> GetEventLineById(int id);
    }
}