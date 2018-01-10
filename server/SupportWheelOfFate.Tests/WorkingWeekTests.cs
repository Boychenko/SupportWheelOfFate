using System;
using System.Collections.Generic;
using SupportWheelOfFate.Domain.BusinessObjects;
using Xunit;

namespace SupportWheelOfFate.Tests
{
    public class WorkingWeekTests
    {
        [Theory]
        [MemberData(nameof(ConsctructionTestsParameters))]
        public void WorkingWeek_ShouldBeConstructed_Correctly(DateTime date, DateTime expectedStart, DateTime expectedEnd)
        {
            var week = new WorkingWeek(date);

            Assert.Equal(expectedStart, week.Start);
            Assert.Equal(expectedEnd, week.End);
        }

        [Theory]
        [MemberData(nameof(ContainsTestsParameters))]
        public void WorkingWeek_Contains_Tests(DateTime weekDate, DateTime? checkDate, bool expected)
        {
            var week = new WorkingWeek(weekDate);

            Assert.Equal(expected, week.Contains(checkDate));
        }


        [Theory]
        [MemberData(nameof(IsAfterTestsParameters))]
        public void WorkingWeek_IsAfter_Tests(DateTime weekDate, DateTime? checkDate, bool expected)
        {
            var week = new WorkingWeek(weekDate);

            Assert.Equal(expected, week.IsAfter(checkDate));
        }

        [Theory]
        [MemberData(nameof(ToStringTestParameters))]
        public void WorkingWeek_ToString(DateTime weekDate, string expected)
        {
            Assert.Equal(expected, new WorkingWeek(weekDate).ToString());
        }

        public static IEnumerable<object[]> ToStringTestParameters()
        {
            yield return new object[] { new DateTime(2018, 1, 7), "01/01/2018 - 05/01/2018" };
        }

         public static IEnumerable<object[]> IsAfterTestsParameters()
        {
            yield return new object[] { new DateTime(2018, 1, 7), null, false };
            yield return new object[] { new DateTime(2018, 1, 7), new DateTime(2018, 1, 1), false };
            yield return new object[] { new DateTime(2018, 1, 7), new DateTime(2018, 1, 2), false };
            yield return new object[] { new DateTime(2018, 1, 7), new DateTime(2018, 1, 3), false };
            yield return new object[] { new DateTime(2018, 1, 7), new DateTime(2018, 1, 4), false };
            yield return new object[] { new DateTime(2018, 1, 7), new DateTime(2018, 1, 5), false };
            yield return new object[] { new DateTime(2018, 1, 7), new DateTime(2018, 1, 6), false };
            yield return new object[] { new DateTime(2018, 1, 7), new DateTime(2018, 1, 7), false };
            yield return new object[] { new DateTime(2018, 1, 7), new DateTime(2017, 1, 7), true };
            yield return new object[] { new DateTime(2018, 1, 7), new DateTime(2017, 1, 8), true };
        }

        public static IEnumerable<object[]> ContainsTestsParameters()
        {
            yield return new object[] { new DateTime(2018, 1, 7), null, false };
            yield return new object[] { new DateTime(2018, 1, 7), new DateTime(2018, 1, 1), true };
            yield return new object[] { new DateTime(2018, 1, 7), new DateTime(2018, 1, 2), true };
            yield return new object[] { new DateTime(2018, 1, 7), new DateTime(2018, 1, 3), true };
            yield return new object[] { new DateTime(2018, 1, 7), new DateTime(2018, 1, 4), true };
            yield return new object[] { new DateTime(2018, 1, 7), new DateTime(2018, 1, 5), true };
            yield return new object[] { new DateTime(2018, 1, 7), new DateTime(2018, 1, 6), false };
            yield return new object[] { new DateTime(2018, 1, 7), new DateTime(2018, 1, 7), false };
            yield return new object[] { new DateTime(2018, 1, 7), new DateTime(2017, 1, 7), false };
        }

        public static IEnumerable<object[]> ConsctructionTestsParameters()
        {
            yield return new object[] { new DateTime(2018, 1, 8), new DateTime(2018, 1, 8), new DateTime(2018, 1, 12) };
            yield return new object[] { new DateTime(2018, 1, 7), new DateTime(2018, 1, 1), new DateTime(2018, 1, 5) };
            yield return new object[] { new DateTime(2018, 1, 6), new DateTime(2018, 1, 1), new DateTime(2018, 1, 5) };
            yield return new object[] { new DateTime(2018, 1, 5), new DateTime(2018, 1, 1), new DateTime(2018, 1, 5) };
            yield return new object[] { new DateTime(2018, 1, 4), new DateTime(2018, 1, 1), new DateTime(2018, 1, 5) };
            yield return new object[] { new DateTime(2018, 1, 3), new DateTime(2018, 1, 1), new DateTime(2018, 1, 5) };
            yield return new object[] { new DateTime(2018, 1, 2), new DateTime(2018, 1, 1), new DateTime(2018, 1, 5) };
            yield return new object[] { new DateTime(2018, 1, 1), new DateTime(2018, 1, 1), new DateTime(2018, 1, 5) };
            yield return new object[] { new DateTime(2018, 1, 7, 10, 15, 11), new DateTime(2018, 1, 1), new DateTime(2018, 1, 5) };
            yield return new object[] { new DateTime(2018, 1, 6, 10, 15, 11), new DateTime(2018, 1, 1), new DateTime(2018, 1, 5) };
            yield return new object[] { new DateTime(2018, 1, 5, 10, 15, 11), new DateTime(2018, 1, 1), new DateTime(2018, 1, 5) };
            yield return new object[] { new DateTime(2018, 1, 4, 10, 15, 11), new DateTime(2018, 1, 1), new DateTime(2018, 1, 5) };
            yield return new object[] { new DateTime(2018, 1, 3, 10, 15, 11), new DateTime(2018, 1, 1), new DateTime(2018, 1, 5) };
            yield return new object[] { new DateTime(2018, 1, 2, 10, 15, 11), new DateTime(2018, 1, 1), new DateTime(2018, 1, 5) };
            yield return new object[] { new DateTime(2018, 1, 1, 10, 15, 11), new DateTime(2018, 1, 1), new DateTime(2018, 1, 5) };
        }
    }
}
