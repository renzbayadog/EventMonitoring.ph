using System;
using System.Collections.Generic;

namespace EventMonitoring.ph.Data.Entities;

public partial class EventLine
{
    public int EventLineId { get; set; }

    public string EventLineName { get; set; }

    public bool IsDeleted { get; set; }

    public int? CreatedById { get; set; }

    public DateOnly? CreateDate { get; set; }

    public int? UpdatedById { get; set; }

    public DateOnly? UpdateDate { get; set; }

    public int? DeletedById { get; set; }

    public DateOnly? DeleteDate { get; set; }

    public virtual ICollection<EventTitle> EventTitles { get; set; } = new List<EventTitle>();
}
