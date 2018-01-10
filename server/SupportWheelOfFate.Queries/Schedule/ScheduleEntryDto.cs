using System;

namespace SupportWheelOfFate.Queries.Schedule
{
    public class ScheduleEntryDto
    {
        public string Engineer { get; set; }
        public DateTime Date { get; set; }
        public int Shift { get; set; }
    }
}
