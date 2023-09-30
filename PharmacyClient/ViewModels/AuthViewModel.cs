﻿using PharmacyClient.Helpers;
using PharmacyClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PharmacyClient.ViewModels
{
    public class AuthViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public static int role = 0;
        private RelayCommand _signIn;
        public RelayCommand SignIn
        {
            get
            {
                return _signIn ??
                  (_signIn = new RelayCommand(obj =>
                  {
                      string login = Login;
                      string password = Password;
                      Users currentUser = new Users();
                      if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
                      {
                          using (AptekaEntities context = new AptekaEntities())
                          {
                              currentUser = context.Users.Where(u => u.U_LOGIN == login && u.U_PASS == password).FirstOrDefault();
                          }
                          if (currentUser != null)
                          {
                              role = currentUser.U_ROLE;
                              MainWindow menuWindow = new MainWindow();
                              menuWindow.Show();
                              Application.Current.MainWindow.Close();
                          }
                          else
                          {
                              MessageBox.Show("Такого пользователя не существует!");
                          }
                      }
                      else
                      {
                          MessageBox.Show("Логин или пароль не был введен");
                      }
                  }));
            }
        }
    }
}