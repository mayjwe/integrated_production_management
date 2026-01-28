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
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Page
    {
        public Registration()
        {
            InitializeComponent();
            LoadEmployees();
            LoadRoles();
        }

        private void LoadEmployees()
        {
            var context = Integrated_productionEntities2.GetContext();
            cbEmployee.ItemsSource = context.Employee.ToList();
        }

        private void LoadRoles()
        {
            var context = Integrated_productionEntities2.GetContext();
            cbRole.ItemsSource = context.Role.ToList();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = tbUsername.Text.Trim();
            string password = tbPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbEmployee.SelectedValue == null || cbRole.SelectedValue == null)
            {
                MessageBox.Show("Выберите сотрудника и роль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var context = Integrated_productionEntities2.GetContext();

                var existingUser = context.User.FirstOrDefault(u => u.username == username);
                if (existingUser != null)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string hashedPassword = Hash.HashPassword(password);

                var newUser = new User
                {
                    username = username,
                    password_hash = hashedPassword,
                    id_employee = Convert.ToInt64(cbEmployee.SelectedValue)
                };

                context.User.Add(newUser);
                context.SaveChanges();

                var userRole = new UserRole
                {
                    id_user = newUser.id_user,
                    id_role = Convert.ToInt64(cbRole.SelectedValue)
                };

                context.UserRole.Add(userRole);
                context.SaveChanges();

                MessageBox.Show("Регистрация успешна!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}