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
    /// Логика взаимодействия для AddEditSupplier.xaml
    /// </summary>
    public partial class AddEditSupplier : Page
    {
        private Supplier _supplier = new Supplier();
        private bool _isEditing = false;

        public AddEditSupplier(Supplier supplier = null)
        {
            InitializeComponent();

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.UpdateNavigationButtons(false);
            }

            if (supplier != null && supplier.id_supplier > 0)
            {
                _supplier = supplier;
                _isEditing = true;
                LoadSupplierData();
            }
        }

        private void LoadSupplierData()
        {
            txtInn.Text = _supplier.inn.ToString();
            txtName.Text = _supplier.name ?? "";
            txtPhone.Text = _supplier.phone ?? "";
            txtRating.Text = _supplier.rating.ToString();
            txtTermsPayment.Text = _supplier.terms_payment ?? "";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!long.TryParse(txtInn.Text, out long inn) || inn <= 0)
            {
                MessageBox.Show("Введите корректный ИНН!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string innStr = inn.ToString();
            if (innStr.Length != 10 && innStr.Length != 12)
            {
                MessageBox.Show("ИНН должен содержать 10 или 12 цифр!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите наименование поставщика!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtRating.Text, out int rating) || rating < 1 || rating > 10)
            {
                MessageBox.Show("Введите корректный рейтинг (1-10)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var context = Integrated_productionEntities2.GetContext();

                if (!_isEditing)
                {
                    var existingSupplier = context.Supplier.FirstOrDefault(s => s.inn == inn);
                    if (existingSupplier != null)
                    {
                        MessageBox.Show("Поставщик с таким ИНН уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _supplier.inn = inn;
                    _supplier.name = txtName.Text;
                    _supplier.phone = txtPhone.Text;
                    _supplier.rating = rating;
                    _supplier.terms_payment = txtTermsPayment.Text;

                    context.Supplier.Add(_supplier);
                }
                else
                {
                    var existingSupplier = context.Supplier.FirstOrDefault(s => s.inn == inn && s.id_supplier != _supplier.id_supplier);
                    if (existingSupplier != null)
                    {
                        MessageBox.Show("Поставщик с таким ИНН уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var supplierToUpdate = context.Supplier.Find(_supplier.id_supplier);
                    if (supplierToUpdate != null)
                    {
                        supplierToUpdate.inn = inn;
                        supplierToUpdate.name = txtName.Text;
                        supplierToUpdate.phone = txtPhone.Text;
                        supplierToUpdate.rating = rating;
                        supplierToUpdate.terms_payment = txtTermsPayment.Text;
                    }
                }

                context.SaveChanges();

                MessageBox.Show("Поставщик успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}