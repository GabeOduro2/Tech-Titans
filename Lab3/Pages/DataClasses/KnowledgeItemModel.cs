using System.ComponentModel.DataAnnotations;

namespace Lab3.Pages.DataClasses
{
    public class KnowledgeItemModel
    {
        public int KnowledgeId { get; set; }
        [Required(ErrorMessage = "Must Enter a Title")] public string? Title { get; set; }
        public string Category { get; set; }
        [Required(ErrorMessage = "Must Enter Some Information")] public string? Information { get; set; }
        public int? UserID { get; set; }
        public User User { get; set; } 

        public KnowledgeItemModel() 
        {

            User = new User();
        }

    }
}
