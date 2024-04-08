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

        public IActionResult OnPostUpload(IFormFile ResourceFiles)
        {
            if (ResourceFiles != null && ResourceFiles.Length > 0)
            {
                string uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ResourceFiles");
                string filePath = Path.Combine(uploadsDir, ResourceFiles.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ResourceFiles.CopyTo(fileStream);
                }

                return RedirectToPage();
            }

            return Page();
        }

        public IActionResult OnPostDelete(string fileName)
        {
            string uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ResourceFiles");
            string filePath = Path.Combine(uploadsDir, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return RedirectToPage();
        }

    }
}
