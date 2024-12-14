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
    /// Логика взаимодействия для AddRequestPage.xaml
    /// </summary>
    public partial class AddRequestPage : Page
    {
        private readonly Entities db;

        public AddRequestPage()
        {
            InitializeComponent();
            db = new Entities();
            LoadUsers();
            LoadEquipment();
        }

        // Загружаем список пользователей
        private void LoadUsers()
        {
            var users = db.Users.ToList();
            Users.ItemsSource = users;
            Users.DisplayMemberPath = "FullName";
            Users.SelectedValuePath = "UserID";
        }

        // Загружаем список оборудования
        private void LoadEquipment()
        {
            var equipmentList = db.Equipment.ToList();
            Equipment.ItemsSource = equipmentList;
            Equipment.DisplayMemberPath = "EquipmentType";
            Equipment.SelectedValuePath = "EquipmentID";
        }

        // Обработчик клика по кнопке "Add Request"
        private void AddRequest_Click(object sender, RoutedEventArgs e)
        {
            // Проверка, что элемент выбран в Priority
            if (Priority.SelectedItem == null)
            {
                MessageBox.Show("Please select a priority.");
                return;
            }

            // Проверка, что выбран пользователь и оборудование
            if (Users.SelectedItem == null || Equipment.SelectedItem == null)
            {
                MessageBox.Show("Please select a user and equipment.");
                return;
            }

            // Получаем строковое значение выбранного элемента приоритета
            var selectedPriority = ((ComboBoxItem)Priority.SelectedItem).Content.ToString();

            // Создание новой заявки
            var newRequest = new RepairRequests
            {
                ProblemDescription = ProblemDescription.Text,
                Priority = selectedPriority, // Присваиваем выбранный приоритет
                Status = "Registered",
                RegistrationDate = DateTime.Now,
                UserID = (int)Users.SelectedValue, // Используем выбранного пользователя
                EquipmentID = (int)Equipment.SelectedValue // Используем выбранное оборудование
            };

            // Добавление в базу данных и сохранение изменений
            db.RepairRequests.Add(newRequest);
            db.SaveChanges();

            // Сообщение об успешном добавлении
            MessageBox.Show("Request Added Successfully");

            NavigationService.Navigate(new Pages.RequestsPage());
        }
    }
}