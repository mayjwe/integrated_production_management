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
using integrated_production_management.Pages;

namespace integrated_production_management
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isAuthenticated = false;

        public MainWindow()
        {
            InitializeComponent();

            NavButtonsPanel.Visibility = Visibility.Collapsed;

            FrmMain.Navigate(new Autho());
            UpdateNavigationButtons(false);
        }

        public void SetAuthenticated(string userInfo)
        {
            _isAuthenticated = true;
            NavButtonsPanel.Visibility = Visibility.Visible;
            FrmMain.Navigate(new ProductsPage());
            UpdateNavigationButtons(true);
        }

        private void FrmMain_ContentRendered(object sender, EventArgs e)
        {
            if (!_isAuthenticated)
            {
                UpdateNavigationButtons(false);
                return;
            }

            var isListPage = FrmMain.Content is ProductsPage ||
                            FrmMain.Content is MaterialsPage ||
                            FrmMain.Content is SuppliersPage ||
                            FrmMain.Content is EmployeesPage ||
                            FrmMain.Content is EquipmentsPage ||
                            FrmMain.Content is PurchasesPage ||
                            FrmMain.Content is StockPage ||
                            FrmMain.Content is AssignResourcesPage;

            UpdateNavigationButtons(isListPage);
        }

        public void UpdateNavigationButtons(bool isListPage)
        {
            if (!_isAuthenticated)
            {
                NavButtonsPanel.Visibility = Visibility.Collapsed;
                if (FrmMain.Content is Autho)
                {
                    btnBack.Visibility = Visibility.Collapsed;
                }
                else
                {
                    btnBack.Visibility = Visibility.Visible;
                }
                return;
            }

            if (isListPage)
            {
                NavButtonsPanel.Visibility = Visibility.Visible;
                btnBack.Visibility = Visibility.Collapsed;
            }
            else
            {
                NavButtonsPanel.Visibility = Visibility.Collapsed;
                btnBack.Visibility = Visibility.Visible;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (FrmMain.CanGoBack)
            {
                FrmMain.GoBack();
                UpdateNavigationButtons(true);
            }
        }

        private void NavigateToPage(Page page)
        {
            if (FrmMain.Content?.GetType() == page.GetType())
            {
                FrmMain.Navigate(new Page());
                FrmMain.Navigate(page);
            }
            else
            {
                FrmMain.Navigate(page);
            }
            UpdateNavigationButtons(true);
        }

        private void btnProducts_Click(object sender, RoutedEventArgs e) => NavigateToPage(new ProductsPage());
        private void btnMaterials_Click(object sender, RoutedEventArgs e) => NavigateToPage(new MaterialsPage());
        private void btnSuppliers_Click(object sender, RoutedEventArgs e) => NavigateToPage(new SuppliersPage());
        private void btnEmployees_Click(object sender, RoutedEventArgs e) => NavigateToPage(new EmployeesPage());
        private void btnEquipments_Click(object sender, RoutedEventArgs e) => NavigateToPage(new EquipmentsPage());
        private void btnPurchases_Click(object sender, RoutedEventArgs e) => NavigateToPage(new PurchasesPage());
        private void btnWarehouse_Click(object sender, RoutedEventArgs e) => NavigateToPage(new StockPage());
        private void btnAssignResources_Click(object sender, RoutedEventArgs e) => NavigateToPage(new AssignResourcesPage());
        private void btnCreateOrder_Click(object sender, RoutedEventArgs e) => NavigateToPage(new CreateOrderPage());
        private void btnStatuses_Click(object sender, RoutedEventArgs e) => NavigateToPage(new StatusesPage());
    }
}