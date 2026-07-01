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

                var myTasks = _taskRepository
                    .GetAll()
                    .Where(t => string.Equals(
                        t.OsobaPrzypisana,
                        _loggedUser.Username,
                        StringComparison.OrdinalIgnoreCase))
                    .Where(t => !HideCompletedTasks || t.Status != QueueTaskStatus.Zakonczone)
                    .OrderBy(t => t.Status)
                    .ThenBy(t => t.Termin ?? DateTime.MaxValue)
                    .ToList();

                foreach (var task in myTasks)
                    Tasks.Add(task);
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
            return SelectedTask?.Status == QueueTaskStatus.Nowe;
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
    }
}