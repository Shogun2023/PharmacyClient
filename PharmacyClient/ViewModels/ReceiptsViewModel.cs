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
    public class ReceiptsViewModel : BaseViewModel
    {
        public ObservableCollection<Receipts> ListReceipts { get; set; } = new ObservableCollection<Receipts>();
        public ObservableCollection<ReceiptsDTO> ListReceiptsDTO { get; set; } = new ObservableCollection<ReceiptsDTO>();
        internal CollectionViewSource CvsReceipts { get; set; }
        public ReceiptsViewModel()
        {
            ListReceipts = GetReceipts();
            ListReceiptsDTO = GetReceiptsDTO();
            CvsReceipts = new CollectionViewSource();
            CvsReceipts.Source = this.ListReceiptsDTO;
            CvsReceipts.Filter += ApplyFilter;
        }
        public ObservableCollection<Receipts> GetReceipts()
        {
            using (var context = new AptekaEntities())
            {
                var query = from receipt in context.Receipts
                            select receipt;
                if (query.Count() != 0)
                {
                    foreach (var r in query)
                    {
                        ListReceipts.Add(r);
                    }
                }
            }
            return ListReceipts;
        }
        public ObservableCollection<ReceiptsDTO> GetReceiptsDTO()
        {
            DrugsViewModel vmDrugs = new DrugsViewModel();
            SuppliersViewModel vmSuppliers = new SuppliersViewModel();
            List<Drugs> drugs = new List<Drugs>();
            List<Suppliers> suppliers = new List<Suppliers>();
            foreach (Drugs d in vmDrugs.ListDrugs)
            {
                drugs.Add(d);
            }
            foreach (Suppliers s in vmSuppliers.ListSuppliers)
            {
                suppliers.Add(s);
            }
            Finder drugFinder;
            Finder supplierFinder;
            foreach (var r in ListReceipts)
            {
                drugFinder = new Finder(r.R_DRUG);
                Drugs dr = drugs.Find(new Predicate<Drugs>(drugFinder.DrugPredicate));
                supplierFinder = new Finder(r.R_SUPPLIER);
                Suppliers sup = suppliers.Find(new Predicate<Suppliers>(supplierFinder.SupplierPredicate));
                ListReceiptsDTO.Add(new ReceiptsDTO
                {
                    R_ID = r.R_ID,
                    R_DR_HEADING = $"{dr.DR_NAME}, {dr.DR_FORM}, {dr.DR_DOSE}",
                    R_SUP_NAME = sup.SUP_NAME,
                    R_AMOUNT = r.R_AMOUNT,
                    R_DATE = r.R_DATE,
                    R_PRICE = r.R_PRICE
                });
            }
            return ListReceiptsDTO;
        }
        private void Refresh()
        {
            ListReceipts.Clear();
            ListReceipts = GetReceipts();
            ListReceiptsDTO.Clear();
            ListReceiptsDTO = GetReceiptsDTO();
        }
        private string _receiptFilter;
        public string ReceiptFilter
        {
            get { return _receiptFilter; }
            set
            {
                _receiptFilter = value;
                OnFilterChanged();
            }
        }
        private void OnFilterChanged()
        {
            CvsReceipts.View.Refresh();
        }
        public ICollectionView CollectionReceiptsDTO
        {
            get { return CvsReceipts.View; }
        }
        void ApplyFilter(object sender, FilterEventArgs e)
        {
            ReceiptsDTO rec = (ReceiptsDTO)e.Item;

            if (string.IsNullOrWhiteSpace(this.ReceiptFilter) || this.ReceiptFilter.Length == 0)
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = rec.R_DR_HEADING.Contains(ReceiptFilter) || rec.R_SUP_NAME.Contains(ReceiptFilter) ||
                    rec.R_DATE.ToString().Contains(ReceiptFilter);
            }
        }
        private ReceiptsDTO selectedReceiptDTO;
        public ReceiptsDTO SelectedReceiptDTO
        {
            get
            {
                return selectedReceiptDTO;
            }
            set
            {
                selectedReceiptDTO = value;
                OnPropertyChanged("SelectedReceiptDTO");
                EditReceipt.CanExecute(true);
            }
        }
        private RelayCommand _addReceipt;
        public RelayCommand AddReceipt
        {
            get
            {
                return _addReceipt ??
                    (_addReceipt = new RelayCommand(obj =>
                    {
                        Receipts newReceipt = new Receipts();
                        ReceiptsDTO newReceiptDTO = new ReceiptsDTO { R_DATE = DateTime.Now };
                        DrugsViewModel vmDrugs = new DrugsViewModel();
                        SuppliersViewModel vmSuppliers = new SuppliersViewModel();
                        NewReceiptWindow newReceiptWindow = new NewReceiptWindow
                        {
                            Title = "Новый приход",
                            DataContext = newReceiptDTO,
                        };
                        newReceiptWindow.cbDrugHeading.ItemsSource = vmDrugs.ListDrugsDTO;
                        newReceiptWindow.cbSupplierName.ItemsSource = vmSuppliers.ListSuppliers;
                        newReceiptWindow.ShowDialog();
                        if (newReceiptWindow.DialogResult == true)
                        {
                            var currentDrugDTO = newReceiptWindow.cbDrugHeading.SelectedItem as DrugsDTO;
                            var currentSupplier = newReceiptWindow.cbSupplierName.SelectedItem as Suppliers;
                            newReceipt.R_DRUG = currentDrugDTO.DR_ID;
                            newReceipt.R_SUPPLIER = currentSupplier.SUP_ID;
                            newReceipt.R_AMOUNT = newReceiptDTO.R_AMOUNT;
                            newReceipt.R_DATE = newReceiptDTO.R_DATE;
                            newReceipt.R_PRICE = newReceiptDTO.R_PRICE;
                            using (var context = new AptekaEntities())
                            {
                                try
                                {
                                    context.Receipts.Add(newReceipt);
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
        private RelayCommand _editReceipt;
        public RelayCommand EditReceipt
        {
            get
            {
                return _editReceipt ??
                (_editReceipt = new RelayCommand(obj =>
                {
                    ReceiptsDTO editReceipt = SelectedReceiptDTO;
                    DrugsViewModel vmDrugs = new DrugsViewModel();
                    SuppliersViewModel vmSuppliers = new SuppliersViewModel();
                    NewReceiptWindow newReceiptWindow = new NewReceiptWindow
                    {
                        Title = "Редактирование прихода",
                        DataContext = editReceipt
                    };
                    List<DrugsDTO> ListDrDTO = new List<DrugsDTO>();
                    List<Suppliers> ListSup = new List<Suppliers>();
                    foreach (var d in vmDrugs.ListDrugsDTO)
                    {
                        ListDrDTO.Add(d);
                    }
                    foreach (var s in vmSuppliers.ListSuppliers)
                    {
                        ListSup.Add(s);
                    }
                    newReceiptWindow.cbDrugHeading.ItemsSource = vmDrugs.ListDrugsDTO;
                    newReceiptWindow.cbSupplierName.ItemsSource = vmSuppliers.ListSuppliers;
                    DrugsDTO tempDrugDTO = ListDrDTO.Single(d => d.DR_HEADING == editReceipt.R_DR_HEADING);
                    Suppliers tempSupplier = ListSup.Single(s => s.SUP_NAME == editReceipt.R_SUP_NAME);
                    newReceiptWindow.cbDrugHeading.SelectedItem = tempDrugDTO;
                    newReceiptWindow.cbSupplierName.SelectedItem = tempSupplier;
                    newReceiptWindow.ShowDialog();
                    if (newReceiptWindow.DialogResult == true)
                    {
                        var currentDrugDTO = newReceiptWindow.cbDrugHeading.SelectedItem as DrugsDTO;
                        var currentSupplier = newReceiptWindow.cbSupplierName.SelectedItem as Suppliers;
                        using (var context = new AptekaEntities())
                        {
                            Receipts receipt = context.Receipts.Find(editReceipt.R_ID);
                            receipt.R_DRUG = currentDrugDTO.DR_ID;
                            receipt.R_SUPPLIER = currentSupplier.SUP_ID;
                            receipt.R_AMOUNT = editReceipt.R_AMOUNT;
                            receipt.R_DATE = editReceipt.R_DATE;
                            receipt.R_PRICE = editReceipt.R_PRICE;
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
                }, (obj) => SelectedReceiptDTO != null && ListReceiptsDTO.Count > 0));
            }
        }
        private RelayCommand _deleteReceipt;
        public RelayCommand DeleteReceipt
        {
            get
            {
                return _deleteReceipt ??
                (_deleteReceipt = new RelayCommand(obj =>
                {
                    ReceiptsDTO currentReceiptDTO = SelectedReceiptDTO;
                    Receipts currentReceipt = ListReceipts.Single(r => r.R_ID == currentReceiptDTO.R_ID);
                    if (currentReceipt != null)
                    {
                        using (var context = new AptekaEntities())
                        {
                            Receipts delReceipt = context.Receipts.Find(currentReceipt.R_ID);
                            MessageBoxResult result = MessageBox.Show("Удалить данные по приходу?", "Предупреждение",
                                MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                            if (result == MessageBoxResult.OK)
                            {
                                try
                                {
                                    context.Receipts.Remove(delReceipt);
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
                }, (obj) => SelectedReceiptDTO != null && ListReceiptsDTO.Count > 0));
            }
        }
    }
}
