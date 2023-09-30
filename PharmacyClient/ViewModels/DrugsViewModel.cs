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
    public class DrugsViewModel : BaseViewModel
    {
        public ObservableCollection<Drugs> ListDrugs { get; set; } = new ObservableCollection<Drugs>();
        public ObservableCollection<DrugsDTO> ListDrugsDTO { get; set; } = new ObservableCollection<DrugsDTO>();
        internal CollectionViewSource CvsDrugs { get; set; }
        public DrugsViewModel()
        {
            ListDrugs = GetDrugs();
            ListDrugsDTO = GetDrugsDTO();
            CvsDrugs = new CollectionViewSource();
            CvsDrugs.Source = this.ListDrugs;
            CvsDrugs.Filter += ApplyFilter;
        }
        public ObservableCollection<Drugs> GetDrugs()
        {
            using (var context = new AptekaEntities())
            {
                var query = from drug in context.Drugs
                            select drug;
                if (query.Count() != 0)
                {
                    foreach (var d in query)
                    {
                        ListDrugs.Add(d);
                    }
                }
            }
            return ListDrugs;
        }
        public ObservableCollection<DrugsDTO> GetDrugsDTO()
        {
            using (var context = new AptekaEntities())
            {
                var query = from drug in context.Drugs
                            orderby drug.DR_NAME
                            select drug;
                if (query.Count() != 0)
                {
                    foreach (var d in query)
                    {
                        ListDrugsDTO.Add(new DrugsDTO
                        {
                            DR_ID = d.DR_ID,
                            DR_HEADING = $"{d.DR_NAME}, {d.DR_FORM}, {d.DR_DOSE}"
                        });
                    }
                }
            }
            return ListDrugsDTO;
        }
        private string _drugFilter;
        public string DrugFilter
        {
            get { return _drugFilter; }
            set
            {
                _drugFilter = value;
                OnFilterChanged();
            }
        }
        private void OnFilterChanged()
        {
            CvsDrugs.View.Refresh();
        }
        public ICollectionView CollectionDrugs
        {
            get { return CvsDrugs.View; }
        }
        void ApplyFilter(object sender, FilterEventArgs e)
        {
            Drugs dr = (Drugs)e.Item;

            if (string.IsNullOrWhiteSpace(this.DrugFilter) || this.DrugFilter.Length == 0)
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = dr.DR_NAME.Contains(DrugFilter);
            }
        }
        private Drugs selectedDrug;
        public Drugs SelectedDrug
        {
            get
            {
                return selectedDrug;
            }
            set
            {
                selectedDrug = value;
                OnPropertyChanged("SelectedDrug");
                EditDrug.CanExecute(true);
            }
        }
        private RelayCommand _addDrug;
        public RelayCommand AddDrug
        {
            get
            {
                return _addDrug ??
                    (_addDrug = new RelayCommand(obj =>
                    {
                        Drugs newDrug = new Drugs();
                        NewDrugWindow newDrugWindow = new NewDrugWindow
                        {
                            Title = "Новый препарат",
                            DataContext = newDrug
                        };
                        newDrugWindow.ShowDialog();
                        if (newDrugWindow.DialogResult == true)
                        {
                            using (var context = new AptekaEntities())
                            {
                                try
                                {
                                    context.Drugs.Add(newDrug);
                                    context.SaveChanges();
                                    ListDrugs.Clear();
                                    ListDrugs = GetDrugs();
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
        private RelayCommand _editDrug;
        public RelayCommand EditDrug
        {
            get
            {
                return _editDrug ??
                (_editDrug = new RelayCommand(obj =>
                {
                    Drugs editDrug = SelectedDrug;
                    NewDrugWindow newDrugWindow = new NewDrugWindow
                    {
                        Title = "Редактирование препарата",
                        DataContext = editDrug
                    };
                    newDrugWindow.ShowDialog();
                    if (newDrugWindow.DialogResult == true)
                    {
                        using (var context = new AptekaEntities())
                        {
                            Drugs drug = context.Drugs.Find(editDrug.DR_ID);
                            drug.DR_NAME = editDrug.DR_NAME.Trim();
                            drug.DR_DESCR = editDrug.DR_DESCR;
                            drug.DR_FORM = editDrug.DR_FORM.Trim();
                            drug.DR_DOSE = editDrug.DR_DOSE.Trim();
                            drug.DR_PRICE = editDrug.DR_PRICE;
                            try
                            {
                                context.SaveChanges();
                                ListDrugs.Clear();
                                ListDrugs = GetDrugs();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("\nОшибка редактирования данных!\n" + ex.Message, "Предупреждение");
                            }
                        }
                    }
                    else
                    {
                        ListDrugs.Clear();
                        ListDrugs = GetDrugs();
                    }
                }, (obj) => SelectedDrug != null && ListDrugs.Count > 0 && AuthViewModel.role != 3));
            }
        }
        private RelayCommand _deleteDrug;
        public RelayCommand DeleteDrug
        {
            get
            {
                return _deleteDrug ??
                (_deleteDrug = new RelayCommand(obj =>
                {
                    Drugs drug = SelectedDrug;
                    using (var context = new AptekaEntities())
                    {
                        Drugs delDrug = context.Drugs.Find(drug.DR_ID);
                        if (delDrug != null)
                        {
                            MessageBoxResult result = MessageBox.Show("Удалить данные по препарату: " + delDrug.DR_NAME, "Предупреждение",
                                MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                            if (result == MessageBoxResult.OK)
                            {
                                try
                                {
                                    context.Drugs.Remove(delDrug);
                                    context.SaveChanges();
                                    ListDrugs.Remove(drug);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка удаления данных!\n" + ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }
                }, (obj) => SelectedDrug != null && ListDrugs.Count > 0 && AuthViewModel.role != 3));
            }
        }
    }
}
