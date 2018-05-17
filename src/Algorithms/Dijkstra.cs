using System.Collections.Generic;
using System.Linq;
using CityMap.Helpers;

namespace CityMap.Algorithms
{
    /// <see cref="http://e-maxx.ru/algo/dijkstra_sparse"/>
    public static class Dijkstra
    {
        public static (Dictionary<ulong, double>, Dictionary<ulong, ulong>) Calculate(ulong startId)
        {
            var distance = SvgHelper.Dictionary.ToDictionary(k => k.Key, v => double.MaxValue);
            var p = new Dictionary<ulong, ulong>(SvgHelper.Dictionary.Count);

            distance[startId] = 0;

            var queue = new SortedSet<(double distance, ulong number)> { (distance[startId], startId) };

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