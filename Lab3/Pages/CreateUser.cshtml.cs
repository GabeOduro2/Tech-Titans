using Lab3.Pages.DataClasses;
using Lab3.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Lab3.Pages
{
    public class CreateUserModel : PageModel
    {
        [BindProperty]
        public User NewUser { get; set; }
        [BindProperty]
        public List<SelectListItem> SelectedUserType { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "Admin", Text = "Administrator" },
            new SelectListItem { Value = "Administrative Efficiency Review", Text = "Administrative Efficiency Review" },
            new SelectListItem { Value = "Administrative Review", Text = "Administrative Efficiency Review" },
            new SelectListItem { Value = "Personal Policy and Administration", Text = "Personal Policy and Administration" },
            new SelectListItem { Value = "Economic Development", Text = "Economic Development" },
            new SelectListItem { Value = "Citizen Communication", Text = "Citizen Communications" },
        };

        public string? CreateMessage { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                // Adds hashed credentials to AUTH DB
                DBClass.CreateHashedUser(NewUser.Username, NewUser.Password);
                DBClass.Lab3AUTHConnection.Close();
                // Adds user's data (without password) to Main DB
                DBClass.InsertUser(NewUser);
                DBClass.Lab3DBConnection.Close();

                CreateMessage = "User " + NewUser.FirstName + " " +
                     NewUser.LastName + " has been created";
            }
            return Page();
        }

        public IActionResult OnPostPopulateHandler()
        {
            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                // Populates form with sample data
                NewUser = new User
                {
                    Username = "sampleUsername",
                    Password = "samplePassword",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@example.com",
                    Phone = "1234567890",
                    Address = "123 Sample St",
                    UserType = "sampleType"
                };
            }
            return Page();
        }

        public IActionResult OnPostClear()
        {
            ModelState.Clear();
            // Clears Form
            NewUser = new User();

            return Page();
        }
    }

}
