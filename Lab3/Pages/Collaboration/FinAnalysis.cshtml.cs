using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using MathNet.Numerics.LinearRegression;
using System.Globalization;
using MathNet.Numerics;
using Lab3.Pages.DataClasses;
using System.Text.Json;

namespace Lab3.Pages.Collaboration
{
    public class FinAnalysisModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FinAnalysisModel> _logger;
        
        private readonly IWebHostEnvironment _env;


        public FinAnalysisModel(IConfiguration configuration, ILogger<FinAnalysisModel> logger, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _logger = logger;
            _env = env;
            TableNames = new List<string>();
            ColumnNames = new List<string>();
            ColumnFinancials = new Dictionary<string, FinancialData>();
        }
        public decimal TotalRevenues { get; set; }
        public decimal TotalExpenses { get; set; }
        public List<string> TableNames { get; set; }
        public List<string> ColumnNames { get; set; }
        public Dictionary<string, List<string>> ColumnData { get; set; }
        public (double Intercept, double Slope) RegressionResult { get; set; }
        public Dictionary<string, FinancialData> ColumnFinancials { get; set; }
        [BindProperty]
        public string SelectedTable { get; set; }
        [BindProperty]
        public List<string> SelectedColumns { get; set; }
        

        public async Task OnGetAsync()
        {
            await GetTableNamesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!string.IsNullOrEmpty(SelectedTable))
            {
                HttpContext.Session.SetString("SelectedTable", SelectedTable);
            }

            await GetColumnNamesAsync(SelectedTable);

            if (SelectedColumns != null && SelectedColumns.Count == 2) // For simple linear regression
            {
                await FetchColumnDataAsync();
                PerformRegression();
            }
            if (SelectedColumns != null && SelectedColumns.Count > 0)
            {
                await FetchColumnDataAsync();
                ProcessColumnDataForRevenueAndExpenses();
                PrepareChartData();
            }

            return Page();
        }
        private List<string> GetBudgetFileNames()
        {
            var budgetFilesPath = Path.Combine(_env.WebRootPath, "BudgetFiles");
            return Directory.GetFiles(budgetFilesPath).Select(Path.GetFileNameWithoutExtension).ToList();
        }

        private async Task GetTableNamesAsync()
        {
            var fileNames = GetBudgetFileNames();
            string connectionString = _configuration.GetConnectionString("Lab3ConnectionString");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                foreach (var fileName in fileNames)
                {
                    var query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = @TableName";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TableName", fileName);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                TableNames.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
        }

        private async Task GetColumnNamesAsync(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                _logger.LogWarning("GetColumnNamesAsync called with null or empty tableName");
                return;
            }

            string connectionString = _configuration.GetConnectionString("Lab3ConnectionString");
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tableName);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        ColumnNames = new List<string>();
                        while (await reader.ReadAsync())
                        {
                            ColumnNames.Add(reader.GetString(0));
                        }
                    }
                }
            }
        }

        private async Task FetchColumnDataAsync()
        {
            var selectedTable = HttpContext.Session.GetString("SelectedTable");
            if (string.IsNullOrEmpty(selectedTable))
            {
                _logger.LogWarning("SelectedTable is not found in the session.");
                return;
            }

            string connectionString = _configuration.GetConnectionString("Lab3ConnectionString");
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                ColumnData = new Dictionary<string, List<string>>();

                foreach (var column in SelectedColumns)
                {
                    if (string.IsNullOrEmpty(column))
                    {
                        _logger.LogWarning("Invalid column name: '{Column}'", column);
                        continue;
                    }

                    var query = $"SELECT [{column}] FROM [{selectedTable}]";
                    _logger.LogInformation("Executing query: {Query}", query);

                    var columnValues = new List<string>();

                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                columnValues.Add(reader[0].ToString());
                            }
                        }
                    }

                    ColumnData.Add(column, columnValues);
                }
            }
        }

        private void PerformRegression()
        {
            try
            {
                var xColumnName = SelectedColumns[0];
                var yColumnName = SelectedColumns[1];

                if (!ColumnData.ContainsKey(xColumnName) || !ColumnData.ContainsKey(yColumnName))
                {
                    _logger.LogWarning("One or both selected columns do not exist in the ColumnData.");
                    return;
                }

                var xDataList = new List<double>();
                var yDataList = new List<double>();

                foreach (var value in ColumnData[xColumnName])
                {
                    if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedValue))
                    {
                        xDataList.Add(parsedValue);
                    }
                    else
                    {
                        _logger.LogWarning($"Unable to parse '{value}' to a double in column {xColumnName}");
                    }
                }

                foreach (var value in ColumnData[yColumnName])
                {
                    if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedValue))
                    {
                        yDataList.Add(parsedValue);
                    }
                    else
                    {
                        _logger.LogWarning($"Unable to parse '{value}' to a double in column {yColumnName}");
                    }
                }

                if (xDataList.Count != yDataList.Count)
                {
                    _logger.LogError("The number of data points in the selected columns do not match.");
                    return;
                }

                RegressionResult = SimpleRegression.Fit(xDataList.ToArray(), yDataList.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing regression analysis");
            }
        }
        private void ProcessColumnDataForRevenueAndExpenses()
        {
            ColumnFinancials = new Dictionary<string, FinancialData>();

            foreach (var kvp in ColumnData)
            {
                var financialData = new FinancialData();

                foreach (var value in kvp.Value)
                {
                    if (decimal.TryParse(value, out decimal number))
                    {
                        if (number > 0)
                        {
                            financialData.TotalRevenue += number;
                        }
                        else if (number < 0)
                        {
                            financialData.TotalExpense += number; // Adds the negative value
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Failed to convert value to decimal: {Value}", value);
                        // Handle or log the error as needed
                    }
                }

                ColumnFinancials[kvp.Key] = financialData;
            }
        }
        public string ChartLabelsJson { get; private set; }
        public string RevenueDataJson { get; private set; }
        public string ExpenseDataJson { get; private set; }

        private void PrepareChartData()
        {
            var labels = new List<string>();
            var revenues = new List<decimal>();
            var expenses = new List<decimal>();

            foreach (var entry in ColumnFinancials)
            {
                labels.Add(entry.Key);
                revenues.Add(entry.Value.TotalRevenue);
                expenses.Add(Math.Abs(entry.Value.TotalExpense)); // Convert to positive values for charting
            }

            ChartLabelsJson = JsonSerializer.Serialize(labels);
            RevenueDataJson = JsonSerializer.Serialize(revenues);
            ExpenseDataJson = JsonSerializer.Serialize(expenses);
        }
        
    }
}

