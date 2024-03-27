using Lab3.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Formats.Asn1;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using CsvHelper;

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

        public IActionResult OnPost()
        {

            var filePaths = new List<string>();
            foreach (var formFile in FileList)
            {
                if (formFile.Length > 0)
                {
                    // full path to file in temp location
                    var filePath = Directory.GetCurrentDirectory() + @"\wwwroot\fileupload\" + formFile.FileName;
                    filePaths.Add(filePath);
                    // Create local copy of file location
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                    ProcessCsvFile(filePath);
                }
            }


            return RedirectToPage("FileHandling");
        }
        public void ProcessCsvFile(string filePath)
        {
            using (var reader = new StreamReader(filePath))

            using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)))

            {
                var records = csv.GetRecords<dynamic>().ToList();

                var columnHeaders = records.First();

                string fileName = Path.GetFileNameWithoutExtension(filePath);

                string createTableQuery = CreateTableQuery(fileName, columnHeaders);

                DBClass.InsertQueryCSV(createTableQuery);

                string tableName = fileName;

                string insertDataQuery = InsertDataQuery(records.Skip(1), columnHeaders, tableName);

                DBClass.InsertQueryCSV(insertDataQuery);

            }

        }

        public string CreateTableQuery(string fileName, dynamic columnHeaders)

        {
            string tableName = Path.GetFileNameWithoutExtension(fileName);

            string createTableQuery = $"CREATE TABLE {tableName} (";

            foreach (var columnHeader in columnHeaders)

            {
                createTableQuery += $"{columnHeader} NVARCHAR(MAX), ";
            }

            createTableQuery = createTableQuery.TrimEnd(',', ' ') + ")";

            return createTableQuery;
        }

        public string InsertDataQuery(IEnumerable<dynamic> data, dynamic columnHeaders, string tableName)

        {
            string insertDataQuery = $"INSERT INTO {tableName} (";

            var columns = ((IDictionary<string, object>)columnHeaders).Keys.ToList();

            foreach (var columnHeader in columnHeaders)

            {
                insertDataQuery += $"{columnHeader}, ";
            }

            insertDataQuery = insertDataQuery.TrimEnd(',', ' ') + ") VALUES ";

            foreach (var record in data)
            {
                insertDataQuery += "(";
                foreach (var columnHeader in columns)
                {
                    var value = ((IDictionary<string, object>)record)[columnHeader];
                    insertDataQuery += $"'{value}', ";
                }
                insertDataQuery = insertDataQuery.TrimEnd(',', ' ') + "), ";
            }
            return insertDataQuery.TrimEnd(',', ' ');
        }
    }

}

