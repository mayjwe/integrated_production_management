using integrated_production_management.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace integrated_production_management.Pages
{
    /// <summary>
    /// Логика взаимодействия для Autho.xaml
    /// </summary>
    public partial class Autho : Page
    {
        public Autho()
        {
            InitializeComponent();
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            string username = tbLogin.Text.Trim();
            string password = tbPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            string hashedPassword = Hash.HashPassword(password);

            try
            {
                var context = Integrated_productionEntities2.GetContext();
                var user = context.User
                    .Include("Employee")
                    .Include("UserRole")
                    .Include("UserRole.Role")
                    .FirstOrDefault(u => u.username == username && u.password_hash == hashedPassword);

                if (user != null)
                {
                    string roleName = "Пользователь";
                    if (user.UserRole.Any())
                    {
                        roleName = user.UserRole.First().Role.title;
                    }

                    string userName = user.Employee?.first_name;
                    string userInfo = $"{user.Employee?.last_name} {user.Employee?.first_name} ({roleName})";

                    MessageBox.Show($"Добро пожаловать, {userName}!");

                    var mainWindow = Application.Current.MainWindow as MainWindow;
                    if (mainWindow != null)
                    {
                        mainWindow.SetAuthenticated(userInfo);
                    }
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Registration());
        }
    }
}