using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

using EventMonitoring.ph.ViewModels; 
using EventMonitoring.ph.Data.Entities; 
using EventMonitoring.ph.Data.Repositories; 
using codegeneratorlib.Helpers; 



namespace EventMonitoring.ph.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EventLinesController : ControllerBase
	{
		private readonly IEventLineRepository _eventlineRepository;

		public EventLinesController(IRepositoryWrapper repoWrapper)
		{
			_eventlineRepository = repoWrapper.EventLine_Repository;
		}

		[HttpGet]
        [Route("List/Page{currPage:int}/PageSize{pageSize:int}")]
        [Route("List")]
		public async Task<IActionResult> GetAllEventLine([FromQuery] EventLineSearch searchInfo,int currPage = 1, int pageSize = 10)
		{
			if(!ModelState.IsValid) return BadRequest();

			List<EventLine> eventlines = await _eventlineRepository.GetAllEventLineQry(searchInfo);

			// Map entity model to view model
			List<EventLineVM> eventlinesVM = new List<EventLineVM>();
			
			foreach(EventLine eventline in eventlines)
			{
				eventlinesVM.Add(new EventLineVM()
				{
					EventLineId = eventline.EventLineId,
					EventLineName = eventline.EventLineName
				});
			}

			Pagination<EventLineVM> pagination = new Pagination<EventLineVM>(eventlinesVM, currPage, pageSize);

			return Ok(pagination);
		}

		[HttpGet]
		[Route("GetById/{id:int}")]
		public async Task<IActionResult> GetEventLineById(int id)
		{
			if (id == 0)
			{
				return NotFound("Not Found");
			}

			try
            {
				EventLine eventline = await _eventlineRepository.GetEventLineById(id);

                if (eventline == null)
				{
					return NotFound("Not Found");
				}

				var oeventline = new EventLineVM()
				{
					EventLineId = eventline.EventLineId,
					EventLineName = eventline.EventLineName
				};

				return Ok(oeventline);

            }
            catch (Exception ex)
            {
				AppHelper.LogMessage(ex.InnerException.ToString());
				return BadRequest(ex.InnerException.ToString());
            }
		}

		[HttpPost("Add")]
		public async Task<IActionResult> PostEventLine(EventLineVM eventline)
		{
			if (!ModelState.IsValid)
            {
                return BadRequest();
            }

			var cdnftpconfig = AppHelper.CDNFTPConfiguration;
            cdnftpconfig.FTPFolderVM = AppHelper.CDNFTPFolder;
			

			EventLine eventlineToAdd = new EventLine()
			{
				EventLineName = eventline.EventLineName
			};

            _eventlineRepository.Add(eventlineToAdd);

            try
            {
                await _eventlineRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
				AppHelper.LogMessage(ex.InnerException.ToString());
				return BadRequest(ex.InnerException.ToString());
            }

            return Ok();
		}

		[HttpPost("Update")]
		public async Task<IActionResult> PutEventLine(EventLineVM eventline)
		{
			if (!ModelState.IsValid)
            {
                return BadRequest();
            }

			var cdnftpconfig = AppHelper.CDNFTPConfiguration;
            cdnftpconfig.FTPFolderVM = AppHelper.CDNFTPFolder;
			

			EventLine eventlineToUpdate = new EventLine()
			{
				EventLineId = eventline.EventLineId,
				EventLineName = eventline.EventLineName
			};

            _eventlineRepository.Update(eventlineToUpdate);

			try
            {
                await _eventlineRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
				AppHelper.LogMessage(ex.InnerException.ToString());
				return BadRequest(ex.InnerException.ToString());
            }
            return Ok();
		}

		[HttpDelete("{id}/Delete")]
        public async Task<IActionResult> DeleteEventLine(int id)
		{
			EventLine eventlineToDelete = await _eventlineRepository.FindFirstAsync(m => m.EventLineId == id);
			
			if(eventlineToDelete == null)
				return BadRequest("Not Found");
			
            _eventlineRepository.Delete(eventlineToDelete);

			try
			{
                await _eventlineRepository.SaveChangesAsync();
			}
			catch (Exception ex)
            {
				AppHelper.LogMessage(ex.InnerException.ToString());
				return BadRequest(ex.InnerException.ToString());
            }
            
			return Ok();
		}

		[HttpPost("delete/bulk")]
        public async Task<IActionResult> DeleteBulk([FromBody]List<int> eventlines)
        {
            if (eventlines.Count > 0)
            {
                foreach (int eventline in eventlines)
                {
					EventLine eventlineToDelete = await _eventlineRepository.FindFirstAsync(m => m.EventLineId == eventline);
					_eventlineRepository.Delete(eventlineToDelete);
                }
				try
				{
					await _eventlineRepository.SaveChangesAsync();
				}
				catch (Exception ex)
				{
					AppHelper.LogMessage(ex.InnerException.ToString());
					return BadRequest(ex.InnerException.ToString());
				}
            }
            return Ok();
        }

		#region EXPORT TO EXCEL

        [HttpGet("export/report")]
        public async Task<IActionResult> ExportEventLine([FromQuery] EventLineSearch searchInfo)
			{
			List<EventLine> eventlines = await _eventlineRepository.GetAllEventLineQry(searchInfo);

			// Map entity model to view model
			List<EventLineVM> eventlinesVM = new List<EventLineVM>();
			
			foreach(EventLine eventline in eventlines)
			{
				eventlinesVM.Add(new EventLineVM()
				{
					EventLineId = eventline.EventLineId,
					EventLineName = eventline.EventLineName
				});
			}
 
            DataTable dt = new DataTable("EventLine");
            dt.Columns.Add("EventLineId", typeof(string));
						dt.Columns.Add("EventLineName", typeof(string));

            DataRow dr;

            foreach (var item in eventlinesVM)
            {
                dr = dt.NewRow();

                dr[0] = item.EventLineId;
			    dr[1] = item.EventLineName;

                dt.Rows.Add(dr);
            }

            var exportExcelHelperService = new ExportExcelHelper();

            var bytes = Convert.ToBase64String(exportExcelHelperService.CreateExcelWorkBook(dt));

            var data = new ExcelData();
			data.Filename = "EventLine Extracted Data";
            data.File = bytes;
            return Ok(data);
        }
        #endregion

	}
}