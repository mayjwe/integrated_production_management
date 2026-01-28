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
    /// Логика взаимодействия для SuppliersPage.xaml
    /// </summary>
    public partial class SuppliersPage : Page
    {
        public SuppliersPage()
        {
            InitializeComponent();

            var suppliers = Integrated_productionEntities2.GetContext().Supplier.ToList();
            ListViewRequest.ItemsSource = suppliers;
        }

        private void btnAddPartner_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditSupplier());
        }

        private void ListViewRequest_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListViewRequest.SelectedItem is Supplier selectedSupplier)
            {
                var context = Integrated_productionEntities2.GetContext();
                var supplier = context.Supplier
                    .FirstOrDefault(s => s.id_supplier == selectedSupplier.id_supplier);

                if (supplier != null)
                {
                    NavigationService.Navigate(new AddEditSupplier(supplier));
                }
                else
                {
                    MessageBox.Show("Поставщик не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = Integrated_productionEntities2.GetContext();
            if (Visibility == Visibility.Visible)
            {
                context.ChangeTracker.Entries().ToList().ForEach(s => s.Reload());
                ListViewRequest.ItemsSource = context.Supplier.ToList();
            }
        }
    }
}