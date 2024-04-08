using Lab3.Pages.DataClasses;
using Lab3.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab3.Pages.Collaboration
{
    public class PersonalPolicyModel : PageModel
    {
        [BindProperty] public string? ErrorMessage { get; set; }
        [BindProperty] public string? NewChatMessage { get; set; }
        public List<Chat> ChatMessages { get; set; }

        public PersonalPolicyModel()
        {
            // Retrieve chat messages from the database
            ChatMessages = DBClass.GetPolicyChatMessages();
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
                DBClass.InsertPolicyChatMessage(newChat);

                // Redirect back to the page
                return RedirectToPage();
            }

            return Page();
        }

        public IActionResult OnPostUpload(IFormFile PolicyFiles)
        {
            if (PolicyFiles != null && PolicyFiles.Length > 0)
            {
                string uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PolicyFiles");
                string filePath = Path.Combine(uploadsDir, PolicyFiles.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    PolicyFiles.CopyTo(fileStream);
                }

                return RedirectToPage();
            }

            return Page();
        }

        public IActionResult OnPostDelete(string fileName)
        {
            string uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PolicyFiles");
            string filePath = Path.Combine(uploadsDir, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return RedirectToPage();
        }
    }
}
