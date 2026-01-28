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
    /// Логика взаимодействия для EmployeesPage.xaml
    /// </summary>
    public partial class EmployeesPage : Page
    {
        public EmployeesPage()
        {
            InitializeComponent();

            var employees = Integrated_productionEntities2.GetContext().Employee
                .Include("Position")
                .Include("Department")
                .Include("Rate")
                .Include("StatusEmployee")
                .ToList();
            ListViewRequest.ItemsSource = employees;
        }

        private void btnAddPartner_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditEmployee());
        }

        private void ListViewRequest_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListViewRequest.SelectedItem is Employee selectedEmployee)
            {
                var context = Integrated_productionEntities2.GetContext();
                var employee = context.Employee
                    .Include("Position")
                    .Include("Department")
                    .Include("Rate")
                    .Include("StatusEmployee")
                    .FirstOrDefault(emp => emp.id_employee == selectedEmployee.id_employee);

                if (employee != null)
                {
                    NavigationService.Navigate(new AddEditEmployee(employee));
                }
                else
                {
                    MessageBox.Show("Сотрудник не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = Integrated_productionEntities2.GetContext();
            if (Visibility == Visibility.Visible)
            {
                context.ChangeTracker.Entries().ToList().ForEach(emp => emp.Reload());
                ListViewRequest.ItemsSource = context.Employee
                    .Include("Position")
                    .Include("Department")
                    .Include("Rate")
                    .Include("StatusEmployee")
                    .ToList();
            }
        }
    }
}