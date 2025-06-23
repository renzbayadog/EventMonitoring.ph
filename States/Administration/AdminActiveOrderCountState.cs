using EventMonitoring.ph.Data.Repositories;
using MediatR;
namespace EventMonitoring.States.Administration
{
    public class AdminActiveEventCountState(IServiceProvider serviceProvider, IRepositoryWrapper wrapper)
    {
        public int AudienceCount { get; set; }
        public int EventsCount { get; set; }
        public int OngoingEventsCount { get; set; }
        public int FinishedEventsCount { get; set; }

        public event Action? StateChanged;

        public async Task GetActiveEventCount()
        {
            //using var scope = serviceProvider.CreateScope();
            //var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            //var response = (await mediator.Send(new GetGenericOrdersCountQry(null, false)));

            var activeEventCount = await wrapper.EventTitle_Repository.GetActiveEventCountState(0);
            AudienceCount = activeEventCount.AudienceCount;
            EventsCount = activeEventCount.EventsCount;
            OngoingEventsCount = activeEventCount.OngoingEventsCount;
            FinishedEventsCount = activeEventCount.FinishedEventsCount;
            StateChanged?.Invoke();
        }
    }
}