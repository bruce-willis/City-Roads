using System;
using System.Collections.Generic;
using System.Linq;
using CityMap.Helpers;

namespace CityMap.Algorithms
{
    public static class Levit
    {
        private enum Status
        {
            Already,
            Now,
            NotYet
        }

        public static (Dictionary<ulong, double>, Dictionary<ulong, ulong>) Calculate(ulong startId = 4198407189)
        {
            var distance = SvgHelper.Dictionary.ToDictionary(k => k.Key, v => double.MaxValue);
            var p = new Dictionary<ulong, ulong>(SvgHelper.Dictionary.Count);
            var status = SvgHelper.Dictionary.ToDictionary(k => k.Key, v => Status.NotYet);
            var queue = new Queue<ulong>(SvgHelper.Dictionary.Count);
            var mainQueue = new Queue<ulong>(SvgHelper.Dictionary.Count);

            distance[startId] = 0;
            status[startId] = Status.Now;
            queue.Enqueue(startId);

            while (queue.Any() || mainQueue.Any())
            {
                var u = mainQueue.Any() ? mainQueue.Dequeue() : queue.Dequeue();

                foreach (var v in SvgHelper.Dictionary[u].Adjency)
                {
                    var len = distance[u] + SvgHelper.Dictionary[u].DistanceBetweenPoints(SvgHelper.Dictionary[v]);
                    switch (status[v])
                    {
                        case Status.NotYet:
                            queue.Enqueue(v);
                            status[v] = Status.Now;
                            if (distance[v] > len)
                            {
                                distance[v] = len;
                                p[v] = u;
                            }
                            break;
                        case Status.Now:
                            if (distance[v] > len)
                            {
                                distance[v] = len;
                                p[v] = u;
                            }
                            break;
                        case Status.Already when distance[v] >
                             len:
                            mainQueue.Enqueue(v);
                            status[v] = Status.Now;
                            distance[v] = len;
                            p[v] = u;
                            break;
                    }
                }

                status[u] = Status.Already;
            }

            return (distance, p);
        }
    }
}