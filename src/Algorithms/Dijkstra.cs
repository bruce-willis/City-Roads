using System.Collections.Generic;
using System.Linq;
using CityMap.Helpers;

namespace CityMap.Algorithms
{
    public static class Dijkstra
    {
        public static (Dictionary<ulong, double>, Dictionary<ulong, ulong>) Calculate(ulong startId = 4198407189)
        {
            var queue = new SortedSet<(double distance, ulong number)>();

            var distance = SvgHelper.Dictionary.ToDictionary(k => k.Key, v => double.MaxValue);
            var p = new Dictionary<ulong, ulong>(SvgHelper.Dictionary.Count);

            distance[startId] = 0;

            queue.Add((distance[startId], startId));

            while (queue.Any())
            {
                var v = queue.First().number;

                queue.Remove(queue.First());

                foreach (var to in SvgHelper.Dictionary[v].Adjency)
                {
                    var len = SvgHelper.Dictionary[v].DistanceBetweenPoints(SvgHelper.Dictionary[to]);

                    if (distance[v] + len < distance[to])
                    {
                        queue.Remove((distance[to], to));
                        distance[to] = distance[v] + len;
                        p[to] = v;
                        queue.Add((distance[to], to));
                    }
                }
            }

            return (distance, p);
        }
    }
}