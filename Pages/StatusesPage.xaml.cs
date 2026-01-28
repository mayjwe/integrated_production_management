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
    /// Логика взаимодействия для StatusesPage.xaml
    /// </summary>
    public partial class StatusesPage : Page
    {
        public StatusesPage()
        {
            InitializeComponent();

            var statuses = Integrated_productionEntities2.GetContext().StatusOrder.ToList();
            ListViewStatuses.ItemsSource = statuses;
        }

        private void ListViewStatuses_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListViewStatuses.SelectedItem is StatusOrder selectedStatus)
            {
                var context = Integrated_productionEntities2.GetContext();
                var status = context.StatusOrder.FirstOrDefault(s => s.id_status_order == selectedStatus.id_status_order);

                if (status != null)
                {
                    NavigationService.Navigate(new EditStatusPage(status));
                }
                else
                {
                    MessageBox.Show("Статус не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = Integrated_productionEntities2.GetContext();
            if (Visibility == Visibility.Visible)
            {
                context.ChangeTracker.Entries().ToList().ForEach(s => s.Reload());
                ListViewStatuses.ItemsSource = context.StatusOrder.ToList();
            }
        }
    }
}