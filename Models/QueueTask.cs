using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QueueManager.Models
{
    public enum TaskStatus
    {
        Nowe,
        WTrakcie,
        Zakonczone,
        Anulowane
    }

    public class QueueTask : INotifyPropertyChanged
    {
        private int _id;
        private string _nazwa = string.Empty;
        private string _opis = string.Empty;
        private int _priorytet;
        private string _autor = string.Empty;
        private string _osobaPrzypisana = string.Empty;
        private DateTime _dataUtworzenia;
        private DateTime? _dataRozpoczecia;
        private DateTime? _dataUkonczenia;
        private TaskStatus _status;
        private TimeSpan _przewidzianyCzas;
        private DateTime? _termin;

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
            set
            {
                if (value < 1 || value > 10)
                    throw new ArgumentOutOfRangeException(nameof(Priorytet), "Zakres 1–10");

                SetField(ref _priorytet, value);
            }
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
            set
            {
                if (SetField(ref _dataRozpoczecia, value))
                    OnPropertyChanged(nameof(RzeczywistyCzas));
            }
        }

        public DateTime? DataUkonczenia
        {
            get => _dataUkonczenia;
            set
            {
                if (SetField(ref _dataUkonczenia, value))
                    OnPropertyChanged(nameof(RzeczywistyCzas));
            }
        }

        public TaskStatus Status
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

        // Wyliczany automatycznie
        public TimeSpan? RzeczywistyCzas =>
            DataRozpoczecia.HasValue && DataUkonczenia.HasValue
                ? DataUkonczenia.Value - DataRozpoczecia.Value
                : null;

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