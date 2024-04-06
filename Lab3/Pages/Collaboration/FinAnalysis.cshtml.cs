using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using MathNet.Numerics.LinearRegression;
using System.Globalization;
using MathNet.Numerics;

namespace Lab3.Pages.Collaboration
{
    public class FinAnalysisModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FinAnalysisModel> _logger;

        public FinAnalysisModel(IConfiguration configuration, ILogger<FinAnalysisModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
            TableNames = new List<string>();
            ColumnNames = new List<string>();
        }

        public List<string> TableNames { get; set; }
        public List<string> ColumnNames { get; set; }
        public Dictionary<string, List<string>> ColumnData { get; set; }
        public (double Intercept, double Slope) RegressionResult { get; set; }

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

            return Page();
        }

        private async Task GetTableNamesAsync()
        {
            string connectionString = _configuration.GetConnectionString("Lab3ConnectionString");
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            TableNames.Add(reader.GetString(0));
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

    }
}

