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
    public class StorageViewModel : BaseViewModel
    {
        public ObservableCollection<Storage> ListStorage { get; set; } = new ObservableCollection<Storage>();
        public ObservableCollection<StorageDTO> ListStorageDTO { get; set; } = new ObservableCollection<StorageDTO>();
        internal CollectionViewSource CvsStorage { get; set; }
        public StorageViewModel()
        {
            ListStorage = GetStorage();
            ListStorageDTO = GetStorageDTO();
            CvsStorage = new CollectionViewSource();
            CvsStorage.Source = this.ListStorageDTO;
            CvsStorage.Filter += ApplyFilter;
        }
        public ObservableCollection<Storage> GetStorage()
        {
            using (var context = new AptekaEntities())
            {
                var query = from storage in context.Storage
                            select storage;
                if (query.Count() != 0)
                {
                    foreach (var s in query)
                    {
                        ListStorage.Add(s);
                    }
                }
            }
            return ListStorage;
        }
        public ObservableCollection<StorageDTO> GetStorageDTO()
        {
            DrugsViewModel vmDrugs = new DrugsViewModel();
            List<Drugs> drugs = new List<Drugs>();
            foreach (Drugs d in vmDrugs.ListDrugs)
            {
                drugs.Add(d);
            }
            Finder finder;
            foreach (var s in ListStorage)
            {
                finder = new Finder(s.ST_DRUG);
                Drugs dr = drugs.Find(new Predicate<Drugs>(finder.DrugPredicate));
                ListStorageDTO.Add(new StorageDTO
                {
                    ST_ID = s.ST_ID,
                    ST_DR_HEADING = $"{dr.DR_NAME}, {dr.DR_FORM}, {dr.DR_DOSE}",
                    ST_DATE = s.ST_DATE,
                    ST_AMOUNT = s.ST_AMOUNT
                });
            }
            return ListStorageDTO;
        }
        private void Refresh()
        {
            ListStorage.Clear();
            ListStorage = GetStorage();
            ListStorageDTO.Clear();
            ListStorageDTO = GetStorageDTO();
        }
        private string _storageFilter;
        public string StorageFilter
        {
            get { return _storageFilter; }
            set
            {
                _storageFilter = value;
                OnFilterChanged();
            }
        }
        private void OnFilterChanged()
        {
            CvsStorage.View.Refresh();
        }
        public ICollectionView CollectionSalesDTO
        {
            get { return CvsStorage.View; }
        }
        void ApplyFilter(object sender, FilterEventArgs e)
        {
            StorageDTO stor = (StorageDTO)e.Item;

            if (string.IsNullOrWhiteSpace(this.StorageFilter) || this.StorageFilter.Length == 0)
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = stor.ST_DR_HEADING.Contains(StorageFilter);
            }
        }
        private StorageDTO selectedStorageDTO;
        public StorageDTO SelectedStorageDTO
        {
            get
            {
                return selectedStorageDTO;
            }
            set
            {
                selectedStorageDTO = value;
                OnPropertyChanged("SelectedStorageDTO");
                EditStorage.CanExecute(true);
            }
        }
        private RelayCommand _addStorage;
        public RelayCommand AddStorage
        {
            get
            {
                return _addStorage ??
                    (_addStorage = new RelayCommand(obj =>
                    {
                        Storage newStorage = new Storage();
                        StorageDTO newStorageDTO = new StorageDTO { ST_DATE = DateTime.Now };
                        DrugsViewModel vmDrugs = new DrugsViewModel();
                        NewStorageWindow newStorageWindow = new NewStorageWindow
                        {
                            Title = "Новое хранение",
                            DataContext = newStorageDTO,
                        };
                        newStorageWindow.cbDrugHeading.ItemsSource = vmDrugs.ListDrugsDTO;
                        newStorageWindow.ShowDialog();
                        if (newStorageWindow.DialogResult == true)
                        {
                            var currentDrugDTO = newStorageWindow.cbDrugHeading.SelectedItem as DrugsDTO;
                            newStorage.ST_DRUG = currentDrugDTO.DR_ID;
                            newStorage.ST_DATE = newStorageDTO.ST_DATE;
                            newStorage.ST_AMOUNT = newStorageDTO.ST_AMOUNT;
                            using (var context = new AptekaEntities())
                            {
                                try
                                {
                                    context.Storage.Add(newStorage);
                                    context.SaveChanges();
                                    Refresh();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка добавления данных!\n" + ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }, (obj) => AuthViewModel.role != 3 && AuthViewModel.role != 4));
            }
        }
        private RelayCommand _editStorage;
        public RelayCommand EditStorage
        {
            get
            {
                return _editStorage ??
                (_editStorage = new RelayCommand(obj =>
                {
                    StorageDTO editStorage = SelectedStorageDTO;
                    DrugsViewModel vmDrugs = new DrugsViewModel();
                    NewStorageWindow newStorageWindow = new NewStorageWindow
                    {
                        Title = "Редактирование хранения",
                        DataContext = editStorage
                    };
                    List<DrugsDTO> ListDrDTO = new List<DrugsDTO>();
                    foreach (var d in vmDrugs.ListDrugsDTO)
                    {
                        ListDrDTO.Add(d);
                    }
                    newStorageWindow.cbDrugHeading.ItemsSource = vmDrugs.ListDrugsDTO;
                    DrugsDTO tempDrugDTO = ListDrDTO.Single(d => d.DR_HEADING == editStorage.ST_DR_HEADING);
                    newStorageWindow.cbDrugHeading.SelectedItem = tempDrugDTO;
                    newStorageWindow.ShowDialog();
                    if (newStorageWindow.DialogResult == true)
                    {
                        var currentDrugDTO = newStorageWindow.cbDrugHeading.SelectedItem as DrugsDTO;
                        using (var context = new AptekaEntities())
                        {
                            Storage storage = context.Storage.Find(editStorage.ST_ID);
                            storage.ST_DRUG = currentDrugDTO.DR_ID;
                            storage.ST_DATE = editStorage.ST_DATE;
                            storage.ST_AMOUNT = editStorage.ST_AMOUNT;
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
                }, (obj) => SelectedStorageDTO != null && ListStorageDTO.Count > 0 &&
                AuthViewModel.role != 3 && AuthViewModel.role != 4));
            }
        }
        private RelayCommand _deleteStorage;
        public RelayCommand DeleteStorage
        {
            get
            {
                return _deleteStorage ??
                (_deleteStorage = new RelayCommand(obj =>
                {
                    StorageDTO currentStorageDTO = SelectedStorageDTO;
                    Storage currentStorage = ListStorage.Single(s => s.ST_ID == currentStorageDTO.ST_ID);
                    if (currentStorage != null)
                    {
                        using (var context = new AptekaEntities())
                        {
                            Storage delStorage = context.Storage.Find(currentStorage.ST_ID);
                            MessageBoxResult result = MessageBox.Show("Удалить данные по хранению?", "Предупреждение",
                                MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                            if (result == MessageBoxResult.OK)
                            {
                                try
                                {
                                    context.Storage.Remove(delStorage);
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
                }, (obj) => SelectedStorageDTO != null && ListStorageDTO.Count > 0 &&
                AuthViewModel.role != 3 && AuthViewModel.role != 4));
            }
        }
    }
}
