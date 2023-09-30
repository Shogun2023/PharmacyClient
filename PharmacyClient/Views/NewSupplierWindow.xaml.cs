using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PharmacyClient.Views
{
    /// <summary>
    /// Логика взаимодействия для NewSupplierWindow.xaml
    /// </summary>
    public partial class NewSupplierWindow : Window
    {
        public NewSupplierWindow()
        {
            InitializeComponent();
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrEmpty(tbSupplierName.Text))
            {
                errors.AppendLine("Введите название поставщика");
            }
            if (string.IsNullOrEmpty(tbSupplierAddress.Text))
            {
                errors.AppendLine("Введите адрес");
            }
            if (string.IsNullOrEmpty(tbSupplierPhone.Text))
            {
                errors.AppendLine("Введите телефон");
            }
            if (string.IsNullOrEmpty(tbSupplierEmail.Text))
            {
                errors.AppendLine("Введите email");
            }
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
            }
            else
            {
                DialogResult = true;
            }
        }
    }
}
