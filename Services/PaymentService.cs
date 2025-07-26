using System;
using System.Collections.Generic;
using System.Linq;
using EventMonitoring.ph.Models;

namespace EventMonitoring.ph.Services
{
    public class PaymentService
    {
        private static List<Payment> _payments = new List<Payment>();
        private static int _nextId = 1;

        public Payment InitiatePayment(decimal amount, string description)
        {
            var payment = new Payment
            {
                Id = _nextId++,
                Amount = amount,
                Status = "Pending",
                Timestamp = DateTime.Now,
                Description = description
            };
            _payments.Add(payment);
            return payment;
        }

        public Payment ConfirmPayment(int id)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == id);
            if (payment != null)
            {
                payment.Status = "Completed";
            }
            return payment;
        }

        public Payment GetPaymentStatus(int id)
        {
            return _payments.FirstOrDefault(p => p.Id == id);
        }
    }
} 