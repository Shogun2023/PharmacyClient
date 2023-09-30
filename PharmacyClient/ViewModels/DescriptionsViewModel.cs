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
    public class DescriptionsViewModel : BaseViewModel
    {
        public ObservableCollection<Descriptions> ListDescriptions { get; set; } = new ObservableCollection<Descriptions>();
        internal CollectionViewSource CvsDescriptions { get; set; }
        public DescriptionsViewModel()
        {
            ListDescriptions = GetDescriptions();
            CvsDescriptions = new CollectionViewSource();
            CvsDescriptions.Source = this.ListDescriptions;
            CvsDescriptions.Filter += ApplyFilter;
        }
        public ObservableCollection<Descriptions> GetDescriptions()
        {
            using (var context = new AptekaEntities())
            {
                var query = from description in context.Descriptions
                            select description;
                if (query.Count() != 0)
                {
                    foreach (var d in query)
                    {
                        ListDescriptions.Add(d);
                    }
                }
            }
            return ListDescriptions;
        }
        private string _descriptionFilter;
        public string DescriptionFilter
        {
            get { return _descriptionFilter; }
            set
            {
                _descriptionFilter = value;
                OnFilterChanged();
            }
        }
        private void OnFilterChanged()
        {
            CvsDescriptions.View.Refresh();
        }
        public ICollectionView CollectionDescriptions
        {
            get { return CvsDescriptions.View; }
        }
        void ApplyFilter(object sender, FilterEventArgs e)
        {
            Descriptions descr = (Descriptions)e.Item;

            if (string.IsNullOrWhiteSpace(this.DescriptionFilter) || this.DescriptionFilter.Length == 0)
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = descr.DES_MANUF.Contains(DescriptionFilter) || descr.DES_GROUP.Contains(DescriptionFilter) ||
                    descr.DES_USE.Contains(DescriptionFilter);
            }
        }
        private Descriptions _selectedDescription;
        public Descriptions SelectedDescription
        {
            get
            {
                return _selectedDescription;
            }
            set
            {
                _selectedDescription = value;
                OnPropertyChanged("SelectedDescription");
                EditDescription.CanExecute(true);
            }
        }
        private RelayCommand _addDescription;
        public RelayCommand AddDescription
        {
            get
            {
                return _addDescription ??
                    (_addDescription = new RelayCommand(obj =>
                    {
                        Descriptions newDescription = new Descriptions();
                        NewDescriptionWindow newDescriptionWindow = new NewDescriptionWindow
                        {
                            Title = "Новое описание",
                            DataContext = newDescription
                        };
                        newDescriptionWindow.ShowDialog();
                        if (newDescriptionWindow.DialogResult == true)
                        {
                            using (var context = new AptekaEntities())
                            {
                                try
                                {
                                    context.Descriptions.Add(newDescription);
                                    context.SaveChanges();
                                    ListDescriptions.Clear();
                                    ListDescriptions = GetDescriptions();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка добавления данных!\n" + ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }, (obj) => AuthViewModel.role != 3));
            }
        }
        private RelayCommand _editDescription;
        public RelayCommand EditDescription
        {
            get
            {
                return _editDescription ??
                (_editDescription = new RelayCommand(obj =>
                {
                    Descriptions editDescription = SelectedDescription;
                    NewDescriptionWindow newDescriptionWindow = new NewDescriptionWindow
                    {
                        Title = "Редактирование описания",
                        DataContext = editDescription
                    };
                    newDescriptionWindow.ShowDialog();
                    if (newDescriptionWindow.DialogResult == true)
                    {
                        using (var context = new AptekaEntities())
                        {
                            Descriptions description = context.Descriptions.Find(editDescription.DES_ID);
                            description.DES_MANUF = editDescription.DES_MANUF.Trim();
                            description.DES_GROUP = editDescription.DES_GROUP.Trim();
                            description.DES_USE = editDescription.DES_USE.Trim();
                            description.DES_STORAGE = editDescription.DES_STORAGE.Trim();
                            try
                            {
                                context.SaveChanges();
                                ListDescriptions.Clear();
                                ListDescriptions = GetDescriptions();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("\nОшибка редактирования данных!\n" + ex.Message, "Предупреждение");
                            }
                        }
                    }
                    else
                    {
                        ListDescriptions.Clear();
                        ListDescriptions = GetDescriptions();
                    }
                }, (obj) => SelectedDescription != null && ListDescriptions.Count > 0 && AuthViewModel.role != 3));
            }
        }
        private RelayCommand _deleteDescription;
        public RelayCommand DeleteDescription
        {
            get
            {
                return _deleteDescription ??
                (_deleteDescription = new RelayCommand(obj =>
                {
                    Descriptions description = SelectedDescription;
                    using (var context = new AptekaEntities())
                    {
                        Descriptions delDescription = context.Descriptions.Find(description.DES_ID);
                        if (delDescription != null)
                        {
                            MessageBoxResult result = MessageBox.Show("Удалить данные по описанию препарата", "Предупреждение",
                                MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                            if (result == MessageBoxResult.OK)
                            {
                                try
                                {
                                    context.Descriptions.Remove(delDescription);
                                    context.SaveChanges();
                                    ListDescriptions.Remove(description);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка удаления данных!\n" + ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }
                }, (obj) => SelectedDescription != null && ListDescriptions.Count > 0 && AuthViewModel.role != 3));
            }
        }
    }
}
