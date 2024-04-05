using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Lab3.Pages
{
    public class FinAnalysisModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public FinAnalysisModel(IConfiguration configuration)
        {
            _configuration = configuration;
            TableNames = new List<string>();
            ColumnNames = new List<string>();
        }

        public List<string> TableNames { get; set; }
        public List<string> ColumnNames { get; set; }

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
            await GetColumnNamesAsync(SelectedTable);
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
                // Handle the case where tableName is null or empty
                return;
            }

            string connectionString = _configuration.GetConnectionString("Lab3ConnectionString");
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName";
                using (var command = new SqlCommand(query, connection))
                {
                    // Ensure that you're adding the parameter correctly
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
    }
}

