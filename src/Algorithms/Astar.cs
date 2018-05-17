using System;
using System.Collections.Generic;
using System.Linq;
using CityMap.Helpers;
using CityMap.Types;

namespace CityMap.Algorithms
{
    public static class Astar
    {
        /// <see cref="https://en.wikipedia.org/wiki/A*_search_algorithm#Pseudocode"/>
        public static (Dictionary<ulong, double>, Dictionary<ulong, ulong>) Calculate(ulong startId, ulong goalId = 286997395, Func<GeoPoint, GeoPoint, double> heuristic = null)
        {
            var closed = new HashSet<ulong>(SvgHelper.Dictionary.Count / 10);
            var g = SvgHelper.Dictionary.ToDictionary(k => k.Key, v => double.MaxValue);
            g[startId] = 0;
            

            var f = SvgHelper.Dictionary.ToDictionary(k => k.Key, v => double.MaxValue);
            f[startId] = g[startId] + heuristic(SvgHelper.Dictionary[startId], SvgHelper.Dictionary[goalId]);

            var queue = new SortedSet<(double distance, ulong number)> {(f[startId], startId)};

            var p = new Dictionary<ulong, ulong>(SvgHelper.Dictionary.Count);

            while (queue.Any())
            {
                var v = queue.First().number;

                if (v == goalId)
                {
                    return (g, p);
                }

                queue.Remove(queue.First());

                closed.Add(v);

                foreach (var to in SvgHelper.Dictionary[v].Adjency)
                {
                    if (closed.Contains(to)) continue;

                    var tentativeScore =
                        g[v] + SvgHelper.Dictionary[v].DistanceBetweenPoints(SvgHelper.Dictionary[to]);
                    if (tentativeScore >= g[to]) continue;

                    queue.Remove((f[to], to));

                    p[to] = v;
                    g[to] = tentativeScore;
                    f[to] = g[to] + heuristic(SvgHelper.Dictionary[to], SvgHelper.Dictionary[goalId]);
                    queue.Add((f[to], to));
                }
            }

            return (null, null);
        }
    }
}