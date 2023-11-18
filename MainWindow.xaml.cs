using password_manager.Entities;
using password_manager.Modals;
using password_manager.Repositrories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
        private System.Windows.Threading.DispatcherTimer timer;

        public MainWindow()
        {
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;

            InitializeComponent();
            LoadAccounts();
            LoadServices();
        }

        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        void GridViewColumnHeaderClickedHandler(object sender,
                                           RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(sortBy, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void AddAccountButton_Click(object sender, RoutedEventArgs e)
        {
            Service service = (Service)serviceNameComboBox.SelectedItem;
            string login = loginTextBox.Text;
            string password = passwordHashTextBox.Text;

            if (service == null || String.IsNullOrEmpty(login) || String.IsNullOrEmpty(password))
            {
                MessageBox.Show("Complete all data !");
                return;
            }

            string passwordHash = Utils.EncryptPassword(key, password);

            accountRepository.AddAccount(login, service.Id, passwordHash);
            
            MessageBox.Show(Utils.CheckPasswordDifficult(password));
            MessageBox.Show("Service added successfully!");
            LoadAccounts();
        }

        private void AddServiceButton_Click(object sender, RoutedEventArgs e)
        {
            string serviceName = serviceNameTextBox.Text;

            if(String.IsNullOrEmpty(serviceName))
            {
                MessageBox.Show("Complete all data !");
                return;
            }

            bool isServiceExists = true;

            try
            {
                isServiceExists = serviceRepository.CheckServiceExists(serviceName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            if (isServiceExists)
            {
                MessageBox.Show("Service already exists!");
                return;
            }

            try
            {
                serviceRepository.AddService(serviceName);
                MessageBox.Show("Service added successfully!");
                LoadServices();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadAccounts()
        {
            try
            {
                List<Account> services = accountRepository.GetAllServices();

                accountListView.ItemsSource = services;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadServices()
        {
            try
            {
                List<Service> services = serviceRepository.GetServices();

                serviceNameComboBox.ItemsSource = services;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

        private void ListView_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            long id = (long)button.Tag;

            Account selectedAccount = accountRepository.GetAccountById(id);

            if (selectedAccount != null)
            {
                UpdateModal modalWindow = new UpdateModal();

                modalWindow.originalInput = selectedAccount.PasswordHashString;

                modalWindow.DataContext = selectedAccount;

                List<Service> services = null;

                try
                {
                    services = serviceRepository.GetServices();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                Service foundService = services.FirstOrDefault(service => service.ServiceName == selectedAccount.ServiceName);

                modalWindow.serviceNameComboBox.ItemsSource = services;

                modalWindow.serviceNameComboBox.SelectedValue = foundService;

                modalWindow.ShowDialog();

                modalWindow.Unloaded += (s, ev) =>
                {
                    LoadAccounts();
                };
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Restart the timer on each text change
            timer.Stop();
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // This is the same as useDebounce hook in React, it will trigger the action after 100ms, when the user stops typing
            timer.Stop();
            string searchTerm = searchTextBox.Text;
            TriggerAction(searchTerm);
        }

        private void TriggerAction(string searchTerm)
        {
            if (searchTerm.Length > 0)
            {
                try
                {
                    List<Account> services = accountRepository.SearchThrough(searchTerm);

                    accountListView.ItemsSource = services;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                LoadAccounts();
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(accountListView.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        private void ListView_SelectionDelete(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            long id = (long)button.Tag;

            try
            {
                accountRepository.DeleteAccount(id);
                MessageBox.Show("Service deleted successfully!");
            }
            catch (Exception)
            {
                MessageBox.Show("Service can't be deleted!");
            }

            LoadAccounts();
        }
    }
}
