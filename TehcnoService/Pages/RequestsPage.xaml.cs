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
    /// Логика взаимодействия для RequestsPage.xaml
    /// </summary>
    public partial class RequestsPage : Page
    {
        private readonly Entities db;

        public RequestsPage()
        {
            InitializeComponent();
            db = new Entities();
            LoadRequests();
        }

        // Загружаем все заявки в DataGrid
        private void LoadRequests()
        {
            var requests = db.RepairRequests
                .Select(r => new
                {
                    r.RequestID,
                    r.Priority,
                    r.Status,
                    Client = r.Users.FullName,
                    Equipment = r.Equipment.EquipmentType,
                    Executor = db.RequestAssignments
                        .Where(ra => ra.RequestID == r.RequestID)
                        .Select(ra => ra.Executors.FullName)
                        .FirstOrDefault(),  // Получаем имя исполнителя
                    r.RegistrationDate  // Добавляем дату и время регистрации заявки
                })
                .ToList();

            RequestsGrid.ItemsSource = requests;
        }

        // Открытие страницы редактирования заявки
        private void EditRequest_Click(object sender, RoutedEventArgs e)
        {
            if (RequestsGrid.SelectedItem != null)
            {
                var selectedRequest = (dynamic)RequestsGrid.SelectedItem;
                int requestId = selectedRequest.RequestID;
                NavigationService.Navigate(new EditRequestPage(requestId));
            }
        }

        // Удаление выбранной заявки
        private void DeleteRequest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RequestsGrid.SelectedItem != null)
                {
                    // Получаем выбранную заявку как динамический объект
                    var selectedRequest = (dynamic)RequestsGrid.SelectedItem;
                    int requestId = selectedRequest.RequestID;

                    // Извлекаем заявку для удаления
                    var requestToDelete = db.RepairRequests.FirstOrDefault(r => r.RequestID == requestId);
                    if (requestToDelete != null)
                    {
                        // Удаляем зависимые записи из таблицы RequestAssignments
                        var relatedAssignments = db.RequestAssignments.Where(r => r.RequestID == requestId).ToList();
                        db.RequestAssignments.RemoveRange(relatedAssignments);

                        // Удаляем саму заявку
                        db.RepairRequests.Remove(requestToDelete);

                        // Сохраняем изменения в базе данных
                        db.SaveChanges();
                        MessageBox.Show("Request and related data deleted successfully.");
                        LoadRequests();  // Перезагружаем список заявок
                    }
                    else
                    {
                        MessageBox.Show("Request not found.");
                    }
                }
                else
                {
                    MessageBox.Show("Please select a request to delete.");
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибки, выводим подробное сообщение
                MessageBox.Show($"Error deleting request: {ex.Message}");
            }
        }
    }
}