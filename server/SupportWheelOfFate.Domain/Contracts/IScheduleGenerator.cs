using System.Collections.Generic;
using SupportWheelOfFate.Domain.BusinessObjects;
using SupportWheelOfFate.Domain.Models;

namespace SupportWheelOfFate.Domain.Contracts
{
    public interface IScheduleGenerator
    {
        List<ScheduleEntry> GenerateSchedule(ScheduleGenerationRequest request);
    }
}
