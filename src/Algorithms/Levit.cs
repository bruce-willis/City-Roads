using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CityMap.Helpers;

namespace CityMap.Algorithms
{
    public static class Levit
    {
        public static (Dictionary<ulong, double>, Dictionary<ulong, ulong>) Calculate(ulong startId = 4198407189)
        {
            var distance = SvgHelper.Dictionary.ToDictionary(k => k.Key, v => double.MaxValue);

            distance[startId] = 0;

            var m0 = new HashSet<ulong>(SvgHelper.Dictionary.Count);
            var m2 = new HashSet<ulong>(SvgHelper.Dictionary.Count);

            var m1_1 = new Queue<ulong>(SvgHelper.Dictionary.Count);
            var m1_2 = new Queue<ulong>(SvgHelper.Dictionary.Count);

            m1_1.Enqueue(startId);

            foreach (var node in SvgHelper.Dictionary.Keys)
            {
                if (node != startId)
                    m2.Add(node);
            }

            while (m1_1.Any() || m1_2.Any())
            {
                var u = m1_2.Any() ? m1_2.Dequeue() : m1_1.Dequeue();

                foreach (var v in SvgHelper.Dictionary[u].Adjency)
                {
                    if (m2.Contains(v))
                    {
                        m1_1.Enqueue(v);
                        m2.Remove(v);
                        distance[v] = Math.Min(distance[v],
                            distance[u] + SvgHelper.Dictionary[u].DistanceBetweenPoints(SvgHelper.Dictionary[v]));
                    }
                    else if (m1_1.Contains(v) || m1_2.Contains(v))
                    {
                        distance[v] = Math.Min(distance[v],
                            distance[u] + SvgHelper.Dictionary[u].DistanceBetweenPoints(SvgHelper.Dictionary[v]));
                    }
                    else if (m0.Contains(v) && distance[v] >
                             distance[u] + SvgHelper.Dictionary[u].DistanceBetweenPoints(SvgHelper.Dictionary[v]))
                    {
                        m1_2.Enqueue(v);
                        m0.Remove(v);
                        distance[v] = distance[u] +
                                      SvgHelper.Dictionary[u].DistanceBetweenPoints(SvgHelper.Dictionary[v]);
                    }
                }

                m0.Add(u);
            }

            return (distance, null);
        }
    }
}