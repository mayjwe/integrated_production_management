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
    /// Логика взаимодействия для StockPage.xaml
    /// </summary>
    public partial class StockPage : Page
    {
        public StockPage()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            var context = Integrated_productionEntities2.GetContext();

            var stockBalances = context.StockBalance
                .Include("Material")
                .Include("Material.Unit")
                .Include("Warehouse")
                .Include("Warehouse.TypeWarehouse")
                .ToList();
            lvMaterials.ItemsSource = stockBalances;
        }

        private void btnChangeWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is StockBalance stockBalance)
            {
                var window = new ChangeWarehouseWindow(stockBalance);
                if (window.ShowDialog() == true)
                {
                    LoadData();
                }
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                LoadData();
            }
        }
    }
}