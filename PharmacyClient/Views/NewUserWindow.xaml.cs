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
    /// Логика взаимодействия для NewUserWindow.xaml
    /// </summary>
    public partial class NewUserWindow : Window
    {
        public NewUserWindow()
        {
            InitializeComponent();
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrEmpty(tbUserName.Text))
            {
                errors.AppendLine("Введите имя пользователя");
            }
            if (string.IsNullOrEmpty(tbUserSName.Text))
            {
                errors.AppendLine("Введите фамилию пользователя");
            }
            if (string.IsNullOrEmpty(tbUserLogin.Text))
            {
                errors.AppendLine("Введите логин");
            }
            if (string.IsNullOrEmpty(tbUserPass.Text))
            {
                errors.AppendLine("Введите пароль");
            }
            if (cbUserRole.SelectedItem == null)
            {
                errors.AppendLine("Выберите роль пользователя");
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
