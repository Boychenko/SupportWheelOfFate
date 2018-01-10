using System;
using System.Collections.Generic;
using SupportWheelOfFate.Domain.Models;

namespace SupportWheelOfFate.Domain.BusinessObjects
{
    public class ScheduleGenerationRequest
    {
        public DateTime Date { get; set; }

        public List<Engineer> Engineers { get; set; }

        public List<ScheduleEntry> LastDueDayEntries { get; set; }
    }
}
