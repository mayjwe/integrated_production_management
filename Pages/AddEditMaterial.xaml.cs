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
    /// Логика взаимодействия для AddEditMaterial.xaml
    /// </summary>
    public partial class AddEditMaterial : Page
    {
        private Material _material = new Material();
        private bool _isEditing = false;

        public AddEditMaterial(Material material = null)
        {
            InitializeComponent();

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.UpdateNavigationButtons(false);
            }

            LoadTypeMaterials();
            LoadUnits();

            if (material != null && material.id_material > 0)
            {
                _material = material;
                _isEditing = true;
                LoadMaterialData();
            }
        }

        private void LoadMaterialData()
        {
            txtCode.Text = _material.code.ToString();
            txtName.Text = _material.name ?? "";
            txtMinRemain.Text = _material.min_remain.ToString();
            txtCurrentPrice.Text = _material.current_price.ToString();

            cbTypeMaterial.SelectedValue = _material.id_type_material;
            cbUnit.SelectedValue = _material.id_unit;
        }

        private void LoadTypeMaterials()
        {
            var context = Integrated_productionEntities2.GetContext();
            cbTypeMaterial.ItemsSource = context.TypeMaterial.ToList();
        }

        private void LoadUnits()
        {
            var context = Integrated_productionEntities2.GetContext();
            cbUnit.ItemsSource = context.Unit.ToList();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!long.TryParse(txtCode.Text, out long code) || code <= 0)
            {
                MessageBox.Show("Введите корректный код материала!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите наименование материала!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtMinRemain.Text, out decimal minRemain) || minRemain < 0)
            {
                MessageBox.Show("Введите корректный минимальный остаток!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtCurrentPrice.Text, out decimal currentPrice) || currentPrice < 0)
            {
                MessageBox.Show("Введите корректную цену!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbTypeMaterial.SelectedValue == null)
            {
                MessageBox.Show("Выберите тип материала!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbUnit.SelectedValue == null)
            {
                MessageBox.Show("Выберите единицу измерения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var context = Integrated_productionEntities2.GetContext();

                if (!_isEditing)
                {
                    var existingMaterial = context.Material.FirstOrDefault(m => m.code == code);
                    if (existingMaterial != null)
                    {
                        MessageBox.Show("Материал с таким кодом уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _material.code = code;
                    _material.name = txtName.Text;
                    _material.min_remain = minRemain;
                    _material.current_price = currentPrice;
                    _material.id_type_material = Convert.ToInt64(cbTypeMaterial.SelectedValue);
                    _material.id_unit = Convert.ToInt64(cbUnit.SelectedValue);

                    context.Material.Add(_material);
                }
                else
                {
                    var existingMaterial = context.Material.FirstOrDefault(m => m.code == code && m.id_material != _material.id_material);
                    if (existingMaterial != null)
                    {
                        MessageBox.Show("Материал с таким кодом уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var materialToUpdate = context.Material.Find(_material.id_material);
                    if (materialToUpdate != null)
                    {
                        materialToUpdate.code = code;
                        materialToUpdate.name = txtName.Text;
                        materialToUpdate.min_remain = minRemain;
                        materialToUpdate.current_price = currentPrice;
                        materialToUpdate.id_type_material = Convert.ToInt64(cbTypeMaterial.SelectedValue);
                        materialToUpdate.id_unit = Convert.ToInt64(cbUnit.SelectedValue);
                    }
                }

                context.SaveChanges();

                MessageBox.Show("Материал успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}