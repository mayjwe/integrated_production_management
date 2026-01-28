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
    /// Логика взаимодействия для EditStatusPage.xaml
    /// </summary>
    public partial class EditStatusPage : Page
    {
        private StatusOrder _status;
        private Integrated_productionEntities2 db = Integrated_productionEntities2.GetContext();

        public EditStatusPage(StatusOrder status)
        {
            InitializeComponent();
            _status = status;
            LoadData();
        }

        private void LoadData()
        {
            var statuses = db.StatusOrder.ToList();
            cbStatus.ItemsSource = statuses;
            cbStatus.SelectedValue = _status.id_status_order;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cbStatus.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус!");
                return;
            }

            try
            {
                var selectedStatus = (StatusOrder)cbStatus.SelectedItem;
                var statusToUpdate = db.StatusOrder.Find(_status.id_status_order);

                if (statusToUpdate != null)
                {
                    statusToUpdate.title = selectedStatus.title;
                    db.SaveChanges();
                    MessageBox.Show("Статус успешно обновлен!");
                    NavigationService.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}