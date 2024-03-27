using Lab3.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Lab3.Pages.Collaboration
{
    public class DBDisplayModel : PageModel
    {
        public DataTable StateHeight { get; set; } = new DataTable();
        public DataTable CityPopulation { get; set; } = new DataTable();
        [BindProperty] public bool IsChecked { get; set; }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                if (IsChecked == false)
                {
                    StateHeight = new DataTable();
                    // Read from DB new tables
                    string sqlQuery = "SELECT * FROM StateHeight";
                    DBClass.GeneralReaderQuery(sqlQuery);
                    DBClass.Lab3DBConnection.Close();
                    return Page();
                }
                else
                {
                    CityPopulation = new DataTable();
                    // Read from DB new tables
                    string sqlQuery2 = "SELECT * FROM CityPopulation";
                    DBClass.GeneralReaderQuery(sqlQuery2);
                    DBClass.Lab3DBConnection.Close();
                    return Page();
                }
            }
            else
            {
                // Send user to login page
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/DBLogin");
            }
        }

        public void OnPost()
        {
            if (IsChecked == false)
            {
                IsChecked = true;
            }
            else
            {
                IsChecked = false;
            }
        }
    }
}
