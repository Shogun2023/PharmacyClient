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
    public class SuppliersViewModel : BaseViewModel
    {
        public ObservableCollection<Suppliers> ListSuppliers { get; set; } = new ObservableCollection<Suppliers>();
        internal CollectionViewSource CvsSuppliers { get; set; }
        public SuppliersViewModel()
        {
            ListSuppliers = GetSuppliers();
            CvsSuppliers = new CollectionViewSource();
            CvsSuppliers.Source = this.ListSuppliers;
            CvsSuppliers.Filter += ApplyFilter;
        }
        public ObservableCollection<Suppliers> GetSuppliers()
        {
            using (var context = new AptekaEntities())
            {
                var query = from supplier in context.Suppliers
                            select supplier;
                if (query.Count() != 0)
                {
                    foreach (var s in query)
                    {
                        ListSuppliers.Add(s);
                    }
                }
            }
            return ListSuppliers;
        }
        private string _supplierFilter;
        public string SupplierFilter
        {
            get { return _supplierFilter; }
            set
            {
                _supplierFilter = value;
                OnFilterChanged();
            }
        }
        private void OnFilterChanged()
        {
            CvsSuppliers.View.Refresh();
        }
        public ICollectionView CollectionSuppliers
        {
            get { return CvsSuppliers.View; }
        }
        void ApplyFilter(object sender, FilterEventArgs e)
        {
            Suppliers sup = (Suppliers)e.Item;

            if (string.IsNullOrWhiteSpace(this.SupplierFilter) || this.SupplierFilter.Length == 0)
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = sup.SUP_NAME.Contains(SupplierFilter);
            }
        }
        private Suppliers _selectedSupplier;
        public Suppliers SelectedSupplier
        {
            get
            {
                return _selectedSupplier;
            }
            set
            {
                _selectedSupplier = value;
                OnPropertyChanged("SelectedSupplier");
                EditSupplier.CanExecute(true);
            }
        }
        private RelayCommand _addSupplier;
        public RelayCommand AddSupplier
        {
            get
            {
                return _addSupplier ??
                    (_addSupplier = new RelayCommand(obj =>
                    {
                        Suppliers newSupplier = new Suppliers();
                        NewSupplierWindow newSupplierWindow = new NewSupplierWindow
                        {
                            Title = "Новый поставщик",
                            DataContext = newSupplier
                        };
                        newSupplierWindow.ShowDialog();
                        if (newSupplierWindow.DialogResult == true)
                        {
                            using (var context = new AptekaEntities())
                            {
                                try
                                {
                                    context.Suppliers.Add(newSupplier);
                                    context.SaveChanges();
                                    ListSuppliers.Clear();
                                    ListSuppliers = GetSuppliers();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка добавления данных!\n" + ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }, (obj) => AuthViewModel.role != 4));
            }
        }
        private RelayCommand _editSupplier;
        public RelayCommand EditSupplier
        {
            get
            {
                return _editSupplier ??
                (_editSupplier = new RelayCommand(obj =>
                {
                    Suppliers editSupplier = SelectedSupplier;
                    NewSupplierWindow newSupplierWindow = new NewSupplierWindow
                    {
                        Title = "Редактирование поставщика",
                        DataContext = editSupplier
                    };
                    newSupplierWindow.ShowDialog();
                    if (newSupplierWindow.DialogResult == true)
                    {
                        using (var context = new AptekaEntities())
                        {
                            Suppliers supplier = context.Suppliers.Find(editSupplier.SUP_ID);
                            supplier.SUP_NAME = editSupplier.SUP_NAME.Trim();
                            supplier.SUP_ADDRESS = editSupplier.SUP_ADDRESS.Trim();
                            supplier.SUP_PHONE = editSupplier.SUP_PHONE.Trim();
                            supplier.SUP_EMAIL = editSupplier.SUP_EMAIL.Trim();
                            try
                            {
                                context.SaveChanges();
                                ListSuppliers.Clear();
                                ListSuppliers = GetSuppliers();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("\nОшибка редактирования данных!\n" + ex.Message, "Предупреждение");
                            }
                        }
                    }
                    else
                    {
                        ListSuppliers.Clear();
                        ListSuppliers = GetSuppliers();
                    }
                }, (obj) => SelectedSupplier != null && ListSuppliers.Count > 0 && AuthViewModel.role != 4));
            }
        }
        private RelayCommand _deleteSupplier;
        public RelayCommand DeleteSupplier
        {
            get
            {
                return _deleteSupplier ??
                (_deleteSupplier = new RelayCommand(obj =>
                {
                    Suppliers supplier = SelectedSupplier;
                    using (var context = new AptekaEntities())
                    {
                        Suppliers delSupplier = context.Suppliers.Find(supplier.SUP_ID);
                        if (delSupplier != null)
                        {
                            MessageBoxResult result = MessageBox.Show("Удалить данные по поставщику: " + delSupplier.SUP_NAME, "Предупреждение",
                                MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                            if (result == MessageBoxResult.OK)
                            {
                                try
                                {
                                    context.Suppliers.Remove(delSupplier);
                                    context.SaveChanges();
                                    ListSuppliers.Remove(supplier);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка удаления данных!\n" + ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }
                }, (obj) => SelectedSupplier != null && ListSuppliers.Count > 0 && AuthViewModel.role != 4));
            }
        }
    }
}
