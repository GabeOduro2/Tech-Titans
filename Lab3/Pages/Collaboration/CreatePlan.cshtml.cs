using Lab3.Pages.DataClasses;
using Lab3.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Lab3.Pages.Collaboration
{
    public class CreatePlanModel : PageModel
    {
        [BindProperty] public Plan NewPlan { get; set; }
        [BindProperty] public int NumSteps { get; set; }
        [BindProperty] public int SelectedUsers { get; set; }
        [BindProperty(SupportsGet = true)] public List<SelectListItem> Users { get; set; }
        [BindProperty] public string? Message { get; set; }

        public IActionResult OnGet(int Steps)
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                // Display page
                LoadUsers();
                return Page();
            }
            else
            {
                // Send user to login page
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/DBLogin");
            }
        }

        public IActionResult OnPostSave()
        {
            if (NewPlan.Name != null)
            {
                DBClass.InsertNewPlan(NewPlan);
                Message = NewPlan.Name + " has been created";
            }
            LoadUsers();
            // Add logic to redirect to page after creating in DB
            return Page();
        }

        public void LoadUsers()
        {
            Users = new List<SelectListItem>();

            // Populating User SELECT control
            SqlDataReader UserReader = DBClass.GeneralReaderQuery("SELECT * FROM [USER]");


            while (UserReader.Read())
            {
                string fullName = $"{UserReader["FirstName"]} {UserReader["LastName"]}";
                Users.Add(
                    new SelectListItem(
                        fullName,
                        UserReader["UserID"].ToString()));
            }
            DBClass.Lab3DBConnection.Close();
        }

        //public IActionResult OnPostAddStep()
        //{
        //    if (NewPlan.Name != null)
        //    {
        //        DBClass.InsertNewPlan(NewPlan);

        //    }
        //    return Page();
        //}
    }
}
