using MediatR;
using SupportWheelOfFate.Commands.Schedule;

namespace SupportWheelOfFate.Services
{
    public class CronJobs
    {
        private readonly IMediator _mediator;

        public CronJobs(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void GenerateSchedule()
        {
            _mediator.Send(new GenerateScheduleCommand()).Wait();
        }
    }
}
