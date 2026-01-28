using integrated_production_management.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
    /// Логика взаимодействия для AddEditEmployee.xaml
    /// </summary>
    public partial class AddEditEmployee : Page
    {
        private Employee _employee = new Employee();
        private bool _isEditing = false;

        public AddEditEmployee(Employee employee = null)
        {
            InitializeComponent();

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.UpdateNavigationButtons(false);
            }

            LoadPositions();
            LoadDepartments();
            LoadRates();
            LoadStatuses();

            if (employee != null && employee.id_employee > 0)
            {
                _employee = employee;
                _isEditing = true;
                LoadEmployeeData();
            }
        }

        private void LoadEmployeeData()
        {
            txtServiceNumber.Text = _employee.service_number.ToString();
            txtLastName.Text = _employee.last_name ?? "";
            txtFirstName.Text = _employee.first_name ?? "";
            txtMiddleName.Text = _employee.middle_name ?? "";
            txtQualification.Text = _employee.qualification ?? "";

            cbPosition.SelectedValue = _employee.id_position;
            cbDepartment.SelectedValue = _employee.id_department;
            cbRate.SelectedValue = _employee.id_rate;
            cbStatus.SelectedValue = _employee.id_status_employee;
        }

        private void LoadPositions()
        {
            var context = Integrated_productionEntities2.GetContext();
            cbPosition.ItemsSource = context.Position.ToList();
        }

        private void LoadDepartments()
        {
            var context = Integrated_productionEntities2.GetContext();
            cbDepartment.ItemsSource = context.Department.ToList();
        }

        private void LoadRates()
        {
            var context = Integrated_productionEntities2.GetContext();
            cbRate.ItemsSource = context.Rate.ToList();
        }

        private void LoadStatuses()
        {
            var context = Integrated_productionEntities2.GetContext();
            cbStatus.ItemsSource = context.StatusEmployee.ToList();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!long.TryParse(txtServiceNumber.Text, out long serviceNumber) || serviceNumber <= 0)
            {
                MessageBox.Show("Введите корректный табельный номер!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Введите фамилию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Введите имя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbPosition.SelectedValue == null)
            {
                MessageBox.Show("Выберите должность!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbDepartment.SelectedValue == null)
            {
                MessageBox.Show("Выберите отдел!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbRate.SelectedValue == null)
            {
                MessageBox.Show("Выберите ставку!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbStatus.SelectedValue == null)
            {
                MessageBox.Show("Выберите статус!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var context = Integrated_productionEntities2.GetContext();

                if (!_isEditing)
                {
                    var existingEmployee = context.Employee.FirstOrDefault(emp => emp.service_number == serviceNumber);
                    if (existingEmployee != null)
                    {
                        MessageBox.Show("Сотрудник с таким табельным номером уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _employee.service_number = serviceNumber;
                    _employee.last_name = txtLastName.Text;
                    _employee.first_name = txtFirstName.Text;
                    _employee.middle_name = txtMiddleName.Text;
                    _employee.qualification = txtQualification.Text;
                    _employee.id_position = Convert.ToInt64(cbPosition.SelectedValue);
                    _employee.id_department = Convert.ToInt64(cbDepartment.SelectedValue);
                    _employee.id_rate = Convert.ToInt64(cbRate.SelectedValue);
                    _employee.id_status_employee = Convert.ToInt64(cbStatus.SelectedValue);

                    context.Employee.Add(_employee);
                }
                else
                {
                    var existingEmployee = context.Employee.FirstOrDefault(emp => emp.service_number == serviceNumber && emp.id_employee != _employee.id_employee);
                    if (existingEmployee != null)
                    {
                        MessageBox.Show("Сотрудник с таким табельным номером уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var employeeToUpdate = context.Employee.Find(_employee.id_employee);
                    if (employeeToUpdate != null)
                    {
                        employeeToUpdate.service_number = serviceNumber;
                        employeeToUpdate.last_name = txtLastName.Text;
                        employeeToUpdate.first_name = txtFirstName.Text;
                        employeeToUpdate.middle_name = txtMiddleName.Text;
                        employeeToUpdate.qualification = txtQualification.Text;
                        employeeToUpdate.id_position = Convert.ToInt64(cbPosition.SelectedValue);
                        employeeToUpdate.id_department = Convert.ToInt64(cbDepartment.SelectedValue);
                        employeeToUpdate.id_rate = Convert.ToInt64(cbRate.SelectedValue);
                        employeeToUpdate.id_status_employee = Convert.ToInt64(cbStatus.SelectedValue);
                    }
                }

                context.SaveChanges();

                MessageBox.Show("Сотрудник успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}