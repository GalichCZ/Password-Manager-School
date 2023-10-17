using Npgsql;
using password_manager.Entities;
using password_manager.Modals;
using password_manager.Repositrories;
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

namespace password_manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string key = "b14ca5898a4e4133bbce2ea2315a1916";
        private static readonly ServiceRepository serviceRepository = new ServiceRepository();

        public MainWindow()
        { 
            InitializeComponent();
            serviceRepository.CreateTables();
            LoadData();
        }

        private void AddServiceButton_Click(object sender, RoutedEventArgs e)
        {
            string serviceName = serviceNameTextBox.Text;
            string login = loginTextBox.Text;
            string password = passwordHashTextBox.Text;
            string passwordHash = Utils.EncryptPassword(key, password);

            Service newService = new Service(serviceName, login, passwordHash);
            serviceRepository.AddService(newService);
            
            MessageBox.Show(Utils.CheckPasswordDifficult(password));
            MessageBox.Show("Service added successfully!");
            LoadData();
        }

        private void LoadData()
        {
            List<Service> services = serviceRepository.GetAllServices();

            serviceListView.ItemsSource = services;
        }

        private void PasswordTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock? textBlock = sender as TextBlock;

            if (textBlock != null)
            {
                Clipboard.SetText(Utils.DecryptPassword(key, textBlock.Text));
            }
        }

        private void IDTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Service selectedService = (Service)serviceListView.SelectedItem;
            if (selectedService != null)
            {
                UpdateModal modalWindow = new UpdateModal();

                modalWindow.DataContext = selectedService;

                modalWindow.ShowDialog();
            }
        }
    }
}
