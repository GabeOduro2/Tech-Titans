using Lab3.Pages.DataClasses;
using Lab3.Pages.DB;
using Lab3.Pages.KnowledgeItemPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Lab3.Pages.KnowledgeItemPages
{
    public class AddKnowledgeItem : PageModel
    {
        [BindProperty] public KnowledgeItemModel NewItem { get; set; }
        [BindProperty] public List<SelectListItem> Users { get; set; }
        [BindProperty] public string? Category { get; set; }
        public string[] Categories = new[] { "Knowledge Item", "SWOT" };
        [BindProperty] public string? CreateMessage { get; set; }

        public IActionResult OnGet(KnowledgeItemModel category)
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

        public void LoadUsers()
        {
            // Populating User SELECT control
            SqlDataReader UserReader = DBClass.GeneralReaderQuery("SELECT * FROM [USER]");
            Users = new List<SelectListItem>();

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


        public IActionResult OnPostCategory(KnowledgeItemModel category)
        {
            if (Category == "SWOT")
            {
                NewItem.Information = "Strengths: \nWeaknesses: \nOpportunities: \nThreats: ";
            }
            LoadUsers();
            return Page();
        }

        public IActionResult OnPostSubmit()
        {
            if (NewItem.Title != null && NewItem.Information != null)
            {
                DBClass.InsertKnowledgeItem(NewItem, Category);

                DBClass.Lab3DBConnection.Close();

                CreateMessage = "Knowledge Item: " + NewItem.Title + " has been created";
            }
            LoadUsers();
            return Page();
        }

        public IActionResult OnPostPopulateHandler()
        {
            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                Category = "Knowledge Item";
                NewItem.Category = Category;
                NewItem.Title = "Surf Swag";
                NewItem.UserID = 1;
                NewItem.Information = "Super Cool Yo!";
            }
            return Page();
        }

        public IActionResult OnPostClear()
        {
            ModelState.Clear();
            NewItem.Title = "";
            //NewItem.UserID = null;
            NewItem.Information = "";
            return Page();
        }
    }
}
