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
using EventMonitoring.Helpers;



namespace EventMonitoring.ph.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EventAudiencesController : ControllerBase
	{
		private readonly IEventAudienceRepository _eventaudienceRepository;

		public EventAudiencesController(IRepositoryWrapper repoWrapper)
		{
			_eventaudienceRepository = repoWrapper.EventAudience_Repository;
		}

		[HttpGet]
        [Route("List/Page{currPage:int}/PageSize{pageSize:int}")]
        [Route("List")]
		public async Task<IActionResult> GetAllEventAudience([FromQuery] EventAudienceSearch searchInfo,int currPage = 1, int pageSize = 10)
		{
			if(!ModelState.IsValid) return BadRequest();

			List<EventAudience> eventaudiences = await _eventaudienceRepository.GetAllEventAudienceQry(searchInfo);

			// Map entity model to view model
			List<EventAudienceVM> eventaudiencesVM = new List<EventAudienceVM>();
			
			foreach(EventAudience eventaudience in eventaudiences)
			{
				eventaudiencesVM.Add(new EventAudienceVM()
				{
					EventAudienceId = eventaudience.EventAudienceId,
					EventTitleId = eventaudience.EventTitle?.EventTitleId,
					UserId = eventaudience.User.Id,
					QrCode = eventaudience.QrCode,
					EventRemarks = eventaudience.EventRemarks,
					EventTitleVenueName = eventaudience.EventTitle?.EventTitleVenueName,
					EventTitleDescription = eventaudience.EventTitle?.EventTitleDescription,
					EventTitleStartTimeDate = eventaudience.EventTitle?.EventTitleStartTimeDate,
					EventTitleEndTimeDate = eventaudience.EventTitle?.EventTitleEndTimeDate,
					EventTitleStatus = eventaudience.EventTitle.EventTitleStatus,
					FirstName = eventaudience.User?.FirstName,
					MiddleInitial = eventaudience.User?.MiddleInitial,
					LastName = eventaudience.User?.LastName,
					UserName = eventaudience.User?.UserName,
					EmailAddress = eventaudience.User.Email
				});
			}

			Pagination<EventAudienceVM> pagination = new Pagination<EventAudienceVM>(eventaudiencesVM, currPage, pageSize);

			return Ok(pagination);
		}

		[HttpGet]
		[Route("GetById/{id:int}")]
		public async Task<IActionResult> GetEventAudienceById(int id)
		{
			if (id == 0)
			{
				return NotFound("Not Found");
			}

			try
            {
				EventAudience eventaudience = await _eventaudienceRepository.GetEventAudienceById(id);

                if (eventaudience == null)
				{
					return NotFound("Not Found");
				}

				var oeventaudience = new EventAudienceVM()
				{
					EventAudienceId = eventaudience.EventAudienceId,
					EventTitleId = eventaudience.EventTitle?.EventTitleId,
					UserId = eventaudience.User.Id,
					QrCode = eventaudience.QrCode,
					EventRemarks = eventaudience.EventRemarks,
					EventTitleVenueName = eventaudience.EventTitle?.EventTitleVenueName,
					EventTitleDescription = eventaudience.EventTitle?.EventTitleDescription,
					EventTitleStartTimeDate = eventaudience.EventTitle?.EventTitleStartTimeDate,
					EventTitleEndTimeDate = eventaudience.EventTitle?.EventTitleEndTimeDate,
					EventTitleStatus = eventaudience.EventTitle.EventTitleStatus,
					FirstName = eventaudience.User?.FirstName,
					MiddleInitial = eventaudience.User?.MiddleInitial,
					LastName = eventaudience.User?.LastName,
					UserName = eventaudience.User?.UserName,
					EmailAddress = eventaudience.User?.Email
				};

				return Ok(oeventaudience);

            }
            catch (Exception ex)
            {
				AppHelper.LogMessage(ex.InnerException.ToString());
				return BadRequest(ex.InnerException.ToString());
            }
		}

		[HttpPost("Add")]
		public async Task<IActionResult> PostEventAudience(EventAudienceVM eventaudience)
		{
			if (!ModelState.IsValid)
            {
                return BadRequest();
            }

			var cdnftpconfig = AppHelper.CDNFTPConfiguration;
            cdnftpconfig.FTPFolderVM = AppHelper.CDNFTPFolder;
			

			EventAudience eventaudienceToAdd = new EventAudience()
			{
				EventTitleId = eventaudience.EventTitleId,
				UserId = eventaudience.UserId,
				QrCode = eventaudience.QrCode,
				EventRemarks = eventaudience.EventRemarks
			};

            _eventaudienceRepository.Add(eventaudienceToAdd);

            try
            {
                await _eventaudienceRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
				AppHelper.LogMessage(ex.InnerException.ToString());
				return BadRequest(ex.InnerException.ToString());
            }

            return Ok();
		}

		[HttpPost("Update")]
		public async Task<IActionResult> PutEventAudience(EventAudienceVM eventaudience)
		{
			if (!ModelState.IsValid)
            {
                return BadRequest();
            }

			var cdnftpconfig = AppHelper.CDNFTPConfiguration;
            cdnftpconfig.FTPFolderVM = AppHelper.CDNFTPFolder;
			

			EventAudience eventaudienceToUpdate = new EventAudience()
			{
				EventAudienceId = eventaudience.EventAudienceId,
				EventTitleId = eventaudience.EventTitleId,
				UserId = eventaudience.UserId,
				QrCode = eventaudience.QrCode,
				EventRemarks = eventaudience.EventRemarks
			};

            _eventaudienceRepository.Update(eventaudienceToUpdate);

			try
            {
                await _eventaudienceRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
				AppHelper.LogMessage(ex.InnerException.ToString());
				return BadRequest(ex.InnerException.ToString());
            }
            return Ok();
		}

		[HttpDelete("{id}/Delete")]
        public async Task<IActionResult> DeleteEventAudience(int id)
		{
			EventAudience eventaudienceToDelete = await _eventaudienceRepository.FindFirstAsync(m => m.EventAudienceId == id);
			
			if(eventaudienceToDelete == null)
				return BadRequest("Not Found");
			
            _eventaudienceRepository.Delete(eventaudienceToDelete);

			try
			{
                await _eventaudienceRepository.SaveChangesAsync();
			}
			catch (Exception ex)
            {
				AppHelper.LogMessage(ex.InnerException.ToString());
				return BadRequest(ex.InnerException.ToString());
            }
            
			return Ok();
		}

		[HttpPost("delete/bulk")]
        public async Task<IActionResult> DeleteBulk([FromBody]List<int> eventaudiences)
        {
            if (eventaudiences.Count > 0)
            {
                foreach (int eventaudience in eventaudiences)
                {
					EventAudience eventaudienceToDelete = await _eventaudienceRepository.FindFirstAsync(m => m.EventAudienceId == eventaudience);
					_eventaudienceRepository.Delete(eventaudienceToDelete);
                }
				try
				{
					await _eventaudienceRepository.SaveChangesAsync();
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
        public async Task<IActionResult> ExportEventAudience([FromQuery] EventAudienceSearch searchInfo)
        {
			List<EventAudience> eventaudiences = await _eventaudienceRepository.GetAllEventAudienceQry(searchInfo);

			// Map entity model to view model
			List<EventAudienceVM> eventaudiencesVM = new List<EventAudienceVM>();
			
			foreach(EventAudience eventaudience in eventaudiences)
			{
				eventaudiencesVM.Add(new EventAudienceVM()
				{
					EventAudienceId = eventaudience.EventAudienceId,
					EventTitleId = eventaudience.EventTitle?.EventTitleId,
					UserId = eventaudience.User.Id,
					QrCode = eventaudience.QrCode,
					EventRemarks = eventaudience.EventRemarks,
					EventTitleVenueName = eventaudience.EventTitle?.EventTitleVenueName,
					EventTitleDescription = eventaudience.EventTitle?.EventTitleDescription,
					EventTitleStartTimeDate = eventaudience.EventTitle?.EventTitleStartTimeDate,
					EventTitleEndTimeDate = eventaudience.EventTitle?.EventTitleEndTimeDate,
					EventTitleStatus = eventaudience.EventTitle.EventTitleStatus,
					FirstName = eventaudience.User?.FirstName,
					MiddleInitial = eventaudience.User?.MiddleInitial,
					LastName = eventaudience.User?.LastName,
					UserName = eventaudience.User?.UserName,
					EmailAddress = eventaudience.User?.Email
				});
			}
 
            DataTable dt = new DataTable("EventAudience");
            dt.Columns.Add("EventAudienceId", typeof(string));
						dt.Columns.Add("EventTitleId", typeof(string));
						dt.Columns.Add("UserId", typeof(string));
						dt.Columns.Add("QrCode", typeof(string));
						dt.Columns.Add("EventRemarks", typeof(string));
						dt.Columns.Add("EventTitleVenueName", typeof(string));
						dt.Columns.Add("EventTitleDescription", typeof(string));
						dt.Columns.Add("EventTitleStartTimeDate", typeof(string));
						dt.Columns.Add("EventTitleEndTimeDate", typeof(string));
						dt.Columns.Add("EventTitleStatus", typeof(string));
						dt.Columns.Add("FirstName", typeof(string));
						dt.Columns.Add("MiddleInitial", typeof(string));
						dt.Columns.Add("LastName", typeof(string));
						dt.Columns.Add("UserName", typeof(string));
						dt.Columns.Add("EmailAddress", typeof(string));

            DataRow dr;

            foreach (var item in eventaudiencesVM)
            {
                dr = dt.NewRow();

                dr[0] = item.EventAudienceId;
						dr[1] = item.EventTitleId;
						dr[2] = item.UserId;
						dr[3] = item.QrCode;
						dr[4] = item.EventRemarks;
						dr[5] = item.EventTitleVenueName;
						dr[6] = item.EventTitleDescription;
						dr[7] = item.EventTitleStartTimeDate;
						dr[8] = item.EventTitleEndTimeDate;
						dr[9] = item.EventTitleStatus;
						dr[10] = item.FirstName;
						dr[11] = item.MiddleInitial;
						dr[12] = item.LastName;
						dr[13] = item.UserName;
						dr[14] = item.EmailAddress;

                dt.Rows.Add(dr);
            }

            var exportExcelHelperService = new ExportExcelHelper();

            var bytes = Convert.ToBase64String(exportExcelHelperService.CreateExcelWorkBook(dt));

            var data = new ExcelData();
			data.Filename = "EventAudience Extracted Data";
            data.File = bytes;
            return Ok(data);
        }
        #endregion

	}
}