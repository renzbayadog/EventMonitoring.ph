using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace EventMonitoring.ph.ViewModels
{
   public class EventLineVM
   {
		[Display(Name = "Event Line Id")]
		public int EventLineId { get; set; }

		[Display(Name = "Event Line Name*")]
		[Required(ErrorMessage = "Event Line Name is required")]
		[MaxLength(255)]
		public string EventLineName { get; set; }
   }

   public class EventLineSearch
   {
        public string Keyword { get; set; }
		public string SortOrder { get; set; }
		public string EventLineName { get; set; }
   }
}