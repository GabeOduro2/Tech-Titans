using Lab3.Pages.DataClasses;
using Lab3.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Lab3.Pages.Collaboration
{
    public class CollaborateAreaModel : PageModel
    {
        [BindProperty] public List<SelectListItem> CollabAreas { get; set; }
        [BindProperty] public List<SelectListItem> SelectKnowledgeItem { get; set; }
        [BindProperty] public List<KnowledgeItemModel> KnowledgeItems { get; set; } = new List<KnowledgeItemModel>();
        [BindProperty] public CollabClass CurrentCollab { get; set; }
        [BindProperty] public string? ErrorMessage { get; set; }
        [BindProperty] public string NewChatMessage { get; set; }
        public List<Chat> ChatMessages { get; set; }
        [BindProperty] public CollabClass NewCollab { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                // Retrieve chat messages from the database
                ChatMessages = DBClass.GetChatMessages();

                // Populate collaboration areas dropdown
                SqlDataReader CollabReader = DBClass.GeneralReaderQuery("SELECT * FROM Collaboration");
                CollabAreas = new List<SelectListItem>();
                while (CollabReader.Read())
                {
                    CollabAreas.Add(new SelectListItem(
                        CollabReader["Name"].ToString(),
                        CollabReader["CollabID"].ToString()));
                }
                DBClass.Lab3DBConnection.Close();

                // Populate knowledge items dropdown
                SqlDataReader knowledgeReader = DBClass.KnowledgeReader();
                SelectKnowledgeItem = new List<SelectListItem>();
                while (knowledgeReader.Read())
                {
                    KnowledgeItems.Add(new KnowledgeItemModel
                    {
                        KnowledgeId = Convert.ToInt32(knowledgeReader["KnowledgeID"]),
                        Title = knowledgeReader["Title"].ToString(),
                        Category = knowledgeReader["Category"].ToString(),
                        Information = knowledgeReader["Information"].ToString()
                    });
                    SelectKnowledgeItem.Add(new SelectListItem(
                        knowledgeReader["Title"].ToString(),
                        knowledgeReader["KnowledgeID"].ToString()));
                }
                DBClass.Lab3DBConnection.Close();

                return Page();
            }
            else
            {
                // Redirect to login page if user not logged in
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/DBLogin");
            }
        }

        public IActionResult OnPost()
        {
            if (NewCollab != null && NewCollab.Name != null) // Check if NewCollab is not null before accessing its properties
            {
                DBClass.InsertNewCollabArea(NewCollab);
                DBClass.Lab3DBConnection.Close();
            }
            return Page();
        }

        public IActionResult OnPostChat()
        {
            // Check if NewChatMessage is null or empty
            if (string.IsNullOrEmpty(NewChatMessage))
            {
                // Set error message
                ErrorMessage = "Message cannot be empty.";
                return RedirectToPage(); // Redirect back to the page
            }

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




        public IActionResult OnPostCSVFile()
        {
            return RedirectToPage("/FileUpload");
        }
    }
}
