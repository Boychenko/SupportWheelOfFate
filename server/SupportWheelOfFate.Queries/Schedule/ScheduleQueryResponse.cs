using System;
using System.Collections.Generic;

namespace SupportWheelOfFate.Queries.Schedule
{
    public class ScheduleQueryResponse
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<ScheduleEntryDto> Entries { get; set; }
        public string NextWeek{ get; set; }
        public string PreviousWeek { get; set; }
    }
}
