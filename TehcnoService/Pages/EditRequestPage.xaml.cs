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
    /// Логика взаимодействия для EditRequestPage.xaml
    /// </summary>
    public partial class EditRequestPage : Page
    {
        private readonly Entities db;
        private readonly int _requestId;

        public EditRequestPage(int requestId)
        {
            InitializeComponent();
            db = new Entities();
            _requestId = requestId;
            LoadRequestData();
            LoadExecutors();
        }

        // Загружаем текущие данные заявки
        private void LoadRequestData()
        {
            var request = db.RepairRequests
                .Where(r => r.RequestID == _requestId)
                .Select(r => new
                {
                    r.RequestID,
                    r.ProblemDescription,
                    r.Priority,
                    r.Status,
                    ExecutorID = db.RequestAssignments
                        .Where(ra => ra.RequestID == r.RequestID)
                        .Select(ra => ra.ExecutorID)
                        .FirstOrDefault()
                })
                .FirstOrDefault();

            if (request != null)
            {
                ProblemDescription.Text = request.ProblemDescription;
                Priority.SelectedItem = Priority.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == request.Priority);
                Status.SelectedItem = Status.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == request.Status);

                // Выбираем исполнителя
                Executors.SelectedValue = request.ExecutorID;
            }
            else
            {
                MessageBox.Show("Request not found.");
            }
        }

        // Загружаем список исполнителей
        private void LoadExecutors()
        {
            var executors = db.Executors.ToList();
            Executors.ItemsSource = executors;
            Executors.DisplayMemberPath = "FullName";
            Executors.SelectedValuePath = "ExecutorID";
        }

        // Сохраняем изменения
        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            // Получаем данные из формы
            var selectedPriority = ((ComboBoxItem)Priority.SelectedItem).Content.ToString();
            var selectedStatus = ((ComboBoxItem)Status.SelectedItem).Content.ToString();
            var selectedExecutorId = Executors.SelectedValue != null ? (int)Executors.SelectedValue : (int?)null;

            // Находим заявку в базе данных
            var request = db.RepairRequests.FirstOrDefault(r => r.RequestID == _requestId);
            if (request != null)
            {
                request.ProblemDescription = ProblemDescription.Text;
                request.Priority = selectedPriority;
                request.Status = selectedStatus;

                // Если выбран исполнитель, добавляем/обновляем запись в RequestAssignments
                if (selectedExecutorId.HasValue)
                {
                    var assignment = db.RequestAssignments
                        .FirstOrDefault(ra => ra.RequestID == _requestId);
                    if (assignment != null)
                    {
                        assignment.ExecutorID = selectedExecutorId.Value;
                    }
                    else
                    {
                        // Если записи в RequestAssignments нет, создаем новую
                        db.RequestAssignments.Add(new RequestAssignments
                        {
                            RequestID = _requestId,
                            ExecutorID = selectedExecutorId.Value,
                            AssignmentDate = DateTime.Now
                        });
                    }
                }

                db.SaveChanges();
                MessageBox.Show("Request updated successfully!");
                NavigationService.Navigate(new Pages.RequestsPage());
            }
            else
            {
                MessageBox.Show("Request not found.");
            }
        }
    }
}