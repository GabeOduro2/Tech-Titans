using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab3.Pages.Collaboration
{
    public class FileHandlingModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPostUpload(IFormFile fileupload)
        {
            if (fileupload != null && fileupload.Length > 0)
            {
                string uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fileupload");
                string filePath = Path.Combine(uploadsDir, fileupload.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    fileupload.CopyTo(fileStream);
                }

                return RedirectToPage();
            }
            return Page();
        }
    }
}
