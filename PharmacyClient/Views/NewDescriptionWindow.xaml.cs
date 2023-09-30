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
    /// Логика взаимодействия для NewDescriptionWindow.xaml
    /// </summary>
    public partial class NewDescriptionWindow : Window
    {
        public NewDescriptionWindow()
        {
            InitializeComponent();
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrEmpty(tbDescrManufacturer.Text))
            {
                errors.AppendLine("Введите производителя");
            }
            if (string.IsNullOrEmpty(tbDescrGroup.Text))
            {
                errors.AppendLine("Введите фармакологическую группу");
            }
            if (string.IsNullOrEmpty(tbDescrUse.Text))
            {
                errors.AppendLine("Введите применение");
            }
            if (string.IsNullOrEmpty(tbDescrStorage.Text))
            {
                errors.AppendLine("Введите условия хранения");
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
