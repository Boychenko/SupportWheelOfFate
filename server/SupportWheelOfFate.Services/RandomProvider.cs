using System;

namespace SupportWheelOfFate.Services
{
    public abstract class RandomProvider
    {
        private static RandomProvider current;

        static RandomProvider()
        {
            current = new DefaultRandomProvider();
        }
        public static RandomProvider Current
        {
            get { return current; }
            set
            {
                current = value ?? throw new ArgumentNullException("value");
            }
        }

        public abstract int Next(int maxValue);

        public static void ResetToDefault()
        {
            current = new DefaultRandomProvider();
        }
    }
}
