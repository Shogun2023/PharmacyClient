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
    /// Логика взаимодействия для NewReceiptWindow.xaml
    /// </summary>
    public partial class NewReceiptWindow : Window
    {
        public NewReceiptWindow()
        {
            InitializeComponent();
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            bool successAmount = int.TryParse(tbReceiptAmount.Text, out int amount);
            bool successPrice = double.TryParse(tbReceiptPrice.Text, out double cost);
            if (cbDrugHeading.SelectedItem == null)
            {
                errors.AppendLine("Выберите препарат");
            }
            if (cbSupplierName.SelectedItem == null)
            {
                errors.AppendLine("Выберите поставщика");
            }
            if (!successAmount || amount <= 0)
            {
                errors.AppendLine("Неправильный ввод количества");
            }
            if (!successPrice || cost <= 0)
            {
                errors.AppendLine("Неправильный ввод цены");
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
