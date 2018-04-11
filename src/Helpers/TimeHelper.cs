using System;
using System.Diagnostics;

namespace CityMap.Helpers
{
    public static class TimeHelper
    {
        public static void MeasureTime(Action action, string description)
        {
            var stopWatch = Stopwatch.StartNew();
            Console.Write($"Start {description}. ");
            action();
            Console.WriteLine($"Elapsed time: {stopWatch.Elapsed}");
        }

        public static T MeasureTimeAlgorithm<T>(Func<T> algorithm, string description)
        {
            var stopWatch = Stopwatch.StartNew();
            Console.Write($"Start {description}. ");
            var returnValue = algorithm();
            Console.WriteLine($"Elapsed time: {stopWatch.Elapsed}");
            return returnValue;
        }
    }
}