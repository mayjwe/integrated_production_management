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
    /// Логика взаимодействия для CreateOrderPage.xaml
    /// </summary>
    public partial class CreateOrderPage : Page
    {
        private Integrated_productionEntities2 db = Integrated_productionEntities2.GetContext();

        public CreateOrderPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var products = db.Product
                    .Include("StatusProduct")
                    .Where(p => p.StatusProduct.title == "активен")
                    .ToList();
                cbProduct.ItemsSource = products;

                var priorities = db.Priority.ToList();
                cbPriority.ItemsSource = priorities;

                dpDeadline.SelectedDate = DateTime.Now.AddDays(7);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtClient.Text))
            {
                MessageBox.Show("Введите клиента");
                return;
            }

            if (cbProduct.SelectedItem == null)
            {
                MessageBox.Show("Выберите продукт");
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество");
                return;
            }

            if (cbPriority.SelectedItem == null)
            {
                MessageBox.Show("Выберите приоритет");
                return;
            }

            if (dpDeadline.SelectedDate == null || dpDeadline.SelectedDate <= DateTime.Now)
            {
                MessageBox.Show("Выберите корректный дедлайн");
                return;
            }

            try
            {
                var product = (Product)cbProduct.SelectedItem;
                var priority = (Priority)cbPriority.SelectedItem;
                var statusOrder = db.StatusOrder.FirstOrDefault(s => s.title == "запланирован");

                if (statusOrder == null)
                {
                    MessageBox.Show("Не найден статус заказа");
                    return;
                }

                var productionOrder = new ProductionOrder
                {
                    clinet = txtClient.Text,
                    id_priority = priority.id_priority,
                    deadline = dpDeadline.SelectedDate.Value,
                    id_status_order = statusOrder.id_status_order
                };

                db.ProductionOrder.Add(productionOrder);
                db.SaveChanges();

                var orderProduct = new OrderProduct
                {
                    id_order = productionOrder.id_order,
                    id_product = product.id_product,
                    quantity = quantity
                };

                db.OrderProduct.Add(orderProduct);
                db.SaveChanges();

                MessageBox.Show("Заказ создан успешно");
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания заказа: {ex.Message}");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}