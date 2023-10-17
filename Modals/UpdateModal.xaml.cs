using password_manager.Entities;
using password_manager.Repositrories;
using System.Windows;

namespace password_manager.Modals
{
    public partial class UpdateModal : Window
    {
        private static readonly ServiceRepository serviceRepository = new ServiceRepository();

        public UpdateModal()
        {
            InitializeComponent();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Service selectedService = (Service)DataContext;

            if (selectedService != null)
            {
                long id = selectedService.Id;
                string serviceName = serviceNameTextBox.Text;
                string login = loginTextBox.Text;
                string password = passwordTextBox.Text;

                Service updatedService = new Service(id, serviceName, login, password);

                serviceRepository.UpdateService(updatedService);

                MessageBox.Show("Service updated successfully!");
                this.Close();
            }
        }
    }
}
