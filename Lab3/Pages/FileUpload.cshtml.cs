using Lab3.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Formats.Asn1;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using System.Data.SqlClient;

namespace Lab3.Pages
{
    public class FileUploadModel : PageModel
    {
        [BindProperty]
        public List<IFormFile> FileList { get; set; }

        public void OnGet()
        {
            // Example adapted from:
            // https://code-maze.com/file-upload-aspnetcore-mvc/
        }

        public async Task<IActionResult> OnPostAsync(IFormFile fileUpload)
        {
            var filePath = Path.Combine("wwwroot", "fileupload", fileUpload.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileUpload.CopyToAsync(stream);
            }

            await ProcessCsvAsync(filePath);
            return RedirectToPage("/FileHandling");

        }
        private async Task ProcessCsvAsync(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                string[] headers = csv.Context.Reader.HeaderRecord;

                // You can infer data types here if needed or default to string
                // For simplicity, I'm assuming all data as string

                // Create SQL table based on headers
                string tableName = "DynamicTable_" + DateTime.Now.Ticks; // Generating a unique table name
                await CreateSqlTableAsync(headers, tableName);

                while (csv.Read())
                {
                    var record = new List<string>();
                    foreach (var header in headers)
                    {
                        record.Add(csv.GetField(header));
                    }

                    // Insert the record into the database
                    await InsertRecordIntoTableAsync(record, tableName,headers);
                }
            }
        }
        private async Task CreateSqlTableAsync(string[] headers, string tableName)
        {
            using (var connection = new SqlConnection("Server=Localhost;Database=Lab3;Trusted_Connection=True"))
            {
                await connection.OpenAsync();
                var createTableCommand = new SqlCommand(GetCreateTableSql(headers, tableName), connection);
                await createTableCommand.ExecuteNonQueryAsync();
            }
        }

        private string GetCreateTableSql(string[] headers, string tableName)
        {
            var columns = string.Join(", ", headers.Select(header => $"[{header}] NVARCHAR(MAX)"));
            return $"CREATE TABLE [{tableName}] ({columns})";
        }
        private async Task InsertRecordIntoTableAsync(List<string> record, string tableName, string[] headers)
        {
            using (var connection = new SqlConnection("Server=Localhost;Database=Lab3;Trusted_Connection=True"))
            {
                await connection.OpenAsync();

                var sql = GetInsertSql(record, tableName, headers);
                var insertCommand = new SqlCommand(sql, connection);

                // Add parameters to SqlCommand
                for (int i = 0; i < record.Count; i++)
                {
                    insertCommand.Parameters.AddWithValue($"@param{i}", record[i]);
                }

                await insertCommand.ExecuteNonQueryAsync();
            }
        }

        private string GetInsertSql(List<string> record, string tableName, string[] headers)
        {
            var columns = string.Join(", ", headers.Select(header => $"[{header}]"));
            var parameters = string.Join(", ", headers.Select((_, i) => $"@param{i}"));
            var sql = $"INSERT INTO [{tableName}] ({columns}) VALUES ({parameters})";

            return sql;
        }

    }
}