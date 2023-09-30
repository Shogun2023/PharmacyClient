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
    /// Логика взаимодействия для NewSaleWindow.xaml
    /// </summary>
    public partial class NewSaleWindow : Window
    {
        public NewSaleWindow()
        {
            InitializeComponent();
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            bool successAmount = int.TryParse(tbSaleAmount.Text, out int amount);
            bool successCost = double.TryParse(tbSaleCost.Text, out double cost);
            if (cbDrugHeading.SelectedItem == null)
            {
                errors.AppendLine("Выберите препарат");
            }
            if (!successAmount || amount <= 0)
            {
                errors.AppendLine("Неправильный ввод количества");
            }
            if (!successCost || cost <= 0)
            {
                errors.AppendLine("Неправильный ввод стоимости");
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
