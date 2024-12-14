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

namespace TehcnoService.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddUserPage.xaml
    /// </summary>
    public partial class AddUserPage : Page
    {
        private readonly Entities db;

        public AddUserPage()
        {
            InitializeComponent();
            db = new Entities();
            LoadUsers();
        }

        // Загружаем пользователей в DataGrid
        private void LoadUsers()
        {
            var users = db.Users.Select(u => new
            {
                u.UserID,
                u.FullName,
                u.ContactInfo
            }).ToList();

            UsersDataGrid.ItemsSource = users;
        }

        // Добавляем нового пользователя
        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox.Text;
            string contactInfo = ContactInfoTextBox.Text;

            // Проверка на наличие всех данных
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(contactInfo))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            // Проверка на уникальность пользователя
            var existingUser = db.Users.FirstOrDefault(u => u.FullName == fullName || u.ContactInfo == contactInfo);
            if (existingUser != null)
            {
                MessageBox.Show("User with the same name or contact already exists.");
                return;
            }

            // Создаем нового пользователя
            var newUser = new Users
            {
                FullName = fullName,
                ContactInfo = contactInfo
            };

            db.Users.Add(newUser);
            db.SaveChanges();
            MessageBox.Show("User added successfully!");

            // Обновляем список пользователей
            LoadUsers();
            ClearFields();
        }

        // Удаляем выбранного пользователя
        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem != null)
            {
                var selectedUser = (dynamic)UsersDataGrid.SelectedItem;
                int userId = selectedUser.UserID;

                var userToDelete = db.Users.FirstOrDefault(u => u.UserID == userId);
                if (userToDelete != null)
                {
                    db.Users.Remove(userToDelete);
                    db.SaveChanges();
                    MessageBox.Show("User deleted successfully!");

                    // Обновляем список пользователей
                    LoadUsers();
                }
            }
            else
            {
                MessageBox.Show("Please select a user to delete.");
            }
        }

        // Очищаем поля ввода
        private void ClearFields()
        {
            FullNameTextBox.Clear();
            ContactInfoTextBox.Clear();
        }
    }
}