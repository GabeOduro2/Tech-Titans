using Lab3.Pages.DataClasses;
using Lab3.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace Lab3.Pages.Collaboration
{
    public class CollaborateAreaModel : PageModel
    {
        [BindProperty] public List<SelectListItem> CollabAreas { get; set; }
        [BindProperty] public List<SelectListItem> SelectKnowledgeItem { get; set; }
        [BindProperty] public List<KnowledgeItemModel> KnowledgeItems { get; set; } = new List<KnowledgeItemModel>();
        [BindProperty] public CollabClass CurrentCollab { get; set; }
        [BindProperty] public Chat Messages { get; set; }
        [BindProperty] public Chat NewMessage { get; set; }
        [BindProperty] public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                // Display page
                SqlDataReader CollabReader = DBClass.GeneralReaderQuery("SELECT * FROM Collaboration");

                CollabAreas = new List<SelectListItem>();

                while (CollabReader.Read())
                {
                    CollabAreas.Add(
                        new SelectListItem(
                            CollabReader["Name"].ToString(),
                            CollabReader["CollabID"].ToString()));
                }

                DBClass.Lab3DBConnection.Close();


                SqlDataReader knowledgeReader = DBClass.KnowledgeReader();

                SelectKnowledgeItem = new List<SelectListItem>();

                while (knowledgeReader.Read())
                {
                    KnowledgeItems.Add(new KnowledgeItemModel
                    {
                        KnowledgeId = Int32.Parse(knowledgeReader["KnowledgeID"].ToString()),
                        Title = knowledgeReader["Title"].ToString(),
                        Category = knowledgeReader["Category"].ToString(),
                        Information = knowledgeReader["Information"].ToString()
                    });

                    SelectKnowledgeItem.Add(
                        new SelectListItem(
                            knowledgeReader["Title"].ToString(),
                            knowledgeReader["KnowledgeID"].ToString()));
                }

                DBClass.Lab3DBConnection.Close();

                return Page();
            }
            else
            {
                // Send user to login page
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/DBLogin");
            }
        }

        public IActionResult OnPostChat()
        {

            return Page();
        }
        public IActionResult OnPostCSVFile()
        {
            return RedirectToPage("/FileUpload");
        }
    }
}
