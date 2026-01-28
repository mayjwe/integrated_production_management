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
using System.Windows.Shapes;

namespace integrated_production_management
{
    /// <summary>
    /// Логика взаимодействия для ChangeWarehouseWindow.xaml
    /// </summary>
    public partial class ChangeWarehouseWindow : Window
    {
        private StockBalance _stockBalance;

        public ChangeWarehouseWindow(StockBalance stockBalance)
        {
            InitializeComponent();
            _stockBalance = stockBalance;
            LoadData();
        }

        private void LoadData()
        {
            txtMaterialName.Text = _stockBalance.Material.name;
            txtCurrentWarehouse.Text = _stockBalance.Warehouse.title;

            var context = Integrated_productionEntities2.GetContext();
            cmbWarehouses.ItemsSource = context.Warehouse
                .Include("TypeWarehouse")
                .ToList();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbWarehouses.SelectedValue == null)
            {
                MessageBox.Show("Выберите новый склад!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var context = Integrated_productionEntities2.GetContext();
                var stockBalanceToUpdate = context.StockBalance.Find(_stockBalance.id_stock_balance);

                if (stockBalanceToUpdate != null)
                {
                    stockBalanceToUpdate.id_warehouse = Convert.ToInt64(cmbWarehouses.SelectedValue);
                    context.SaveChanges();
                    DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}