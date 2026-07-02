using System;
using System.Collections.Generic;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using QueueManager.Models;
using QueueManager.Repositories;

namespace QueueManager.ViewModels
{
    public class StatisticsViewModel
    {
        private readonly TaskRepository _taskRepository = new();

        public ISeries[] StatusSeries { get; }
        public ISeries[] AssignedUsersSeries { get; }
        public ISeries[] PrioritySeries { get; }
        public string AverageActualTime { get; }
        public string TotalActualTime { get; }
        public int CompletedTasksWithDurationCount { get; }

        public Axis[] StatusXAxes { get; }
        public Axis[] AssignedUsersXAxes { get; }
        public Axis[] PriorityXAxes { get; }

        public StatisticsViewModel()
        {
            var tasks = _taskRepository.GetAll();

            var completedTasks = tasks
                .Where(t => t.DataRozpoczecia.HasValue && t.DataUkonczenia.HasValue)
                .Where(t => t.DataUkonczenia.Value >= t.DataRozpoczecia.Value)
                .ToList();

            var totalActualTime = TimeSpan.FromTicks(
                completedTasks.Sum(t =>
                    (t.DataUkonczenia!.Value - t.DataRozpoczecia!.Value).Ticks));

            var averageActualTime = completedTasks.Count > 0
                ? TimeSpan.FromTicks(totalActualTime.Ticks / completedTasks.Count)
                : TimeSpan.Zero;

            CompletedTasksWithDurationCount = completedTasks.Count;
            TotalActualTime = totalActualTime.ToString(@"hh\:mm\:ss");
            AverageActualTime = averageActualTime.ToString(@"hh\:mm\:ss");

            var statusData = new[]
            {
                new
                {
                    Name = "Nowe",
                    Count = tasks.Count(t => t.Status == QueueTaskStatus.Nowe)
                },
                new
                {
                    Name = "W trakcie",
                    Count = tasks.Count(t => t.Status == QueueTaskStatus.WTrakcie)
                },
                new
                {
                    Name = "Zakończone",
                    Count = tasks.Count(t => t.Status == QueueTaskStatus.Zakonczone)
                },
                new
                {
                    Name = "Anulowane",
                    Count = tasks.Count(t => t.Status == QueueTaskStatus.Anulowane)
                }
            };

            StatusSeries =
            [
                new ColumnSeries<int>
                {
                    Name = "Liczba zadań",
                    Values = statusData.Select(x => x.Count).ToArray()
                }
            ];

            StatusXAxes =
            [
                new Axis
                {
                    Labels = statusData.Select(x => x.Name).ToArray()
                }
            ];

            var userData = tasks
                .Where(t => !string.IsNullOrWhiteSpace(t.OsobaPrzypisana))
                .GroupBy(t => t.OsobaPrzypisana)
                .OrderByDescending(group => group.Count())
                .Take(8)
                .ToList();

            AssignedUsersSeries =
            [
                new ColumnSeries<int>
                {
                    Name = "Przypisane zadania",
                    Values = userData.Select(group => group.Count()).ToArray()
                }
            ];

            AssignedUsersXAxes =
            [
                new Axis
                {
                    Labels = userData.Select(group => group.Key).ToArray()
                }
            ];

            var priorityData = Enumerable.Range(1, 10)
                .Select(priority => new
                {
                    Priority = priority,
                    Count = tasks.Count(t => t.Priorytet == priority)
                })
                .ToList();

            PrioritySeries =
            [
                new ColumnSeries<int>
                {
                    Name = "Liczba zadań",
                    Values = priorityData.Select(x => x.Count).ToArray()
                }
            ];

            PriorityXAxes =
            [
                new Axis
                {
                    Labels = priorityData
                        .Select(x => x.Priority.ToString())
                        .ToArray()
                }
            ];

        }
    }
}