using System.Collections.Generic;
using System.Linq;
using CityMap.Helpers;

namespace CityMap.Algorithms.Shortest_Path
{
    /// <see cref="https://neerc.ifmo.ru/wiki/index.php?title=Алгоритм_Левита"/>
    public static class Levit
    {
        private enum Status
        {
            Already,
            Now,
            NotYet
        }

        public static (Dictionary<ulong, double>, Dictionary<ulong, ulong>) Calculate(ulong startId)
        {
            var distance = SvgHelper.Dictionary.ToDictionary(k => k.Key, v => double.MaxValue);
            var p = new Dictionary<ulong, ulong>(SvgHelper.Dictionary.Count);
            var status = SvgHelper.Dictionary.ToDictionary(k => k.Key, v => Status.NotYet);
            var queue = new Queue<ulong>();
            var priorityQueue = new Queue<ulong>();

            distance[startId] = 0;
            status[startId] = Status.Now;
            queue.Enqueue(startId);

            while (queue.Any() || priorityQueue.Any())
            {
                var u = priorityQueue.Any() ? priorityQueue.Dequeue() : queue.Dequeue();

                foreach (var to in SvgHelper.Dictionary[u].Adjency)
                {
                    var len = distance[u] + SvgHelper.Dictionary[u].DistanceBetweenPoints(SvgHelper.Dictionary[to]);
                    switch (status[to])
                    {
                        case Status.NotYet:
                            queue.Enqueue(to);
                            status[to] = Status.Now;
                            if (len < distance[to])
                            {
                                distance[to] = len;
                                p[to] = u;
                            }
                            break;
                        case Status.Now:
                            if (len < distance[to])
                            {
                                distance[to] = len;
                                p[to] = u;
                            }
                            break;
                        case Status.Already when len < distance[to]:
                            priorityQueue.Enqueue(to);
                            status[to] = Status.Now;
                            distance[to] = len;
                            p[to] = u;
                            break;
                    }
                }

                status[u] = Status.Already;
            }

            return (distance, p);
        }
    }
}