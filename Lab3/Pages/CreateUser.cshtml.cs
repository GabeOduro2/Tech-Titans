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
        public List<SelectListItem> SelectedUserType { get; set; } = new List<SelectListItem>
        {
            new() { Value = "Admin", Text = "Administrator" },
            new() { Value = "Finance", Text = "Finance: Tax Base and Spending Level Projections" },
            new() { Value = "Administrative Review", Text = "Administrative Efficiency Review" },
            new() { Value = "Personal Policy and Administration", Text = "Personal Policy and Administration" },
            new() { Value = "Economic Development", Text = "Economic Development: Strategy and Implementation" },
            new() { Value = "PR", Text = "Citizen Communications/ PR" },
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
                NewUser = new User();
            }
            PopulateSelectedUserType();
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
                    Username = "cEsposito",
                    Password = "Password12345",
                    FirstName = "Carla",
                    LastName = "Esposito",
                    Email = "cEsposito@madisonco.com",
                    Phone = "1234567890",
                    Address = "123 Main St",
                    UserType = "Finance"
                };
                PopulateSelectedUserType();
            }
            return Page();
        }

        public IActionResult OnPostClear()
        {
            ModelState.Clear();
            // Clears Form
            NewUser = new User();
            PopulateSelectedUserType();
            return Page();
        }

        public List<SelectListItem> PopulateSelectedUserType()
        {
            SelectedUserType = new List<SelectListItem>()
            {
                new() { Value = "Admin", Text = "Administrator" },
                new() { Value = "Finance", Text = "Finance: Tax Base and Spending Level Projections" },
                new() { Value = "Administrative Review", Text = "Administrative Efficiency Review" },
                new() { Value = "Personal Policy and Administration", Text = "Personal Policy and Administration" },
                new() { Value = "Economic Development", Text = "Economic Development: Strategy and Implementation" },
                new() { Value = "PR", Text = "Citizen Communications/ PR" },
            };
            return SelectedUserType;
        }
    }

}
