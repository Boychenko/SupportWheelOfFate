using System;
using System.Collections.Generic;
using System.Text;
using SupportWheelOfFate.Common.Extentions;
using Xunit;

namespace SupportWheelOfFate.Tests
{
    public class DateTimeExtentionsTests
    {
        [Theory]
        [MemberData(nameof(CheckIsWeekDayParameters))]
        public void DateTimeExtentions_CheckIsWeekDay(DateTime date, bool isWeekDay)
        {
            Assert.Equal(isWeekDay, date.IsWeekDay());
        }

        public static IEnumerable<object[]> CheckIsWeekDayParameters()
        {
            yield return new object[] { new DateTime(2018, 1, 1), true };
            yield return new object[] { new DateTime(2018, 1, 2), true };
            yield return new object[] { new DateTime(2018, 1, 3), true };
            yield return new object[] { new DateTime(2018, 1, 4), true };
            yield return new object[] { new DateTime(2018, 1, 5), true };
            yield return new object[] { new DateTime(2018, 1, 6), false};
            yield return new object[] { new DateTime(2018, 1, 7), false };
        }
    }
}
