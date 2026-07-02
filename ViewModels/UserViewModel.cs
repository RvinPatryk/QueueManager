using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using QueueManager.Helpers;
using QueueManager.Models;
using QueueManager.Repositories;
using QueueManager.Services;

namespace QueueManager.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        private readonly User _loggedUser;
        private readonly TaskRepository _taskRepository = new();

        private QueueTask? _selectedTask;

        private bool _hideCompletedTasks = true;

        private readonly QueueSettingsRepository _queueSettingsRepository = new();
        private readonly TaskSchedulerService _taskSchedulerService = new();

        private QueueTask? _currentTask;
        private QueueTask? _nextTask;
        private SchedulingAlgorithm _selectedAlgorithm;

        public ObservableCollection<QueueTask> Tasks { get; } = new();

        public string LoggedUsername => _loggedUser.Username;

        public QueueTask? SelectedTask
        {
            get => _selectedTask;
            set
            {
                if (SetField(ref _selectedTask, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand RefreshTasksCommand { get; }
        public ICommand StartTaskCommand { get; }
        public ICommand FinishTaskCommand { get; }

        public UserViewModel(User loggedUser)
        {
            _loggedUser = loggedUser;

            RefreshTasksCommand = new RelayCommand(LoadMyTasks);
            StartTaskCommand = new RelayCommand(StartTask, CanStartTask);
            FinishTaskCommand = new RelayCommand(FinishTask, CanFinishTask);

            LoadMyTasks();
        }
        public bool HideCompletedTasks
        {
            get => _hideCompletedTasks;
            set
            {
                if (SetField(ref _hideCompletedTasks, value))
                {
                    LoadMyTasks();
                }
            }
        }

        private void LoadMyTasks()
        {
            try
            {
                Tasks.Clear();

                var allTasks = _taskRepository.GetAll();

                _selectedAlgorithm = _queueSettingsRepository.Get().SelectedAlgorithm;

                var globallyOrderedNewTasks = _taskSchedulerService.OrderTasks(
                    allTasks.Where(task => task.Status == QueueTaskStatus.Nowe),
                    _selectedAlgorithm);

                var myTasks = allTasks
                    .Where(task => string.Equals(
                        task.OsobaPrzypisana,
                        _loggedUser.Username,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();

                CurrentTask = myTasks
                    .Where(task => task.Status == QueueTaskStatus.WTrakcie)
                    .OrderBy(task => task.DataRozpoczecia)
                    .FirstOrDefault();

                NextTask = globallyOrderedNewTasks
                    .FirstOrDefault(task => string.Equals(
                        task.OsobaPrzypisana,
                        _loggedUser.Username,
                        StringComparison.OrdinalIgnoreCase));

                var orderedMyNewTasks = globallyOrderedNewTasks
                    .Where(task => string.Equals(
                        task.OsobaPrzypisana,
                        _loggedUser.Username,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var otherMyTasks = myTasks
                    .Where(task => task.Status != QueueTaskStatus.Nowe)
                    .OrderBy(task => task.Status)
                    .ThenBy(task => task.Termin ?? DateTime.MaxValue)
                    .ToList();

                var displayTasks = orderedMyNewTasks
                    .Concat(otherMyTasks)
                    .Where(task => !HideCompletedTasks ||
                                   task.Status != QueueTaskStatus.Zakonczone)
                    .ToList();

                foreach (var task in displayTasks)
                {
                    Tasks.Add(task);
                }

                OnPropertyChanged(nameof(SelectedAlgorithmName));
            }
            catch (Exception ex)
            {
                AppLogger.Error("Błąd podczas ładowania zadań użytkownika.", ex);
                MessageHelper.ShowError(
                    $"Nie udało się załadować zadań.\n\nSzczegóły: {ex.Message}");
            }
        }

        private void StartTask()
        {
            try
            {
                if (SelectedTask == null)
                    return;

                bool hasTaskInProgress = _taskRepository
                    .GetAll()
                    .Any(task =>
                        string.Equals(
                            task.OsobaPrzypisana,
                            _loggedUser.Username,
                            StringComparison.OrdinalIgnoreCase)
                        && task.Status == QueueTaskStatus.WTrakcie);

                if (hasTaskInProgress)
                {
                    MessageHelper.ShowError(
                        "Nie możesz rozpocząć kolejnego zadania, ponieważ masz już zadanie w trakcie.");

                    return;
                }

                SelectedTask.Status = QueueTaskStatus.WTrakcie;
                SelectedTask.DataRozpoczecia = DateTime.Now;

                _taskRepository.Update(SelectedTask);

                AppLogger.Info(
                    $"Użytkownik '{_loggedUser.Username}' rozpoczął zadanie ID={SelectedTask.Id}.");

                LoadMyTasks();
                CommandManager.InvalidateRequerySuggested();
            }
            catch (Exception ex)
            {
                AppLogger.Error("Błąd podczas rozpoczynania zadania przez użytkownika.", ex);
                MessageHelper.ShowError(
                    $"Nie udało się rozpocząć zadania.\n\nSzczegóły: {ex.Message}");
            }
        }

        private bool CanStartTask()
        {
            bool hasTaskInProgress = Tasks.Any(
                task => task.Status == QueueTaskStatus.WTrakcie);

            return SelectedTask?.Status == QueueTaskStatus.Nowe
                   && !hasTaskInProgress;
        }

        private void FinishTask()
        {
            try
            {
                if (SelectedTask == null)
                    return;

                SelectedTask.Status = QueueTaskStatus.Zakonczone;
                SelectedTask.DataUkonczenia = DateTime.Now;

                _taskRepository.Update(SelectedTask);

                AppLogger.Info(
                    $"Użytkownik '{_loggedUser.Username}' zakończył zadanie ID={SelectedTask.Id}.");

                LoadMyTasks();
                CommandManager.InvalidateRequerySuggested();
            }
            catch (Exception ex)
            {
                AppLogger.Error("Błąd podczas kończenia zadania przez użytkownika.", ex);
                MessageHelper.ShowError(
                    $"Nie udało się zakończyć zadania.\n\nSzczegóły: {ex.Message}");
            }
        }

        private bool CanFinishTask()
        {
            return SelectedTask?.Status == QueueTaskStatus.WTrakcie;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(
            ref T field,
            T value,
            [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        public QueueTask? CurrentTask
        {
            get => _currentTask;
            private set => SetField(ref _currentTask, value);
        }

        public QueueTask? NextTask
        {
            get => _nextTask;
            private set => SetField(ref _nextTask, value);
        }

        public string SelectedAlgorithmName => _selectedAlgorithm.ToString();
    }
}