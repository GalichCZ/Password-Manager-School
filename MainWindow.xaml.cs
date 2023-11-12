using password_manager.Entities;
using password_manager.Modals;
using password_manager.Repositrories;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace password_manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static readonly string key = "b14ca5898a4e4133bbce2ea2315a1916";
        private static readonly AccountRepository accountRepository = new AccountRepository();
        private static readonly ServiceRepository serviceRepository = new ServiceRepository();
        private static readonly SQLiteHelper dbHelper = new SQLiteHelper();
        private System.Windows.Threading.DispatcherTimer timer;

        public MainWindow()
        {
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;

            InitializeComponent();
            LoadAccounts();
            LoadServices();
        }

        private void AddAccountButton_Click(object sender, RoutedEventArgs e)
        {
            Service service = (Service)serviceNameComboBox.SelectedItem;
            string login = loginTextBox.Text;
            string password = passwordHashTextBox.Text;
            string passwordHash = Utils.EncryptPassword(key, password);

            accountRepository.AddAccount(login, service.Id, passwordHash);
            
            MessageBox.Show(Utils.CheckPasswordDifficult(password));
            MessageBox.Show("Service added successfully!");
            LoadAccounts();
            dbHelper.UpdateVirtualTableData();
        }

        private void AddServiceButton_Click(object sender, RoutedEventArgs e)
        {
            string serviceName = serviceNameTextBox.Text;

            bool isServiceExists = serviceRepository.CheckServiceExists(serviceName);

            if (isServiceExists)
            {
                MessageBox.Show("Service already exists!");
                return;
            }

            serviceRepository.AddService(serviceName);

            MessageBox.Show("Service added successfully!");
            LoadServices();
        }

        private void LoadAccounts()
        {
            List<Account> services = accountRepository.GetAllServices();
            
            serviceListView.ItemsSource = services;
        }

        private void LoadServices()
        {
            List<Service> services = serviceRepository.GetServices();

            serviceNameComboBox.ItemsSource = services;
        }

        private void PasswordTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock? textBlock = sender as TextBlock;

            if (textBlock != null)
            {
                MessageBox.Show("Password successfully copied!");
                Clipboard.SetText(Utils.DecryptPassword(key, textBlock.Text));
            }
        }

        private void IDTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Account selectedService = (Account)serviceListView.SelectedItem;
            if (selectedService != null)
            {
                UpdateModal modalWindow = new UpdateModal();

                modalWindow.DataContext = selectedService;

                modalWindow.ShowDialog();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Restart the timer on each text change
            timer.Stop();
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}
