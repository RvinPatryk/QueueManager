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
    public class UserManagementViewModel : INotifyPropertyChanged
    {
        private readonly User _loggedAdmin;
        private readonly UserRepository _userRepository = new();
        private readonly AuthService _authService = new();

        private User? _selectedUser;
        private string _newUsername = string.Empty;
        private string _newPassword = string.Empty;
        private UserRole _newUserRole = UserRole.User;

        public ObservableCollection<User> Users { get; } = new();

        public Array UserRoleOptions => Enum.GetValues(typeof(UserRole));

        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (!SetField(ref _selectedUser, value))
                    return;

                if (value != null)
                {
                    NewUsername = value.Username;
                    NewUserRole = value.Role;
                    NewPassword = string.Empty;
                }
                else
                {
                    NewUsername = string.Empty;
                    NewPassword = string.Empty;
                    NewUserRole = UserRole.User;
                }

                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string NewUsername
        {
            get => _newUsername;
            set
            {
                if (SetField(ref _newUsername, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        public string NewPassword
        {
            get => _newPassword;
            set
            {
                if (SetField(ref _newPassword, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        public UserRole NewUserRole
        {
            get => _newUserRole;
            set => SetField(ref _newUserRole, value);
        }

        public ICommand RefreshUsersCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand UpdateUserRoleCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand ClearFormCommand { get; }

        public UserManagementViewModel(User loggedAdmin)
        {
            _loggedAdmin = loggedAdmin;

            RefreshUsersCommand = new RelayCommand(LoadUsers);
            AddUserCommand = new RelayCommand(AddUser, CanAddUser);
            UpdateUserRoleCommand = new RelayCommand(UpdateUserRole, CanUpdateUserRole);
            DeleteUserCommand = new RelayCommand(DeleteUser, CanDeleteUser);
            ClearFormCommand = new RelayCommand(ClearForm);

            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                Users.Clear();

                foreach (var user in _userRepository
                    .GetAll()
                    .OrderBy(user => user.Username))
                {
                    Users.Add(user);
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Błąd podczas ładowania użytkowników.", ex);
                MessageHelper.ShowError(
                    $"Nie udało się załadować użytkowników.\n\nSzczegóły: {ex.Message}");
            }
        }

        private bool CanAddUser()
        {
            return !string.IsNullOrWhiteSpace(NewUsername)
                   && NewUsername.Trim().Length >= 3
                   && NewPassword.Length >= 6;
        }

        private void AddUser()
        {
            try
            {
                string username = NewUsername.Trim();

                if (_userRepository.UsernameExists(username))
                {
                    MessageHelper.ShowError("Taka nazwa użytkownika jest już zajęta.");
                    return;
                }

                bool registered = _authService.Register(
                    username,
                    NewPassword,
                    NewUserRole);

                if (!registered)
                {
                    MessageHelper.ShowError("Nie udało się utworzyć użytkownika.");
                    return;
                }

                AppLogger.Info(
                    $"Administrator '{_loggedAdmin.Username}' utworzył użytkownika '{username}'.");

                MessageHelper.ShowInfo("Użytkownik został utworzony.");

                ClearForm();
                LoadUsers();
            }
            catch (Exception ex)
            {
                AppLogger.Error("Błąd podczas tworzenia użytkownika przez administratora.", ex);
                MessageHelper.ShowError(
                    $"Nie udało się utworzyć użytkownika.\n\nSzczegóły: {ex.Message}");
            }
        }

        private bool CanUpdateUserRole()
        {
            return SelectedUser != null
                   && SelectedUser.Id != _loggedAdmin.Id;
        }

        private void UpdateUserRole()
        {
            try
            {
                if (SelectedUser == null)
                    return;

                if (SelectedUser.Id == _loggedAdmin.Id)
                {
                    MessageHelper.ShowError(
                        "Nie możesz zmienić roli aktualnie zalogowanego administratora.");
                    return;
                }

                SelectedUser.Username = NewUsername.Trim();
                SelectedUser.Role = NewUserRole;

                _userRepository.Update(SelectedUser);

                AppLogger.Info(
                    $"Administrator '{_loggedAdmin.Username}' zmienił dane użytkownika ID={SelectedUser.Id}.");

                MessageHelper.ShowInfo("Dane użytkownika zostały zapisane.");

                LoadUsers();
            }
            catch (Exception ex)
            {
                AppLogger.Error("Błąd podczas aktualizacji użytkownika.", ex);
                MessageHelper.ShowError(
                    $"Nie udało się zapisać zmian.\n\nSzczegóły: {ex.Message}");
            }
        }

        private bool CanDeleteUser()
        {
            return SelectedUser != null
                   && SelectedUser.Id != _loggedAdmin.Id;
        }

        private void DeleteUser()
        {
            try
            {
                if (SelectedUser == null)
                    return;

                if (SelectedUser.Id == _loggedAdmin.Id)
                {
                    MessageHelper.ShowError(
                        "Nie możesz usunąć aktualnie zalogowanego administratora.");
                    return;
                }

                bool confirmed = MessageHelper.Confirm(
                    $"Czy na pewno chcesz usunąć użytkownika „{SelectedUser.Username}”?");

                if (!confirmed)
                    return;

                string username = SelectedUser.Username;

                _userRepository.Delete(SelectedUser.Id);

                AppLogger.Warning(
                    $"Administrator '{_loggedAdmin.Username}' usunął użytkownika '{username}'.");

                MessageHelper.ShowInfo("Użytkownik został usunięty.");

                ClearForm();
                LoadUsers();
            }
            catch (Exception ex)
            {
                AppLogger.Error("Błąd podczas usuwania użytkownika.", ex);
                MessageHelper.ShowError(
                    $"Nie udało się usunąć użytkownika.\n\nSzczegóły: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            SelectedUser = null;
            NewUsername = string.Empty;
            NewPassword = string.Empty;
            NewUserRole = UserRole.User;

            CommandManager.InvalidateRequerySuggested();
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