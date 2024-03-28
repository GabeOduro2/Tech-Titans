using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lab3.Pages.DB;
using Lab3.Pages.DataClasses;
namespace Lab3.Pages.Collaboration
{
    public class NewCollabAreaModel : PageModel
    {
        [BindProperty] public CollabClass NewCollab { get; set; }


        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (NewCollab.Name != null)
            {
                DBClass.InsertNewCollabArea(NewCollab);

                DBClass.Lab3DBConnection.Close();
            }
            return Page();
        }
    }
}
