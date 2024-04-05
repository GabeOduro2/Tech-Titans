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

        public IActionResult OnGet(int Steps)
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                return Page();
            }
            else
            {
                // Send user to login page
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/DBLogin");
            }
        }


    }
}
