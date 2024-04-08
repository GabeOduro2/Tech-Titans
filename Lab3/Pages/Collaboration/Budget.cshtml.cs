using Lab3.Pages.DataClasses;
using Lab3.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab3.Pages.Collaboration
{
    public class BudgetModel : PageModel
    {
        [BindProperty] public string? ErrorMessage { get; set; }
        [BindProperty] public string? NewChatMessage { get; set; }
        public List<Chat> ChatMessages { get; set; }

        public BudgetModel()
        {
            ChatMessages = DBClass.GetBudgetChatMessages();
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

                Chat newChat = new Chat
                {
                    Username = username,
                    Message = NewChatMessage,
                    Timestamp = timestamp
                };

                DBClass.InsertBudgetChatMessage(newChat);

                return RedirectToPage();
            }

            return Page();
        }

        public IActionResult OnPostUpload(IFormFile BudgetFiles)
        {
            if (BudgetFiles != null && BudgetFiles.Length > 0)
            {
                string uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "BudgetFiles");
                string filePath = Path.Combine(uploadsDir, BudgetFiles.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    BudgetFiles.CopyTo(fileStream);
                }

                return RedirectToPage();
            }

            return Page();
        }

        public IActionResult OnPostDelete(string fileName)
        {
            string uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "BudgetFiles");
            string filePath = Path.Combine(uploadsDir, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return RedirectToPage();
        }
    }
}
