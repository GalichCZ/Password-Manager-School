using password_manager.Entities;
using password_manager.Repositrories;
using System.Windows;
using System.Windows.Controls;

namespace password_manager.Modals
{
    public partial class UpdateModal : Window
    {
        private static readonly AccountRepository serviceRepository = new AccountRepository();
        private static readonly string key = "b14ca5898a4e4133bbce2ea2315a1916";
        public string originalInput = string.Empty;

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

                string passwordStr = passwordTextBox.Text;

                if(string.IsNullOrEmpty(login) || string.IsNullOrEmpty(passwordStr))
                {
                    MessageBox.Show("Login and password cannot be empty!");
                    return;
                }
                
                string password = Utils.EncryptPassword(key, passwordStr);

                try
                {
                    serviceRepository.UpdateService(login, password, service.Id, id);
                    MessageBox.Show("Service updated successfully!");
                    Close();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            // Check if the current text is different from the original input
            bool isTextChanged = !string.Equals(textBox.Text, originalInput);

            // Update the button's IsEnabled property based on whether the text has changed
            updateButton.IsEnabled = isTextChanged;
        }

    }
}
