using PharmacyClient.Helpers;
using PharmacyClient.Models;
using PharmacyClient.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PharmacyClient.ViewModels
{
    public class UsersViewModel : BaseViewModel
    {
        public ObservableCollection<Users> ListUsers { get; set; } = new ObservableCollection<Users>();
        public ObservableCollection<UserRoles> ListUserRoles { get; set; } = new ObservableCollection<UserRoles>();
        public ObservableCollection<UsersDTO> ListUsersDTO { get; set; } = new ObservableCollection<UsersDTO>();
        internal CollectionViewSource CvsUsers { get; set; }
        public UsersViewModel()
        {
            ListUsers = GetUsers();
            ListUserRoles = GetUserRoles();
            ListUsersDTO = GetUsersDTO();
            CvsUsers = new CollectionViewSource();
            CvsUsers.Source = this.ListUsersDTO;
            CvsUsers.Filter += ApplyFilter;
        }
        public ObservableCollection<Users> GetUsers()
        {
            using (var context = new AptekaEntities())
            {
                var query = from user in context.Users
                            select user;
                if (query.Count() != 0)
                {
                    foreach (var u in query)
                    {
                        ListUsers.Add(u);
                    }
                }
            }
            return ListUsers;
        }
        public ObservableCollection<UserRoles> GetUserRoles()
        {
            using (var context = new AptekaEntities())
            {
                var query = from userRole in context.UserRoles
                            select userRole;
                if (query.Count() != 0)
                {
                    foreach (var ur in query)
                    {
                        ListUserRoles.Add(ur);
                    }
                }
            }
            return ListUserRoles;
        }
        public ObservableCollection<UsersDTO> GetUsersDTO()
        {
            List<UserRoles> userRoles = new List<UserRoles>();
            foreach (UserRoles ur in ListUserRoles)
            {
                userRoles.Add(ur);
            }
            Finder finder;
            foreach (var u in ListUsers)
            {
                finder = new Finder(u.U_ROLE);
                UserRoles uRole = userRoles.Find(new Predicate<UserRoles>(finder.UserRolePredicate));
                ListUsersDTO.Add(new UsersDTO
                {
                    U_ID = u.U_ID,
                    U_NAME = u.U_NAME,
                    U_SNAME = u.U_SNAME,
                    U_LOGIN = u.U_LOGIN,
                    U_PASS = u.U_PASS,
                    U_ROLE_NAME = uRole.UR_NAME
                });
            }
            return ListUsersDTO;
        }
        private void Refresh()
        {
            ListUsers.Clear();
            ListUsers = GetUsers();
            ListUsersDTO.Clear();
            ListUsersDTO = GetUsersDTO();
        }
        private string _userFilter;
        public string UserFilter
        {
            get { return _userFilter; }
            set
            {
                _userFilter = value;
                OnFilterChanged();
            }
        }
        private void OnFilterChanged()
        {
            CvsUsers.View.Refresh();
        }
        public ICollectionView CollectionUsersDTO
        {
            get { return CvsUsers.View; }
        }
        void ApplyFilter(object sender, FilterEventArgs e)
        {
            UsersDTO us = (UsersDTO)e.Item;

            if (string.IsNullOrWhiteSpace(this.UserFilter) || this.UserFilter.Length == 0)
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = us.U_NAME.Contains(UserFilter) || us.U_SNAME.Contains(UserFilter) ||
                    us.U_ROLE_NAME.Contains(UserFilter);
            }
        }
        private UsersDTO _selectedUserDTO;
        public UsersDTO SelectedUserDTO
        {
            get
            {
                return _selectedUserDTO;
            }
            set
            {
                _selectedUserDTO = value;
                OnPropertyChanged("SelectedUserDTO");
                EditUser.CanExecute(true);
            }
        }
        private RelayCommand _addUser;
        public RelayCommand AddUser
        {
            get
            {
                return _addUser ??
                    (_addUser = new RelayCommand(obj =>
                    {
                        Users newUser = new Users();
                        UsersDTO newUserDTO = new UsersDTO();
                        NewUserWindow newUserWindow = new NewUserWindow
                        {
                            Title = "Новый пользователь",
                            DataContext = newUserDTO
                        };
                        newUserWindow.cbUserRole.ItemsSource = ListUserRoles;
                        newUserWindow.ShowDialog();
                        if (newUserWindow.DialogResult == true)
                        {
                            var currentUserRole = newUserWindow.cbUserRole.SelectedItem as UserRoles;
                            newUser.U_NAME = newUserDTO.U_NAME.Trim();
                            newUser.U_SNAME = newUserDTO.U_SNAME.Trim();
                            newUser.U_LOGIN = newUserDTO.U_LOGIN.Trim();
                            newUser.U_PASS = newUserDTO.U_PASS.Trim();
                            newUser.U_ROLE = currentUserRole.UR_ID;
                            using (var context = new AptekaEntities())
                            {
                                try
                                {
                                    context.Users.Add(newUser);
                                    context.SaveChanges();
                                    Refresh();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка добавления данных!\n" + ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }, (obj) => true));
            }
        }
        private RelayCommand _editUser;
        public RelayCommand EditUser
        {
            get
            {
                return _editUser ??
                (_editUser = new RelayCommand(obj =>
                {
                    UsersDTO editUser = SelectedUserDTO;
                    NewUserWindow newUserWindow = new NewUserWindow
                    {
                        Title = "Редактирование пользователя",
                        DataContext = editUser
                    };
                    List<UserRoles> ListUsRoles = new List<UserRoles>();
                    foreach (var ur in ListUserRoles)
                    {
                        ListUsRoles.Add(ur);
                    }
                    newUserWindow.cbUserRole.ItemsSource = ListUserRoles;
                    UserRoles tempUserRole = ListUsRoles.Single(ur => ur.UR_NAME == editUser.U_ROLE_NAME);
                    newUserWindow.cbUserRole.SelectedItem = tempUserRole;
                    newUserWindow.ShowDialog();
                    if (newUserWindow.DialogResult == true)
                    {
                        var currentUserRole = newUserWindow.cbUserRole.SelectedItem as UserRoles;
                        using (var context = new AptekaEntities())
                        {
                            Users user = context.Users.Find(editUser.U_ID);
                            user.U_NAME = editUser.U_NAME.Trim();
                            user.U_SNAME = editUser.U_SNAME.Trim();
                            user.U_LOGIN = editUser.U_LOGIN.Trim();
                            user.U_PASS = editUser.U_PASS.Trim();
                            user.U_ROLE = currentUserRole.UR_ID;
                            try
                            {
                                context.SaveChanges();
                                Refresh();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("\nОшибка редактирования данных!\n" + ex.Message, "Предупреждение");
                            }
                        }
                    }
                }, (obj) => SelectedUserDTO != null && ListUsersDTO.Count > 0));
            }
        }
        private RelayCommand _deleteUser;
        public RelayCommand DeleteUser
        {
            get
            {
                return _deleteUser ??
                (_deleteUser = new RelayCommand(obj =>
                {
                    UsersDTO currentUserDTO = SelectedUserDTO;
                    Users currentUser = ListUsers.Single(u => u.U_ID == currentUserDTO.U_ID);
                    if (currentUser != null)
                    {
                        using (var context = new AptekaEntities())
                        {
                            Users delUser = context.Users.Find(currentUser.U_ID);
                            MessageBoxResult result = MessageBox.Show("Удалить данные по пользователю?", "Предупреждение",
                                MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                            if (result == MessageBoxResult.OK)
                            {
                                try
                                {
                                    context.Users.Remove(delUser);
                                    context.SaveChanges();
                                    Refresh();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка удаления данных!\n" + ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }
                }, (obj) => SelectedUserDTO != null && ListUsersDTO.Count > 0));
            }
        }
    }
}
