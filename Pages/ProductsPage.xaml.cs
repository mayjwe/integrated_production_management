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
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();

            var products = Integrated_productionEntities2.GetContext().Product
                .Include("BillOfMaterials")
                .Include("Category")
                .Include("Unit")
                .Include("StatusProduct")
                .ToList();
            ListViewRequest.ItemsSource = products;
        }

        private void btnAddPartner_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditProduct());
        }

        private void ListViewRequest_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListViewRequest.SelectedItem is Product selectedProduct)
            {
                var context = Integrated_productionEntities2.GetContext();
                var product = context.Product
                    .Include("Category")
                    .Include("Unit")
                    .Include("StatusProduct")
                    .FirstOrDefault(p => p.id_product == selectedProduct.id_product);

                if (product != null)
                {
                    NavigationService.Navigate(new AddEditProduct(product));
                }
                else
                {
                    MessageBox.Show("Продукт не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = Integrated_productionEntities2.GetContext();
            if (Visibility == Visibility.Visible)
            {
                context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                ListViewRequest.ItemsSource = context.Product
                    .Include("BillOfMaterials")
                    .Include("Category")
                    .Include("Unit")
                    .Include("StatusProduct")
                    .ToList();
            }
        }
    }
}