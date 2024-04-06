using Lab3.Pages.DataClasses;
using Lab3.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;


namespace Lab3.Pages.Collaboration
{
    public class AdminEffReviewModel : PageModel
    {
        [BindProperty] public string? ErrorMessage { get; set; }
        [BindProperty] public string? NewChatMessage { get; set; }
        public List<Chat> ChatMessages { get; set; }

        public AdminEffReviewModel()
        {
            // Retrieve chat messages from the database
            ChatMessages = DBClass.GetChatMessages();
            DBClass.Lab3DBConnection.Close();
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {             
                return Page();
            }
            else
            {
                // Redirect to login page if user not logged in
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/DBLogin");
            }
        }

        public IActionResult OnPostChat()
        {
            if (!string.IsNullOrEmpty(NewChatMessage))
            {
                string username = HttpContext.Session.GetString("username");
                DateTime timestamp = DateTime.Now;

                // Create a new Chat object with the submitted message, username, and timestamp
                Chat newChat = new Chat
                {
                    Username = username,
                    Message = NewChatMessage,
                    Timestamp = timestamp
                };

                // Insert the new chat message into the database
                DBClass.InsertChatMessage(newChat);

                // Redirect back to the page
                return RedirectToPage();
            }

            return Page();
        }

        public IActionResult OnPostUpload(IFormFile AdminReviewFiles)
        {
            if (AdminReviewFiles != null && AdminReviewFiles.Length > 0)
            {
                // Save the uploaded file to the server
                string uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AdminReviewFiles");
                string filePath = Path.Combine(uploadsDir, AdminReviewFiles.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    AdminReviewFiles.CopyTo(fileStream);
                }

                // Optionally, you can add code here to save information about the uploaded file to the database or perform other operations.

                // Redirect back to the page
                return RedirectToPage();
            }

            // Return the page if no file was uploaded
            return Page();
        }

        public IActionResult OnPostDelete(string fileName)
        {
            string uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AdminReviewFiles");
            string filePath = Path.Combine(uploadsDir, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return RedirectToPage();
        }

    }
}
