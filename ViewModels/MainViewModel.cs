using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using QueueManager.Helpers;
using QueueManager.Models;

namespace QueueManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
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

        private IList _selectedTasks = new ArrayList();

        public ObservableCollection<QueueTask> Tasks { get; set; } = new();

        public int Id
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string Nazwa
        {
            get => _nazwa;
            set => SetField(ref _nazwa, value);
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

        public MainViewModel()
        {
            AddTaskCommand = new RelayCommand(AddTask);
            DeleteTaskCommand = new RelayCommand(DeleteTasks, CanDeleteTasks);
            UpdateTaskCommand = new RelayCommand(UpdateTask, CanUpdateTask);
        }

        private void AddTask()
        {
            var task = new QueueTask
            {
                Id = Id,
                Nazwa = Nazwa,
                Opis = Opis,
                Priorytet = Priorytet,
                Autor = Autor,
                OsobaPrzypisana = OsobaPrzypisana,
                DataUtworzenia = DataUtworzenia,
                DataRozpoczecia = DataRozpoczecia,
                DataUkonczenia = DataUkonczenia,
                Status = Status,
                PrzewidzianyCzas = PrzewidzianyCzas,
                Termin = Termin
            };

            Tasks.Add(task);
            ClearForm();
        }

        private void DeleteTasks()
        {
            var tasksToDelete = SelectedTasks.Cast<QueueTask>().ToList();

            foreach (var task in tasksToDelete)
            {
                Tasks.Remove(task);
            }

            SelectedTasks = new ArrayList();
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
            if (SelectedTasks == null || SelectedTasks.Count != 1)
                return;

            if (SelectedTasks[0] is not QueueTask task)
                return;

            task.Id = Id;
            task.Nazwa = Nazwa;
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

            ClearForm();
        }

        private bool CanUpdateTask()
        {
            return SelectedTasks != null && SelectedTasks.Count == 1;
        }

        private void ClearForm()
        {
            Id = 0;
            Nazwa = string.Empty;
            Opis = string.Empty;
            Priorytet = 1;
            Autor = string.Empty;
            OsobaPrzypisana = string.Empty;
            DataUtworzenia = DateTime.Now;
            DataRozpoczecia = null;
            DataUkonczenia = null;
            Status = QueueTaskStatus.Nowe;
            PrzewidzianyCzas = TimeSpan.FromMinutes(30);
            Termin = null;
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
    }
}