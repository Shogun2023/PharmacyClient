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
    /// Логика взаимодействия для NewDrugWindow.xaml
    /// </summary>
    public partial class NewDrugWindow : Window
    {
        public NewDrugWindow()
        {
            InitializeComponent();
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            bool successPrice = double.TryParse(tbDrugPrice.Text, out double price);
            if (string.IsNullOrEmpty(tbDrugName.Text))
            {
                errors.AppendLine("Введите название препарата");
            }
            if (string.IsNullOrEmpty(tbDrugForm.Text))
            {
                errors.AppendLine("Введите лекарственную форму");
            }
            if (string.IsNullOrEmpty(tbDrugDose.Text))
            {
                errors.AppendLine("Введите дозировку");
            }
            if (!successPrice || price <= 0)
            {
                errors.AppendLine("Неправильный ввод цены реализации");
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
