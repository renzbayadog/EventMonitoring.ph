using System;
using System.Collections.Generic;

namespace EventMonitoring.ph.Data.Entities;

public partial class EventTitle
{
    public int EventTitleId { get; set; }

    public string EventTitleVenueName { get; set; }

    public string EventTitleDescription { get; set; }

    public DateTime EventTitleStartTimeDate { get; set; }

    public DateTime EventTitleEndTimeDate { get; set; }

    public bool EventTitleStatus { get; set; }

    public int? EventLineId { get; set; }

    public bool IsDeleted { get; set; }

    public int? CreatedById { get; set; }

    public DateOnly? CreateDate { get; set; }

    public int? UpdatedById { get; set; }

    public DateOnly? UpdateDate { get; set; }

    public int? DeletedById { get; set; }

    public DateOnly? DeleteDate { get; set; }

    public virtual ICollection<EventAudience> EventAudiences { get; set; } = new List<EventAudience>();

    public virtual EventLine EventLine { get; set; }
}
