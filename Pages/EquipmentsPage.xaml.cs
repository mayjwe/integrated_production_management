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
    /// Логика взаимодействия для EquipmentsPage.xaml
    /// </summary>
    public partial class EquipmentsPage : Page
    {
        public EquipmentsPage()
        {
            InitializeComponent();

            var equipments = Integrated_productionEntities2.GetContext().Equipment
                .Include("TypeEquipment")
                .Include("StatusEquipment")
                .Include("Schedule")
                .ToList();
            ListViewRequest.ItemsSource = equipments;
        }

        private void btnAddPartner_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditEquipment());
        }

        private void ListViewRequest_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListViewRequest.SelectedItem is Equipment selectedEquipment)
            {
                var context = Integrated_productionEntities2.GetContext();
                var equipment = context.Equipment
                    .Include("TypeEquipment")
                    .Include("StatusEquipment")
                    .Include("Schedule")
                    .FirstOrDefault(eq => eq.id_equipment == selectedEquipment.id_equipment);

                if (equipment != null)
                {
                    NavigationService.Navigate(new AddEditEquipment(equipment));
                }
                else
                {
                    MessageBox.Show("Оборудование не найдено", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = Integrated_productionEntities2.GetContext();
            if (Visibility == Visibility.Visible)
            {
                context.ChangeTracker.Entries().ToList().ForEach(eq => eq.Reload());
                ListViewRequest.ItemsSource = context.Equipment
                    .Include("TypeEquipment")
                    .Include("StatusEquipment")
                    .Include("Schedule")
                    .ToList();
            }
        }
    }
}