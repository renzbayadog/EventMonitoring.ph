using System;

namespace EventMonitoring.ph.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } // e.g., Pending, Completed, Failed
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
    }
} 