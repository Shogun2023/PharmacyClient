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
            StringBuilder errors = ErrorCheck(tbDrugName.Text, tbDrugForm.Text, tbDrugDose.Text, tbDrugPrice.Text);
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
            }
            else
            {
                DialogResult = true;
            }
        }
        public StringBuilder ErrorCheck(string name, string form, string dose, string price)
        {
            StringBuilder err = new StringBuilder();
            bool successPrice = double.TryParse(price, out double p);
            if (string.IsNullOrEmpty(name))
            {
                err.AppendLine("Введите название препарата");
            }
            if (string.IsNullOrEmpty(form))
            {
                err.AppendLine("Введите лекарственную форму");
            }
            if (string.IsNullOrEmpty(dose))
            {
                err.AppendLine("Введите дозировку");
            }
            if (!successPrice || p <= 0)
            {
                err.AppendLine("Неправильный ввод цены реализации");
            }
            return err;
        }
    }
}
