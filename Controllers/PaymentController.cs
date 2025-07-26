using Microsoft.AspNetCore.Mvc;
using EventMonitoring.ph.Models;
using EventMonitoring.ph.Services;

namespace EventMonitoring.ph.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController()
        {
            _paymentService = new PaymentService();
        }

        [HttpPost("initiate")]
        public ActionResult<Payment> Initiate([FromBody] InitiatePaymentRequest request)
        {
            var payment = _paymentService.InitiatePayment(request.Amount, request.Description);
            return Ok(payment);
        }

        [HttpPost("confirm/{id}")]
        public ActionResult<Payment> Confirm(int id)
        {
            var payment = _paymentService.ConfirmPayment(id);
            if (payment == null) return NotFound();
            return Ok(payment);
        }

        [HttpGet("status/{id}")]
        public ActionResult<Payment> Status(int id)
        {
            var payment = _paymentService.GetPaymentStatus(id);
            if (payment == null) return NotFound();
            return Ok(payment);
        }
    }

    public class InitiatePaymentRequest
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
} 