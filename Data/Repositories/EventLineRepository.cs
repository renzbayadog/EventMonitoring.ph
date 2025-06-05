using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EventMonitoring.ph.ViewModels; 
using EventMonitoring.ph.Data.Entities; 


namespace EventMonitoring.ph.Data.Repositories
{
    internal class EventLineRepository : RepositoryBase<EventLine>, IEventLineRepository
    {
        private readonly db_ab9d6a_dbrenzContext _context;

        public EventLineRepository(db_ab9d6a_dbrenzContext context) : base(context)
        {
            _context = context;
        }
  
        public async Task<List<EventLine>> GetAllEventLineQry(EventLineSearch searchInfo)
        {
            List<EventLine> eventlines = await _context.EventLines
						
						.AsNoTracking().ToListAsync();
			if (!String.IsNullOrEmpty(searchInfo.Keyword))
			{
				
				eventlines = eventlines.Where(e => 
									e.EventLineName.ToString().ToUpper().Contains(searchInfo.Keyword.ToUpper())
									)
								.ToList();
			}

				//.Where(f => searchInfo.DateFrom == null || f.CreateDate >= searchInfo.DateFrom)
				//.Where(f => searchInfo.DateTo == null || f.CreateDate <= searchInfo.DateTo)
				//.OrderByDescending(s => s.CreateDate).ToList();

				//if (!String.IsNullOrEmpty(searchInfo.SortOrder))
				//{
				//	var sortCurrent = searchInfo.SortOrder.Split("_").Last();
				//	var sortCurrent = searchInfo.SortOrder.Split("_").First();
				//	if (sortCurrent.Equals("DESC"))
				//	{
				//		products.OrderByDescending(a=>a.)
				//	}
				//}
            
            return eventlines;
        }

		public async Task<EventLine> GetEventLineById(int id)
        {
            EventLine eventline = await _context.EventLines
                        
                        .AsNoTracking().FirstOrDefaultAsync(m => m.EventLineId == id);

            return eventline;
        }

    }
}