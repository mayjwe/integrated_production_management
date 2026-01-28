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
    /// Логика взаимодействия для MaterialsPage.xaml
    /// </summary>
    public partial class MaterialsPage : Page
    {
        public MaterialsPage()
        {
            InitializeComponent();

            var materials = Integrated_productionEntities2.GetContext().Material
                .Include("TypeMaterial")
                .Include("Unit")
                .ToList();
            ListViewRequest.ItemsSource = materials;
        }

        private void btnAddPartner_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditMaterial());
        }

        private void ListViewRequest_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListViewRequest.SelectedItem is Material selectedMaterial)
            {
                var context = Integrated_productionEntities2.GetContext();
                var material = context.Material
                    .Include("TypeMaterial")
                    .Include("Unit")
                    .FirstOrDefault(m => m.id_material == selectedMaterial.id_material);

                if (material != null)
                {
                    NavigationService.Navigate(new AddEditMaterial(material));
                }
                else
                {
                    MessageBox.Show("Материал не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = Integrated_productionEntities2.GetContext();
            if (Visibility == Visibility.Visible)
            {
                context.ChangeTracker.Entries().ToList().ForEach(m => m.Reload());
                ListViewRequest.ItemsSource = context.Material
                    .Include("TypeMaterial")
                    .Include("Unit")
                    .ToList();
            }
        }
    }
}