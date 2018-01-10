using System;
using SupportWheelOfFate.Common.Extentions;

namespace SupportWheelOfFate.Domain.BusinessObjects
{

    public struct WorkingWeek
    {
        public WorkingWeek(DateTime date)
        {
            date = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);

            var firstDayOfWeek = DayOfWeek.Monday;

            int offset = firstDayOfWeek - date.DayOfWeek;
            if (offset != 1)
            {
                Start = date.AddDays(offset);
                End = Start.AddDays(4);
            }
            else
            {
                //Sunday
                Start = date.AddDays(-6);
                End = date.AddDays(-2);
            }
        }

        private WorkingWeek(DateTime start, DateTime end) : this()
        {
            Start = start;
            End = end;
        }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public bool Contains(DateTime? date)
        {
            return date >= Start && date <= End;
        }

        public bool IsAfter(DateTime? date)
        {
            return date < Start;
        }

        public WorkingWeek GetNext()
        {
            return new WorkingWeek(Start.AddDays(7), End.AddDays(7));
        }

        public WorkingWeek GetPrevious()
        {
            return new WorkingWeek(Start.AddDays(-7), End.AddDays(-7));
        }

        public static WorkingWeek GetThisOrUpcoming(DateTime date)
        {
            if (date.IsWeekDay())
            {
                return new WorkingWeek(date);
            }
            else
            {
                return new WorkingWeek(date.AddDays(7));
            }
        }

        public override string ToString()
        {
            return $"{Start:dd\\/MM\\/yyyy} - {End:dd\\/MM\\/yyyy}";
        }
    }
}
