using System;
using System.Collections.Generic;
using System.Linq;
using CityMap.Helpers;

namespace CityMap.Algorithms.Travelling_salesman
{
    /// <see cref="https://habr.com/post/209610" />
    public static class SimulatedAnnealing
    {
        private static readonly Random Random = new Random();
        private static List<ulong> _order = DistanceHelper.Destinations.Keys.ToList();


        public static double Calculate(double initialTemperature, double endTemperature, string outputDirectory, int logFrequency = 1000)
        {
            var currentDistance = CalculateTotalDistance(_order);
            var minimalDistance = currentDistance;
            var minimalOrder = _order.ToList();

            var temperature = initialTemperature;
            for (var i = 0; i < 100000; ++i)
            {
                var candidateOrder = ShuffleOrder();
                var candidateDistance = CalculateTotalDistance(candidateOrder);

                if (candidateDistance < currentDistance)
                {
                    currentDistance = candidateDistance;
                    _order = candidateOrder.ToList();
                }
                else
                {
                    var probability = GetTransitionProbability(candidateDistance - currentDistance, temperature);
                    if (IsTransition(probability))
                    {
                        currentDistance = candidateDistance;
                        _order = candidateOrder.ToList();
                    }
                }

                if (currentDistance < minimalDistance)
                {
                    minimalDistance = currentDistance;
                    minimalOrder = _order.ToList();
                }

                temperature = DecreaseTemperature(initialTemperature, i);

                if (temperature <= endTemperature)
                    break;

                if (i % logFrequency == 0) Console.WriteLine($"Iteration №{i}. Min distance - {minimalDistance}");
            }

            Console.WriteLine($"Total length of the path is {minimalDistance}");
            minimalOrder.Insert(0, CommonSalesman.StartId);
            SvgHelper.DisplaySalesmanPath(outputDirectory, "simulated_annealing", minimalOrder);
            return currentDistance;
        }

        private static double CalculateTotalDistance(IReadOnlyList<ulong> order)
        {
            var totalDistance = CommonSalesman.Distances[(CommonSalesman.StartId, order.First())].distance;

            for (var i = 0; i < order.Count - 1; ++i)
                totalDistance += CommonSalesman.Distances[(order[i], order[i + 1])].distance;

            return totalDistance + CommonSalesman.Distances[(order.Last(), CommonSalesman.StartId)].distance;
        }

        private static double DecreaseTemperature(double initialTemperature, int step)
        {
            return initialTemperature * 0.1 / step;
        }

        private static double GetTransitionProbability(double deltaEnergy, double temperature)
        {
            return Math.Exp(-deltaEnergy / temperature);
        }

        private static bool IsTransition(double probability)
        {
            return Random.NextDouble() <= probability;
        }

        private static List<ulong> ShuffleOrder()
        {
            var shuffle = _order.ToList();
            var i = Random.Next(shuffle.Count);
            var j = Random.Next(shuffle.Count);
            var reverseItems = i > j ? shuffle.GetRange(j, i - j + 1) : shuffle.GetRange(i, j - i + 1);
            reverseItems.Reverse();
            for (var t = Math.Min(i, j); t <= Math.Max(i, j); ++t) shuffle[t] = reverseItems[t - Math.Min(i, j)];

            return shuffle;
        }
    }
}