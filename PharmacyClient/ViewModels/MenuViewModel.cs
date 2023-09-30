using PharmacyClient.Helpers;
using PharmacyClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyClient.ViewModels
{
    public class MenuViewModel
    {
        private RelayCommand _openDrugs;
        private RelayCommand _openSales;
        private RelayCommand _openStorage;
        private RelayCommand _openReceipts;
        private RelayCommand _openSuppliers;
        private RelayCommand _openDescriptions;
        private RelayCommand _openUsers;
        public RelayCommand OpenDrugs
        {
            get
            {
                return _openDrugs ??
                    (_openDrugs = new RelayCommand(obj =>
                    {
                        DrugsWindow newDrugsWindow = new DrugsWindow();
                        newDrugsWindow.Show();
                    }, (obj) => AuthViewModel.role != 4 && AuthViewModel.role != 5));
            }
        }
        public RelayCommand OpenSales
        {
            get
            {
                return _openSales ??
                    (_openSales = new RelayCommand(obj =>
                    {
                        SalesWindow newSalesWindow = new SalesWindow();
                        newSalesWindow.Show();
                    }, (obj) => AuthViewModel.role != 4 && AuthViewModel.role != 5));
            }
        }
        public RelayCommand OpenStorage
        {
            get
            {
                return _openStorage ??
                    (_openStorage = new RelayCommand(obj =>
                    {
                        StorageWindow newStorageWindow = new StorageWindow();
                        newStorageWindow.Show();
                    }, (obj) => true));
            }
        }
        public RelayCommand OpenReceipts
        {
            get
            {
                return _openReceipts ??
                    (_openReceipts = new RelayCommand(obj =>
                    {
                        ReceiptsWindow newReceiptsWindow = new ReceiptsWindow();
                        newReceiptsWindow.Show();
                    }, (obj) => AuthViewModel.role != 3 && AuthViewModel.role != 5));
            }
        }
        public RelayCommand OpenSuppliers
        {
            get
            {
                return _openSuppliers ??
                    (_openSuppliers = new RelayCommand(obj =>
                    {
                        SuppliersWindow newSuppliersWindow = new SuppliersWindow();
                        newSuppliersWindow.Show();
                    }, (obj) => AuthViewModel.role != 3 && AuthViewModel.role != 5));
            }
        }
        public RelayCommand OpenDescriptions
        {
            get
            {
                return _openDescriptions ??
                    (_openDescriptions = new RelayCommand(obj =>
                    {
                        DescriptionsWindow newDescriptionsWindow = new DescriptionsWindow();
                        newDescriptionsWindow.Show();
                    }, (obj) => AuthViewModel.role != 4 && AuthViewModel.role != 5));
            }
        }
        public RelayCommand OpenUsers
        {
            get
            {
                return _openUsers ??
                    (_openUsers = new RelayCommand(obj =>
                    {
                        UsersWindow newUsersWindow = new UsersWindow();
                        newUsersWindow.Show();
                    }, (obj) => AuthViewModel.role == 1));
            }
        }
    }
}
