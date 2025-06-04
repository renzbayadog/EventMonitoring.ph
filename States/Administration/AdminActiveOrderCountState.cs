using MediatR;
namespace EventMonitoring.States.Administration
{
    public class AdminActiveOrderCountState(IServiceProvider serviceProvider)
    {
        public int ProcessingCount { get; set; }
        public int DeliveringCount { get; set; }
        public int DeliveredCount { get; set; }
        public int CanceledCount { get; set; }

        public event Action? StateChanged;

        public async Task GetActiveOrdersCount()
        {
            //using var scope = serviceProvider.CreateScope();
            //var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            //var response = (await mediator.Send(new GetGenericOrdersCountQry(null, false)));
            ProcessingCount = 20;
            DeliveringCount = 30;
            DeliveredCount = 40;
            CanceledCount = 50;
            StateChanged?.Invoke();
        }
    }
}