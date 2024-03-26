using Lab3.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab3.Pages
{
    public class DBLoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnGet(String logout)
        {
            // Checks if user has logged out
            if (logout == "true")
            {
                HttpContext.Session.Clear();
                ViewData["LoginMessage"] = "Successfully Logged Out!";
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (DBClass.StoredProcedureLogin(Username, Password))
            {
                // Logs User in and sets the session state
                HttpContext.Session.SetString("username", Username);
                DBClass.Lab3AUTHConnection.Close();
                return RedirectToPage("SecureLoginLanding");
            }
            else
            {
                // Incorrect login Message
                ViewData["LoginMessage"] = "Username and/or Password Incorrect";
                DBClass.Lab3AUTHConnection.Close();
                return Page();
            }
        }

        public IActionResult OnPostLogoutHandler()
        {
            HttpContext.Session.Clear();
            return Page();
        }
    }
}
