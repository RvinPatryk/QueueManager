using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QueueManager.Models
{
    public class User : INotifyPropertyChanged
    {
        private int _id;
        private string _username = string.Empty;
        private string _passwordHash = string.Empty;
        private UserRole _role;
        private DateTime _createdAt = DateTime.Now;

        public int Id
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string Username
        {
            get => _username;
            set => SetField(ref _username, value);
        }

        public string PasswordHash
        {
            get => _passwordHash;
            set => SetField(ref _passwordHash, value);
        }

        public UserRole Role
        {
            get => _role;
            set => SetField(ref _role, value);
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set => SetField(ref _createdAt, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
