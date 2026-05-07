using System;
using System.Collections.Generic;
using System.Linq;
using QueueManager.Models;

namespace QueueManager.Services
{
    public class TaskSchedulerService
    {
        private readonly Random _random = new();

        public QueueTask? GetNextTask(IEnumerable<QueueTask> tasks, SchedulingAlgorithm algorithm)
        {
            var availableTasks = tasks
                .Where(t => t.Status == QueueTaskStatus.Nowe)
                .ToList();

            if (availableTasks.Count == 0)
                return null;

            return algorithm switch
            {
                SchedulingAlgorithm.FIFO => GetFifo(availableTasks),
                SchedulingAlgorithm.LIFO => GetLifo(availableTasks),
                SchedulingAlgorithm.Priority => GetPriority(availableTasks),
                SchedulingAlgorithm.SJF => GetSjf(availableTasks),
                SchedulingAlgorithm.LJF => GetLjf(availableTasks),
                SchedulingAlgorithm.Random => GetRandom(availableTasks),
                SchedulingAlgorithm.EDF => GetEdf(availableTasks),
                SchedulingAlgorithm.WeightedRandom => GetWeightedRandom(availableTasks),
                _ => GetFifo(availableTasks)
            };
        }

        private QueueTask? GetFifo(List<QueueTask> tasks)
        {
            return tasks
                .OrderBy(t => t.DataUtworzenia)
                .FirstOrDefault();
        }

        private QueueTask? GetLifo(List<QueueTask> tasks)
        {
            return tasks
                .OrderByDescending(t => t.DataUtworzenia)
                .FirstOrDefault();
        }

        private QueueTask? GetPriority(List<QueueTask> tasks)
        {
            return tasks
                .OrderByDescending(t => t.Priorytet)
                .ThenBy(t => t.DataUtworzenia)
                .FirstOrDefault();
        }

        private QueueTask? GetSjf(List<QueueTask> tasks)
        {
            return tasks
                .OrderBy(t => t.PrzewidzianyCzas)
                .ThenByDescending(t => t.Priorytet)
                .ThenBy(t => t.DataUtworzenia)
                .FirstOrDefault();
        }

        private QueueTask? GetLjf(List<QueueTask> tasks)
        {
            return tasks
                .OrderByDescending(t => t.PrzewidzianyCzas)
                .ThenByDescending(t => t.Priorytet)
                .ThenBy(t => t.DataUtworzenia)
                .FirstOrDefault();
        }

        private QueueTask? GetRandom(List<QueueTask> tasks)
        {
            int index = _random.Next(tasks.Count);
            return tasks[index];
        }

        private QueueTask? GetEdf(List<QueueTask> tasks)
        {
            return tasks
                .OrderBy(t => t.Termin ?? DateTime.MaxValue)
                .ThenByDescending(t => t.Priorytet)
                .ThenBy(t => t.DataUtworzenia)
                .FirstOrDefault();
        }

        private QueueTask? GetWeightedRandom(List<QueueTask> tasks)
        {
            int totalWeight = tasks.Sum(t => Math.Max(1, t.Priorytet));

            int randomValue = _random.Next(1, totalWeight + 1);

            int cumulativeWeight = 0;

            foreach (var task in tasks)
            {
                cumulativeWeight += Math.Max(1, task.Priorytet);

                if (randomValue <= cumulativeWeight)
                    return task;
            }

            return tasks.LastOrDefault();
        }
    }
}