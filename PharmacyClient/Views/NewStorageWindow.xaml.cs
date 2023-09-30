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
    /// Логика взаимодействия для NewStorageWindow.xaml
    /// </summary>
    public partial class NewStorageWindow : Window
    {
        public NewStorageWindow()
        {
            InitializeComponent();
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            bool successAmount = int.TryParse(tbStorageAmount.Text, out int amount);
            if (cbDrugHeading.SelectedItem == null)
            {
                errors.AppendLine("Выберите препарат");
            }
            if (!successAmount || amount <= 0)
            {
                errors.AppendLine("Неправильный ввод количества");
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
