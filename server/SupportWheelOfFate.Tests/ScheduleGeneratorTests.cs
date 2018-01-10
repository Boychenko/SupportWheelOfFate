using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Ploeh.AutoFixture;
using SupportWheelOfFate.Common;
using SupportWheelOfFate.Domain.BusinessObjects;
using SupportWheelOfFate.Domain.Models;
using SupportWheelOfFate.Services;
using Xunit;

namespace SupportWheelOfFate.Tests
{
    public class ScheduleGeneratorTests
    {
        [Fact]
        public void ScheduleGenerator_FirstWeekDayRun_ShouldGenerateTwoWeeksFromCurrent()
        {
            SetupRandom(10);
            var engineers = GetEngineers();
            var generator = new ScheduleGenerator();
            var lastDayEntries = new List<ScheduleEntry>();
            var result = generator.GenerateSchedule(new ScheduleGenerationRequest
            {
                Date = new DateTime(2018, 1, 4),
                Engineers = engineers,
                LastDueDayEntries = lastDayEntries
            });

            AssertScheduleRules(result, lastDayEntries);
            Assert.Equal(Constants.ShiftsPerDay * 10, result.Count);
        }

        [Fact]
        public void ScheduleGenerator_FirstWeekendDayRun_ShouldGenerateOneNextWeek()
        {
            SetupRandom();

            var engineers = GetEngineers();
            var generator = new ScheduleGenerator();
            var lastDayEntries = new List<ScheduleEntry>();
            var result = generator.GenerateSchedule(new ScheduleGenerationRequest
            {
                Date = new DateTime(2018, 1, 6),
                Engineers = engineers,
                LastDueDayEntries = lastDayEntries
            });

            AssertScheduleRules(result, lastDayEntries);
            Assert.Equal(Constants.ShiftsPerDay * 5, result.Count);
        }


        [Fact]
        public void ScheduleGenerator_WeekDayRun_ShouldGenerateNextWeek()
        {
            SetupRandom();
            var fixture = new Fixture();
            var engineers = GetEngineers();
            var generator = new ScheduleGenerator();
            var lastDayEntries = GetDayEntries(engineers.Take(2).ToList(), new DateTime(2018, 1, 5));

            var result = generator.GenerateSchedule(new ScheduleGenerationRequest
            {
                Date = new DateTime(2018, 1, 4),
                Engineers = engineers,
                LastDueDayEntries = lastDayEntries
            });

            AssertScheduleRules(result, lastDayEntries);
            Assert.Equal(Constants.ShiftsPerDay * 5, result.Count);
            Assert.NotEqual(engineers[0].Id, result[0].EngineerId);
            Assert.NotEqual(engineers[1].Id, result[0].EngineerId);
        }

        [Fact]
        public void ScheduleGenerator_WeekendDayRun_ShouldGenerateNextWeek()
        {
            SetupRandom();
            var fixture = new Fixture();
            var engineers = GetEngineers();
            var generator = new ScheduleGenerator();
            var lastDayEntries = new List<ScheduleEntry>();

            var result = generator.GenerateSchedule(new ScheduleGenerationRequest
            {
                Date = new DateTime(2018, 1, 6),
                Engineers = engineers,
                LastDueDayEntries = lastDayEntries
            });

            AssertScheduleRules(result, lastDayEntries);
            Assert.Equal(Constants.ShiftsPerDay * 5, result.Count);
        }


        [Fact]
        public void ScheduleGenerator_WeekendDayRun_ShouldCallRandomForAllCounts()
        {
            var randomMock = SetupRandom();
            var fixture = new Fixture();
            var engineers = GetEngineers();
            var generator = new ScheduleGenerator();
            var lastDayEntries = new List<ScheduleEntry>();

            var result = generator.GenerateSchedule(new ScheduleGenerationRequest
            {
                Date = new DateTime(2018, 1, 6),
                Engineers = engineers,
                LastDueDayEntries = lastDayEntries
            });

            AssertScheduleRules(result, lastDayEntries);
            Assert.Equal(Constants.ShiftsPerDay * 5, result.Count);
            for (int i = 1; i <= engineers.Count; i++)
            {
                randomMock.Verify(r => r.Next(It.Is<int>(p => p == i)), Times.Once);
            }
        }

        [Fact]
        public void ScheduleGenerator_WeekDayRunHaveNextWeekEntries_ShouldGenerateEmpty()
        {
            SetupRandom();

            var engineers = GetEngineers();
            var generator = new ScheduleGenerator();
            var lastDayEntries = GetDayEntries(engineers.Take(2).ToList(), new DateTime(2018, 1, 8));
            var result = generator.GenerateSchedule(new ScheduleGenerationRequest
            {
                Date = new DateTime(2018, 1, 4),
                Engineers = engineers,
                LastDueDayEntries = lastDayEntries
            });

            AssertScheduleRules(result, lastDayEntries);
            Assert.Empty(result);
        }

        private void AssertScheduleRules(List<ScheduleEntry> schedule, List<ScheduleEntry> lastDueDayEntries)
        {
            //check sequence
            var entries = new List<ScheduleEntry>(schedule);
            entries.InsertRange(0, lastDueDayEntries);

            var days = entries.GroupBy(e => e.Date).OrderBy(gr => gr.Key).ToList();

            for (var i = 0; i < days.Count - 1; i++)
            {
                foreach (var entry in days[i])
                {
                    Assert.DoesNotContain(days[i + 1], e => e.EngineerId == entry.EngineerId);
                }
            }

            //check same day
            foreach (var day in entries.GroupBy(e => e.Date))
            {
                Assert.Equal(day.Count(), day.Select(e => e.EngineerId).Distinct().Count());
            }

            //check shifts count equal to weeks count
            Assert.True(schedule.GroupBy(e => e.EngineerId).All(gr => gr.Count() == schedule.Count / 10));
        }

        private List<ScheduleEntry> GetDayEntries(List<Engineer> engineers, DateTime date)
        {
            var fixture = new Fixture();
            return new List<ScheduleEntry>
            {
                fixture.Build<ScheduleEntry>()
                .With(e => e.Date, date)
                .With(e => e.Shift, 0)
                .With(e => e.Engineer, engineers[0])
                .With(e => e.EngineerId, engineers[0].Id)
                .Create(),
                fixture.Build<ScheduleEntry>()
                .With(e => e.Date, date)
                .With(e => e.Shift, 1)
                .With(e => e.Engineer, engineers[1])
                .With(e => e.EngineerId, engineers[1].Id)
                .Create(),
            };
        }

        private List<Engineer> GetEngineers()
        {
            var fixture = new Fixture();
            var id = 1;
            fixture.Customize<long>(c => c.FromSeed(i => i + id++));
            return fixture.CreateMany<Engineer>(10).ToList();
        }

        private Mock<RandomProvider> SetupRandom(int timesReturnMax = 0)
        {
            var timeProviderStub = new Mock<RandomProvider>();
            var counter = timesReturnMax;
            timeProviderStub.Setup(tp => tp.Next(It.IsAny<int>())).Returns<int>(x =>
            {
                if (counter-- > 0)
                {
                    return x - 1;
                }
                return 0;
            });
            RandomProvider.Current = timeProviderStub.Object;
            return timeProviderStub;
        }
    }
}
