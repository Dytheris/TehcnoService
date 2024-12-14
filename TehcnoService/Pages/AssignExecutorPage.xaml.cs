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
    /// Логика взаимодействия для AssignExecutorPage.xaml
    /// </summary>
    public partial class AssignExecutorPage : Page
    {
        private readonly Entities db;
        private readonly int _requestId;

        public AssignExecutorPage(int requestId)
        {
            InitializeComponent();
            db = new Entities();
            _requestId = requestId;
            LoadExecutors();
        }

        private void LoadExecutors()
        {
            var executors = db.Executors.ToList();
            ExecutorComboBox.ItemsSource = executors;
            ExecutorComboBox.DisplayMemberPath = "FullName";
            ExecutorComboBox.SelectedValuePath = "ExecutorID";
        }

        private void AssignExecutor_Click(object sender, RoutedEventArgs e)
        {
            if (ExecutorComboBox.SelectedValue == null)
            {
                MessageBox.Show("Please select an executor.");
                return;
            }

            var assignment = new RequestAssignments
            {
                RequestID = _requestId,
                ExecutorID = (int)ExecutorComboBox.SelectedValue,
                AssignmentDate = DateTime.Now
            };

            db.RequestAssignments.Add(assignment);
            db.SaveChanges();

            MessageBox.Show("Executor assigned successfully!");
            NavigationService.GoBack();
        }
    }
}