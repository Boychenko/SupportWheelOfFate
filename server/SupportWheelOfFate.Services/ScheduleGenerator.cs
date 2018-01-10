using System.Collections.Generic;
using System.Linq;
using SupportWheelOfFate.Common;
using SupportWheelOfFate.Common.Extentions;
using SupportWheelOfFate.Domain.BusinessObjects;
using SupportWheelOfFate.Domain.Contracts;
using SupportWheelOfFate.Domain.Models;

namespace SupportWheelOfFate.Services
{
    /// <summary>
    /// Assumptions for generator.
    /// From rules: Each engineer should have completed one whole day of support in any 2 week period
    /// I'm assuming that any 2 week period means: two any week from Monday to Friday. 
    /// Because if we take any day as start for period and this requirement 
    /// we will get static schedule so we will be able to generate schedule for two week and use it
    /// Another variant is to split calendar to consecutive 2 weeks periods without overlapping which give us opportunity
    /// to have two shift per week not in consecutive days but this doesn't sound like "ANY 2 week period".
    /// 
    /// So all rules are collapsed to 1: An engineer should have one half day shifts per week
    /// </summary>
    public class ScheduleGenerator : IScheduleGenerator
    {
        public List<ScheduleEntry> GenerateSchedule(ScheduleGenerationRequest request)
        {
            var date = request.Date;
            //var week = WorkingWeek.GetThisOrUpcoming(date);
            var week = new WorkingWeek(date);

            var lastDueDayEntries = request.LastDueDayEntries;

            var lastEntryDate = lastDueDayEntries.LastOrDefault()?.Date;
            if (!date.IsWeekDay() || week.Contains(lastEntryDate))
            {
                week = week.GetNext();
            }

            if (lastEntryDate.HasValue)
            {
                //if week have entries or future entries exists just exit
                if (!week.IsAfter(lastEntryDate))
                {
                    return new List<ScheduleEntry>();
                }

                //entries is too old to consider
                var previousWeek = week.GetPrevious();
                if (!previousWeek.Contains(lastEntryDate))
                {
                    lastDueDayEntries = new List<ScheduleEntry>();
                }
            }

            var result = GenerateSchedule(request.Engineers, week, lastDueDayEntries);
            if (week.Contains(date)) //first call was for current week let generate one more
            {
                result.AddRange(
                    GenerateSchedule(
                        request.Engineers, week.GetNext(),
                        result.TakeLast(Constants.ShiftsPerDay).ToList()));
            }

            return result;
        }

        private List<ScheduleEntry> GenerateSchedule(List<Engineer> engineers, WorkingWeek week, List<ScheduleEntry> previousDayEntries)
        {
            var workingDay = week.Start;
            var entriesToCheck = previousDayEntries;
            var result = new List<ScheduleEntry>();

            while (workingDay <= week.End)
            {
                var candidates = GetCandidates(engineers, entriesToCheck);
                for (int i = 0; i < Constants.ShiftsPerDay; i++)
                {
                    var engineer = candidates[RandomProvider.Current.Next(candidates.Count)];
                    candidates.Remove(engineer);
                    result.Add(new ScheduleEntry
                    {
                        Date = workingDay,
                        EngineerId = engineer.Id,
                        Shift = i
                    });
                }
                entriesToCheck = result;
                workingDay = workingDay.AddDays(1);
            }

            return result;
        }

        private List<Engineer> GetCandidates(List<Engineer> candidates, List<ScheduleEntry> previousEntries)
        {
            return candidates.Where(c => previousEntries.All(e => e.EngineerId != c.Id)).ToList();
        }
    }
}
