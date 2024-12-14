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
    /// Логика взаимодействия для AddExecutorPage.xaml
    /// </summary>
    public partial class AddExecutorPage : Page
    {
        private readonly Entities db;

        public AddExecutorPage()
        {
            InitializeComponent();
            db = new Entities();
            LoadExecutors();
        }

        // Загружаем исполнителей в DataGrid
        private void LoadExecutors()
        {
            var executors = db.Executors.Select(e => new
            {
                e.ExecutorID,
                e.FullName,
            }).ToList();

            ExecutorsDataGrid.ItemsSource = executors;
        }

        // Добавляем нового исполнителя
        private void AddExecutor_Click(object sender, RoutedEventArgs e)
        {
            string fullName = ExecutorNameTextBox.Text;

            // Проверка на наличие данных
            if (string.IsNullOrEmpty(fullName))
            {
                MessageBox.Show("Please fill in the name.");
                return;
            }

            // Проверка на уникальность имени исполнителя
            var existingExecutor = db.Executors.FirstOrDefault(r => r.FullName == fullName);
            if (existingExecutor != null)
            {
                MessageBox.Show("Executor with the same name already exists.");
                return;
            }

            // Создаем нового исполнителя
            var newExecutor = new Executors
            {
                FullName = fullName,
            };

            db.Executors.Add(newExecutor);
            db.SaveChanges();
            MessageBox.Show("Executor added successfully!");

            // Обновляем список исполнителей
            LoadExecutors();
            ClearFields();
        }

        // Удаляем выбранного исполнителя
        private void DeleteExecutor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ExecutorsDataGrid.SelectedItem != null)
                {
                    // Приводим выбранный элемент к типу, который содержит ExecutorID
                    var selectedExecutor = ExecutorsDataGrid.SelectedItem as dynamic;

                    if (selectedExecutor != null)
                    {
                        int executorId = selectedExecutor.ExecutorID;

                        // Ищем исполнителя в базе данных
                        var executorToDelete = db.Executors.FirstOrDefault(r => r.ExecutorID == executorId);

                        if (executorToDelete != null)
                        {
                            // Проверяем, есть ли записи, которые ссылаются на этого исполнителя
                            var relatedRequests = db.RequestAssignments.Where(r => r.ExecutorID == executorId).ToList();
                            if (relatedRequests.Any())
                            {
                                MessageBox.Show("Cannot delete this executor because there are active requests assigned to them.");
                                return;
                            }

                            db.Executors.Remove(executorToDelete);
                            db.SaveChanges();
                            MessageBox.Show("Executor deleted successfully!");

                            // Обновляем список исполнителей
                            LoadExecutors();
                        }
                        else
                        {
                            MessageBox.Show("Executor not found.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select an executor to delete.");
                }
            }
            catch (Exception ex)
            {
                // Выводим подробную информацию об ошибке
                MessageBox.Show($"An error occurred: {ex.Message}\n{ex.StackTrace}");
            }
        }
        // Очищаем поля ввода
        private void ClearFields()
        {
            ExecutorNameTextBox.Clear();
        }
    }
}