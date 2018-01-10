using System;

namespace SupportWheelOfFate.Domain.Models
{
    public class ScheduleEntry : Entity
    {
        public long EngineerId { get; set; }

        public Engineer Engineer { get; set; }

        public DateTime Date { get; set; }

        public int Shift { get; set; }
    }
}
