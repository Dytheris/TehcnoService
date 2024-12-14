using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для StatisticsPage.xaml
    /// </summary>
    public partial class StatisticsPage : Page
    {
        private readonly Entities db;

        public StatisticsPage()
        {
            InitializeComponent();
            db = new Entities();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            var totalCompleted = db.RepairRequests.Count(r => r.Status == "Completed");
            var averageCompletionTime = db.RepairRequests
                 .Where(r => r.Status == "Completed" && r.RegistrationDate != null && r.CompletionDate != null)
                 .Select(r => new
                 {
                     CompletionTime = DbFunctions.DiffMinutes(r.RegistrationDate, r.CompletionDate)
                 })
                 .Where(x => x.CompletionTime.HasValue)
                 .Select(x => x.CompletionTime.Value)
                 .DefaultIfEmpty(0)
                 .Average();

            var faultTypeCounts = db.RepairRequests
                .GroupBy(r => r.ProblemDescription)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToList();

            CompletedRequests.Text = $"Total Completed Requests: {totalCompleted}";
            AverageCompletionTime.Text = $"Average Completion Time: {Math.Round(averageCompletionTime, 2)} minutes";

            FaultTypesStats.Text = "Fault Types:\n" + string.Join("\n",
                faultTypeCounts.Select(ft => $"{ft.Type}: {ft.Count} requests"));
        }
    }
}