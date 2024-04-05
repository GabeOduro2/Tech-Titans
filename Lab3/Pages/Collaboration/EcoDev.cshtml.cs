using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lab3.Pages.DataClasses;
using Lab3.Pages.DB;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Lab3.Pages.Collaboration
{
    public class EcoDevModel : PageModel
    {
        [BindProperty] public string? ErrorMessage { get; set; }
        [BindProperty] public string? NewChatMessage { get; set; }
        public List<Chat> ChatMessages { get; set; }

        public EcoDevModel()
        {
            // Retrieve chat messages from the database
            ChatMessages = DBClass.GetEcoDevChatMessages();
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
                DBClass.InsertEcoDevChatMessage(newChat);

                // Redirect back to the page
                return RedirectToPage();
            }

            return Page();
        }

        public IActionResult OnPostUpload(IFormFile EcoDevfiles)
        {
            if (EcoDevfiles != null && EcoDevfiles.Length > 0)
            {
                string uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EcoDevFiles");
                string filePath = Path.Combine(uploadsDir, EcoDevfiles.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    EcoDevfiles.CopyTo(fileStream);
                }

                return RedirectToPage();
            }

            return Page();
        }

        public IActionResult OnPostDelete(string fileName)
        {
            string uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EcoDevFiles");
            string filePath = Path.Combine(uploadsDir, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return RedirectToPage();
        }
    }
}
