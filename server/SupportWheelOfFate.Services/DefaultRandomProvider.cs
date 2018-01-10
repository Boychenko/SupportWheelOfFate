using System;

namespace SupportWheelOfFate.Services
{
    internal class DefaultRandomProvider : RandomProvider
    {
        private readonly Random _random = new Random();

        public override int Next(int maxValue)
        {
            return _random.Next(maxValue);
        }
    }
}