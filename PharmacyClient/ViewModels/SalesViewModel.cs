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
    public class SalesViewModel : BaseViewModel
    {
        public ObservableCollection<Sales> ListSales { get; set; } = new ObservableCollection<Sales>();
        public ObservableCollection<SalesDTO> ListSalesDTO { get; set; } = new ObservableCollection<SalesDTO>();
        internal CollectionViewSource CvsSales { get; set; }
        public SalesViewModel()
        {
            ListSales = GetSales();
            ListSalesDTO = GetSalesDTO();
            CvsSales = new CollectionViewSource();
            CvsSales.Source = this.ListSalesDTO;
            CvsSales.Filter += ApplyFilter;
        }
        public ObservableCollection<Sales> GetSales()
        {
            using (var context = new AptekaEntities())
            {
                var query = from sale in context.Sales
                            select sale;
                if (query.Count() != 0)
                {
                    foreach (var s in query)
                    {
                        ListSales.Add(s);
                    }
                }
            }
            return ListSales;
        }
        public ObservableCollection<SalesDTO> GetSalesDTO()
        {
            DrugsViewModel vmDrugs = new DrugsViewModel();
            AuthViewModel authViewModel = new AuthViewModel();
            List<Drugs> drugs = new List<Drugs>();
            foreach (Drugs d in vmDrugs.ListDrugs)
            {
                drugs.Add(d);
            }
            Finder finder;
            foreach (var s in ListSales)
            {
                finder = new Finder(s.SAL_DRUG);
                Drugs dr = drugs.Find(new Predicate<Drugs>(finder.DrugPredicate));
                ListSalesDTO.Add(new SalesDTO
                {
                    SAL_ID = s.SAL_ID,
                    SAL_DR_HEADING = $"{dr.DR_NAME}, {dr.DR_FORM}, {dr.DR_DOSE}",
                    SAL_AMOUNT = s.SAL_AMOUNT,
                    SAL_DATE = s.SAL_DATE,
                    SAL_COST = s.SAL_COST,
                });
            }
            return ListSalesDTO;
        }
        private void Refresh()
        {
            ListSales.Clear();
            ListSales = GetSales();
            ListSalesDTO.Clear();
            ListSalesDTO = GetSalesDTO();
        }
        private string _saleFilter;
        public string SaleFilter
        {
            get { return _saleFilter; }
            set
            {
                _saleFilter = value;
                OnFilterChanged();
            }
        }
        private void OnFilterChanged()
        {
            CvsSales.View.Refresh();
        }
        public ICollectionView CollectionSalesDTO
        {
            get { return CvsSales.View; }
        }
        void ApplyFilter(object sender, FilterEventArgs e)
        {
            SalesDTO sal = (SalesDTO)e.Item;

            if (string.IsNullOrWhiteSpace(this.SaleFilter) || this.SaleFilter.Length == 0)
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = sal.SAL_DR_HEADING.Contains(SaleFilter) || sal.SAL_DATE.ToString().Contains(SaleFilter);
            }
        }
        private SalesDTO _selectedSaleDTO;
        public SalesDTO SelectedSaleDTO
        {
            get
            {
                return _selectedSaleDTO;
            }
            set
            {
                _selectedSaleDTO = value;
                OnPropertyChanged("SelectedSaleDTO");
                EditSale.CanExecute(true);
            }
        }
        private RelayCommand _addSale;
        public RelayCommand AddSale
        {
            get
            {
                return _addSale ??
                    (_addSale = new RelayCommand(obj =>
                    {
                        Sales newSale = new Sales();
                        SalesDTO newSaleDTO = new SalesDTO { SAL_DATE = DateTime.Now };
                        DrugsViewModel vmDrugs = new DrugsViewModel();
                        NewSaleWindow newSaleWindow = new NewSaleWindow
                        {
                            Title = "Новая продажа",
                            DataContext = newSaleDTO,
                        };
                        newSaleWindow.cbDrugHeading.ItemsSource = vmDrugs.ListDrugsDTO;
                        newSaleWindow.ShowDialog();
                        if (newSaleWindow.DialogResult == true)
                        {
                            var currentDrugDTO = newSaleWindow.cbDrugHeading.SelectedItem as DrugsDTO;
                            newSale.SAL_DRUG = currentDrugDTO.DR_ID;
                            newSale.SAL_AMOUNT = newSaleDTO.SAL_AMOUNT;
                            newSale.SAL_DATE = newSaleDTO.SAL_DATE;
                            newSale.SAL_COST = newSaleDTO.SAL_COST;
                            using (var context = new AptekaEntities())
                            {
                                try
                                {
                                    context.Sales.Add(newSale);
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
        private RelayCommand _editSale;
        public RelayCommand EditSale
        {
            get
            {
                return _editSale ??
                (_editSale = new RelayCommand(obj =>
                {
                    SalesDTO editSale = SelectedSaleDTO;
                    DrugsViewModel vmDrugs = new DrugsViewModel();
                    NewSaleWindow newSaleWindow = new NewSaleWindow
                    {
                        Title = "Редактирование продажи",
                        DataContext = editSale
                    };
                    List<DrugsDTO> ListDrDTO = new List<DrugsDTO>();
                    foreach (var d in vmDrugs.ListDrugsDTO)
                    {
                        ListDrDTO.Add(d);
                    }
                    newSaleWindow.cbDrugHeading.ItemsSource = vmDrugs.ListDrugsDTO;
                    DrugsDTO tempDrugDTO = ListDrDTO.Single(d => d.DR_HEADING == editSale.SAL_DR_HEADING);
                    newSaleWindow.cbDrugHeading.SelectedItem = tempDrugDTO;
                    newSaleWindow.ShowDialog();
                    if (newSaleWindow.DialogResult == true)
                    {
                        var currentDrugDTO = newSaleWindow.cbDrugHeading.SelectedItem as DrugsDTO;
                        using (var context = new AptekaEntities())
                        {
                            Sales sale = context.Sales.Find(editSale.SAL_ID);
                            sale.SAL_DRUG = currentDrugDTO.DR_ID;
                            sale.SAL_AMOUNT = editSale.SAL_AMOUNT;
                            sale.SAL_DATE = editSale.SAL_DATE;
                            sale.SAL_COST = editSale.SAL_COST;
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
                }, (obj) => SelectedSaleDTO != null && ListSalesDTO.Count > 0));
            }
        }
        private RelayCommand _deleteSale;
        public RelayCommand DeleteSale
        {
            get
            {
                return _deleteSale ??
                (_deleteSale = new RelayCommand(obj =>
                {
                    SalesDTO currentSaleDTO = SelectedSaleDTO;
                    Sales currentSale = ListSales.Single(s => s.SAL_ID == currentSaleDTO.SAL_ID);
                    if (currentSale != null)
                    {
                        using (var context = new AptekaEntities())
                        {
                            Sales delSale = context.Sales.Find(currentSale.SAL_ID);
                            MessageBoxResult result = MessageBox.Show("Удалить данные по продаже?", "Предупреждение",
                                MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                            if (result == MessageBoxResult.OK)
                            {
                                try
                                {
                                    context.Sales.Remove(delSale);
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
                }, (obj) => SelectedSaleDTO != null && ListSalesDTO.Count > 0));
            }
        }
    }
}
