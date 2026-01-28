using integrated_production_management.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для AddEditPurchaseOrder.xaml
    /// </summary>
    public partial class AddEditPurchaseOrder : Page
    {
        private PurchaseOrder _purchaseOrder = new PurchaseOrder();
        private bool _isEditing = false;
        private List<PurchaseOrderDetail> _orderDetails = new List<PurchaseOrderDetail>();

        public AddEditPurchaseOrder(PurchaseOrder purchaseOrder = null)
        {
            InitializeComponent();

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.UpdateNavigationButtons(false);
            }

            LoadSuppliers();
            LoadMaterials();

            if (purchaseOrder != null && purchaseOrder.id_purchase_order > 0)
            {
                _purchaseOrder = purchaseOrder;
                _isEditing = true;
                LoadPurchaseOrderData();
            }

            lvMaterials.ItemsSource = _orderDetails;
        }

        private void LoadPurchaseOrderData()
        {
            cbSupplier.SelectedValue = _purchaseOrder.id_supplier;

            var context = Integrated_productionEntities2.GetContext();
            var details = context.PurchaseOrderDetail
                .Where(d => d.id_purchase_order == _purchaseOrder.id_purchase_order)
                .Include("Material")
                .ToList();

            _orderDetails.AddRange(details);
            lvMaterials.Items.Refresh();
        }

        private void LoadSuppliers()
        {
            var context = Integrated_productionEntities2.GetContext();
            cbSupplier.ItemsSource = context.Supplier.ToList();
        }

        private void LoadMaterials()
        {
            var context = Integrated_productionEntities2.GetContext();
            cbMaterial.ItemsSource = context.Material.Include("Unit").ToList();
        }

        private void btnAddMaterial_Click(object sender, RoutedEventArgs e)
        {
            if (cbMaterial.SelectedItem == null)
            {
                MessageBox.Show("Выберите материал!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtQuantity.Text, out decimal quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtUnitPrice.Text, out decimal unitPrice) || unitPrice < 0)
            {
                MessageBox.Show("Введите корректную цену!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var material = (Material)cbMaterial.SelectedItem;

            var existing = _orderDetails.FirstOrDefault(d => d.id_material == material.id_material);
            if (existing != null)
            {
                existing.quantity = quantity;
                existing.unit_price = unitPrice;
                existing.conditions = txtConditions.Text;
            }
            else
            {
                var detail = new PurchaseOrderDetail
                {
                    id_material = material.id_material,
                    Material = material,
                    quantity = quantity,
                    unit_price = unitPrice,
                    conditions = txtConditions.Text
                };
                _orderDetails.Add(detail);
            }

            lvMaterials.Items.Refresh();
            txtQuantity.Text = "1";
            txtUnitPrice.Text = "0";
            txtConditions.Text = "";
        }

        private void DeleteMaterialButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is PurchaseOrderDetail detailToRemove)
            {
                _orderDetails.Remove(detailToRemove);
                lvMaterials.Items.Refresh();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cbSupplier.SelectedValue == null)
            {
                MessageBox.Show("Выберите поставщика!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_orderDetails.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один материал в заказ!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var context = Integrated_productionEntities2.GetContext();

                var productionOrder = new ProductionOrder
                {
                    clinet = "Закупка материалов",
                    id_priority = 1,
                    deadline = DateTime.Now.AddDays(30),
                    id_status_order = 1
                };

                context.ProductionOrder.Add(productionOrder);
                context.SaveChanges();

                if (!_isEditing)
                {
                    _purchaseOrder.id_supplier = Convert.ToInt64(cbSupplier.SelectedValue);
                    _purchaseOrder.id_order = productionOrder.id_order;

                    context.PurchaseOrder.Add(_purchaseOrder);
                    context.SaveChanges();

                    foreach (var detail in _orderDetails)
                    {
                        detail.id_purchase_order = _purchaseOrder.id_purchase_order;
                        context.PurchaseOrderDetail.Add(detail);
                    }
                }
                else
                {
                    var purchaseToUpdate = context.PurchaseOrder.Find(_purchaseOrder.id_purchase_order);
                    if (purchaseToUpdate != null)
                    {
                        purchaseToUpdate.id_supplier = Convert.ToInt64(cbSupplier.SelectedValue);

                        var existingDetails = context.PurchaseOrderDetail
                            .Where(d => d.id_purchase_order == _purchaseOrder.id_purchase_order)
                            .ToList();
                        context.PurchaseOrderDetail.RemoveRange(existingDetails);

                        foreach (var detail in _orderDetails)
                        {
                            detail.id_purchase_order = _purchaseOrder.id_purchase_order;
                            context.PurchaseOrderDetail.Add(detail);
                        }
                    }
                }

                context.SaveChanges();

                MessageBox.Show("Заказ на закупку успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}