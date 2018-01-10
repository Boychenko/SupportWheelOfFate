using System;
using MediatR;
namespace SupportWheelOfFate.Queries.Schedule
{
    public class ScheduleQuery : IRequest<ScheduleQueryResponse>
    {
        public DateTime? Date { get; set; }
    }
}
