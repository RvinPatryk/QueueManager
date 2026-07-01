using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using QueueManager.Helpers;
using QueueManager.Models;
using System.Windows.Data;
using QueueManager.Services;
using QueueManager.Views;
using Microsoft.Win32;
using QueueManager.Repositories;
using QueueManager.Models;


namespace QueueManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int _nextId = 1;

        private int _id;
        private string _nazwa = string.Empty;
        private string _opis = string.Empty;
        private int _priorytet = 1;
        private string _autor = string.Empty;
        private string _osobaPrzypisana = string.Empty;
        private DateTime _dataUtworzenia = DateTime.Now;
        private DateTime? _dataRozpoczecia;
        private DateTime? _dataUkonczenia;
        private QueueTaskStatus _status = QueueTaskStatus.Nowe;
        private TimeSpan _przewidzianyCzas = TimeSpan.FromMinutes(30);
        private DateTime? _termin;
        private readonly User _loggedUser;

        private IList _selectedTasks = new ArrayList();
        private string _searchText = string.Empty;
        private QueueTaskStatus? _selectedStatusFilter;

        public ObservableCollection<QueueTask> Tasks { get; set; } = new();

        public Array StatusOptions => Enum.GetValues(typeof(QueueTaskStatus));

        public ObservableCollection<string> Usernames { get; } = new();

        public void LoadUsernames()
        {
            Usernames.Clear();

            foreach (var username in _userRepository
                         .GetAll()
                         .OrderBy(user => user.Username)
                         .Select(user => user.Username))
            {
                Usernames.Add(username);
            }
        }

        public int Id
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string Nazwa
        {
            get => _nazwa;
            set
            {
                if (SetField(ref _nazwa, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        public string Opis
        {
            get => _opis;
            set => SetField(ref _opis, value);
        }

        public int Priorytet
        {
            get => _priorytet;
            set => SetField(ref _priorytet, value);
        }

        public string Autor
        {
            get => _autor;
            set => SetField(ref _autor, value);
        }

        public string OsobaPrzypisana
        {
            get => _osobaPrzypisana;
            set => SetField(ref _osobaPrzypisana, value);
        }

        public DateTime DataUtworzenia
        {
            get => _dataUtworzenia;
            set => SetField(ref _dataUtworzenia, value);
        }

        public DateTime? DataRozpoczecia
        {
            get => _dataRozpoczecia;
            set => SetField(ref _dataRozpoczecia, value);
        }

        public DateTime? DataUkonczenia
        {
            get => _dataUkonczenia;
            set => SetField(ref _dataUkonczenia, value);
        }

        public QueueTaskStatus Status
        {
            get => _status;
            set => SetField(ref _status, value);
        }

        public TimeSpan PrzewidzianyCzas
        {
            get => _przewidzianyCzas;
            set => SetField(ref _przewidzianyCzas, value);
        }

        public DateTime? Termin
        {
            get => _termin;
            set => SetField(ref _termin, value);
        }

        public IList SelectedTasks
        {
            get => _selectedTasks;
            set
            {
                if (SetField(ref _selectedTasks, value))
                {
                    LoadSelectedTaskToForm();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ICommand AddTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand UpdateTaskCommand { get; }
        public ICommand ShowLogsCommand { get; }

        public ICollectionView TasksView { get; }

        public Array StatusFilterOptions => Enum.GetValues(typeof(QueueTaskStatus));

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetField(ref _searchText, value))
                    TasksView.Refresh();
            }
        }

        public QueueTaskStatus? SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                if (SetField(ref _selectedStatusFilter, value))
                    TasksView.Refresh();
            }
        }

        public ICommand ClearFiltersCommand { get; }


        public MainViewModel(User loggedUser)
        {
            _loggedUser = loggedUser;
            Autor = _loggedUser.Username;
            Id = _nextId;

            TasksView = CollectionViewSource.GetDefaultView(Tasks);
            TasksView.Filter = FilterTasks;

            AddTaskCommand = new RelayCommand(AddTask, CanAddTask);
            DeleteTaskCommand = new RelayCommand(DeleteTasks, CanDeleteTasks);
            UpdateTaskCommand = new RelayCommand(UpdateTask, CanUpdateTask);
            ClearFiltersCommand = new RelayCommand(ClearFilters);

            SelectNextTaskCommand = new RelayCommand(SelectNextTask, CanSelectNextTask);
            StartNextTaskCommand = new RelayCommand(StartNextTask, CanStartNextTask);
            FinishSelectedTasksCommand = new RelayCommand(FinishSelectedTasks, CanFinishSelectedTasks);
            ShowLogsCommand = new RelayCommand(ShowLogs);
            ExportTasksCommand = new RelayCommand(ExportTasks, CanExportTasks);
            
            LoadUsernames();
            LoadTasksFromDatabase();
            RefreshStatistics();
        }

        private bool FilterTasks(object obj)
        {
            if (obj is not QueueTask task)
                return false;

            bool matchesSearch =
                string.IsNullOrWhiteSpace(SearchText)
                || task.Nazwa.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                || task.Opis.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                || task.Autor.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                || task.OsobaPrzypisana.Contains(SearchText, StringComparison.OrdinalIgnoreCase);

            bool matchesStatus =
                SelectedStatusFilter == null
                || task.Status == SelectedStatusFilter;

            return matchesSearch && matchesStatus;
        }

        private void ClearFilters()
        {
            SearchText = string.Empty;
            SelectedStatusFilter = null;
            TasksView.Refresh();
        }

        private void AddTask()
        {
            try
            {
                if (!ValidateTaskForm())
                    return;
                var task = new QueueTask
                {
                    Id = _nextId++,
                    Nazwa = Nazwa.Trim(),
                    Opis = Opis,
                    Priorytet = Priorytet,
                    Autor = Autor,
                    OsobaPrzypisana = OsobaPrzypisana,
                    DataUtworzenia = DateTime.Now,
                    DataRozpoczecia = DataRozpoczecia,
                    DataUkonczenia = DataUkonczenia,
                    Status = Status,
                    PrzewidzianyCzas = PrzewidzianyCzas,
                    Termin = Termin
                };

                _taskRepository.Add(task);
                Tasks.Add(task);
                AppLogger.Info($"Dodano zadanie ID={task.Id}, Nazwa='{task.Nazwa}', Priorytet={task.Priorytet}, Status={task.Status}.");
                ClearForm();
                TasksView.Refresh();
                CommandManager.InvalidateRequerySuggested();
                RefreshStatistics();
            }
            catch (Exception ex)
            {
                AppLogger.Error("Błąd podczas dodawania zadania.", ex);
                MessageHelper.ShowError($"Nie udało się dodać zadania.\n\nSzczegóły: {ex.Message}");
            }
        }

        private bool CanAddTask()
        {
            return !string.IsNullOrWhiteSpace(Nazwa)
                   && Nazwa.Trim().Length >= 3
                   && Priorytet >= 1
                   && Priorytet <= 10
                   && PrzewidzianyCzas > TimeSpan.Zero;
        }

        private void DeleteTasks()
        {
            try
            {
                if (SelectedTasks == null || SelectedTasks.Count == 0)
                    return;

                var result = MessageBox.Show(
                    $"Czy na pewno chcesz usunąć zaznaczone zadania: {SelectedTasks.Count}?",
                    "Potwierdzenie usunięcia",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return;

                var tasksToDelete = SelectedTasks.Cast<QueueTask>().ToList();

                AppLogger.Warning($"Usuwanie zadań. Liczba={tasksToDelete.Count}, ID=[{string.Join(", ", tasksToDelete.Select(t => t.Id))}].");

                _taskRepository.DeleteMany(tasksToDelete.Select(t => t.Id));

                foreach (var task in tasksToDelete)
                {
                    Tasks.Remove(task);
                }

                SelectedTasks = new ArrayList();
                ClearForm();
                NextTask = null;
                TasksView.Refresh();
                CommandManager.InvalidateRequerySuggested();
                RefreshStatistics();
            }
            catch (Exception ex)
            {
                AppLogger.Error("Błąd podczas usuwania zadań.", ex);
                MessageHelper.ShowError($"Nie udało się usunąć zadań.\n\nSzczegóły: {ex.Message}");
            }
        }

        private bool CanDeleteTasks()
        {
            return SelectedTasks != null && SelectedTasks.Count > 0;
        }

        private void LoadSelectedTaskToForm()
        {
            if (SelectedTasks == null || SelectedTasks.Count != 1)
                return;

            if (SelectedTasks[0] is not QueueTask task)
                return;

            Id = task.Id;
            Nazwa = task.Nazwa;
            Opis = task.Opis;
            Priorytet = task.Priorytet;
            Autor = task.Autor;
            OsobaPrzypisana = task.OsobaPrzypisana;
            DataUtworzenia = task.DataUtworzenia;
            DataRozpoczecia = task.DataRozpoczecia;
            DataUkonczenia = task.DataUkonczenia;
            Status = task.Status;
            PrzewidzianyCzas = task.PrzewidzianyCzas;
            Termin = task.Termin;
        }

        private void UpdateTask()
        {
            try
            {
                if (SelectedTasks == null || SelectedTasks.Count != 1)
                    return;

                if (SelectedTasks[0] is not QueueTask task)
                    return;

                if (!ValidateTaskForm())
                    return;

                if (string.IsNullOrWhiteSpace(Nazwa))
                    return;

                task.Nazwa = Nazwa.Trim();
                task.Opis = Opis;
                task.Priorytet = Priorytet;
                task.Autor = Autor;
                task.OsobaPrzypisana = OsobaPrzypisana;
                task.DataUtworzenia = DataUtworzenia;
                task.DataRozpoczecia = DataRozpoczecia;
                task.DataUkonczenia = DataUkonczenia;
                task.Status = Status;
                task.PrzewidzianyCzas = PrzewidzianyCzas;
                task.Termin = Termin;

                _taskRepository.Update(task);

                AppLogger.Info($"Zaktualizowano zadanie ID={task.Id}, Nazwa='{task.Nazwa}', Status={task.Status}.");
                ClearForm();
                TasksView.Refresh();
                CommandManager.InvalidateRequerySuggested();
                RefreshStatistics();
            }
            catch (Exception ex)
            {
                AppLogger.Error("Błąd podczas aktualizacji zadania.", ex);
                MessageHelper.ShowError($"Nie udało się zaktualizować zadania.\n\nSzczegóły: {ex.Message}");
            }
        }

        private bool CanUpdateTask()
        {
            return SelectedTasks != null
                   && SelectedTasks.Count == 1
                   && !string.IsNullOrWhiteSpace(Nazwa)
                   && Nazwa.Trim().Length >= 3
                   && Priorytet >= 1
                   && Priorytet <= 10
                   && PrzewidzianyCzas > TimeSpan.Zero;
        }

        private void ClearForm()
        {
            Id = _nextId;
            Nazwa = string.Empty;
            Opis = string.Empty;
            Priorytet = 1;
            Autor = _loggedUser.Username;
            OsobaPrzypisana = string.Empty;
            DataUtworzenia = DateTime.Now;
            DataRozpoczecia = null;
            DataUkonczenia = null;
            Status = QueueTaskStatus.Nowe;
            PrzewidzianyCzas = TimeSpan.FromMinutes(30);
            Termin = null;
        }

        private bool ValidateTaskForm()
        {
            if (string.IsNullOrWhiteSpace(Nazwa))
            {
                MessageHelper.ShowError("Nazwa zadania jest wymagana.");
                return false;
            }

            if (Nazwa.Trim().Length < 3)
            {
                MessageHelper.ShowError("Nazwa zadania musi mieć co najmniej 3 znaki.");
                return false;
            }

            if (Nazwa.Trim().Length > 100)
            {
                MessageHelper.ShowError("Nazwa zadania może mieć maksymalnie 100 znaków.");
                return false;
            }

            if (Priorytet < 1 || Priorytet > 10)
            {
                MessageHelper.ShowError("Priorytet musi być w zakresie od 1 do 10.");
                return false;
            }

            if (PrzewidzianyCzas <= TimeSpan.Zero)
            {
                MessageHelper.ShowError("Przewidziany czas musi być większy od zera.");
                return false;
            }

            if (Termin.HasValue && Termin.Value.Date < DataUtworzenia.Date)
            {
                MessageHelper.ShowError("Termin nie może być wcześniejszy niż data utworzenia.");
                return false;
            }

            if (DataRozpoczecia.HasValue &&
                DataUkonczenia.HasValue &&
                DataUkonczenia.Value < DataRozpoczecia.Value)
            {
                MessageHelper.ShowError("Data ukończenia nie może być wcześniejsza niż data rozpoczęcia.");
                return false;
            }

            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(name);
            return true;
        }

        private readonly TaskSchedulerService _schedulerService = new();

        private readonly TaskExportService _taskExportService = new();

        private readonly TaskRepository _taskRepository = new();

        private readonly UserRepository _userRepository = new();

        private SchedulingAlgorithm _selectedAlgorithm = SchedulingAlgorithm.FIFO;
        private QueueTask? _nextTask;

        public Array AlgorithmOptions => Enum.GetValues(typeof(SchedulingAlgorithm));

        public SchedulingAlgorithm SelectedAlgorithm
        {
            get => _selectedAlgorithm;
            set
            {
                if (SetField(ref _selectedAlgorithm, value))
                {
                    NextTask = null;
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public QueueTask? NextTask
        {
            get => _nextTask;
            set => SetField(ref _nextTask, value);
        }

        public ICommand SelectNextTaskCommand { get; }
        public ICommand StartNextTaskCommand { get; }
        public ICommand FinishSelectedTasksCommand { get; }
        public ICommand ExportTasksCommand { get; }

        private void SelectNextTask()
        {
            try
            {
                NextTask = _schedulerService.GetNextTask(Tasks, SelectedAlgorithm);

                if (NextTask != null)
                {
                    AppLogger.Info($"Wybrano następne zadanie algorytmem {SelectedAlgorithm}: ID={NextTask.Id}, Nazwa='{NextTask.Nazwa}'.");
                }
                else
                {
                    AppLogger.Info($"Nie wybrano zadania algorytmem {SelectedAlgorithm}. Brak zadań Nowe.");
                }

                if (NextTask == null)
                    MessageHelper.ShowInfo("Brak nowych zadań do wyboru.");

                CommandManager.InvalidateRequerySuggested();
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError($"Nie udało się wybrać następnego zadania.\n\nSzczegóły: {ex.Message}");
            }
        }

        private bool CanSelectNextTask()
        {
            return Tasks.Any(t => t.Status == QueueTaskStatus.Nowe);
        }

        private void StartNextTask()
        {
            try
            {
                if (NextTask == null)
                {
                    MessageHelper.ShowError("Nie wybrano zadania do rozpoczęcia.");
                    return;
                }

                if (NextTask.Status != QueueTaskStatus.Nowe)
                {
                    MessageHelper.ShowError("Można rozpocząć tylko zadanie ze statusem Nowe.");
                    return;
                }

                NextTask.Status = QueueTaskStatus.WTrakcie;
                NextTask.DataRozpoczecia = DateTime.Now;

                _taskRepository.Update(NextTask);

                AppLogger.Info($"Rozpoczęto zadanie ID={NextTask.Id}, Nazwa='{NextTask.Nazwa}'.");

                NextTask = null;

                TasksView.Refresh();
                CommandManager.InvalidateRequerySuggested();
                RefreshStatistics();
            }
            catch (Exception ex)
            {

                MessageHelper.ShowError($"Nie udało się rozpocząć zadania.\n\nSzczegóły: {ex.Message}");
            }
        }

        private bool CanStartNextTask()
        {
            return NextTask != null && NextTask.Status == QueueTaskStatus.Nowe;
        }

        private void FinishSelectedTasks()
        {
            try
            {
                if (SelectedTasks == null || SelectedTasks.Count == 0)
                    return;

                var tasksToFinish = SelectedTasks
                    .Cast<QueueTask>()
                    .Where(t => t.Status == QueueTaskStatus.WTrakcie)
                    .ToList();

                if (tasksToFinish.Count == 0)
                {
                    MessageHelper.ShowInfo("Zaznaczone zadania nie są w statusie W trakcie.");
                    return;
                }

                foreach (var task in tasksToFinish)
                {
                    task.Status = QueueTaskStatus.Zakonczone;
                    task.DataUkonczenia = DateTime.Now;

                    _taskRepository.Update(task);
                }

                AppLogger.Info($"Zakończono zadania. Liczba={tasksToFinish.Count}, ID=[{string.Join(", ", tasksToFinish.Select(t => t.Id))}].");

                TasksView.Refresh();
                CommandManager.InvalidateRequerySuggested();
                RefreshStatistics();
            }
            catch (Exception ex)
            {
                AppLogger.Error("Błąd podczas kończenia zadań.", ex);
                MessageHelper.ShowError($"Nie udało się zakończyć zadania.\n\nSzczegóły: {ex.Message}");
            }
        }

        private bool CanFinishSelectedTasks()
        {
            return SelectedTasks != null
                   && SelectedTasks.Cast<QueueTask>().Any(t => t.Status == QueueTaskStatus.WTrakcie);
        }

        

        public int TotalTasksCount => Tasks.Count;

        public int NewTasksCount => Tasks.Count(t => t.Status == QueueTaskStatus.Nowe);

        public int InProgressTasksCount => Tasks.Count(t => t.Status == QueueTaskStatus.WTrakcie);

        public int CompletedTasksCount => Tasks.Count(t => t.Status == QueueTaskStatus.Zakonczone);

        public int CancelledTasksCount => Tasks.Count(t => t.Status == QueueTaskStatus.Anulowane);

        public string AverageEstimatedTime
        {
            get
            {
                if (Tasks.Count == 0)
                    return "brak";

                var averageTicks = (long)Tasks.Average(t => t.PrzewidzianyCzas.Ticks);
                return new TimeSpan(averageTicks).ToString(@"hh\:mm\:ss");
            }
        }

        public string NearestDeadline
        {
            get
            {
                var nearest = Tasks
                    .Where(t => t.Termin.HasValue && t.Status != QueueTaskStatus.Zakonczone)
                    .OrderBy(t => t.Termin)
                    .FirstOrDefault();

                return nearest?.Termin?.ToString("dd.MM.yyyy") ?? "brak";
            }
        }

        private void RefreshStatistics()
        {
            OnPropertyChanged(nameof(TotalTasksCount));
            OnPropertyChanged(nameof(NewTasksCount));
            OnPropertyChanged(nameof(InProgressTasksCount));
            OnPropertyChanged(nameof(CompletedTasksCount));
            OnPropertyChanged(nameof(CancelledTasksCount));
            OnPropertyChanged(nameof(AverageEstimatedTime));
            OnPropertyChanged(nameof(NearestDeadline));
        }

        private void ShowLogs()
        {
            try
            {
                string logs = AppLogger.ReadAll();

                var logWindow = new LogWindow(logs);
                logWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError($"Nie udało się wyświetlić logów.\n\nSzczegóły: {ex.Message}");
            }
        }

        private void ExportTasks()
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    Title = "Eksport zadań do CSV",
                    Filter = "CSV files (.csv)|.csv",
                    FileName = $"zadania_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                bool? result = dialog.ShowDialog();

                if (result != true)
                    return;

                _taskExportService.ExportToCsv(Tasks, dialog.FileName);

                AppLogger.Info($"Wyeksportowano zadania do pliku: {dialog.FileName}");
                MessageHelper.ShowInfo("Eksport zadań zakończony pomyślnie.");
            }
            catch (Exception ex)
            {
                AppLogger.Error("Błąd podczas eksportu zadań.", ex);
                MessageHelper.ShowError($"Nie udało się wyeksportować zadań.\n\nSzczegóły: {ex.Message}");
            }
        }

        private bool CanExportTasks()
        {
            return Tasks.Count > 0;
        }

        private void LoadTasksFromDatabase()
        {
            try
            {
                var tasksFromDatabase = _taskRepository.GetAll();

                foreach (var task in tasksFromDatabase)
                {
                    Tasks.Add(task);
                }

                _nextId = Tasks.Count > 0
                    ? Tasks.Max(task => task.Id) + 1
                    : 1;

                Id = _nextId;

                AppLogger.Info($"Załadowano zadania z bazy danych. Liczba: {Tasks.Count}.");
            }
            catch (Exception ex)
            {
                AppLogger.Error("Błąd podczas ładowania zadań z bazy danych.", ex);
                MessageHelper.ShowError(
                    $"Nie udało się załadować danych z bazy.\n\nSzczegóły: {ex.Message}");
            }
        }
    }
}