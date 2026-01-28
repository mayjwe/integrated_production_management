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
    /// Логика взаимодействия для AssignResourcesPage.xaml
    /// </summary>
    public partial class AssignResourcesPage : Page
    {
        private Integrated_productionEntities2 db = Integrated_productionEntities2.GetContext();

        public AssignResourcesPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var employees = db.Employee
                    .Include("StatusEmployee")
                    .Include("Position")
                    .Where(e => e.StatusEmployee.title == "работает")
                    .ToList();

                cbEmployee.ItemsSource = employees;

                var equipment = db.Equipment
                    .Include("StatusEquipment")
                    .Include("TypeEquipment")
                    .Include("Schedule")
                    .Where(e => e.StatusEquipment.title == "в работе")
                    .ToList();

                cbEquipment.ItemsSource = equipment;

                dpStartDate.SelectedDate = DateTime.Now;
                dpEndDate.SelectedDate = DateTime.Now.AddHours(8);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void cbEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbEmployee.SelectedItem is Employee employee)
            {
                txtQualification.Text = employee.qualification ?? "не указана";
            }
        }

        private void cbEquipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbEquipment.SelectedItem is Equipment equipment)
            {
                txtSpecs.Text = equipment.TypeEquipment?.title ?? "не указаны";
                txtMaintenance.Text = equipment.Schedule?.title ?? "не указан";
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Эта функциональность требует выбора производственного этапа");
        }
    }
}