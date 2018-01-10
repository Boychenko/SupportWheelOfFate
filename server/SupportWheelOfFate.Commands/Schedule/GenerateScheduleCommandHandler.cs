using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SupportWheelOfFate.Common;
using SupportWheelOfFate.Database;
using SupportWheelOfFate.Domain.BusinessObjects;
using SupportWheelOfFate.Domain.Contracts;

namespace SupportWheelOfFate.Commands.Schedule
{
    public class GenerateScheduleCommandHandler : AsyncRequestHandler<GenerateScheduleCommand>
    {
        private readonly IScheduleGenerator _scheduleGenerator;
        private readonly SupportWheelContext _context;

        public GenerateScheduleCommandHandler(IScheduleGenerator scheduleGenerator, SupportWheelContext context)
        {
            _scheduleGenerator = scheduleGenerator ?? throw new ArgumentNullException(nameof(scheduleGenerator));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected override async Task HandleCore(GenerateScheduleCommand request)
        {
            //In more complex scenario we can introduce repository and UOW to be able to inject them 
            // sample of UOW + repos can be found here https://github.com/Boychenko/sample-todo/tree/master/server/Core
            //and add tests to handlers but often there are should pretty simple logic
            var engineers = await _context.Engineers.ToListAsync();

            var lastDueDateEntries = await _context.ScheduleEntries
                .OrderByDescending(e => e.Date)
                .Take(Constants.ShiftsPerDay)
                .ToListAsync();

            var generationRequest = new ScheduleGenerationRequest
            {
                Engineers = engineers,
                LastDueDayEntries = lastDueDateEntries,
                Date = DateTime.Today
            };

            var result = _scheduleGenerator.GenerateSchedule(generationRequest);

            if (result.Any())
            {
                foreach (var entry in result)
                {
                    _context.Add(entry);
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
