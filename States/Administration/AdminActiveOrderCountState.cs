using EventMonitoring.ph.Data.Repositories;
using MediatR;
namespace EventMonitoring.States.Administration
{
    public class AdminActiveOrderCountState(IServiceProvider serviceProvider, IRepositoryWrapper wrapper)
    {
        public int AudienceCount { get; set; }
        public int EventsCount { get; set; }
        public int OngoingEventsCount { get; set; }
        public int FinishedEventsCount { get; set; }

        public event Action? StateChanged;

        public async Task GetActiveEventCount(int eventTitleId = 0)
        {
            //using var scope = serviceProvider.CreateScope();
            //var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            //var response = (await mediator.Send(new GetGenericOrdersCountQry(null, false)));

            var activeEventCount = await wrapper.EventTitle_Repository.GetActiveEventCountState(eventTitleId);
            AudienceCount = activeEventCount.AudienceCount;
            EventsCount = activeEventCount.EventsCount;
            OngoingEventsCount = activeEventCount.OngoingEventsCount;
            FinishedEventsCount = activeEventCount.FinishedEventsCount;
            StateChanged?.Invoke();
        }
    }
}