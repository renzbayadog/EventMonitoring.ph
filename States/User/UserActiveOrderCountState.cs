using MediatR;

namespace EventMonitoring.States.User
{
    public class UserActiveOrderCountState(IServiceProvider serviceProvider)
    {
        public int ProcessingCount { get; set; }
        public int DeliveringCount { get; set; }
        public int DeliveredCount { get; set; }
        public int CanceledCount { get; set; }

        public event Action StateChanged;

        public async Task GetActiveOrdersCount(string userId)
        {
            //using var scope = serviceProvider.CreateScope();
            //var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            //var response = (await mediator.Send(new GetGenericOrdersCountQry(userId, false)));
            ProcessingCount = 20;
            DeliveringCount = 30;
            DeliveredCount = 40;
            CanceledCount = 50;
            StateChanged?.Invoke();
        }
    }
}