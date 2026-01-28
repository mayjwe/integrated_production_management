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
    /// Логика взаимодействия для AddEditEquipment.xaml
    /// </summary>
    public partial class AddEditEquipment : Page
    {
        private Equipment _equipment = new Equipment();
        private bool _isEditing = false;

        public AddEditEquipment(Equipment equipment = null)
        {
            InitializeComponent();

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.UpdateNavigationButtons(false);
            }

            LoadTypeEquipments();
            LoadStatuses();
            LoadSchedules();

            if (equipment != null && equipment.id_equipment > 0)
            {
                _equipment = equipment;
                _isEditing = true;
                LoadEquipmentData();
            }
        }

        private void LoadEquipmentData()
        {
            txtInventoryNumber.Text = _equipment.inventory_number.ToString();
            txtModel.Text = _equipment.model ?? "";
            dpDateInput.SelectedDate = _equipment.date_input;

            cbTypeEquipment.SelectedValue = _equipment.id_type_equipment;
            cbStatus.SelectedValue = _equipment.id_status_equipment;
            cbSchedule.SelectedValue = _equipment.id_schedule;
        }

        private void LoadTypeEquipments()
        {
            var context = Integrated_productionEntities2.GetContext();
            cbTypeEquipment.ItemsSource = context.TypeEquipment.ToList();
        }

        private void LoadStatuses()
        {
            var context = Integrated_productionEntities2.GetContext();
            cbStatus.ItemsSource = context.StatusEquipment.ToList();
        }

        private void LoadSchedules()
        {
            var context = Integrated_productionEntities2.GetContext();
            cbSchedule.ItemsSource = context.Schedule.ToList();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!long.TryParse(txtInventoryNumber.Text, out long inventoryNumber) || inventoryNumber <= 0)
            {
                MessageBox.Show("Введите корректный инвентарный номер!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtModel.Text))
            {
                MessageBox.Show("Введите модель оборудования!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpDateInput.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату ввода в эксплуатацию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbTypeEquipment.SelectedValue == null)
            {
                MessageBox.Show("Выберите тип оборудования!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbStatus.SelectedValue == null)
            {
                MessageBox.Show("Выберите статус!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbSchedule.SelectedValue == null)
            {
                MessageBox.Show("Выберите график профилактики!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var context = Integrated_productionEntities2.GetContext();

                if (!_isEditing)
                {
                    var existingEquipment = context.Equipment.FirstOrDefault(eq => eq.inventory_number == inventoryNumber);
                    if (existingEquipment != null)
                    {
                        MessageBox.Show("Оборудование с таким инвентарным номером уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _equipment.inventory_number = inventoryNumber;
                    _equipment.model = txtModel.Text;
                    _equipment.date_input = dpDateInput.SelectedDate.Value;
                    _equipment.id_type_equipment = Convert.ToInt64(cbTypeEquipment.SelectedValue);
                    _equipment.id_status_equipment = Convert.ToInt64(cbStatus.SelectedValue);
                    _equipment.id_schedule = Convert.ToInt64(cbSchedule.SelectedValue);

                    context.Equipment.Add(_equipment);
                }
                else
                {
                    var existingEquipment = context.Equipment.FirstOrDefault(eq => eq.inventory_number == inventoryNumber && eq.id_equipment != _equipment.id_equipment);
                    if (existingEquipment != null)
                    {
                        MessageBox.Show("Оборудование с таким инвентарным номером уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var equipmentToUpdate = context.Equipment.Find(_equipment.id_equipment);
                    if (equipmentToUpdate != null)
                    {
                        equipmentToUpdate.inventory_number = inventoryNumber;
                        equipmentToUpdate.model = txtModel.Text;
                        equipmentToUpdate.date_input = dpDateInput.SelectedDate.Value;
                        equipmentToUpdate.id_type_equipment = Convert.ToInt64(cbTypeEquipment.SelectedValue);
                        equipmentToUpdate.id_status_equipment = Convert.ToInt64(cbStatus.SelectedValue);
                        equipmentToUpdate.id_schedule = Convert.ToInt64(cbSchedule.SelectedValue);
                    }
                }

                context.SaveChanges();

                MessageBox.Show("Оборудование успешно сохранено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}