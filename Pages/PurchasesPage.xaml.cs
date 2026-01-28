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
    /// Логика взаимодействия для PurchasesPage.xaml
    /// </summary>
    public partial class PurchasesPage : Page
    {
        public PurchasesPage()
        {
            InitializeComponent();

            var purchases = Integrated_productionEntities2.GetContext().PurchaseOrder
                .Include("Supplier")
                .Include("PurchaseOrderDetail")
                .Include("PurchaseOrderDetail.Material")
                .ToList();
            ListViewRequest.ItemsSource = purchases;
        }

        private void btnAddPartner_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditPurchaseOrder());
        }

        private void ListViewRequest_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListViewRequest.SelectedItem is PurchaseOrder selectedPurchase)
            {
                var context = Integrated_productionEntities2.GetContext();
                var purchase = context.PurchaseOrder
                    .Include("Supplier")
                    .Include("PurchaseOrderDetail")
                    .Include("PurchaseOrderDetail.Material")
                    .FirstOrDefault(p => p.id_purchase_order == selectedPurchase.id_purchase_order);

                if (purchase != null)
                {
                    NavigationService.Navigate(new AddEditPurchaseOrder(purchase));
                }
                else
                {
                    MessageBox.Show("Закупка не найдена", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = Integrated_productionEntities2.GetContext();
            if (Visibility == Visibility.Visible)
            {
                context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                ListViewRequest.ItemsSource = context.PurchaseOrder
                    .Include("Supplier")
                    .Include("PurchaseOrderDetail")
                    .Include("PurchaseOrderDetail.Material")
                    .ToList();
            }
        }
    }
}