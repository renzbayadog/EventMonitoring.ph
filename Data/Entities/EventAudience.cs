using System;
using System.Collections.Generic;

namespace EventMonitoring.ph.Data.Entities;

public partial class EventAudience
{
    public int EventAudienceId { get; set; }

    public int? EventTitleId { get; set; }

    public int? UserId { get; set; }

    public string QrCode { get; set; }

    public string EventRemarks { get; set; }

    public bool IsDeleted { get; set; }

    public int? CreatedById { get; set; }

    public DateOnly? CreateDate { get; set; }

    public int? UpdatedById { get; set; }

    public DateOnly? UpdateDate { get; set; }

    public int? DeletedById { get; set; }

    public DateOnly? DeleteDate { get; set; }

    public virtual EventTitle EventTitle { get; set; }
}
