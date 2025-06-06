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
	public class EventTitlesController : ControllerBase
	{
		private readonly IEventTitleRepository _eventtitleRepository;

		public EventTitlesController(IRepositoryWrapper repoWrapper)
		{
			_eventtitleRepository = repoWrapper.EventTitle_Repository;
		}

		[HttpGet]
        [Route("List/Page{currPage:int}/PageSize{pageSize:int}")]
        [Route("List")]
		public async Task<IActionResult> GetAllEventTitle([FromQuery] EventTitleSearch searchInfo,int currPage = 1, int pageSize = 10)
		{
			if(!ModelState.IsValid) return BadRequest();

			List<EventTitle> eventtitles = await _eventtitleRepository.GetAllEventTitleQry(searchInfo);

			// Map entity model to view model
			List<EventTitleVM> eventtitlesVM = new List<EventTitleVM>();
			
			foreach(EventTitle eventtitle in eventtitles)
			{
				eventtitlesVM.Add(new EventTitleVM()
				{
					EventTitleId = eventtitle.EventTitleId,
					EventTitleVenueName = eventtitle.EventTitleVenueName,
					EventTitleDescription = eventtitle.EventTitleDescription,
					EventTitleStartTimeDate = eventtitle.EventTitleStartTimeDate,
					EventTitleEndTimeDate = eventtitle.EventTitleEndTimeDate,
					EventTitleStatus = eventtitle.EventTitleStatus,
					EventLineId = eventtitle.EventLine?.EventLineId,
					EventLineName = eventtitle.EventLine?.EventLineName
				});
			}

			Pagination<EventTitleVM> pagination = new Pagination<EventTitleVM>(eventtitlesVM, currPage, pageSize);

			return Ok(pagination);
		}

		[HttpGet]
		[Route("GetById/{id:int}")]
		public async Task<IActionResult> GetEventTitleById(int id)
		{
			if (id == 0)
			{
				return NotFound("Not Found");
			}

			try
            {
				EventTitle eventtitle = await _eventtitleRepository.GetEventTitleById(id);

                if (eventtitle == null)
				{
					return NotFound("Not Found");
				}

				var oeventtitle = new EventTitleVM()
				{
					EventTitleId = eventtitle.EventTitleId,
					EventTitleVenueName = eventtitle.EventTitleVenueName,
					EventTitleDescription = eventtitle.EventTitleDescription,
					EventTitleStartTimeDate = eventtitle.EventTitleStartTimeDate,
					EventTitleEndTimeDate = eventtitle.EventTitleEndTimeDate,
					EventTitleStatus = eventtitle.EventTitleStatus,
					EventLineId = eventtitle.EventLine?.EventLineId,
					EventLineName = eventtitle.EventLine?.EventLineName
				};

				return Ok(oeventtitle);

            }
            catch (Exception ex)
            {
				AppHelper.LogMessage(ex.InnerException.ToString());
				return BadRequest(ex.InnerException.ToString());
            }
		}

		[HttpPost("Add")]
		public async Task<IActionResult> PostEventTitle(EventTitleVM eventtitle)
		{
			if (!ModelState.IsValid)
            {
                return BadRequest();
            }

			var cdnftpconfig = AppHelper.CDNFTPConfiguration;
            cdnftpconfig.FTPFolderVM = AppHelper.CDNFTPFolder;
			

			EventTitle eventtitleToAdd = new EventTitle()
			{
				EventTitleVenueName = eventtitle.EventTitleVenueName,
				EventTitleDescription = eventtitle.EventTitleDescription,
				EventTitleStartTimeDate = eventtitle.EventTitleStartTimeDate,
				EventTitleEndTimeDate = eventtitle.EventTitleEndTimeDate,
				EventTitleStatus = eventtitle.EventTitleStatus,
				EventLineId = eventtitle.EventLineId
			};

            _eventtitleRepository.Add(eventtitleToAdd);

            try
            {
                await _eventtitleRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
				AppHelper.LogMessage(ex.InnerException.ToString());
				return BadRequest(ex.InnerException.ToString());
            }

            return Ok();
		}

		[HttpPost("Update")]
		public async Task<IActionResult> PutEventTitle(EventTitleVM eventtitle)
		{
			if (!ModelState.IsValid)
            {
                return BadRequest();
            }

			var cdnftpconfig = AppHelper.CDNFTPConfiguration;
            cdnftpconfig.FTPFolderVM = AppHelper.CDNFTPFolder;
			

			EventTitle eventtitleToUpdate = new EventTitle()
			{
				EventTitleId = eventtitle.EventTitleId,
				EventTitleVenueName = eventtitle.EventTitleVenueName,
				EventTitleDescription = eventtitle.EventTitleDescription,
				EventTitleStartTimeDate = eventtitle.EventTitleStartTimeDate,
				EventTitleEndTimeDate = eventtitle.EventTitleEndTimeDate,
				EventTitleStatus = eventtitle.EventTitleStatus,
				EventLineId = eventtitle.EventLineId
			};

            _eventtitleRepository.Update(eventtitleToUpdate);

			try
            {
                await _eventtitleRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
				AppHelper.LogMessage(ex.InnerException.ToString());
				return BadRequest(ex.InnerException.ToString());
            }
            return Ok();
		}

		[HttpDelete("{id}/Delete")]
        public async Task<IActionResult> DeleteEventTitle(int id)
		{
			EventTitle eventtitleToDelete = await _eventtitleRepository.FindFirstAsync(m => m.EventTitleId == id);
			
			if(eventtitleToDelete == null)
				return BadRequest("Not Found");
			
            _eventtitleRepository.Delete(eventtitleToDelete);

			try
			{
                await _eventtitleRepository.SaveChangesAsync();
			}
			catch (Exception ex)
            {
				AppHelper.LogMessage(ex.InnerException.ToString());
				return BadRequest(ex.InnerException.ToString());
            }
            
			return Ok();
		}

		[HttpPost("delete/bulk")]
        public async Task<IActionResult> DeleteBulk([FromBody]List<int> eventtitles)
        {
            if (eventtitles.Count > 0)
            {
                foreach (int eventtitle in eventtitles)
                {
					EventTitle eventtitleToDelete = await _eventtitleRepository.FindFirstAsync(m => m.EventTitleId == eventtitle);
					_eventtitleRepository.Delete(eventtitleToDelete);
                }
				try
				{
					await _eventtitleRepository.SaveChangesAsync();
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
        public async Task<IActionResult> ExportEventTitle([FromQuery] EventTitleSearch searchInfo)
        {
			List<EventTitle> eventtitles = await _eventtitleRepository.GetAllEventTitleQry(searchInfo);

			// Map entity model to view model
			List<EventTitleVM> eventtitlesVM = new List<EventTitleVM>();
			
			foreach(EventTitle eventtitle in eventtitles)
			{
				eventtitlesVM.Add(new EventTitleVM()
				{
					EventTitleId = eventtitle.EventTitleId,
					EventTitleVenueName = eventtitle.EventTitleVenueName,
					EventTitleDescription = eventtitle.EventTitleDescription,
					EventTitleStartTimeDate = eventtitle.EventTitleStartTimeDate,
					EventTitleEndTimeDate = eventtitle.EventTitleEndTimeDate,
					EventTitleStatus = eventtitle.EventTitleStatus,
					EventLineId = eventtitle.EventLine?.EventLineId,
					EventLineName = eventtitle.EventLine?.EventLineName
				});
			}
 
            DataTable dt = new DataTable("EventTitle");
            dt.Columns.Add("EventTitleId", typeof(string));
						dt.Columns.Add("EventTitleVenueName", typeof(string));
						dt.Columns.Add("EventTitleDescription", typeof(string));
						dt.Columns.Add("EventTitleStartTimeDate", typeof(string));
						dt.Columns.Add("EventTitleEndTimeDate", typeof(string));
						dt.Columns.Add("EventTitleStatus", typeof(string));
						dt.Columns.Add("EventLineId", typeof(string));
						dt.Columns.Add("EventLineName", typeof(string));

            DataRow dr;

            foreach (var item in eventtitlesVM)
            {
                dr = dt.NewRow();

                dr[0] = item.EventTitleId;
						dr[1] = item.EventTitleVenueName;
						dr[2] = item.EventTitleDescription;
						dr[3] = item.EventTitleStartTimeDate;
						dr[4] = item.EventTitleEndTimeDate;
						dr[5] = item.EventTitleStatus;
						dr[6] = item.EventLineId;
						dr[7] = item.EventLineName;

                dt.Rows.Add(dr);
            }

            var exportExcelHelperService = new ExportExcelHelper();

            var bytes = Convert.ToBase64String(exportExcelHelperService.CreateExcelWorkBook(dt));

            var data = new ExcelData();
			data.Filename = "EventTitle Extracted Data";
            data.File = bytes;
            return Ok(data);
        }
        #endregion

	}
}