using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportWheelOfFate.Common.Extentions;
using SupportWheelOfFate.Database;
using SupportWheelOfFate.Domain.BusinessObjects;
using SupportWheelOfFate.Domain.Models;

namespace SupportWheelOfFate.Queries.Schedule
{
    public class ScheduleQueryHandler : AsyncRequestHandler<ScheduleQuery, ScheduleQueryResponse>
    {
        private readonly SupportWheelContext _supportContext;

        public ScheduleQueryHandler(SupportWheelContext supportContext)
        {
            _supportContext = supportContext;
        }

        protected async override Task<ScheduleQueryResponse> HandleCore(ScheduleQuery request)
        {
            var requestedWeek = new WorkingWeek(request.Date ?? DateTime.Today);
            var previousWeek = requestedWeek.GetPrevious();
            var nextWeek = requestedWeek.GetNext();

            var entries = await AddWeekFilter(_supportContext.ScheduleEntries, requestedWeek)
                .Include(e => e.Engineer)
                .OrderBy(e => e.Date)
                .ThenBy(e => e.Shift)
                .ToListAsync();
            var hasPrevious = await AddWeekFilter(_supportContext.ScheduleEntries, previousWeek).AnyAsync();
            var hasNext = await AddWeekFilter(_supportContext.ScheduleEntries, nextWeek).AnyAsync();

            return new ScheduleQueryResponse
            {
                Start = requestedWeek.Start,
                End = requestedWeek.End,
                NextWeek = hasNext ? nextWeek.Start.ToQueryParam() : null,
                PreviousWeek = hasPrevious ? previousWeek.Start.ToQueryParam() : null,
                Entries = entries.Select(e => (new ScheduleEntryDto {
                    Date = DateTime.SpecifyKind(e.Date, DateTimeKind.Utc),
                    Engineer = e.Engineer.Name,
                    Shift = e.Shift
                })).ToList()
            };
        }

        private IQueryable<ScheduleEntry> AddWeekFilter(IQueryable<ScheduleEntry> query, WorkingWeek week)
        {
            return query.Where(e => e.Date >= week.Start && e.Date <= week.End);
        }
    }
}
