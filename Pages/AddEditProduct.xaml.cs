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
    /// Логика взаимодействия для AddEditProduct.xaml
    /// </summary>
    public partial class AddEditProduct : Page
    {
        private Product _product = new Product();
        private bool _isEditing = false;

        public AddEditProduct(Product product = null)
        {
            InitializeComponent();

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.UpdateNavigationButtons(false);
            }

            LoadCategories();
            LoadUnits();
            LoadStatuses();

            if (product != null && product.id_product > 0)
            {
                _product = product;
                _isEditing = true;
                LoadProductData();
            }
        }

        private void LoadProductData()
        {
            txtArticle.Text = _product.article.ToString();
            txtName.Text = _product.name ?? "";
            txtDescription.Text = _product.description ?? "";

            cbCategory.SelectedValue = _product.id_category;
            cbUnit.SelectedValue = _product.id_unit;
            cbStatus.SelectedValue = _product.id_status_product;
        }

        private void LoadCategories()
        {
            var context = Integrated_productionEntities2.GetContext();
            cbCategory.ItemsSource = context.Category.ToList();
        }

        private void LoadUnits()
        {
            var context = Integrated_productionEntities2.GetContext();
            cbUnit.ItemsSource = context.Unit.ToList();
        }

        private void LoadStatuses()
        {
            var context = Integrated_productionEntities2.GetContext();
            cbStatus.ItemsSource = context.StatusProduct.ToList();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!long.TryParse(txtArticle.Text, out long article) || article <= 0)
            {
                MessageBox.Show("Введите корректный артикул!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (article.ToString().Length != 6)
            {
                MessageBox.Show("Артикул должен состоять из 6 цифр!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название продукта!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbCategory.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbUnit.SelectedValue == null)
            {
                MessageBox.Show("Выберите единицу измерения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbStatus.SelectedValue == null)
            {
                MessageBox.Show("Выберите статус!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var context = Integrated_productionEntities2.GetContext();

                if (!_isEditing)
                {
                    var existingProduct = context.Product.FirstOrDefault(p => p.article == article);
                    if (existingProduct != null)
                    {
                        MessageBox.Show("Продукт с таким артикулом уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _product.article = article;
                    _product.name = txtName.Text;
                    _product.description = txtDescription.Text;
                    _product.id_category = Convert.ToInt64(cbCategory.SelectedValue);
                    _product.id_unit = Convert.ToInt64(cbUnit.SelectedValue);
                    _product.id_status_product = Convert.ToInt64(cbStatus.SelectedValue);

                    context.Product.Add(_product);
                }
                else
                {
                    var existingProduct = context.Product.FirstOrDefault(p => p.article == article && p.id_product != _product.id_product);
                    if (existingProduct != null)
                    {
                        MessageBox.Show("Продукт с таким артикулом уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var productToUpdate = context.Product.Find(_product.id_product);
                    if (productToUpdate != null)
                    {
                        productToUpdate.article = article;
                        productToUpdate.name = txtName.Text;
                        productToUpdate.description = txtDescription.Text;
                        productToUpdate.id_category = Convert.ToInt64(cbCategory.SelectedValue);
                        productToUpdate.id_unit = Convert.ToInt64(cbUnit.SelectedValue);
                        productToUpdate.id_status_product = Convert.ToInt64(cbStatus.SelectedValue);
                    }
                }

                context.SaveChanges();

                MessageBox.Show("Продукт успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}