using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using TehcnoService.Pages;

namespace TehcnoService
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenRequestsPage(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.RequestsPage());
        }

        private void OpenAddRequestPage(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.AddRequestPage());
        }

        private void OpenStatisticsPage(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.StatisticsPage());
        }

        private void OpenAddUserPage(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate (new Pages.AddUserPage());
        }

        private void OpenAddExecutorPage(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.AddExecutorPage());
        }
    }
}