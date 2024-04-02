using Lab3.Pages.DataClasses;
using Lab3.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using System.Data.SqlClient;

namespace Lab3.Pages.KnowledgeItemPages
{
    public class SearchKnowledgeItemModel : PageModel
    {
        [BindProperty(SupportsGet = true)] public string? KeywordSearch { get; set; }
        [BindProperty] public int SelectedUserID { get; set; }
        [BindProperty(SupportsGet = true)] public List<SelectListItem> Users { get; set; }
        [BindProperty(SupportsGet = true)] public List<KnowledgeItemModel> KnowledgeItems { get; set; }
        [BindProperty] public User User { get; set; }
        [BindProperty] public string ErrorMessage { get; set; }

        public SearchKnowledgeItemModel()
        {
            KnowledgeItems = new List<KnowledgeItemModel>();
            Users = new List<SelectListItem>();

        }

        public IActionResult OnGet(int userID)
        {
            // Reading from DB
            LoadUsers();
            LoadKnowledge();

            if (HttpContext.Session.GetString("username") != null)
            {
                // Display page
                return Page();
            }
            else
            {
                // Send user to login page
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/DBLogin");
            }
        }

        public IActionResult OnPostSearch(string KeywordSearch)
        {
            if (!string.IsNullOrEmpty(KeywordSearch))
            {
                //Perform search based on keyword
                SearchKnowledgeItems(KeywordSearch);
            }
            else
            {
                // Display all Items if no user selected
                LoadKnowledge();
            }
            LoadUsers();
            return Page();
        }

        public IActionResult OnPostFilter()
        {
            if (SelectedUserID != 0)
            {
                // Filter by user
                FilterKnowledgeItems(SelectedUserID);
            }
            else
            {
                // Display all Items if no user selected
                LoadKnowledge();
            }
            LoadUsers();
            return Page();
        }

        public void LoadKnowledge()
        {
            KnowledgeItems = new List<KnowledgeItemModel>();

            // @ Symbol for a verbatim / multi-line string
            string sqlQuery = @"select KnowledgeItem.Title,
            KnowledgeItem.Information, [User].FirstName, [User].LastName
            from KnowledgeItem, [User]
            WHERE KnowledgeItem.UserID = [User].UserID;";

            SqlDataReader KnowledgeReader = DBClass.GeneralReaderQuery(sqlQuery);

            while (KnowledgeReader.Read())
            {
                KnowledgeItems.Add(new KnowledgeItemModel
                {
                    Title = KnowledgeReader["Title"].ToString(),
                    Information = KnowledgeReader["Information"].ToString(),
                    User = new User
                    {
                        FirstName = KnowledgeReader["FirstName"].ToString(),
                        LastName = KnowledgeReader["LastName"].ToString()
                    }
                });
            }
            DBClass.Lab3DBConnection.Close();
        }

        public void LoadUsers()
        {
            Users = new List<SelectListItem>();

            // Populating User SELECT control
            SqlDataReader UserReader = DBClass.GeneralReaderQuery("SELECT * FROM [USER]");


            while (UserReader.Read())
            {
                string fullName = $"{UserReader["FirstName"]} {UserReader["LastName"]}";
                Users.Add(
                    new SelectListItem(
                        fullName,
                        UserReader["UserID"].ToString()));
            }
            DBClass.Lab3DBConnection.Close();
        }
        public void SearchKnowledgeItems(string keyword)
        {
            KnowledgeItems = new List<KnowledgeItemModel>();
            // Execute the query and populate KnowledgeItems list
            string query = @"
        SELECT KnowledgeItem.Title, KnowledgeItem.Information,
               [User].FirstName, [User].LastName
        FROM KnowledgeItem
        INNER JOIN [User] ON KnowledgeItem.UserID = [User].UserID
        WHERE KnowledgeItem.Title LIKE @keyword OR KnowledgeItem.Information LIKE @keyword";

            SqlParameter[] parameters = { new SqlParameter("@Keyword", "%" + keyword + "%") };

            // Pass the query to the GeneralReaderQuery method with parameters
            SqlDataReader reader = DBClass.GeneralReaderQuery(query, parameters);
            {
                while (reader.Read())
                {
                    KnowledgeItems.Add(new KnowledgeItemModel
                    {
                        Title = reader["Title"].ToString(),
                        Information = reader["Information"].ToString(),
                        User = new User
                        {
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString()
                        }
                    });
                }
                DBClass.Lab3DBConnection.Close();
            }

        }

        public List<KnowledgeItemModel> FilterKnowledgeItems(int userID)
        {
            KnowledgeItems = new List<KnowledgeItemModel>();

            // @ Symbol for a verbatim / multi-line string
            string query = @"
                    SELECT KnowledgeItem.Title, KnowledgeItem.Information,
                           [User].FirstName, [User].LastName
                    FROM KnowledgeItem
                    INNER JOIN [User] ON KnowledgeItem.UserID = [User].UserID
                    WHERE KnowledgeItem.UserID = " + userID;

            SqlDataReader KnowledgeReader = DBClass.GeneralReaderQuery(query);

            while (KnowledgeReader.Read())
            {
                KnowledgeItems.Add(new KnowledgeItemModel
                {
                    Title = KnowledgeReader["Title"].ToString(),
                    Information = KnowledgeReader["Information"].ToString(),
                    User = new User
                    {
                        FirstName = KnowledgeReader["FirstName"].ToString(),
                        LastName = KnowledgeReader["LastName"].ToString()
                    }
                });
            }
            DBClass.Lab3DBConnection.Close();
            return KnowledgeItems;
        }
        public IActionResult OnPostClear()
        {
            ModelState.Clear();
            LoadUsers();
            LoadKnowledge();
            KeywordSearch = null;
            SelectedUserID = 0;
            return Page();
        }
        public IActionResult OnPostAddKnowledge()
        {
            return RedirectToPage("AddKnowledgeItem");
        }
    }
}
