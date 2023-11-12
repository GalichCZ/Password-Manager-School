using password_manager.Entities;
using password_manager.Repositrories;
using System.Windows;

namespace password_manager.Modals
{
    public partial class UpdateModal : Window
    {
        private static readonly AccountRepository serviceRepository = new AccountRepository();

        public UpdateModal()
        {
            InitializeComponent();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Account selectedAccount = (Account)DataContext;

            if (selectedAccount != null)
            {
                long id = selectedAccount.Id;
                Service service = (Service)serviceNameComboBox.SelectedItem;
                string login = loginTextBox.Text;
                string password = passwordTextBox.Text;

                serviceRepository.UpdateService(login, password, service.Id, id);

                MessageBox.Show("Service updated successfully!");
                this.Close();
            }
        }
    }
}
