using Lab3.Pages.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Lab3.Pages
{
    public class DBLoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string? UserType { get; set; }

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
            // Verifies credentials
            if (DBClass.StoredProcedureLogin(Username, Password))
            {
                // Logs User in and sets the session state
                HttpContext.Session.SetString("username", Username);
                DBClass.Lab3AUTHConnection.Close();
                
                // Queries User's type in DB to add to SessionState  
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new("@Username",Username)
                };
                string sqlQuery = "SELECT UserType FROM [User] WHERE Username = @Username";
                SqlDataReader TypeReader = DBClass.GeneralReaderQuery(sqlQuery, parameters);
                while (TypeReader.Read())
                {
                    UserType = TypeReader["UserType"].ToString();
                }

                if (UserType != null)
                {
                    HttpContext.Session.SetString("userType", UserType);
                }
                DBClass.Lab3DBConnection.Close();
                return RedirectToPage("/SecureLoginLanding");
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
